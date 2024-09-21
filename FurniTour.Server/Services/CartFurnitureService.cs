using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Cart;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace FurniTour.Server.Services
{
    public class CartFurnitureService : ICartFurnitureService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CartFurnitureService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task AddToCartAsync(int furnitureId, int quantity)
        {
            if (quantity <= 0)
            {
                return;
            }
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            if (user == null)
            {
                return;
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

            }
        }
        public List<CartItemViewModel> GetCartFurniture()
        {
            var User = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User?.Identity.Name).Result;
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
                                Items.Add(new CartItemViewModel
                                {
                                    Id = item.Id,
                                    Name = Furniture.Name,
                                    Description = Furniture.Description,
                                    Image = Convert.ToBase64String(Furniture.Image),
                                    Price = Furniture.Price,
                                    Quantity = item.Quantity
                                });
                            }
                        }
                        return Items;
                    }
                }
            }
            return null;
        }

        public async Task RemoveFromCartAsync(int id)
        {
            var CartItem = await context.CartItems.Where(ci => ci.Id == id).FirstOrDefaultAsync();
            if (CartItem != null)
            {
                context.CartItems.Remove(CartItem);
                await context.SaveChangesAsync();
            }
            return;
        }

        public async Task UpdateCartAsync(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return;
            }
            var CartItem = await context.CartItems.Where(ci => ci.Id == id).FirstOrDefaultAsync();
            if (CartItem != null)
            {
                CartItem.Quantity = quantity;
                context.CartItems.Update(CartItem);
                await context.SaveChangesAsync();
            }
            return;
        }
    }
}
