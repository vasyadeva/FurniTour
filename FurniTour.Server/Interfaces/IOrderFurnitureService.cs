using FurniTour.Server.Models.Order;

namespace FurniTour.Server.Interfaces
{
    public interface IOrderFurnitureService
    {
        public Task Order(OrderModel order);
        public List<OrderViewModel> MyOrders();
        public List<OrderViewModel> AdminOrders();
        public Task<bool> ChangeOrderStateAsync(int id, int state);
    }
}
