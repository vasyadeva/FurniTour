using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Cart;

namespace FurniTour.Server.Interfaces
{
    public interface ICartFurnitureService
    {
        public Task<string> AddToCartAsync(int furnitureId, int quantity);
        public Task<string> RemoveFromCartAsync(int id);
        public Task<string> UpdateCartAsync(int id, int quantity);
        
        public List<CartItemViewModel> GetCartFurniture();

        #region AIMethods
        Task<string> AddToCartCopilot(int furnitureId, int quantity, string userID);
        Task<string> RemoveFromCartCopilot(int id, string userID);
        Task<string> UpdateCartCopilot(int id, int quantity, string userID);
        Task<List<CartItemViewModel>> GetCartFurnitureCopilot(string userID);
        #endregion

    }
}
