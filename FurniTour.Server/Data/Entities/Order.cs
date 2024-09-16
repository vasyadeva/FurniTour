using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? Phone { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Comment { get; set; }
        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
        [ForeignKey(nameof(OrderStateId))]
        public OrderState OrderState { get; set; }
        public int OrderStateId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
