using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class Clicks
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public int FurnitureId { get; set; } 
        public int InteractionCount { get; set; }
        public DateTime LastInteractionTime { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        [ForeignKey(nameof(FurnitureId))]
        public Furniture Furniture { get; set; }
    }
}
