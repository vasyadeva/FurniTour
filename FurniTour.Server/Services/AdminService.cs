using FurniTour.Server.Constants;
using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region User Management

        public async Task<List<UserAdminModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserAdminModel>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                
                result.Add(new UserAdminModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }
            
            return result;
        }

        public async Task<UserAdminModel> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;
                
            var roles = await _userManager.GetRolesAsync(user);
            
            return new UserAdminModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault() ?? "User"
            };
        }

        public async Task<string> UpdateUserRoleAsync(string userId, string roleId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "Користувача не знайдено";
                
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
            }
            
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return "Роль не знайдено";
                
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded)
                return string.Empty;
                
            return "Помилка при зміні ролі користувача: " + string.Join(", ", result.Errors.Select(e => e.Description));
        }

        public async Task<bool> CanDeleteUserAsync(string userId)
        {
            // Check if user has orders
            var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == userId);
            if (hasOrders)
                return false;
                
            // Check if user is a master with furniture
            var isMasterWithFurniture = await _context.Furnitures.AnyAsync(f => f.MasterId == userId);
            if (isMasterWithFurniture)
                return false;
                
            // Check if user has reviews
            var hasReviews = await _context.FurnitureReviews.AnyAsync(r => r.UserId == userId) ||
                           await _context.MasterReviews.AnyAsync(r => r.UserId == userId) ||
                           await _context.ManufacturerReviews.AnyAsync(r => r.UserId == userId);
            if (hasReviews)
                return false;
                
            // Check if user has individual orders
            var hasIndividualOrders = await _context.IndividualOrders.AnyAsync(o => o.UserId == userId);
            if (hasIndividualOrders)
                return false;
                
            return true;
        }

        public async Task<string> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return "Користувача не знайдено";
                
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return string.Empty;
                
            return "Помилка при видаленні користувача: " + string.Join(", ", result.Errors.Select(e => e.Description));
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        #endregion

        #region Color Management

        public async Task<List<Color>> GetAllColorsAsync()
        {
            return await _context.Colors.ToListAsync();
        }

        public async Task<Color> GetColorByIdAsync(int id)
        {
            return await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<string> CreateColorAsync(ColorCreateModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return "Назва кольору не може бути порожньою";
                
            var exists = await _context.Colors.AnyAsync(c => c.Name.ToLower() == model.Name.ToLower());
            if (exists)
                return "Колір з такою назвою вже існує";
                
            await _context.Colors.AddAsync(new Color { Name = model.Name });
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> UpdateColorAsync(ColorUpdateModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return "Назва кольору не може бути порожньою";
                
            var color = await _context.Colors.FindAsync(model.Id);
            if (color == null)
                return "Колір не знайдено";
                
            var exists = await _context.Colors.AnyAsync(c => c.Name.ToLower() == model.Name.ToLower() && c.Id != model.Id);
            if (exists)
                return "Колір з такою назвою вже існує";
                
            color.Name = model.Name;
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<bool> IsColorInUseAsync(int colorId)
        {
            return await _context.Furnitures.AnyAsync(f => f.ColorId == colorId);
        }

        public async Task<string> DeleteColorAsync(int colorId)
        {
            var color = await _context.Colors.FindAsync(colorId);
            if (color == null)
                return "Колір не знайдено";
                
            if (await IsColorInUseAsync(colorId))
                return "Неможливо видалити колір, оскільки він використовується в товарах";
                
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        #endregion

        #region Category Management

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<string> CreateCategoryAsync(CategoryCreateModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return "Назва категорії не може бути порожньою";
                
            var exists = await _context.Categories.AnyAsync(c => c.Name.ToLower() == model.Name.ToLower());
            if (exists)
                return "Категорія з такою назвою вже існує";
                
            await _context.Categories.AddAsync(new Category { Name = model.Name });
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> UpdateCategoryAsync(CategoryUpdateModel model)
        {
            if (model.Name.IsNullOrEmpty())
                return "Назва категорії не може бути порожньою";
                
            var category = await _context.Categories.FindAsync(model.Id);
            if (category == null)
                return "Категорію не знайдено";
                
            var exists = await _context.Categories.AnyAsync(c => c.Name.ToLower() == model.Name.ToLower() && c.Id != model.Id);
            if (exists)
                return "Категорія з такою назвою вже існує";
                
            category.Name = model.Name;
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<bool> IsCategoryInUseAsync(int categoryId)
        {
            return await _context.Furnitures.AnyAsync(f => f.CategoryId == categoryId);
        }

        public async Task<string> DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                return "Категорію не знайдено";
                
            if (await IsCategoryInUseAsync(categoryId))
                return "Неможливо видалити категорію, оскільки вона використовується в товарах";
                
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return string.Empty;
        }       
        #endregion

        #region Manufacturer Management

        public async Task<List<Manufacturer>> GetAllManufacturersAsync()
        {
            return await _context.Manufacturers.ToListAsync();
        }

        public async Task<Manufacturer> GetManufacturerByIdAsync(int id)
        {
            return await _context.Manufacturers.FindAsync(id);
        }

        public async Task<string> CreateManufacturerAsync(ManufacturerCreateModel model)
        {
            // Check if manufacturer with this name already exists
            var existingManufacturer = await _context.Manufacturers
                .FirstOrDefaultAsync(m => m.Name.ToLower() == model.Name.ToLower());
                
            if (existingManufacturer != null)
                return "Виробник з такою назвою вже існує";

            var manufacturer = new Manufacturer
            {
                Name = model.Name
            };

            _context.Manufacturers.Add(manufacturer);
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> UpdateManufacturerAsync(ManufacturerUpdateModel model)
        {
            var manufacturer = await _context.Manufacturers.FindAsync(model.Id);
            if (manufacturer == null)
                return "Виробника не знайдено";

            // Check if another manufacturer with this name already exists
            var existingManufacturer = await _context.Manufacturers
                .FirstOrDefaultAsync(m => m.Name.ToLower() == model.Name.ToLower() && m.Id != model.Id);
                
            if (existingManufacturer != null)
                return "Виробник з такою назвою вже існує";

            manufacturer.Name = model.Name;
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<bool> IsManufacturerInUseAsync(int manufacturerId)
        {
            // Check if manufacturer is used in furniture
            return await _context.Furnitures.AnyAsync(f => f.ManufacturerId == manufacturerId);
        }

        public async Task<string> DeleteManufacturerAsync(int manufacturerId)
        {
            var manufacturer = await _context.Manufacturers.FindAsync(manufacturerId);
            if (manufacturer == null)
                return "Виробника не знайдено";
                
            if (await IsManufacturerInUseAsync(manufacturerId))
                return "Неможливо видалити виробника, оскільки він використовується в товарах";
                
            _context.Manufacturers.Remove(manufacturer);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        
        #endregion
    }
}
