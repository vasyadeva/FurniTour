using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurniTour.Server.Data.Entities
{
    public class MasterReview
    {
        public int Id { get; set; }
        public string MasterId { get; set; }
        [ForeignKey(nameof(MasterId))]
        public IdentityUser Master { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
