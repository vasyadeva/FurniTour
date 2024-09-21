using FurniTour.Server.Models.Item;

namespace FurniTour.Server.Interfaces
{
    public interface IItemFurnitureService
    {
        public List<ItemViewModel> getAll();
        public bool AddItem(ItemModel itemModel);
        public ItemViewModel Details(int id);
        public bool Edit(int id, ItemViewModel itemModel);
        public bool DeleteItem(int id);
    }
}
