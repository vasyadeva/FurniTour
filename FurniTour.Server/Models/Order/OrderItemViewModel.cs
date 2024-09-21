namespace FurniTour.Server.Models.Order
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public byte[]? Photo { get; set; }
    }
}
