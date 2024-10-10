using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext context;
        private readonly IAuthService authService;

        public ProfileService(ApplicationDbContext context, IAuthService authService)
        {
            this.context = context;
            this.authService = authService;
        }

        public  async Task<ManufacturerProfileModel> GetManufacturerProfile(string name)
        {
            var manufacturer = context.Manufacturers.FirstOrDefault(c => c.Name == name);
            if (manufacturer != null)
            {
                var reviews = context.ManufacturerReviews.Where(c => c.ManufacturerId == manufacturer.Id).Select(x => new ManufacturerReviewsModel
                {
                    Comment = x.Comment,
                    Rating = x.Rating,
                    Username = context.Users.Where(c => c.Id == x.UserId).FirstOrDefault().UserName
                }).ToList();
                var manufacturerProfile = new ManufacturerProfileModel
                {
                    Name = manufacturer.Name,
                    Reviews = reviews
                };
                return manufacturerProfile;
            }
            return null;
        }

        public async Task<MasterProfileModel> GetMasterProfile(string username)
        {
            var check = authService.CheckMasterByUsername(username);
            if (check.IsNullOrEmpty())
            {
                var master = await context.Users.FirstOrDefaultAsync(c => c.UserName == username);
                if (master != null)
                {
                    var reviews = context.MasterReviews.Where(c => c.MasterId == master.Id).Select(x => new MasterReviewsModel
                    {
                        Comment = x.Comment,
                        Rating = x.Rating,
                        Username = context.Users.Where(c => c.Id == x.UserId).FirstOrDefault().UserName
                    }).ToList();
                    var masterProfile = new MasterProfileModel
                    {
                        Username = master.UserName,
                        Email = master.Email,
                        PhoneNumber = master.PhoneNumber,
                        Reviews = reviews
                    };
                    return masterProfile;
                }

            }
            return null;
        }

        public string MakeManufacturerReview(AddManufacturerReview review)
        {
            var manufacturer = context.Manufacturers.FirstOrDefault(c => c.Name == review.Name);
            if (manufacturer != null)
            {
                var usercheck = authService.IsAuthenticated();
                if (usercheck.IsNullOrEmpty())
                {
                    var manufacturerReview = new ManufacturerReview
                    {
                        Comment = review.Comment,
                        Rating = review.Rating,
                        ManufacturerId = manufacturer.Id,
                        UserId = authService.GetUser().Id
                    };
                    context.Add(manufacturerReview);
                    context.SaveChanges();
                    return string.Empty;
                }
                return usercheck;
            }
            return "Manufacturer not found";
        }

        public string MakeMasterReview(AddMasterReview review)
        {
            var check = authService.CheckMasterByUsername(review.UserName);
            if (check.IsNullOrEmpty())
            {
                var usercheck = authService.IsAuthenticated();
                if (usercheck.IsNullOrEmpty())
                {
                    var masterReview = new MasterReview
                    {
                        Comment = review.Comment,
                        Rating = review.Rating,
                        MasterId = context.Users.FirstOrDefault(c => c.UserName == review.UserName).Id,
                        UserId = authService.GetUser().Id
                    };
                    context.Add(masterReview);
                    context.SaveChanges();
                    return string.Empty;
                }
                return usercheck;
            }
            return check;
        }
    }
}
