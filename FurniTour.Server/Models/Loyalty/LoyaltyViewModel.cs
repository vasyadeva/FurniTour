namespace FurniTour.Server.Models.Loyalty
{
    public class LoyaltyViewModel
    {
        public int LoyaltyLevel { get; set; }
        public string LoyaltyName { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal NextLevelThreshold { get; set; }
        public decimal AmountToNextLevel { get; set; }
    }
}
