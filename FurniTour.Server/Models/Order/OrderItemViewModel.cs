using System.Reflection.Metadata;

namespace FurniTour.Server.Models.Order
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Master { get; set; }
        public string? Manufacturer { get; set; }
        public string? Description { get; set; }
        public string Color { get; set; }
        public string Category { get; set; }
        //public byte[]? Photo { get; set; }
    }
}
