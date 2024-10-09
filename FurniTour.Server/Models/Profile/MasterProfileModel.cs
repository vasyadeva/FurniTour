namespace FurniTour.Server.Models.Profile
{
    public class MasterProfileModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<MasterReviewsModel> Reviews { get; set; }
    }
}
