using FurniTour.Server.Models.Item;
using FurniTour.Server.Models.Profile;

namespace FurniTour.Server.Models.Api.AI
{
    public class MasterProfileAIModel
    {
            public string Username { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public List<MasterReviewsModel> Reviews { get; set; }
            public List<FurnitureReviewModel>? FurnitureReviews { get; set; }
    }
}
