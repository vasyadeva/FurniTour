﻿using FurniTour.Server.Constants;
using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Item;
using GroqApiLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Nodes;

namespace FurniTour.Server.Services
{
    public class ItemFurnitureService : IItemFurnitureService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthService authService;
        private readonly IConfiguration configuration;

        public ItemFurnitureService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor
            , IAuthService authService, IConfiguration configuration)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.authService = authService;
            this.configuration = configuration;
        }

        public List<ItemViewModel> getAll()
        {
            var itemObj = context.Furnitures.Include(c => c.Reviews).Include(c => c.AdditionalPhotos).ToList();
            var itemList = new List<ItemViewModel>();
            foreach (var item in itemObj)
            {
                var it = MapFurnitureToViewModel(item);
                itemList.Add(it);
            }    
            return itemList;
        }        
        public List<ItemViewModel> getFilteredItems(ItemFilterModel model)
        {
            var Items = context.Furnitures.AsQueryable();
            if (model.categoryID != 0)
            {
                Items = Items.Where(c => c.CategoryId == model.categoryID);
            }
            if (model.colorID != 0)
            {
                Items = Items.Where(c => c.ColorId == model.colorID);
            }
            if (model.manufacturerID != 0)
            {
                Items = Items.Where(c => c.ManufacturerId == model.manufacturerID);      
            }
            if (!string.IsNullOrEmpty(model.masterID))
            {
                Items = Items.Where(c => c.MasterId == model.masterID);
            }
            if (model.minPrice != 0)
            {
                Items = Items.Where(c => c.Price >= model.minPrice);
            }
            if (model.maxPrice != 0)
            {
                Items = Items.Where(c => c.Price <= model.maxPrice);
            }
            if (!string.IsNullOrEmpty(model.searchString))
            {
                Items = Items.Where(c => c.Name.Contains(model.searchString) || c.Description.Contains(model.searchString));
            }
            var itemObj = Items.ToList();
            if (itemObj != null)
            {
                var itemListModel = new List<ItemViewModel>();
                foreach (var item in itemObj)
                {                    var Manufacturer = string.Empty;
                    var Master = string.Empty;
                    if (item.ManufacturerId != null)
                    {
                        var manufacturerEntity = context.Manufacturers.FirstOrDefault(c => c.Id == item.ManufacturerId);
                        Manufacturer = manufacturerEntity?.Name ?? string.Empty;
                    }
                    if (item.MasterId != null)
                    {
                        var masterEntity = context.Users.FirstOrDefault(c => c.Id == item.MasterId);
                        Master = masterEntity?.UserName ?? string.Empty;
                    }
                    var categoryEntity = context.Categories.FirstOrDefault(c => c.Id == item.CategoryId);
                    var category = categoryEntity?.Name ?? string.Empty;
                    var colorEntity = context.Colors.FirstOrDefault(c => c.Id == item.ColorId);
                    var color = colorEntity?.Name ?? string.Empty;
                    var itemModel = new ItemViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        Image = Convert.ToBase64String(item.Image),
                        Category = category,
                        Color = color,
                        Manufacturer = Manufacturer,
                        Master = Master

                    };
                    itemListModel.Add(itemModel);
                }
                return itemListModel;
            }
            return null;

        }
        public async Task<string> AddItem(ItemModel itemModel)
        {
            byte[] photoData = Convert.FromBase64String(itemModel.Image);
            var check = authService.CheckRoleMasterOrAdmin();
            if (check.IsNullOrEmpty())
            {
                if (!CategoryExists(itemModel.CategoryId))
                {
                    return "Category not found";
                }
                if (!ColorExists(itemModel.ColorId))
                {
                    return "Color not found";
                }
                
                Furniture itemObj = null;
                
                switch (authService.IsMaster())
                {
                    case "":
                        itemObj = new Furniture
                        {
                            Name = itemModel.Name,
                            Description = itemModel.Description,
                            Price = itemModel.Price,
                            Image = photoData,
                            CategoryId = itemModel.CategoryId,
                            ColorId = itemModel.ColorId,
                            MasterId = authService.GetUser().Id
                        };
                        break;
                    default:
                        if (itemModel.ManufacturerId == null || itemModel.ManufacturerId == 0)
                        {
                            return "ManufacturerId is required";
                        }
                        itemObj = new Furniture
                        {
                            Name = itemModel.Name,
                            Description = itemModel.Description,
                            Price = itemModel.Price,
                            Image = photoData,
                            CategoryId = itemModel.CategoryId,
                            ColorId = itemModel.ColorId,
                            ManufacturerId = itemModel.ManufacturerId
                        };
                        break;
                }
                
                await context.Furnitures.AddAsync(itemObj);
                await context.SaveChangesAsync();
                
                // Add additional photos if provided
                if (itemModel.AdditionalPhotos != null && itemModel.AdditionalPhotos.Any())
                {
                    for (int i = 0; i < itemModel.AdditionalPhotos.Count; i++)
                    {
                        var photoBase64 = itemModel.AdditionalPhotos[i];
                        string description = "";
                        
                        // Get description if available
                        if (itemModel.PhotoDescriptions != null && 
                            i < itemModel.PhotoDescriptions.Count &&
                            !string.IsNullOrEmpty(itemModel.PhotoDescriptions[i]))
                        {
                            description = itemModel.PhotoDescriptions[i];
                        }
                        
                        var additionalPhoto = new FurnitureAdditionalPhoto
                        {
                            FurnitureId = itemObj.Id,
                            PhotoData = Convert.FromBase64String(photoBase64),
                            Description = description
                        };
                        
                        await context.FurnitureAdditionalPhotos.AddAsync(additionalPhoto);
                    }
                    
                    await context.SaveChangesAsync();
                }
                
                return string.Empty;
            }
            return check;
        }        public ItemViewModel Details(int id)
        {
            var itemObj = context.Furnitures
                .Include(f => f.Reviews)
                .Include(f => f.AdditionalPhotos)
                .FirstOrDefault(x => x.Id == id);
            
            if (itemObj != null)
            {
                return MapFurnitureToViewModel(itemObj);
            }
            return new ItemViewModel(); // Return empty view model instead of null
        }public async Task<string> Edit(int id, ItemUpdateModel itemModel)
        {
            byte[] photoData = Convert.FromBase64String(itemModel.Image);
            authService.CheckRoleMasterOrAdmin();
            if (authService.CheckRoleMasterOrAdmin().IsNullOrEmpty())
            {
                if (!CategoryExists(itemModel.CategoryId))
                {
                    return "Category not found";
                }
                if (!ColorExists(itemModel.ColorId))
                {
                    return "Color not found";
                }
                var itemObj = await context.Furnitures
                    .Include(f => f.AdditionalPhotos)
                    .FirstOrDefaultAsync(x => x.Id == id);
                    
                if (itemObj != null)
                {
                    itemObj.Name = itemModel.Name;
                    itemObj.Description = itemModel.Description;
                    itemObj.Price = itemModel.Price;
                    itemObj.Image = photoData;
                    itemObj.CategoryId = itemModel.CategoryId;
                    itemObj.ColorId = itemModel.ColorId;
                    if (itemModel.ManufacturerId != null && itemModel.ManufacturerId != 0 && authService.IsAdmin() == string.Empty)
                    {
                        itemObj.ManufacturerId = itemModel.ManufacturerId;
                    }
                    
                    // Handle photo removal
                    if (itemModel.PhotoIdsToRemove != null && itemModel.PhotoIdsToRemove.Count > 0)
                    {
                        var photosToRemove = itemObj.AdditionalPhotos
                            .Where(p => itemModel.PhotoIdsToRemove.Contains(p.Id))
                            .ToList();
                        
                        if (photosToRemove.Any())
                        {
                            foreach (var photo in photosToRemove)
                            {
                                context.FurnitureAdditionalPhotos.Remove(photo);
                            }
                        }
                    }
                    
                    // Add new photos
                    if (itemModel.AdditionalPhotos != null && itemModel.AdditionalPhotos.Count > 0)
                    {
                        for (int i = 0; i < itemModel.AdditionalPhotos.Count; i++)
                        {
                            var photoBase64 = itemModel.AdditionalPhotos[i];
                            if (string.IsNullOrEmpty(photoBase64))
                                continue;
                                
                            var description = "";
                            if (itemModel.PhotoDescriptions != null && i < itemModel.PhotoDescriptions.Count)
                            {
                                description = itemModel.PhotoDescriptions[i] ?? "";
                            }
                                
                            var newPhoto = new Data.Entities.FurnitureAdditionalPhoto
                            {
                                FurnitureId = id,
                                PhotoData = Convert.FromBase64String(photoBase64),
                                Description = description
                            };
                            
                            await context.FurnitureAdditionalPhotos.AddAsync(newPhoto);
                        }
                    }
                    
                    await context.SaveChangesAsync();
                    return string.Empty;
                }
                return "Item not found";
            }
            return authService.CheckRoleMasterOrAdmin();
        }

        public async Task<string> DeleteItem(int id)
        {
            authService.CheckRoleMasterOrAdmin();
            if (authService.CheckRoleMasterOrAdmin().IsNullOrEmpty())
            {
                var itemObj = context.Furnitures.FirstOrDefault(x => x.Id == id);
                if (itemObj == null)
                {
                    return "Item not found";
                }
                if (itemObj.MasterId == authService.GetUser().Id || authService.CheckRoleAdmin().IsNullOrEmpty())
                {
                    context.Furnitures.Remove(itemObj);
                    await context.SaveChangesAsync();
                    return string.Empty;
                }
                return "This isn't your item or you aren't admin";
            }
            return authService.CheckRoleMasterOrAdmin();
        }
        private bool ColorExists(int id)
        {
            return context.Colors.Any(e => e.Id == id);
        }

        private bool CategoryExists(int id)
        {
            return context.Categories.Any(e => e.Id == id);
        }

        public List<Category> GetCategories()
        {
            return context.Categories.ToList();
        }

        public List<Data.Entities.Color> GetColors()
        {
            return context.Colors.ToList();
        }

        public byte[] GetImage(int id)
        {
            var item = context.Furnitures.FirstOrDefault(c => c.Id == id);
            return item?.Image ?? Array.Empty<byte>();
        }
   

        public async Task<List<ItemViewModel>> GetItemsByDescriptionAsync(string description, int category,
           decimal minprice, decimal maxprice, int color)
        {
            var items = context.Furnitures.AsQueryable();

            if (color > 0)
            {
                items = items.Where(i => i.ColorId == color);
            }

            if (category > 0)
            {
                items = items.Where(i => i.CategoryId == category);
            }

            if (minprice > 0)
            {
                items = items.Where(i => i.Price >= minprice);
            }

            if (maxprice > 0)
            {
                items = items.Where(i => i.Price <= maxprice);
            }


            var filteredItems = items;
            var aiFilteredItems = filteredItems;

            if (!string.IsNullOrEmpty(description))
            {

                var sampleItems = await filteredItems
                    .Take(5)
                    .Select(i => new { i.Name, i.Description })
                    .ToListAsync();

                if (sampleItems.Any())
                {
                    var api = configuration["key:api"];
                    if (string.IsNullOrEmpty(api))
                    {
                        throw new ArgumentNullException(nameof(api), "API key is missing.");
                    }

                    var groqApi = new GroqApiClient(api);
                    var request = new JsonObject
                    {
                        ["model"] = "deepseek-r1-distill-llama-70b",
                        ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = $@"Here are some examples of furniture descriptions from our database:

{string.Join("\n\n", sampleItems.Select((s, i) => $"Example {i+1}:\nName: {s.Name}\nDescription: {s.Description}"))}

Based on these examples, analyze the following user query: '{description}'

Extract keywords that would be most effective for searching similar items in our database.
Format your response exactly as: 'keywords: keyword1, keyword2, keyword3'

If no relevant keywords can be extracted, respond with: 'keywords: none'"
                    }
                }
                    };

                    var apiresult = await groqApi.CreateChatCompletionAsync(request);
                    var aiResponse = apiresult?["choices"]?[0]?["message"]?["content"]?.ToString();

                    if (!string.IsNullOrEmpty(aiResponse))
                    {
                        // Extract keywords from the AI response
                        var keywordsMatch = System.Text.RegularExpressions.Regex.Match(aiResponse, @"keywords:\s*(.*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (keywordsMatch.Success && keywordsMatch.Groups.Count > 1 &&
                            !keywordsMatch.Groups[1].Value.Trim().Equals("none", StringComparison.OrdinalIgnoreCase))
                        {
                            var keywords = keywordsMatch.Groups[1].Value
                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(k => k.Trim().ToLower())
                                .ToList();

                            if (keywords.Any())
                            {
                                aiFilteredItems = filteredItems.Where(i => false);

                                foreach (var keyword in keywords)
                                {
  
                                    aiFilteredItems = aiFilteredItems.Union(
                                        filteredItems.Where(i =>
                                            i.Name.ToLower().Contains(keyword) ||
                                            i.Description.ToLower().Contains(keyword))
                                    );
                                }


                                if (await aiFilteredItems.AnyAsync())
                                {
                                    filteredItems = aiFilteredItems;
                                }
                                
                            }
                        }
                    }
                }
            }

 
            var finalItems = await filteredItems
                .Include(c => c.Color)
                .Include(c => c.Category)
                .ToListAsync();

 
            var result = finalItems.Select(c => new ItemViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                Image = configuration["PublicUrl"] + "/api/item/image/" + c.Id.ToString(),
                Category = c.Category.Name,
                Color = c.Color.Name,
                Manufacturer = c.ManufacturerId.HasValue ?
                    context.Manufacturers.FirstOrDefault(m => m.Id == c.ManufacturerId)?.Name : string.Empty,
                Master = !string.IsNullOrEmpty(c.MasterId) ?
                    context.Users.FirstOrDefault(u => u.Id == c.MasterId)?.UserName : string.Empty
            }).ToList();

            return result;
        }

        public async Task<List<FurnitureReviewModel>> GetFurnitureReviews(int itemId)
        {
            var furniture = await context.Furnitures
                .Include(f => f.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(f => f.Id == itemId);
                
            if (furniture == null || furniture.Reviews == null)
            {
                return new List<FurnitureReviewModel>();
            }
            
            return furniture.Reviews.Select(r => new FurnitureReviewModel
            {
                Id = r.Id,
                Comment = r.Comment,
                Rating = r.Rating,
                Username = r.User?.UserName ?? "Unknown",
                CreatedAt = r.CreatedAt
            }).ToList();
        }
        
        public async Task<string> AddItemReview(AddFurnitureReviewModel reviewModel)
        {
            var userCheck = authService.IsAuthenticated();
            if (!userCheck.IsNullOrEmpty())
            {
                return userCheck;
            }
            
            var furniture = await context.Furnitures.FindAsync(reviewModel.FurnitureId);
            if (furniture == null)
            {
                return "Furniture not found";
            }
            
            // Check if user already reviewed this furniture
            var userId = authService.GetUser().Id;
            var existingReview = await context.FurnitureReviews
                .FirstOrDefaultAsync(r => r.FurnitureId == reviewModel.FurnitureId && r.UserId == userId);
                
            if (existingReview != null)
            {
                // Update existing review
                existingReview.Comment = reviewModel.Comment;
                existingReview.Rating = reviewModel.Rating;
                existingReview.CreatedAt = DateTime.Now;
            }
            else
            {
                // Create new review
                var review = new FurnitureReview
                {
                    FurnitureId = reviewModel.FurnitureId,
                    UserId = userId,
                    Comment = reviewModel.Comment,
                    Rating = reviewModel.Rating,
                    CreatedAt = DateTime.Now
                };
                
                await context.FurnitureReviews.AddAsync(review);
            }
            
            await context.SaveChangesAsync();
            return string.Empty;
        }        
        public async Task<string> GetReviewsSummary(int itemId)
        {
            var reviewsList = await GetFurnitureReviews(itemId);
            var reviews = reviewsList.Select(c => c.Comment).ToList();
            Console.WriteLine($"GetReviewsSummary: Found {reviews.Count} reviews for item {itemId}");
            
            if (reviews == null || !reviews.Any())
            {
                Console.WriteLine("GetReviewsSummary: No reviews found");
                return "Поки що немає відгуків для цього товару.";
            }
            
            var api = configuration["key:api"];
            if (string.IsNullOrEmpty(api))
            {
                Console.WriteLine("GetReviewsSummary: API key is missing");
                return "";
            }

            string reviewstext = "";
            foreach (var rev in reviews)
            {
                reviewstext += "," + rev;
            }
            var groqApi = new GroqApiClient(api);            var request = new JsonObject
            {
                ["model"] = "meta-llama/llama-4-maverick-17b-128e-instruct",
                ["messages"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["role"] = "user",
                        ["content"] = $"Summarize the reviews without for item only in Ukraininan language!!! . reviews: {reviewstext}"
                    }
                }
            };
            
            try
            {
                var result = await groqApi.CreateChatCompletionAsync(request);
                var aiResponse = result?["choices"]?[0]?["message"]?["content"]?.ToString();
                  if (!string.IsNullOrEmpty(aiResponse))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(aiResponse, @"Резюме: (.+)", System.Text.RegularExpressions.RegexOptions.Singleline);
                    if (match.Success)
                    {
                        return match.Groups[1].Value.Trim();
                    }
                    return aiResponse;
                }                
                // Fallback if AI doesn't respond as expected
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public byte[] GetAdditionalImage(int photoId)
        {
            var photo = context.FurnitureAdditionalPhotos.FirstOrDefault(p => p.Id == photoId);
            if (photo != null)
            {
                return photo.PhotoData ?? Array.Empty<byte>();
            }
            return Array.Empty<byte>();
        }

        private ItemViewModel MapFurnitureToViewModel(Furniture furniture)
        {
            // Extract manufacturer and master info
            var manufacturer = string.Empty;
            var master = string.Empty;
            
            if (furniture.ManufacturerId.HasValue)
            {
                var manufacturerEntity = context.Manufacturers.FirstOrDefault(c => c.Id == furniture.ManufacturerId);
                if (manufacturerEntity != null)
                {
                    manufacturer = manufacturerEntity.Name;
                }
            }
            
            if (!string.IsNullOrEmpty(furniture.MasterId))
            {
                var masterEntity = context.Users.FirstOrDefault(c => c.Id == furniture.MasterId);
                if (masterEntity != null)
                {
                    master = masterEntity.UserName;
                }
            }
            
            // Extract category and color
            var category = context.Categories.FirstOrDefault(c => c.Id == furniture.CategoryId)?.Name ?? string.Empty;
            var color = context.Colors.FirstOrDefault(c => c.Id == furniture.ColorId)?.Name ?? string.Empty;
            
            // Get reviews if included
            var reviews = new List<FurnitureReviewModel>();
            if (furniture.Reviews != null)
            {
                reviews = furniture.Reviews.Select(r => new FurnitureReviewModel
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    Username = context.Users.FirstOrDefault(u => u.Id == r.UserId)?.UserName ?? "Unknown",
                    CreatedAt = r.CreatedAt
                }).ToList();
            }
            
            // Get additional photos if included
            var additionalPhotos = new List<FurnitureAdditionalPhotoModel>();
            if (furniture.AdditionalPhotos != null)
            {
                additionalPhotos = furniture.AdditionalPhotos.Select(p => new FurnitureAdditionalPhotoModel
                {
                    Id = p.Id,
                    PhotoData = Convert.ToBase64String(p.PhotoData),
                    Description = p.Description ?? string.Empty
                }).ToList();
            }
            
            // Calculate ratings
            double averageRating = 0;
            if (reviews.Any())
            {
                averageRating = reviews.Average(r => r.Rating);
            }
            
            return new ItemViewModel
            {
                Id = furniture.Id,
                Name = furniture.Name,
                Description = furniture.Description,
                Price = furniture.Price,
                Image = Convert.ToBase64String(furniture.Image),
                Category = category,
                Color = color,
                Manufacturer = manufacturer,
                Master = master,
                Reviews = reviews,
                AdditionalPhotos = additionalPhotos,
                AverageRating = averageRating,
                ReviewCount = reviews.Count
            };
        }

        public async Task<int> GetAdditionalPhotoCount(int itemId)
        {
            var furniture = await context.Furnitures
                .Include(f => f.AdditionalPhotos)
                .FirstOrDefaultAsync(f => f.Id == itemId);
                
            if (furniture == null || furniture.AdditionalPhotos == null)
            {
                return 0;
            }
            
            return furniture.AdditionalPhotos.Count;
        }

        // Forward the GetItemReviews calls to GetFurnitureReviews for backward compatibility
        public async Task<List<FurnitureReviewModel>> GetItemReviews(int itemId)
        {
            // Forward to the implementation in GetFurnitureReviews
            var furniture = await context.Furnitures
                .Include(f => f.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(f => f.Id == itemId);
                
            if (furniture == null || furniture.Reviews == null)
            {
                return new List<FurnitureReviewModel>();
            }
            
            return furniture.Reviews.Select(r => new FurnitureReviewModel
            {
                Id = r.Id,
                Comment = r.Comment,
                Rating = r.Rating,
                Username = r.User?.UserName ?? "Unknown",
                CreatedAt = r.CreatedAt
            }).ToList();
        }
    }
}




