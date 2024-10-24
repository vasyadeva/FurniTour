using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Item;

namespace FurniTour.Server.Interfaces
{
    public interface IRecomendationService
    {
        public Task<List<ItemViewModel>> GetRecommendationsAsync();
    }
}
