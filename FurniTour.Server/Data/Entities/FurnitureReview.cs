using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class FurnitureReview
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        
        public int FurnitureId { get; set; }
        [ForeignKey(nameof(FurnitureId))]
        public Furniture Furniture { get; set; }
        
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
