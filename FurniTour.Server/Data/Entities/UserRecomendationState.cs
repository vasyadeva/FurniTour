using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class UserRecomendationState
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Id користувача
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } // Користувач, якому належить стан рекомендацій
        public DateTime LastRecommendationCheck { get; set; } // Час останньої перевірки рекомендацій

    }
}
