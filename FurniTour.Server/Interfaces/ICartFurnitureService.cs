using FurniTour.Server.Models;

namespace FurniTour.Server.Interfaces
{
    public interface ICartFurnitureService
    {
        public Task AddToCartAsync(int furnitureId, int quantity);
        public Task RemoveFromCartAsync(int id);
        public Task UpdateCartAsync(int id, int quantity);
        
        public Task<IEnumerable<CartItemViewModel>> GetCartFurnitureAsync();

    }
}
