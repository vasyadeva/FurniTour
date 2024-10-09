using FurniTour.Server.Constants;
using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Item;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Services
{
    public class ItemFurnitureService : IItemFurnitureService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthService authService;
        public ItemFurnitureService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor
            , IAuthService authService)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.authService = authService;
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
                    
                    var itemModel = new ItemViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        Image = Convert.ToBase64String(item.Image),
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
                switch(authService.IsMaster())
                {
                    case "":
                        var itemObj = new Furniture
                        {
                            Name = itemModel.Name,
                            Description = itemModel.Description,
                            Price = itemModel.Price,
                            Image = photoData,
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
                var itemModel = new ItemViewModel
                {
                    Id = itemObj.Id,
                    Name = itemObj.Name,
                    Description = itemObj.Description,
                    Price = itemObj.Price,
                    Image = Convert.ToBase64String(itemObj.Image),
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
                var itemObj = context.Furnitures.FirstOrDefault(x => x.Id == id);
                if (itemObj != null)
                {
                    itemObj.Name = itemModel.Name;
                    itemObj.Description = itemModel.Description;
                    itemObj.Price = itemModel.Price;
                    itemObj.Image = photoData;
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
    }
}
