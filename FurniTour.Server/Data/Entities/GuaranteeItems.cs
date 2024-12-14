using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class GuaranteeItems
    {
        public int Id { get; set; }
        public int GuaranteeId { get; set; }
        [ForeignKey(nameof(GuaranteeId))]
        public Guarantee Guarantee { get; set; }
        public int OrderItemId { get; set; }
        [ForeignKey(nameof(OrderItemId))]
        public OrderItem OrderItem { get; set; }
    }
}
