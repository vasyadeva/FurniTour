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
                    if (item.ManufacturerId !=null)
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
        throw new ArgumentNullException(nameof(api), "API key is missing.");
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
        ["model"] = "mixtral-8x7b-32768",
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




    }


}
