namespace FurniTour.Server.Models.Cart
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Master { get; set; }
        public string? Manufacturer { get; set; }
    }
}
