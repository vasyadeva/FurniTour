using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Guarantee;
using FurniTour.Server.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurniTour.Server.Services
{
    public class GuaranteeService : IGuaranteeService
    {
        private readonly ApplicationDbContext context;
        private readonly IAuthService authService;
        private readonly ILogger<GuaranteeService> logger;
        private readonly INotificationService _notificationService;

        public GuaranteeService(ApplicationDbContext context, IAuthService authService, ILogger<GuaranteeService> logger, INotificationService notificationService)
        {
            this.context = context;
            this.authService = authService;
            this.logger = logger;
            _notificationService = notificationService;
        }

        public async Task<string> AddGuarantee(GuaranteeAddModel guarantee)
        {
            if (guarantee == null)
            {
                return "Запит не може бути порожнім";
            }
            
            var user = authService.GetUser();
            if (user == null)
            {
                return "Користувача не знайдено";
            }
            
            // Create guarantee entity with required fields
            var guaranteeEntity = new Guarantee
            {
                UserId = user.Id,
                User = user,  // Required field
                Status = GuaranteeStatusConst.Pending,
                Comment = guarantee.Comment,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                GuaranteeItems = new List<GuaranteeItems>(),
                GuaranteePhotos = new List<GuaranteePhoto>()
            };

            if (guarantee.IsIndividualOrder)
            {
                // Handle individual order
                if (!guarantee.IndividualOrderId.HasValue)
                {
                    return "Ідентифікатор індивідуального замовлення обов'язковий";
                }

                var individualOrder = context.IndividualOrders.Find(guarantee.IndividualOrderId);
                if (individualOrder == null)
                {
                    return "Індивідуальне замовлення не знайдено";
                }

                // Check if order belongs to the user
                if (individualOrder.UserId != user.Id)
                {
                    return "Індивідуальне замовлення не належить поточному користувачу";
                }

                // Set the individual order ID
                guaranteeEntity.IndividualOrderId = guarantee.IndividualOrderId;
                
                // Add the guarantee to the database first to get its ID
                context.Guarantees.Add(guaranteeEntity);
                context.SaveChanges();
            }
            else
            {
                // Handle regular order
                if (!guarantee.OrderId.HasValue)
                {
                    return "Ідентифікатор замовлення обов'язковий";
                }

                var order = context.Orders.Find(guarantee.OrderId);
                if (order == null)
                {
                    return "Замовлення не знайдено";
                }

                // Check if order belongs to the user
                if (order.UserId != user.Id)
                {
                    return "Замовлення не належить поточному користувачу";
                }

                if (!IsGuaranteeValid(guarantee.OrderId.Value))
                {
                    return "Час для гарантійного обслуговування минув";
                }

                // Set the order ID
                guaranteeEntity.OrderId = guarantee.OrderId;

                // Process order items
                if (guarantee.Items == null || guarantee.Items.Count == 0)
                {
                    return "Не вибрано жодного товару для гарантії";
                }

                // Filter out null or invalid items and check if any remain
                var validItems = guarantee.Items.Where(i => i > 0).ToList();
                if (validItems.Count == 0)
                {
                    return "Не вибрано жодного дійсного товару для гарантії";
                }

                // Add the guarantee to the database first to get its ID
                context.Guarantees.Add(guaranteeEntity);
                context.SaveChanges();

                // Process each valid item
                int successCount = 0;
                foreach (var itemId in validItems)
                {
                    // Explicitly log the orderItem lookup for debugging
                    logger.LogInformation($"Searching for OrderItem with ID: {itemId}");
                    
                    var orderItem = context.OrderItems.Find(itemId);
                    if (orderItem != null)
                    {
                        logger.LogInformation($"Found OrderItem: {orderItem.Id}, FurnitureId: {orderItem.FurnitureId}");
                        
                        var guaranteeItem = new GuaranteeItems
                        {
                            GuaranteeId = guaranteeEntity.Id,
                            OrderItemId = orderItem.Id
                        };
                        context.GuaranteeItems.Add(guaranteeItem);
                        successCount++;
                    }
                    else
                    {
                        logger.LogWarning($"OrderItem with ID {itemId} not found");
                    }
                }
                
                if (successCount == 0)
                {
                    return "Не знайдено жодного дійсного товару з вказаними ідентифікаторами";
                }
            }
            
            // Process photos (common for both order types)
            if (guarantee.Photos != null && guarantee.Photos.Any())
            {
                foreach (var photo in guarantee.Photos)
                {
                    var photoEntity = new GuaranteePhoto
                    {
                        GuaranteeId = guaranteeEntity.Id,
                        Photo = Convert.FromBase64String(photo)
                    };
                    context.GuaranteePhotos.Add(photoEntity);
                }
            }
            
            context.SaveChanges();
              // Відправка сповіщення адміністраторам про нову гарантійну заявку
            await _notificationService.NotifyNewGuaranteeAsync(guaranteeEntity.Id);
            
            return string.Empty;
        }

        public async Task<GuaranteeModel> GetGuarantee(int guaranteeId)
        {
            var guarantee = await context.Guarantees
                .Include(c => c.GuaranteePhotos)
                .Include(g => g.GuaranteeItems)
                .ThenInclude(gi => gi.OrderItem)
                .ThenInclude(oi => oi.Furniture)
                .Include(g => g.IndividualOrder)
                .FirstOrDefaultAsync(g => g.Id == guaranteeId);

            if (guarantee == null)
            {
                return null;
            }

            var user = guarantee.UserId != null ? await authService.GetUserById(guarantee.UserId) : null;
            var model = new GuaranteeModel
            {
                Id = guarantee.Id,
                UserName = user == null ? string.Empty : user.UserName ?? string.Empty,
                OrderId = guarantee.OrderId,
                IndividualOrderId = guarantee.IndividualOrderId,
                IsIndividualOrder = guarantee.IndividualOrderId.HasValue,
                Status = guarantee.Status,
                Comment = guarantee.Comment,
                DateCreated = guarantee.DateCreated,
                DateModified = guarantee.DateModified,
                Items = new List<GuaranteeItemModel>(),
                Photos = new List<string>()
            };

            if (guarantee.GuaranteeItems != null)
            {
                foreach (var item in guarantee.GuaranteeItems)
                {
                    if (item.OrderItem != null)
                    {
                        model.Items.Add(new GuaranteeItemModel
                        {
                            Id = item.Id,
                            FurnitureId = item.OrderItem.FurnitureId,
                            FurnitureName = item.OrderItem.Furniture?.Name ?? "Unknown Furniture",
                            Quantity = item.OrderItem.Quantity
                        });
                    }
                }
            }

            if (guarantee.GuaranteePhotos != null)
            {
                foreach (var photo in guarantee.GuaranteePhotos)
                {
                    model.Photos.Add(Convert.ToBase64String(photo.Photo));
                }
            }
            
            return model;
        }

        public async Task<List<GuaranteeModel>> GetGuarantees()
        {
            var guarantees = await context.Guarantees
                .Include(c => c.GuaranteePhotos)
                .Include(g => g.GuaranteeItems)
                .ThenInclude(gi => gi.OrderItem)
                .ThenInclude(oi => oi.Furniture)
                .Include(g => g.IndividualOrder)
                .ToListAsync();

            var models = new List<GuaranteeModel>();
            
            foreach (var guarantee in guarantees)
            {
                var user = guarantee.UserId != null ? await authService.GetUserById(guarantee.UserId) : null;
                var model = new GuaranteeModel
                {
                    Id = guarantee.Id,
                    UserName = user == null ? string.Empty : user.UserName ?? string.Empty,
                    OrderId = guarantee.OrderId,
                    IndividualOrderId = guarantee.IndividualOrderId,
                    IsIndividualOrder = guarantee.IndividualOrderId.HasValue,
                    Status = guarantee.Status,
                    Comment = guarantee.Comment,
                    DateCreated = guarantee.DateCreated,
                    DateModified = guarantee.DateModified,
                    Items = new List<GuaranteeItemModel>(),
                    Photos = new List<string>()
                };
                
                foreach (var item in guarantee.GuaranteeItems)
                {
                    if (item.OrderItem != null)
                    {
                        model.Items.Add(new GuaranteeItemModel
                        {
                            Id = item.Id,
                            FurnitureId = item.OrderItem.FurnitureId,
                            FurnitureName = item.OrderItem.Furniture?.Name ?? "Unknown Furniture",
                            Quantity = item.OrderItem.Quantity
                        });
                    }
                }
                
                foreach (var photo in guarantee.GuaranteePhotos)
                {
                    model.Photos.Add(Convert.ToBase64String(photo.Photo));
                }
                
                models.Add(model);
            }
            
            return models;
        }

        public List<GuaranteeModel> GetMyGuarantees()
        {
            var user = authService.GetUser();
            if (user == null)
            {
                return new List<GuaranteeModel>(); // Return empty list instead of null
            }
            
            var guarantees = context.Guarantees.Where(g => g.UserId == user.Id)
                .Include(c => c.GuaranteePhotos)
                .Include(g => g.GuaranteeItems)
                .ThenInclude(gi => gi.OrderItem)
                .ThenInclude(oi => oi.Furniture)
                .Include(g => g.IndividualOrder)
                .ToList();
                
            var models = new List<GuaranteeModel>();
            
            foreach (var guarantee in guarantees)
            {
                var model = new GuaranteeModel
                {
                    Id = guarantee.Id,
                    UserName = guarantee.User?.UserName ?? string.Empty,
                    OrderId = guarantee.OrderId,
                    IndividualOrderId = guarantee.IndividualOrderId,
                    IsIndividualOrder = guarantee.IndividualOrderId.HasValue,
                    Status = guarantee.Status,
                    Comment = guarantee.Comment,
                    DateCreated = guarantee.DateCreated,
                    DateModified = guarantee.DateModified,
                    Items = new List<GuaranteeItemModel>(),
                    Photos = new List<string>()
                };
                
                foreach (var item in guarantee.GuaranteeItems)
                {
                    if (item.OrderItem != null)
                    {
                        model.Items.Add(new GuaranteeItemModel
                        {
                            Id = item.Id,
                            FurnitureId = item.OrderItem.FurnitureId,
                            FurnitureName = item.OrderItem.Furniture?.Name ?? "Unknown Furniture",
                            Quantity = item.OrderItem.Quantity
                        });
                    }
                }
                
                foreach (var photo in guarantee.GuaranteePhotos)
                {
                    model.Photos.Add(Convert.ToBase64String(photo.Photo));
                }
                
                models.Add(model);
            }
            
            return models;
        }

        // Fixed method to check order date instead of guarantee date
        public bool IsGuaranteeValid(int orderId)
        {
            // Find the order with the given ID
            var order = context.Orders.Find(orderId);
            
            if (order == null)
            {
                logger.LogWarning($"Order with ID {orderId} not found when checking guarantee validity");
                return false;
            }
            
            // Check if the order is recent enough for guarantee service
            // Orders should be less than 365 days old to be eligible for guarantee
            var daysFromOrder = (DateTime.Now - order.DateCreated).TotalDays;
            
            logger.LogInformation($"Order {orderId} was created {daysFromOrder} days ago");
            
            // Orders should be less than 365 days old
            var isValid = daysFromOrder <= 365;
            
            if (!isValid)
            {
                logger.LogWarning($"Order {orderId} is too old for guarantee service (created {daysFromOrder} days ago)");
            }
            
            return isValid;
        }

        public void UpdateGuarantee(int guaranteeId, string status)
        {
            var guar = context.Guarantees.Find(guaranteeId);
            if (guar != null)
            {
                guar.Status = status;
                guar.DateModified = DateTime.Now;
                context.Guarantees.Update(guar);
                context.SaveChanges();
                
                // Відправка сповіщення про зміну статусу гарантійної заявки
                _notificationService.NotifyGuaranteeStatusChangedAsync(guaranteeId, status).Wait();
            }
        }
    }
}
