using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        public string UserId { get; set; }

    }
}
