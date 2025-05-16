using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class Guarantee
    {
        public int Id { get; set; }

        public required string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public required IdentityUser User { get; set; }
        
        public int? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; } 
        
        public int? IndividualOrderId { get; set; }
        [ForeignKey(nameof(IndividualOrderId))]
        public IndividualOrder? IndividualOrder { get; set; }
        
        public required string Status { get; set; }
        public required string Comment { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateModified { get; set; } = DateTime.Now;
        public required ICollection<GuaranteeItems> GuaranteeItems { get; set; }
        public required ICollection<GuaranteePhoto> GuaranteePhotos { get; set; }
    }
}
