using FurniTour.Server.Constants;
using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Item;
using GroqApiLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
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
            var itemObj = context.Furnitures.ToList();
            if (itemObj != null)
            {
                var itemListModel = new List<ItemViewModel>();
                foreach (var item in itemObj)
                {
                    var Manufacturer = string.Empty;
                    var Master = string.Empty;
                    if (item.ManufacturerId != null)
                    {
                        Manufacturer = context.Manufacturers.Where(c => c.Id == item.ManufacturerId).FirstOrDefault().Name;
                    }
                    if (item.MasterId != null)
                    {
                        Master = context.Users.Where(c => c.Id == item.MasterId).FirstOrDefault().UserName;
                    }
                    var category = context.Categories.Where(c => c.Id == item.CategoryId).FirstOrDefault().Name;
                    var color = context.Colors.Where(c => c.Id == item.ColorId).FirstOrDefault().Name;
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



        public List<ItemViewModel> getFilteredItems(ItemFilterModel model)
        {
            var Items = context.Furnitures.AsQueryable();
            if (model.categoryID != null && model.categoryID != 0)
            {
                Items = Items.Where(c => c.CategoryId == model.categoryID);
            }
            if (model.colorID != null && model.colorID != 0)
            {
                Items = Items.Where(c => c.ColorId == model.colorID);
            }
            if (model.manufacturerID != null && model.manufacturerID != 0)
            {
                Items = Items.Where(c => c.ManufacturerId == model.manufacturerID);
            }
            if (model.masterID != null && model.masterID != string.Empty)
            {
                Items = Items.Where(c => c.MasterId == model.masterID);
            }
            if (model.minPrice != null && model.minPrice != 0)
            {
                Items = Items.Where(c => c.Price >= model.minPrice);
            }
            if (model.maxPrice != null && model.maxPrice != 0)
            {
                Items = Items.Where(c => c.Price <= model.maxPrice);
            }
            if (model.searchString != null && model.searchString != string.Empty)
            {
                Items = Items.Where(c => c.Name.Contains(model.searchString) || c.Description.Contains(model.searchString));
            }
            var itemObj = Items.ToList();
            if (itemObj != null)
            {
                var itemListModel = new List<ItemViewModel>();
                foreach (var item in itemObj)
                {
                    var Manufacturer = string.Empty;
                    var Master = string.Empty;
                    if (item.ManufacturerId != null)
                    {
                        Manufacturer = context.Manufacturers.Where(c => c.Id == item.ManufacturerId).FirstOrDefault().Name;
                    }
                    if (item.MasterId != null)
                    {
                        Master = context.Users.Where(c => c.Id == item.MasterId).FirstOrDefault().UserName;
                    }
                    var category = context.Categories.Where(c => c.Id == item.CategoryId).FirstOrDefault().Name;
                    var color = context.Colors.Where(c => c.Id == item.ColorId).FirstOrDefault().Name;
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
                switch (authService.IsMaster())
                {
                    case "":
                        var itemObj = new Furniture
                        {
                            Name = itemModel.Name,
                            Description = itemModel.Description,
                            Price = itemModel.Price,
                            Image = photoData,
                            CategoryId = itemModel.CategoryId,
                            ColorId = itemModel.ColorId,
                            MasterId = authService.GetUser().Id
                        };
                        await context.Furnitures.AddAsync(itemObj);
                        await context.SaveChangesAsync();
                        return string.Empty;
                    default:
                        if (itemModel.ManufacturerId == null || itemModel.ManufacturerId == 0)
                        {
                            return "ManufacturerId is required";
                        }
                        var itemObjAdm = new Furniture
                        {
                            Name = itemModel.Name,
                            Description = itemModel.Description,
                            Price = itemModel.Price,
                            Image = photoData,
                            CategoryId = itemModel.CategoryId,
                            ColorId = itemModel.ColorId,
                            ManufacturerId = itemModel.ManufacturerId
                        };
                        await context.Furnitures.AddAsync(itemObjAdm);
                        await context.SaveChangesAsync();
                        return string.Empty;
                }

            }
            return check;
        }

        public ItemViewModel Details(int id)
        {
            var itemObj = context.Furnitures.FirstOrDefault(x => x.Id == id);
            if (itemObj != null)
            {
                var Manufacturer = string.Empty;
                var Master = string.Empty;
                if (itemObj.ManufacturerId != null)
                {
                    Manufacturer = context.Manufacturers.Where(c => c.Id == itemObj.ManufacturerId).FirstOrDefault().Name;
                }
                if (itemObj.MasterId != null)
                {
                    Master = context.Users.Where(c => c.Id == itemObj.MasterId).FirstOrDefault().UserName;
                }
                var category = context.Categories.Where(c => c.Id == itemObj.CategoryId).FirstOrDefault().Name;
                var color = context.Colors.Where(c => c.Id == itemObj.ColorId).FirstOrDefault().Name;
                var itemModel = new ItemViewModel
                {
                    Id = itemObj.Id,
                    Name = itemObj.Name,
                    Description = itemObj.Description,
                    Price = itemObj.Price,
                    Image = Convert.ToBase64String(itemObj.Image),
                    Category = category,
                    Color = color,
                    Manufacturer = Manufacturer,
                    Master = Master
                };
                return itemModel;
            }
            return null;
        }


        public async Task<string> Edit(int id, ItemUpdateModel itemModel)
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
                var itemObj = context.Furnitures.FirstOrDefault(x => x.Id == id);
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

        public async Task<ItemViewModel> GetItemsByDescriptionAsync(string description)
        {
            var items = getAll();
            if (items == null || !items.Any())
            {
                throw new InvalidOperationException("No items found.");
            }

            var api = configuration["key:api"];
            if (string.IsNullOrEmpty(api))
            {
                throw new ArgumentNullException("api", "API key is missing.");
            }

            var groqApi = new GroqApiClient(api);

            var itemDescriptions = items.Select(item => new
            {
                item.Id,
                item.Name,
                item.Description,
                item.Price,
                item.Category,
                item.Color,
                item.Manufacturer,
                item.Master
            }).ToList();

            var request = new JsonObject
            {
                ["model"] = "deepseek-r1-distill-llama-70b",
                ["messages"] = new JsonArray
        {
            new JsonObject
            {
                ["role"] = "user",
                ["content"] = $"Here is a list of items: {JsonSerializer.Serialize(itemDescriptions)}. Please return the ID of the most relevant item in the format 'ID: <id>'. User wants: {description}"
            }
        }
            };

            var result = await groqApi.CreateChatCompletionAsync(request);
            var aiResponse = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            var match = System.Text.RegularExpressions.Regex.Match(aiResponse, @"ID: (\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int itemId))
            {
                var item = items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    return item;
                }
            }

            throw new InvalidOperationException("AI response did not contain a valid item ID.");
        }

        public byte[] GetImage(int id)
        {
            var item = context.Furnitures.FirstOrDefault(c => c.Id == id);
            return item.Image;
        }

        //        public async Task<List<ItemViewModel>> GetItemsByDescriptionAsync2(string description, int category,
        //            decimal minprice, decimal maxprice, int color)
        //        {
        //            var items = context.Furnitures.AsQueryable();
        //            var resultItems = items;

        //            // Якщо є опис товару, використовуємо AI для його аналізу
        //            if (!string.IsNullOrEmpty(description))
        //            {
        //                // Отримуємо випадкову обмежену вибірку описів товарів для прикладів
        //                var sampleItems = await context.Furnitures
        //                    .OrderBy(x => Guid.NewGuid())  // Випадковий порядок
        //                    .Take(5)  // Обмежена кількість прикладів
        //                    .Select(i => new { i.Name, i.Description })
        //                    .ToListAsync();

        //                var api = configuration["key:api"];
        //                if (string.IsNullOrEmpty(api))
        //                {
        //                    throw new ArgumentNullException(nameof(api), "API key is missing.");
        //                }

        //                var groqApi = new GroqApiClient(api);
        //                var request = new JsonObject
        //                {
        //                    ["model"] = "deepseek-r1-distill-llama-70b",
        //                    ["messages"] = new JsonArray
        //                    {
        //                        new JsonObject
        //                        {
        //                            ["role"] = "user",
        //                            ["content"] = $@"Here are some examples of furniture descriptions from our database:

        //{string.Join("\n\n", sampleItems.Select((s, i) => $"Example {i+1}:\nName: {s.Name}\nDescription: {s.Description}"))}

        //Based on these examples, analyze the following user query: '{description}'

        //Extract keywords that would be most effective for searching similar items in our database.
        //Format your response exactly as: 'keywords: keyword1, keyword2, keyword3'

        //If no relevant keywords can be extracted, respond with: 'keywords: none'"
        //                        }
        //                    }
        //                };

        //                var result = await groqApi.CreateChatCompletionAsync(request);
        //                var aiResponse = result?["choices"]?[0]?["message"]?["content"]?.ToString();

        //                if (!string.IsNullOrEmpty(aiResponse))
        //                {
        //                    // Витягуємо ключові слова з відповіді AI
        //                    var keywordsMatch = System.Text.RegularExpressions.Regex.Match(aiResponse, @"keywords:\s*(.*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //                    if (keywordsMatch.Success && keywordsMatch.Groups.Count > 1 && 
        //                        !keywordsMatch.Groups[1].Value.Trim().Equals("none", StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        var keywords = keywordsMatch.Groups[1].Value
        //                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        //                            .Select(k => k.Trim().ToLower())
        //                            .ToList();

        //                        if (keywords.Any())
        //                        {
        //                            // Створюємо запит для пошуку товарів з ключовими словами
        //                            resultItems = items.Where(i => false);  // Пустий набір для початку

        //                            foreach (var keyword in keywords)
        //                            {
        //                                // Об'єднуємо результати пошуку за кожним ключовим словом
        //                                resultItems = resultItems.Union(
        //                                    items.Where(i => 
        //                                        i.Name.ToLower().Contains(keyword) || 
        //                                        i.Description.ToLower().Contains(keyword))
        //                                );
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            // Якщо немає результатів за ключовими словами або ключові слова не знайдені,
        //            // забезпечуємо, щоб було повернуто хоча б базовий набір товарів
        //            if (!await resultItems.AnyAsync())
        //            {
        //                resultItems = items;
        //            }

        //            // Застосовуємо передані фільтри
        //            if (color > 0)
        //            {
        //                resultItems = resultItems.Where(i => i.Color.Id == color);
        //            }

        //            if (category > 0)
        //            {
        //                resultItems = resultItems.Where(i => i.Category.Id == category);
        //            }

        //            // Застосовуємо цінові фільтри
        //            if (minprice > 0)
        //            {
        //                resultItems = resultItems.Where(i => i.Price >= minprice);
        //            }

        //            if (maxprice > 0)
        //            {
        //                resultItems = resultItems.Where(i => i.Price <= maxprice);
        //            }

        //            // Якщо після всіх фільтрів нічого не знайдено, повертаємо 5 випадкових товарів
        //            var finalItems = await resultItems.Include(c => c.Color)
        //                    .Include(c => c.Category).ToListAsync();
        //            if (!finalItems.Any())
        //            {
        //                finalItems = await items
        //                    .OrderBy(x => Guid.NewGuid())
        //                    .Take(5)
        //                    .Include(c => c.Color)
        //                    .Include(c => c.Category)
        //                    .ToListAsync();
        //            }

        //            var filteredItems = finalItems.Select(c => new ItemViewModel
        //            {
        //                Id = c.Id,
        //                Name = c.Name,
        //                Description = c.Description,
        //                Price = c.Price,
        //                Image = "https://newbrushedgrape62.conveyor.cloud/api/item/image/" + c.Id.ToString(),
        //                Category = c.Category.Name,
        //                Color = c.Color.Name
        //            }).ToList();

        //            return filteredItems;
        //        }

        public async Task<List<ItemViewModel>> GetItemsByDescriptionAsync2(string description, int category,
    decimal minprice, decimal maxprice, int color)
        {
            // Apply basic filters
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

            // Store the filtered items before AI filtering
            var filteredItems = items;
            var aiFilteredItems = filteredItems;

            // Apply AI filtering if description is provided
            if (!string.IsNullOrEmpty(description))
            {
                // Get sample items for the AI context from the already filtered items
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
                                // Start with an empty query
                                aiFilteredItems = filteredItems.Where(i => false);

                                foreach (var keyword in keywords)
                                {
                                    // Apply keyword filtering within the already filtered items
                                    aiFilteredItems = aiFilteredItems.Union(
                                        filteredItems.Where(i =>
                                            i.Name.ToLower().Contains(keyword) ||
                                            i.Description.ToLower().Contains(keyword))
                                    );
                                }

                                // Check if any items were found using AI filtering
                                if (await aiFilteredItems.AnyAsync())
                                {
                                    // Use the AI-filtered items
                                    filteredItems = aiFilteredItems;
                                }
                                // If no items found with AI filtering, we'll keep the original filtered items
                            }
                        }
                    }
                }
            }

            // Add necessary includes and execute the query
            var finalItems = await filteredItems
                .Include(c => c.Color)
                .Include(c => c.Category)
                .ToListAsync();

            // Map to view models
            var result = finalItems.Select(c => new ItemViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                Image = "https://nextshinybag45.conveyor.cloud/api/item/image/" + c.Id.ToString(),
                Category = c.Category.Name,
                Color = c.Color.Name,
                Manufacturer = c.ManufacturerId.HasValue ?
                    context.Manufacturers.FirstOrDefault(m => m.Id == c.ManufacturerId)?.Name : string.Empty,
                Master = !string.IsNullOrEmpty(c.MasterId) ?
                    context.Users.FirstOrDefault(u => u.Id == c.MasterId)?.UserName : string.Empty
            }).ToList();

            return result;
        }
    }
}




