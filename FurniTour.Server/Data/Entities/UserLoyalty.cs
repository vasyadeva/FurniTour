using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class UserLoyalty
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        
        [Precision(18, 2)]
        public decimal TotalSpent { get; set; }
        
        public int LoyaltyLevel { get; set; } // 0 = no level, 1 = bronze, 2 = silver, 3 = gold
        
        [Precision(18, 2)]
        public decimal CurrentDiscount { get; set; } // 0.03 = 3%, 0.07 = 7%, 0.15 = 15%
        
        public DateTime LastUpdated { get; set; }
    }
}
