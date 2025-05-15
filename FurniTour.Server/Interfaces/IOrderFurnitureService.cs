using FurniTour.Server.Models.Order;
using FurniTour.Server.Models.Order.AI;

namespace FurniTour.Server.Interfaces
{
    public interface IOrderFurnitureService
    {
        public Task<string> Order(OrderModel order);
        public List<OrderViewModel> MyOrders();
        public List<OrderViewModel> AdminOrders();
        public Task<string> ChangeOrderStateAsync(int id, int state);
        Task<List<OrderViewModel>> MyOrdersAI(string userID);
        Task<string> OrderAI(OrderAIModel model);
    }
}
