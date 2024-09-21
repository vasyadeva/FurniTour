using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Cart;

namespace FurniTour.Server.Interfaces
{
    public interface ICartFurnitureService
    {
        public Task AddToCartAsync(int furnitureId, int quantity);
        public Task RemoveFromCartAsync(int id);
        public Task UpdateCartAsync(int id, int quantity);
        
        public List<CartItemViewModel> GetCartFurniture();

    }
}
