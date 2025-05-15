using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Cart;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;

namespace FurniTour.Server.Services
{
    public class CartFurnitureService : ICartFurnitureService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthService authService;

        public CartFurnitureService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor
           , IAuthService authService)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.authService = authService;
        }
        public async Task<string> AddToCartAsync(int furnitureId, int quantity)
        {
            if (quantity <= 0)
            {
                return "Item quantity must be greater than 0";
            }
            var user = authService.GetUser();
            if (user == null)
            {
                return "You are not logged in";
            }
            var Cart = await context.Carts.Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
            if (Cart == null)
            {
                Cart = new Cart
                {
                    UserId = user.Id
                };
                context.Carts.Add(Cart);
                context.SaveChanges();
            }
            var CartExists = context.Carts.Where(c => c.UserId == user.Id).FirstOrDefault();
            if (CartExists != null)
            {
                var CartItemsExists = await context.CartItems.Where(ci => ci.CartId == CartExists.Id && ci.FurnitureId == furnitureId).FirstOrDefaultAsync();
                if (CartItemsExists != null)
                {
                    CartItemsExists.Quantity += quantity;
                    context.CartItems.Update(CartItemsExists);
                    await context.SaveChangesAsync();
                }

                var CartItem = new CartItem
                {
                    FurnitureId = furnitureId,
                    Quantity = quantity,
                    CartId = CartExists.Id
                };
                await context.CartItems.AddAsync(CartItem);
                await context.SaveChangesAsync();
                return string.Empty;
            }
            return "Some problem occured while adding item to cart";
        }
        public List<CartItemViewModel> GetCartFurniture()
        {
            var User = authService.GetUser();
            if (User != null)
            {
                var Cart = context.Carts.Where(c => c.UserId == User.Id).FirstOrDefault();
                if (Cart == null)
                {
                    Cart = new Cart
                    {
                        UserId = User.Id
                    };
                    context.Carts.Add(Cart);
                    context.SaveChanges();
                }
                var CartExists = context.Carts.Where(c => c.UserId == User.Id).FirstOrDefault();
                if (CartExists != null)
                {
                    var CartItems = context.CartItems.Where(ci => ci.CartId == CartExists.Id).ToList();

                    if (CartItems.Count > 0)
                    {
                        var Items = new List<CartItemViewModel>();
                        foreach (var item in CartItems)
                        {
                            var Furniture = context.Furnitures.Where(f => f.Id == item.FurnitureId).FirstOrDefault();
                            if (Furniture != null)
                            {
                                var Manufacturer = string.Empty;
                                var Master = string.Empty;
                                if (Furniture.ManufacturerId != null)
                                {
                                    Manufacturer = context.Manufacturers.Where(c => c.Id == Furniture.ManufacturerId).FirstOrDefault().Name;
                                }
                                if (Furniture.MasterId != null)
                                {
                                    Master = context.Users.Where(c => c.Id == Furniture.MasterId).FirstOrDefault().UserName;
                                }
                                var category = context.Categories.Where(c => c.Id == Furniture.CategoryId).FirstOrDefault()?.Name ?? "Unknown";
                                var color = context.Colors.Where(c => c.Id == Furniture.ColorId).FirstOrDefault()?.Name ?? "Unknown";

                                Items.Add(new CartItemViewModel
                                {
                                    Id = item.Id,
                                    Name = Furniture.Name,
                                    Description = Furniture.Description,
                                    Image = Convert.ToBase64String(Furniture.Image),
                                    Price = Furniture.Price,
                                    Quantity = item.Quantity,
                                    Manufacturer = Manufacturer,
                                    Master = Master,
                                    Category = category,
                                    Color = color
                                });
                            }
                        }
                        return Items;
                    }
                }
            }
            return null;
        }

        public async Task<string> RemoveFromCartAsync(int id)
        {
            var isAuth = authService.IsAuthenticated();
            if (isAuth.IsNullOrEmpty())
            {

                var CartUser = await context.Carts.Where(c => c.UserId == authService.GetUser().Id).FirstOrDefaultAsync();
                if (CartUser == null)
                {
                    return "Cart is empty";
                }
                var CartItem = await context.CartItems.Where(ci => ci.Id == id && ci.CartId == CartUser.Id).FirstOrDefaultAsync();
                if (CartItem != null)
                {
                    context.CartItems.Remove(CartItem);
                    await context.SaveChangesAsync();
                    return string.Empty;
                }
                return "Some error occurred while removing item from cart";
            }
            return isAuth;
        }

        public async Task<string> UpdateCartAsync(int id, int quantity)
        {
            var User = authService.GetUser();
            if (User == null)
            {
                return "You are not logged in";
            }
            if (quantity <= 0)
            {
                return "Item quantity must be greater than 0";
            }
            var CartUser = await context.Carts.Where(c => c.UserId == User.Id).FirstOrDefaultAsync();
            if (CartUser == null)
            {
                return "Cart is empty";
            }
            var CartItem = await context.CartItems.Where(ci => ci.Id == id && ci.CartId == CartUser.Id).FirstOrDefaultAsync();
            if (CartItem != null)
            {
                CartItem.Quantity = quantity;
                context.CartItems.Update(CartItem);
                await context.SaveChangesAsync();
                return string.Empty;
            }
            return "Some error occurred while updating cart";
        }

        #region AImethods
        public async Task<string> AddToCartCopilot(int furnitureId, int quantity, string userID)
        {
            if (quantity <= 0)
            {
                return "Item quantity must be greater than 0";
            }
            var user = await authService.GetUserById(userID);
            if (user == null)
            {
                return "You are not logged in";
            }
            var Cart = await context.Carts.Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
            if (Cart == null)
            {
                Cart = new Cart
                {
                    UserId = user.Id
                };
                context.Carts.Add(Cart);
                context.SaveChanges();
            }
            var CartExists = context.Carts.Where(c => c.UserId == user.Id).FirstOrDefault();
            if (CartExists != null)
            {
                var CartItemsExists = await context.CartItems.Where(ci => ci.CartId == CartExists.Id && ci.FurnitureId == furnitureId).FirstOrDefaultAsync();
                if (CartItemsExists != null)
                {
                    CartItemsExists.Quantity += quantity;
                    context.CartItems.Update(CartItemsExists);
                    await context.SaveChangesAsync();
                }

                var CartItem = new CartItem
                {
                    FurnitureId = furnitureId,
                    Quantity = quantity,
                    CartId = CartExists.Id
                };
                await context.CartItems.AddAsync(CartItem);
                await context.SaveChangesAsync();
                return string.Empty;
            }
            return "Some problem occured while adding item to cart";
        }


        public async Task<string> RemoveFromCartCopilot(int id, string userID)
        {
            var isAuth = authService.IsAuthenticated();
            if (isAuth.IsNullOrEmpty())
            {
                var CartUser = await context.Carts.Where(c => c.UserId == userID).FirstOrDefaultAsync();
                if (CartUser == null)
                {
                    return "Cart is empty";
                }
                var CartItem = await context.CartItems.Where(ci => ci.Id == id && ci.CartId == CartUser.Id).FirstOrDefaultAsync();
                if (CartItem != null)
                {
                    context.CartItems.Remove(CartItem);
                    await context.SaveChangesAsync();
                    return string.Empty;
                }
                return "Some error occurred while removing item from cart";
            }
            return isAuth;
        }

        public async Task<string> UpdateCartCopilot(int id, int quantity, string userID)
        {
            var User = await authService.GetUserById(userID);
            if (User == null)
            {
                return "You are not logged in";
            }
            if (quantity <= 0)
            {
                return "Item quantity must be greater than 0";
            }
            var CartUser = await context.Carts.Where(c => c.UserId == User.Id).FirstOrDefaultAsync();
            if (CartUser == null)
            {
                return "Cart is empty";
            }
            var CartItem = await context.CartItems.Where(ci => ci.Id == id && ci.CartId == CartUser.Id).FirstOrDefaultAsync();
            if (CartItem != null)
            {
                CartItem.Quantity = quantity;
                context.CartItems.Update(CartItem);
                await context.SaveChangesAsync();
                return string.Empty;
            }
            return "Some error occurred while updating cart";
        }

        public async Task<List<CartItemViewModel>> GetCartFurnitureCopilot(string userID)
        {
            var User = await authService.GetUserById(userID);
            if (User != null)
            {
                var Cart = context.Carts.Where(c => c.UserId == User.Id).FirstOrDefault();
                if (Cart == null)
                {
                    Cart = new Cart
                    {
                        UserId = User.Id
                    };
                    context.Carts.Add(Cart);
                    context.SaveChanges();
                }
                var CartExists = context.Carts.Where(c => c.UserId == User.Id).FirstOrDefault();
                if (CartExists != null)
                {
                    var CartItems = context.CartItems.Where(ci => ci.CartId == CartExists.Id).ToList();
                    if (CartItems.Count > 0)
                    {
                        var Items = new List<CartItemViewModel>();
                        foreach (var item in CartItems)
                        {
                            var Furniture = context.Furnitures.Where(f => f.Id == item.FurnitureId).FirstOrDefault();
                            if (Furniture != null)
                            {
                                var Manufacturer = string.Empty;
                                var Master = string.Empty;
                                if (Furniture.ManufacturerId != null)
                                {
                                    Manufacturer = context.Manufacturers.Where(c => c.Id == Furniture.ManufacturerId).FirstOrDefault().Name;
                                }
                                if (Furniture.MasterId != null)
                                {
                                    Master = context.Users.Where(c => c.Id == Furniture.MasterId).FirstOrDefault().UserName;
                                }
                                var category = context.Categories.Where(c => c.Id == Furniture.CategoryId).FirstOrDefault()?.Name ?? "Unknown";
                                var color = context.Colors.Where(c => c.Id == Furniture.ColorId).FirstOrDefault()?.Name ?? "Unknown";
                                Items.Add(new CartItemViewModel
                                {
                                    Id = item.Id,
                                    Name = Furniture.Name,
                                    Description = Furniture.Description,
                                    Image = null ,//Convert.ToBase64String(Furniture.Image),
                                    Price = Furniture.Price,
                                    Quantity = item.Quantity,
                                    Manufacturer = Manufacturer,
                                    Master = Master,
                                    Category = category,
                                    Color = color
                                });
                            }
                        }
                        return Items;
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
