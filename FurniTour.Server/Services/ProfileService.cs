using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Api.AI;
using FurniTour.Server.Models.Auth;
using FurniTour.Server.Models.Item;
using FurniTour.Server.Models.Profile;
using GroqApiLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FurniTour.Server.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext context;
        private readonly IAuthService authService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;

        public ProfileService(ApplicationDbContext context, IAuthService authService, UserManager<IdentityUser> userManager,

            IConfiguration configuration)
        {
            this.context = context;
            this.authService = authService;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<ManufacturerProfileModel> GetManufacturerProfile(string name)
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
            return "Виробника не знайдено";
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

        public async Task<MasterProfileModel> GetMasterByDescription(string description)
        {
            var MasterRole = context.Roles.FirstOrDefault(c => c.Name == "Master");
            var usersWithPermission = userManager.GetUsersInRoleAsync("Master").Result;

            // Then get a list of the ids of these users
            var idsWithPermission = usersWithPermission.Select(u => u.Id);

            // Now get the users in our database with the same ids
            var users = context.Users.Where(u => idsWithPermission.Contains(u.Id)).ToListAsync();

            if (users == null)
            {
                return null;
            }

            var MasterProfileModels = users.Result.Select(u => new MasterProfileModel
            {
                Username = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Reviews = context.MasterReviews.Where(c => c.MasterId == u.Id).Select(x => new MasterReviewsModel
                {
                    Comment = x.Comment,
                    Rating = x.Rating,
                    Username = context.Users.Where(c => c.Id == x.UserId).FirstOrDefault().UserName
                }).ToList()
            }).ToList();

            var api = configuration["key:api"];

            var groqApi = new GroqApiClient(api);

            var request = new JsonObject
            {
                ["model"] = "deepseek-r1-distill-llama-70b",
                ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = $"Here is a list of masters of furniture: {JsonSerializer.Serialize(MasterProfileModels)}. Please return the username of the most relevant master in the format 'username: <username>'. User wants: {description}"
                }
            }

            };
            var result = await groqApi.CreateChatCompletionAsync(request);
            var aiResponse = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            var match = System.Text.RegularExpressions.Regex.Match(aiResponse, @"username:\s*(\w+)");
            if (match.Success)
            {
                var username = match.Groups[1].Value;
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
            return null; // Ensure a return statement is present at the end of the method
        }

        public async Task<List<MasterProfileAIModel>> GetMasterByDescription2(string description, int category, int pricePolicy)
        {
            var masters = await userManager.GetUsersInRoleAsync("Master");
            var orders = context.Orders.
                Include(c => c.OrderItems).
                ThenInclude(c => c.Furniture).
                AsQueryable();
            if (category != 0)
            {
                orders = orders.Where(c => c.OrderItems.Any(d => d.Furniture.CategoryId == category));
            }

            switch (pricePolicy)
            {
                case 1:
                    orders = orders.Where(c => c.OrderItems.Average(q => q.Furniture.Price) < 10000);
                    break;
                case 2:
                    orders = orders.Where(c => (c.OrderItems.Average(q => q.Furniture.Price) > 10000) &&
                       (c.OrderItems.Average(q => q.Furniture.Price) < 50000));
                    break;
                case 3:
                    orders = orders.Where(c => c.OrderItems.Average(q => q.Furniture.Price) > 50000);
                    break;
                default:
                    break;
            }

            var filteredMasters = orders
      .SelectMany(order => order.OrderItems)
      .Where(item => item.Furniture != null && item.Furniture.MasterId != null)
      .Select(item => item.Furniture.MasterId)
      .Distinct()
      .ToList();


            

            var mastersList = new List<MasterProfileAIModel>();
            foreach (var master in masters)
            {
                if (filteredMasters.Contains(master.Id))
                {
                    var reviews = context.MasterReviews.Where(c => c.MasterId == master.Id).Select(x => new MasterReviewsModel
                    {
                        Comment = x.Comment,
                        Rating = x.Rating,
                        Username = context.Users.Where(c => c.Id == x.UserId).FirstOrDefault().UserName
                    }).ToList();
                    var itemReviews = context.FurnitureReviews.Include(c => c.Furniture).Where(c => c.Furniture.MasterId == master.Id).Select(x => new FurnitureReviewModel
                    {
                        Comment = x.Comment,
                        Rating = x.Rating,
                        Username = context.Users.Where(c => c.Id == x.UserId).FirstOrDefault().UserName
                    }).ToList();
                    var masterProfile = new MasterProfileAIModel
                    {
                        Username = master.UserName,
                        Email = master.Email,
                        PhoneNumber = master.PhoneNumber,
                        Reviews = reviews,
                        FurnitureReviews = itemReviews
                    };
                    mastersList.Add(masterProfile);
                }
            }


            if (!string.IsNullOrEmpty(description))
            {
                var api = configuration["key:api"];
                var groqApi = new GroqApiClient(api);
                var request = new JsonObject
                {
                    ["model"] = "deepseek-r1-distill-llama-70b",
                    ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = $"Here is a list of masters of furniture and their reviews: {JsonSerializer.Serialize(mastersList)}." +
                        $" Based on these examples, analyze the following user query: '{description}'\r\n\r\nExtract keywords that would be most effective for searching similar masters in our database.\r\nFormat your response exactly as: 'keywords: keyword1, keyword2, keyword3'\r\n\r\n" +
                        $"If no relevant keywords can be extracted, respond with: 'keywords: none'"
                    }
                }
                };
                var result = await groqApi.CreateChatCompletionAsync(request);
                var aiResponse = result?["choices"]?[0]?["message"]?["content"]?.ToString();

                if (!string.IsNullOrEmpty(aiResponse)) {
                    var match = System.Text.RegularExpressions.Regex.Match(aiResponse, @"keywords:\s*(.+)");
                    if (match.Success)
                    {
                        var keywords = match.Groups[1].Value.Split(',').Select(k => k.Trim()).ToList();
                        var filteredMastersList = mastersList.Where(m =>
                            keywords.Any(k => m.Username.Contains(k, StringComparison.OrdinalIgnoreCase))).ToList();
                        //return filteredMastersList.FirstOrDefault();

                        if (filteredMastersList.Count > 0)
                        {
                            var Masters = filteredMastersList.Take(5).ToList();
                            var AIMasters = new List<MasterProfileAIModel>();
                            foreach (var master in Masters)
                            {
                                var reviews = context.MasterReviews.Where(c => c.MasterId == master.Username).Select(x => new MasterReviewsModel
                                {
                                    Comment = x.Comment,
                                    Rating = x.Rating,
                                    Username = context.Users.Where(c => c.Id == x.UserId).FirstOrDefault().UserName
                                }).ToList();
                                var itemReviews = context.FurnitureReviews.Include(c => c.Furniture).Where(c => c.Furniture.MasterId == master.Username).Select(x => new FurnitureReviewModel
                                {
                                    Comment = x.Comment,
                                    Rating = x.Rating,
                                    Username = context.Users.Where(c => c.Id == x.UserId).FirstOrDefault().UserName
                                }).ToList();
                                var masterProfile = new MasterProfileAIModel
                                {
                                    Username = master.Username,
                                    Email = master.Email,
                                    PhoneNumber = master.PhoneNumber,
                                    Reviews = reviews,
                                    FurnitureReviews = itemReviews
                                };
                                AIMasters.Add(masterProfile);
                            }
                            return AIMasters;

                        }
                       
                    }

                }

            }            return mastersList.Take(5).ToList();
        }

        public async Task<ProfileModel> GetPublicProfile(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                var publicProfile = new ProfileModel
                {
                    Username = user.UserName,
                    Email = "Приватна інформація", // Hide email for public profiles
                    Phonenumber = "Приватна інформація", // Hide phone for public profiles
                    Role = roles.FirstOrDefault() ?? "User"
                };
                return publicProfile;
            }
            return null;
        }

    }
}
