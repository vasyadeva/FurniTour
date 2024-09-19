using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace FurniTour.Server.Services
{
    public class ItemFurnitureService : IItemFurnitureService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ItemFurnitureService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public List<ItemViewModel> getAll()
        {
            var itemObj = context.Furnitures.ToList();
            if (itemObj != null)
            {
                var itemListModel = new List<ItemViewModel>();
                foreach (var item in itemObj)
                {
                    
                    var itemModel = new ItemViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        Image = Convert.ToBase64String(item.Image)
                    };
                    itemListModel.Add(itemModel);
                }
                return itemListModel;
            }
            return null;
        }

        public bool AddItem(ItemModel itemModel)
        {
            byte[] photoData = Convert.FromBase64String(itemModel.Image);
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            if (user != null)
            {
                var role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                if (role == "Master")
                {
                    var itemObj = new Furniture
                    {
                        Name = itemModel.Name,
                        Description = itemModel.Description,
                        Price = itemModel.Price,
                        Image = photoData,
                        MasterId = user.Id
                    };
                    context.Furnitures.Add(itemObj);
                    context.SaveChanges();
                    return true;
                }
            }
            return true;
        }

        public ItemViewModel Details(int id)
        {
            var itemObj = context.Furnitures.FirstOrDefault(x => x.Id == id);
            if (itemObj != null)
            {
                var itemModel = new ItemViewModel
                {
                    Id = itemObj.Id,
                    Name = itemObj.Name,
                    Description = itemObj.Description,
                    Price = itemObj.Price,
                    Image = Convert.ToBase64String(itemObj.Image)
                };
                return itemModel;
            }
            return null;
        }
        

        public bool Edit(int id, ItemViewModel itemModel)
        {
            byte[] photoData = Convert.FromBase64String(itemModel.Image);
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            if (user != null)
            {
                var role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                if (role == "Master")
                {
                    var itemObj = context.Furnitures.FirstOrDefault(x => x.Id == id);
                    if (itemObj != null)
                    {
                        itemObj.Name = itemModel.Name;
                        itemObj.Description = itemModel.Description;
                        itemObj.Price = itemModel.Price;
                        itemObj.Image = photoData;
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DeleteItem(int id)
        {
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            if (user != null)
            {
                var role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                if (role == "Master")
                {
                    var itemObj = context.Furnitures.FirstOrDefault(x => x.Id == id);
                    if (itemObj != null)
                    {
                        context.Furnitures.Remove(itemObj);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
