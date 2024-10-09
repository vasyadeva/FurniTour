using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace FurniTour.Server.Models.Profile
{
    public class AddMasterReview
    {
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
