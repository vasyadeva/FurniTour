using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class CachedRecommendation
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Id користувача
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } // Користувач, якому належить кеш рекомендацій
        public DateTime CachedTime { get; set; } // Час кешування
        public List<int> RecommendedFurnitureIds { get; set; } // Список рекомендованих меблів

    }
}
