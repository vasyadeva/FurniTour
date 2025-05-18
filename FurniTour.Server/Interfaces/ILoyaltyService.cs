using FurniTour.Server.Models.Loyalty;

namespace FurniTour.Server.Interfaces
{
    public interface ILoyaltyService
    {
        /// <summary>
        /// Get user's loyalty information
        /// </summary>
        Task<LoyaltyViewModel> GetUserLoyaltyAsync(string userId);
        
        /// <summary>
        /// Get user's current discount as a decimal (0.03 = 3%)
        /// </summary>
        Task<decimal> GetUserDiscountAsync(string userId);
        
        /// <summary>
        /// Update user's total spent amount when a new order is placed
        /// </summary>
        Task UpdateUserSpendingAsync(string userId, decimal amount);
        
        /// <summary>
        /// Calculate the applicable discount for a given order total
        /// </summary>
        decimal CalculateDiscountedTotal(decimal orderTotal, decimal discountRate);
    }
}
