using FurniTour.Server.Models.Item;

namespace FurniTour.Server.Interfaces
{
    public interface IItemFurnitureService
    {
        public List<ItemViewModel> getAll();
        public Task<string> AddItem(ItemModel itemModel);
        public ItemViewModel Details(int id);
        public Task<string> Edit(int id, ItemViewModel itemModel);
        public Task<string> DeleteItem(int id);
    }
}
