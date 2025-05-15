using FurniTour.Server.Models.Order;
using FurniTour.Server.Models.IndividualOrder;

namespace FurniTour.Server.Interfaces
{
    public interface IIndividualOrderService
    {
        /// <summary>
        /// Отримати список індивідуальних замовлень поточного користувача
        /// </summary>
        Task<List<IndividualOrderViewModel>> GetMyIndividualOrdersAsync();
        
        /// <summary>
        /// Створити нове індивідуальне замовлення
        /// </summary>
        Task<string> CreateIndividualOrderAsync(IndividualOrderModel order);
        
        /// <summary>
        /// Отримати список всіх індивідуальних замовлень (для адміністратора або майстра)
        /// </summary>
        Task<List<IndividualOrderViewModel>> GetAllIndividualOrdersAsync();
        
        /// <summary>
        /// Змінити статус індивідуального замовлення
        /// </summary>
        Task<string> ChangeIndividualOrderStatusAsync(int id, int newStatusId);
        
        /// <summary>
        /// Призначити майстра для індивідуального замовлення
        /// </summary>
        Task<string> AssignMasterToOrderAsync(int orderId, string masterId);
        
        /// <summary>
        /// Встановити оцінку вартості для індивідуального замовлення
        /// </summary>
        Task<string> SetEstimatedPriceAsync(int orderId, decimal price);
        
        /// <summary>
        /// Встановити фінальну вартість для індивідуального замовлення
        /// </summary>
        Task<string> SetFinalPriceAsync(int orderId, decimal price);
        
        /// <summary>
        /// Додати коментар майстра до індивідуального замовлення
        /// </summary>
        Task<string> AddMasterNotesAsync(int orderId, string notes);
          /// <summary>
        /// Отримати деталі конкретного індивідуального замовлення
        /// </summary>
        Task<IndividualOrderViewModel> GetIndividualOrderDetailsAsync(int orderId);
        
        /// <summary>
        /// Отримати список всіх цінових категорій для індивідуальних замовлень
        /// </summary>
        Task<List<Models.IndividualOrder.PriceCategoryViewModel>> GetPriceCategoriesAsync();
    }
}
