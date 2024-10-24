namespace FurniTour.Server.Data.Entities
{
    public class CachedRecommendation
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Id користувача
        public DateTime CachedTime { get; set; } // Час кешування
        public List<int> RecommendedFurnitureIds { get; set; } // Список рекомендованих меблів

    }
}
