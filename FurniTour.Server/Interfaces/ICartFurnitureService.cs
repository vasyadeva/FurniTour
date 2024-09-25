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

    }
}
