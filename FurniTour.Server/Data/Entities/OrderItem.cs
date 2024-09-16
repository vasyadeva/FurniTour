namespace FurniTour.Server.Data.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int FurnitureId { get; set; }
        public Furniture Furniture { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int Quantity { get; set; }
    }
}