using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Admin;
using Microsoft.AspNetCore.Identity;

namespace FurniTour.Server.Interfaces
{
    public interface IAdminService
    {
        // Users management
        Task<List<UserAdminModel>> GetAllUsersAsync();
        Task<UserAdminModel> GetUserByIdAsync(string id);
        Task<string> UpdateUserRoleAsync(string userId, string roleId);
        Task<bool> CanDeleteUserAsync(string userId);
        Task<string> DeleteUserAsync(string userId);
        Task<List<IdentityRole>> GetAllRolesAsync();
        
        // Colors management
        Task<List<Color>> GetAllColorsAsync();
        Task<Color> GetColorByIdAsync(int id);
        Task<string> CreateColorAsync(ColorCreateModel model);
        Task<string> UpdateColorAsync(ColorUpdateModel model);
        Task<bool> IsColorInUseAsync(int colorId);
        Task<string> DeleteColorAsync(int colorId);
          // Categories management
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<string> CreateCategoryAsync(CategoryCreateModel model);
        Task<string> UpdateCategoryAsync(CategoryUpdateModel model);
        Task<bool> IsCategoryInUseAsync(int categoryId);
        Task<string> DeleteCategoryAsync(int categoryId);
          // Manufacturers management
        Task<List<Manufacturer>> GetAllManufacturersAsync();
        Task<Manufacturer> GetManufacturerByIdAsync(int id);
        Task<string> CreateManufacturerAsync(ManufacturerCreateModel model);
        Task<string> UpdateManufacturerAsync(ManufacturerUpdateModel model);
        Task<bool> IsManufacturerInUseAsync(int manufacturerId);
        Task<string> DeleteManufacturerAsync(int manufacturerId);
    }
}
