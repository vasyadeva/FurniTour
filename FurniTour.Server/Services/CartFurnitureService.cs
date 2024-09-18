using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models;
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
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
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
                await context.Carts.AddAsync(Cart);
                await context.SaveChangesAsync();
            }
            var CartExists = await context.Carts.Where(c => c.UserId == user.Id).FirstOrDefaultAsync();
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
                    CartId = CartExists.Id,
                    FurnitureId = furnitureId,
                    Quantity = quantity
                };
                await context.CartItems.AddAsync(CartItem);
                await context.SaveChangesAsync();

            }
        }

        public async Task<IEnumerable<CartItemViewModel>> GetCartFurnitureAsync()
        {
            var User = userManager.GetUserAsync(httpContextAccessor.HttpContext.User).Result;
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
                    await context.SaveChangesAsync();
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
                            var Furniture = await context.Furnitures.Where(f => f.Id == item.FurnitureId).FirstOrDefaultAsync();
                            if (Furniture != null)
                            {
                                Items.Add(new CartItemViewModel
                                {
                                    Id = item.Id,
                                    Name = Furniture.Name,
                                    Description = Furniture.Description,
                                    Image = Furniture.Image,
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
