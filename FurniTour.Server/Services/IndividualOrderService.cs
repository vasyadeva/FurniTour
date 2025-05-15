using FurniTour.Server.Constants;
using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.IndividualOrder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;

namespace FurniTour.Server.Services
{
    public class IndividualOrderService : IIndividualOrderService
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IAuthService authService;

        public IndividualOrderService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            IAuthService authService)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.authService = authService;
        }

        public async Task<List<IndividualOrderViewModel>> GetMyIndividualOrdersAsync()
        {
            var user = authService.GetUser();
            if (user == null)
            {
                return null;
            }

            var orders = await context.IndividualOrders
                .Include(o => o.PriceCategory)
                .Include(o => o.IndividualOrderStatus)
                .Include(o => o.Master)
                .Where(o => o.UserId == user.Id)
                .ToListAsync();

            if (!orders.Any())
            {
                return new List<IndividualOrderViewModel>();
            }

            return orders.Select(order => new IndividualOrderViewModel
            {
                Id = order.Id,
                Name = order.Name,
                Address = order.Address,
                Phone = order.Phone ?? string.Empty,
                DateCreated = order.DateCreated,
                Description = order.Description,
                Photo = order.Photo != null ? $"api/individualorder/image/{order.Id}" : string.Empty,
                EstimatedPrice = order.EstimatedPrice,
                FinalPrice = order.FinalPrice,
                MasterNotes = order.MasterNotes ?? string.Empty,
                PriceCategory = order.PriceCategory.Name,
                Status = order.IndividualOrderStatus.Name,
                DateCompleted = order.DateCompleted,
                MasterName = order.Master?.UserName ?? "Не призначено",
                UserName = context.Users.FirstOrDefault(u => u.Id == order.UserId)?.UserName ?? "Невідомо"
            }).ToList();
        }

        public async Task<string> CreateIndividualOrderAsync(IndividualOrderModel model)
        {
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Address) || 
                string.IsNullOrEmpty(model.Phone) || string.IsNullOrEmpty(model.Description))
            {
                return "Всі обов'язкові поля повинні бути заповнені";
            }

            var user = authService.GetUser();
            if (user == null)
            {
                return "Користувач не авторизований";
            }

            var priceCategory = await context.PriceCategories.FindAsync(model.PriceCategoryId);
            if (priceCategory == null)
            {
                return "Вказана цінова категорія не існує";
            }            byte[]? photoBytes = null;
            if (model.Photo != null && model.Photo.Length > 0)
            {
                // Перевіряємо розмір файлу (обмеження 10MB)
                if (model.Photo.Length > 10 * 1024 * 1024) 
                {
                    return "Розмір файлу перевищує допустимий ліміт (10MB)";
                }

                // Перевіряємо тип файлу (дозволяємо зображення та PDF)
                string extension = Path.GetExtension(model.Photo.FileName).ToLower();
                if (!".jpg.jpeg.png.gif.pdf".Contains(extension))
                {
                    return "Недопустимий тип файлу. Дозволені формати: JPG, PNG, GIF, PDF";
                }

                using (var memoryStream = new MemoryStream())
                {
                    await model.Photo.CopyToAsync(memoryStream);
                    photoBytes = memoryStream.ToArray();
                }
            }            // Check if master exists
            if (string.IsNullOrEmpty(model.MasterId))
            {
                return "Необхідно вказати майстра для замовлення";
            }

            var master = await userManager.FindByIdAsync(model.MasterId);
            if (master == null || !await userManager.IsInRoleAsync(master, Constants.Roles.Master))
            {
                return "Вказаний майстер не існує або не має ролі майстра";
            }

            var order = new IndividualOrder
            {
                UserId = user.Id,
                MasterId = model.MasterId,
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                DateCreated = DateTime.Now,
                Description = model.Description,
                Photo = photoBytes,
                PriceCategoryId = model.PriceCategoryId,
                IndividualOrderStatusId = 1 // "Нове індивідуальне замовлення"
            };

            context.IndividualOrders.Add(order);
            await context.SaveChangesAsync();

            return string.Empty;
        }

        public async Task<List<IndividualOrderViewModel>> GetAllIndividualOrdersAsync()
        {
            var check = authService.CheckRoleMasterOrAdmin();
            if (!string.IsNullOrEmpty(check))
            {
                return null;
            }

            var isMaster = authService.IsRole(Roles.Master);
            var userId = authService.GetUser()?.Id;

            var query = context.IndividualOrders
                .Include(o => o.PriceCategory)
                .Include(o => o.IndividualOrderStatus)
                .Include(o => o.Master)
                .Include(o => o.User)
                .AsQueryable();

            // Якщо це майстер, показати тільки замовлення, призначені йому або без призначеного майстра
            if (isMaster && !string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.MasterId == userId || o.MasterId == null);
            }

            var orders = await query.ToListAsync();

            return orders.Select(order => new IndividualOrderViewModel
            {
                Id = order.Id,
                Name = order.Name,
                Address = order.Address,
                Phone = order.Phone ?? string.Empty,
                DateCreated = order.DateCreated,
                Description = order.Description,
                Photo = order.Photo != null ? $"api/individualorder/image/{order.Id}" : string.Empty,
                EstimatedPrice = order.EstimatedPrice,
                FinalPrice = order.FinalPrice,
                MasterNotes = order.MasterNotes ?? string.Empty,
                PriceCategory = order.PriceCategory.Name,
                Status = order.IndividualOrderStatus.Name,
                DateCompleted = order.DateCompleted,
                MasterName = order.Master?.UserName ?? "Не призначено",
                UserName = order.User.UserName ?? "Невідомо"
            }).ToList();
        }

        public async Task<string> ChangeIndividualOrderStatusAsync(int id, int newStatusId)
        {
            var isAuth = authService.IsAuthenticated();
            if (!string.IsNullOrEmpty(isAuth))
            {
                return "Користувач неавторизований";
            }

            var isMaster = authService.IsRole(Roles.Master);
            var isAdmin = authService.IsRole(Roles.Administrator);
            var isUser = authService.IsRole(Roles.User);
            var currentUserId = authService.GetUser()?.Id;

            var order = await context.IndividualOrders
                .Include(o => o.IndividualOrderStatus)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return "Замовлення не знайдено";
            }

            // Перевірка прав доступу
            if (isUser && order.UserId != currentUserId)
            {
                return "Ви не маєте прав для зміни цього замовлення";
            }

            if (isMaster && !isAdmin && order.MasterId != null && order.MasterId != currentUserId)
            {
                return "Ви не призначені майстром для цього замовлення";
            }

            // Перевірка статусу
            var newStatus = await context.IndividualOrderStatuses.FindAsync(newStatusId);
            if (newStatus == null)
            {
                return "Вказаний статус не існує";
            }

            // Перевірка логіки переходів статусів
            var validationMessage = ValidateStatusChange(order.IndividualOrderStatusId, newStatusId, isUser, isAdmin || isMaster);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                return validationMessage;
            }

            // Якщо статус змінюється на "Виконано", записати дату завершення
            if (newStatusId == 7 || newStatusId == 8)
            {
                order.DateCompleted = DateTime.Now;
            }

            order.IndividualOrderStatusId = newStatusId;
            context.IndividualOrders.Update(order);
            await context.SaveChangesAsync();

            return string.Empty;
        }        private string ValidateStatusChange(int currentStatusId, int newStatusId, bool isUser, bool isAdminOrMaster)
        {
            // Логіка для користувачів
            if (isUser)
            {
                // Користувачі можуть лише:
                // 1. Скасувати своє замовлення (статус 2), якщо воно ще не у виробництві
                // 2. Підтвердити доставку (статус 8), якщо замовлення доставлено (статус 7)
                if (newStatusId == 2) // Скасовано замовником
                {
                    if (currentStatusId > 4) // Якщо вже у виробництві або далі
                    {
                        return "Неможливо скасувати замовлення, яке вже у виробництві";
                    }
                    return string.Empty;
                }
                else if (currentStatusId == 7 && newStatusId == 8) // Підтвердження доставки
                {
                    return string.Empty;
                }
                return "Користувач не має прав змінювати цей статус";
            }

            // Логіка для адміністраторів/майстрів
            if (isAdminOrMaster)
            {
                // Скасовані замовлення не можна змінювати
                if (currentStatusId == 2 || currentStatusId == 3)
                {
                    return "Неможливо змінити статус скасованого замовлення";
                }
                
                // Дозволяємо адмінам/майстрам встановлювати будь-який статус, окрім спеціальних випадків
                // Не дозволяємо змінювати статус назад, окрім скасування адміністратором (status 3)
                if (newStatusId < currentStatusId && newStatusId != 3)
                {
                    return "Неможливо повернутися до попереднього статусу";
                }
                
                // Дозволяємо будь-які переходи між статусами для адміністраторів/майстрів
                // Видалено обмеження на перестрибування статусів
            }
            
            return string.Empty;
        }

        public async Task<string> AssignMasterToOrderAsync(int orderId, string masterId)
        {
            var isAdmin = authService.IsRole(Roles.Administrator);
            if (!isAdmin)
            {
                return "Тільки адміністратор може призначати майстрів";
            }

            var order = await context.IndividualOrders.FindAsync(orderId);
            if (order == null)
            {
                return "Замовлення не знайдено";
            }

            // Перевірка, чи існує користувач з роллю майстра
            var master = await userManager.FindByIdAsync(masterId);
            if (master == null || !await userManager.IsInRoleAsync(master, Roles.Master))
            {
                return "Вказаний користувач не є майстром";
            }

            order.MasterId = masterId;
            context.IndividualOrders.Update(order);
            await context.SaveChangesAsync();

            return string.Empty;
        }

        public async Task<string> SetEstimatedPriceAsync(int orderId, decimal price)
        {
            var userId = authService.GetUser()?.Id;
            if (userId == null)
            {
                return "Користувач не авторизований";
            }

            var order = await context.IndividualOrders.FindAsync(orderId);
            if (order == null)
            {
                return "Замовлення не знайдено";
            }

            // Перевірка, чи є користувач майстром цього замовлення або адміністратором
            var isMaster = order.MasterId == userId;
            var isAdmin = authService.IsRole(Roles.Administrator);

            if (!isMaster && !isAdmin)
            {
                return "Ви не маєте прав для встановлення ціни";
            }

            order.EstimatedPrice = price;
            context.IndividualOrders.Update(order);
            await context.SaveChangesAsync();

            return string.Empty;
        }

        public async Task<string> SetFinalPriceAsync(int orderId, decimal price)
        {
            var userId = authService.GetUser()?.Id;
            if (userId == null)
            {
                return "Користувач не авторизований";
            }

            var order = await context.IndividualOrders.FindAsync(orderId);
            if (order == null)
            {
                return "Замовлення не знайдено";
            }

            // Перевірка, чи є користувач майстром цього замовлення або адміністратором
            var isMaster = order.MasterId == userId;
            var isAdmin = authService.IsRole(Roles.Administrator);

            if (!isMaster && !isAdmin)
            {
                return "Ви не маєте прав для встановлення фінальної ціни";
            }

            order.FinalPrice = price;
            context.IndividualOrders.Update(order);
            await context.SaveChangesAsync();

            return string.Empty;
        }

        public async Task<string> AddMasterNotesAsync(int orderId, string notes)
        {
            var userId = authService.GetUser()?.Id;
            if (userId == null)
            {
                return "Користувач не авторизований";
            }

            var order = await context.IndividualOrders.FindAsync(orderId);
            if (order == null)
            {
                return "Замовлення не знайдено";
            }

            // Перевірка, чи є користувач майстром цього замовлення або адміністратором
            var isMaster = order.MasterId == userId;
            var isAdmin = authService.IsRole(Roles.Administrator);

            if (!isMaster && !isAdmin)
            {
                return "Ви не маєте прав для додавання коментаря майстра";
            }

            order.MasterNotes = notes;
            context.IndividualOrders.Update(order);
            await context.SaveChangesAsync();

            return string.Empty;
        }

        public async Task<IndividualOrderViewModel> GetIndividualOrderDetailsAsync(int orderId)
        {
            var userId = authService.GetUser()?.Id;
            if (userId == null)
            {
                return null;
            }

            var order = await context.IndividualOrders
                .Include(o => o.PriceCategory)
                .Include(o => o.IndividualOrderStatus)
                .Include(o => o.Master)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            // Перевірка прав доступу (власник замовлення, майстер замовлення або адміністратор)
            var isOwner = order.UserId == userId;
            var isMaster = order.MasterId == userId;
            var isAdmin = authService.IsRole(Roles.Administrator);

            if (!isOwner && !isMaster && !isAdmin)
            {
                return null;
            }

            return new IndividualOrderViewModel
            {
                Id = order.Id,
                Name = order.Name,
                Address = order.Address,
                Phone = order.Phone ?? string.Empty,
                DateCreated = order.DateCreated,
                Description = order.Description,
                Photo = order.Photo != null ? $"api/individualorder/image/{order.Id}" : string.Empty,
                EstimatedPrice = order.EstimatedPrice,
                FinalPrice = order.FinalPrice,
                MasterNotes = order.MasterNotes ?? string.Empty,
                PriceCategory = order.PriceCategory.Name,
                Status = order.IndividualOrderStatus.Name,
                DateCompleted = order.DateCompleted,                MasterName = order.Master?.UserName ?? "Не призначено",
                UserName = order.User.UserName ?? "Невідомо"
            };
        }

        public async Task<List<Models.IndividualOrder.PriceCategoryViewModel>> GetPriceCategoriesAsync()
        {
            var categories = await context.PriceCategories.ToListAsync();
            
            return categories.Select(c => new Models.IndividualOrder.PriceCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description ?? string.Empty
            }).ToList();
        }
    }
}
