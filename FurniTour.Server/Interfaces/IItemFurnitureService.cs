using FurniTour.Server.Models;

namespace FurniTour.Server.Interfaces
{
    public interface IItemFurnitureService
    {
        public List<ItemModel> getAll();
        public bool AddItem(ItemModel itemModel);
        public ItemModel Details(int id);
        public bool Edit(int id, ItemModel itemModel);
        public bool DeleteItem(int id);
    }
}
