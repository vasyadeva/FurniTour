using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int FurnitureId { get; set; }
        [ForeignKey(nameof(FurnitureId))]
        public Furniture Furniture { get; set; }
        public int CartId { get; set; }
        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }
        public int Quantity { get; set; }
    }
}
