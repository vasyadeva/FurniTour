using System.Text.Json.Serialization;

namespace FurniTour.Server.Models.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Comment { get; set; }
        public DateTime DateCreated { get; set; }
        public string OrderState { get; set; }
        public decimal Price { get; set; }
        
        [JsonPropertyName("originalPrice")]
        public decimal OriginalPrice { get; set; }
        
        [JsonPropertyName("appliedDiscount")]
        public decimal AppliedDiscount { get; set; }
        
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}
