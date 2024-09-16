namespace FurniTour.Server.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int FurnitureId { get; set; }
        public Furniture Furniture { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int Quantity { get; set; }
    }
}
