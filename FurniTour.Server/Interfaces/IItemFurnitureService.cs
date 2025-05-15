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
    }
}
