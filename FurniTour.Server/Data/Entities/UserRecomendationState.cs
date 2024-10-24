namespace FurniTour.Server.Data.Entities
{
    public class UserRecomendationState
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Id користувача
        public DateTime LastRecommendationCheck { get; set; } // Час останньої перевірки рекомендацій

    }
}
