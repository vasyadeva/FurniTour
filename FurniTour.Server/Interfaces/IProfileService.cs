﻿using FurniTour.Server.Models.Api.AI;
using FurniTour.Server.Models.Auth;
using FurniTour.Server.Models.Profile;

namespace FurniTour.Server.Interfaces
{
    public interface IProfileService
    {
        public Task<MasterProfileModel> GetMasterProfile(string username);
        public Task<ManufacturerProfileModel> GetManufacturerProfile(string name);
        public string MakeManufacturerReview(AddManufacturerReview review);
        public string MakeMasterReview(AddMasterReview review);
        Task<List<MasterProfileAIModel>> GetMasterByDescription(string description, int category, int pricePolicy);
        Task<ProfileModel> GetPublicProfile(string username);
    }
}
