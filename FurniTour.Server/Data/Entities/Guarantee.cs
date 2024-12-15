using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class Guarantee
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } 
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public ICollection<GuaranteeItems> GuaranteeItems { get; set; }
        public ICollection<GuaranteePhoto> GuaranteePhotos { get; set; }
    }
}
