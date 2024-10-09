namespace FurniTour.Server.Models.Item
{
    public class ItemUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int? ManufacturerId { get; set; }
    }
}
