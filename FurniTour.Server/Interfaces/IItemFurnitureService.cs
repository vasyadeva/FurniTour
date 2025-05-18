using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Item;

namespace FurniTour.Server.Interfaces
{
    public interface IItemFurnitureService
    {
        public List<ItemViewModel> getAll();
        List<ItemViewModel> getFilteredItems(ItemFilterModel model);
        public List<Category> GetCategories();
        public List<Color> GetColors();
        public Task<string> AddItem(ItemModel itemModel);
        public ItemViewModel Details(int id);
        public Task<string> Edit(int id, ItemUpdateModel itemModel);
        public Task<string> DeleteItem(int id);
        public Task<ItemViewModel> GetItemsByDescriptionAsync(string description);
        Task<List<ItemViewModel>> GetItemsByDescriptionAsync2(string description, int category,
    decimal minprice, decimal maxprice, int color);
        byte[] GetImage(int id);
          // New methods for reviews and additional photos
        Task<List<FurnitureReviewModel>> GetFurnitureReviews(int itemId);
        Task<List<FurnitureReviewModel>> GetItemReviews(int itemId);  // Alias for backward compatibility
        Task<string> AddItemReview(AddFurnitureReviewModel reviewModel);
        Task<string> GetReviewsSummary(int itemId);
        byte[] GetAdditionalImage(int photoId);
        Task<int> GetAdditionalPhotoCount(int itemId);
    }
}
