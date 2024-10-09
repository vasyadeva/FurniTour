using FurniTour.Server.Models.Profile;

namespace FurniTour.Server.Interfaces
{
    public interface IProfileService
    {
        public Task<MasterProfileModel> GetMasterProfile(string username);
        public string MakeMasterReview(AddMasterReview review);
    }
}
