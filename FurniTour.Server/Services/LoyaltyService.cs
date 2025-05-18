using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Loyalty;
using Microsoft.EntityFrameworkCore;

namespace FurniTour.Server.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        
        // Loyalty thresholds
        private const decimal BRONZE_THRESHOLD = 50000M;
        private const decimal SILVER_THRESHOLD = 100000M;
        private const decimal GOLD_THRESHOLD = 150000M;
        
        // Discount rates
        private const decimal BRONZE_DISCOUNT = 0.03M; // 3%
        private const decimal SILVER_DISCOUNT = 0.07M; // 7% 
        private const decimal GOLD_DISCOUNT = 0.15M; // 15%

        public LoyaltyService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<LoyaltyViewModel> GetUserLoyaltyAsync(string userId)
        {
            // Ensure user loyalty record exists
            await EnsureUserLoyaltyExistsAsync(userId);
            
            // Get the user's loyalty record
            var userLoyalty = await _context.UserLoyalties
                .FirstOrDefaultAsync(ul => ul.UserId == userId);
            
            if (userLoyalty == null)
            {
                return new LoyaltyViewModel
                {
                    LoyaltyLevel = 0,
                    LoyaltyName = "Стандарт",
                    DiscountPercent = 0,
                    TotalSpent = 0,
                    NextLevelThreshold = BRONZE_THRESHOLD,
                    AmountToNextLevel = BRONZE_THRESHOLD
                };
            }
            
            // Calculate next level threshold and amount needed
            decimal nextLevelThreshold = 0;
            decimal amountToNextLevel = 0;
            string loyaltyName = "Стандарт";
            
            if (userLoyalty.TotalSpent < BRONZE_THRESHOLD)
            {
                nextLevelThreshold = BRONZE_THRESHOLD;
                amountToNextLevel = BRONZE_THRESHOLD - userLoyalty.TotalSpent;
                loyaltyName = "Стандарт";
            }
            else if (userLoyalty.TotalSpent < SILVER_THRESHOLD)
            {
                nextLevelThreshold = SILVER_THRESHOLD;
                amountToNextLevel = SILVER_THRESHOLD - userLoyalty.TotalSpent;
                loyaltyName = "Бронза";
            }
            else if (userLoyalty.TotalSpent < GOLD_THRESHOLD)
            {
                nextLevelThreshold = GOLD_THRESHOLD;
                amountToNextLevel = GOLD_THRESHOLD - userLoyalty.TotalSpent;
                loyaltyName = "Срібло";
            }
            else
            {
                nextLevelThreshold = 0; // No next level
                amountToNextLevel = 0;
                loyaltyName = "Золото";
            }
            
            return new LoyaltyViewModel
            {
                LoyaltyLevel = userLoyalty.LoyaltyLevel,
                LoyaltyName = loyaltyName,
                DiscountPercent = userLoyalty.CurrentDiscount * 100, // Convert to percentage
                TotalSpent = userLoyalty.TotalSpent,
                NextLevelThreshold = nextLevelThreshold,
                AmountToNextLevel = amountToNextLevel
            };
        }

        public async Task<decimal> GetUserDiscountAsync(string userId)
        {
            // Ensure user loyalty record exists
            await EnsureUserLoyaltyExistsAsync(userId);
            
            // Get user's current discount
            var userLoyalty = await _context.UserLoyalties
                .FirstOrDefaultAsync(ul => ul.UserId == userId);
                
            return userLoyalty?.CurrentDiscount ?? 0;
        }

        public async Task UpdateUserSpendingAsync(string userId, decimal amount)
        {
            // Ensure user loyalty record exists
            await EnsureUserLoyaltyExistsAsync(userId);
            
            // Get the user's loyalty record
            var userLoyalty = await _context.UserLoyalties
                .FirstOrDefaultAsync(ul => ul.UserId == userId);
                
            if (userLoyalty != null)
            {
                userLoyalty.TotalSpent += amount;
                userLoyalty.LastUpdated = DateTime.Now;
                
                // Update loyalty level and discount based on total spent
                UpdateLoyaltyLevelAndDiscount(userLoyalty);
                
                await _context.SaveChangesAsync();
            }
        }

        public decimal CalculateDiscountedTotal(decimal orderTotal, decimal discountRate)
        {
            return orderTotal * (1 - discountRate);
        }
        
        private void UpdateLoyaltyLevelAndDiscount(UserLoyalty userLoyalty)
        {
            if (userLoyalty.TotalSpent >= GOLD_THRESHOLD)
            {
                userLoyalty.LoyaltyLevel = 3; // Gold
                userLoyalty.CurrentDiscount = GOLD_DISCOUNT;
            }
            else if (userLoyalty.TotalSpent >= SILVER_THRESHOLD)
            {
                userLoyalty.LoyaltyLevel = 2; // Silver
                userLoyalty.CurrentDiscount = SILVER_DISCOUNT;
            }
            else if (userLoyalty.TotalSpent >= BRONZE_THRESHOLD)
            {
                userLoyalty.LoyaltyLevel = 1; // Bronze
                userLoyalty.CurrentDiscount = BRONZE_DISCOUNT;
            }
            else
            {
                userLoyalty.LoyaltyLevel = 0; // No loyalty level
                userLoyalty.CurrentDiscount = 0;
            }
        }
        
        private async Task EnsureUserLoyaltyExistsAsync(string userId)
        {
            // Check if user loyalty record exists
            var userLoyalty = await _context.UserLoyalties
                .FirstOrDefaultAsync(ul => ul.UserId == userId);
                
            // If not, create a new record
            if (userLoyalty == null)
            {
                userLoyalty = new UserLoyalty
                {
                    UserId = userId,
                    TotalSpent = 0,
                    LoyaltyLevel = 0,
                    CurrentDiscount = 0,
                    LastUpdated = DateTime.Now
                };
                
                await _context.UserLoyalties.AddAsync(userLoyalty);
                await _context.SaveChangesAsync();
            }
        }
    }
}
