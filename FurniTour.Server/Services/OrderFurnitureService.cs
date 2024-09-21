using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Order;
using Microsoft.AspNetCore.Identity;

namespace FurniTour.Server.Services
{
    public class OrderFurnitureService : IOrderFurnitureService
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;
        public OrderFurnitureService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager) {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }


        public List<OrderViewModel> MyOrders()
        {
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            if (user != null)
            {
                var Orders = context.Orders.Where(o => o.UserId == user.Id).ToList();
                if (Orders.Count > 0)
                {
                    var OrderViewModel = new List<OrderViewModel>();
                    foreach (var Order in Orders)
                    {
                        var OrderItems = context.OrderItems.Where(oi => oi.OrderId == Order.Id).ToList();
                        if (OrderItems.Count > 0)
                        {
                            var OrderItemViewModel = new List<OrderItemViewModel>();
                            foreach (var OrderItem in OrderItems)
                            {
                                var Furniture = context.Furnitures.Where(f => f.Id == OrderItem.FurnitureId).FirstOrDefault();
                                if (Furniture != null)
                                {
                                    OrderItemViewModel.Add(new OrderItemViewModel
                                    {
                                        Name = Furniture.Name,
                                        Price = Furniture.Price,
                                        Quantity = OrderItem.Quantity,
                                        Description = Furniture.Description,
                                        Photo = Furniture.Image
                                    });
                                }
                            }
                            var OrderState = context.OrderStates.Where(os => os.Id == Order.OrderStateId).FirstOrDefault();
                            var OrderStateName = OrderState != null ? OrderState.Name : "";
                            OrderViewModel.Add(new OrderViewModel
                            {
                                Id = Order.Id,
                                DateCreated = Order.DateCreated,
                                Name = Order.Name,
                                Address = Order.Address,
                                Phone = Order.Phone,
                                Comment = Order.Comment,
                                Price = Order.TotalPrice,
                                OrderState = OrderStateName,
                                OrderItems = OrderItemViewModel
                            });
                        }
                    }
                    return OrderViewModel;   
                }
            }
            return null;
        }

        public async Task Order(OrderModel order)
        {
            if (order.Name != null && order.Address != null && order.Phone != null)
            {
                var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
                if (user != null)
                {
                    var Cart = context.Carts.Where(c => c.UserId == user.Id).FirstOrDefault();
                    if (Cart != null)
                    {
                        var CartItems = context.CartItems.Where(ci => ci.CartId == Cart.Id).ToList();
                        if (CartItems.Count > 0)
                        {
                            var orderdb = new Order
                            {
                                UserId = user.Id,
                                OrderStateId = 1,
                                DateCreated = DateTime.Now,
                                TotalPrice = (int)CartItems.Sum(ci => ci.Quantity * context.Furnitures.Where(f => f.Id == ci.FurnitureId).FirstOrDefault().Price),
                                Name = order.Name,
                                Address = order.Address,
                                Phone = order.Phone,
                                Comment = order.Comment
                            };
                            context.Orders.Add(orderdb);
                            await context.SaveChangesAsync();
                            var Order = context.Orders.Where(o => o.UserId == user.Id && o.DateCreated == orderdb.DateCreated).FirstOrDefault();
                            if (Order != null)
                            {
                                foreach (var CartItem in CartItems)
                                {
                                    var Item = context.Furnitures.Where(f => f.Id == CartItem.FurnitureId).FirstOrDefault();
                                    if (Item != null)
                                    {
                                        var OrderItem = new OrderItem
                                        {
                                            OrderId = Order.Id,
                                            FurnitureId = Item.Id,
                                            Quantity = CartItem.Quantity
                                        };
                                        context.OrderItems.Add(OrderItem);
                                        context.CartItems.Remove(CartItem);
                                    }
                                }
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

        public List<OrderViewModel> AdminOrders()
        {
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            if (user != null)
            {
                var isAdmin = userManager.IsInRoleAsync(user, "Administrator").Result;
                var isMaster = userManager.IsInRoleAsync(user, "Master").Result;
                if (isAdmin || isMaster)
                {
                    var Orders = context.Orders.ToList();
                    if (Orders.Count > 0)
                    {
                        var OrderViewModel = new List<OrderViewModel>();
                        foreach (var Order in Orders)
                        {
                            var OrderItems = context.OrderItems.Where(oi => oi.OrderId == Order.Id).ToList();
                            if (OrderItems.Count > 0)
                            {
                                var OrderItemViewModel = new List<OrderItemViewModel>();
                                foreach (var OrderItem in OrderItems)
                                {
                                    var Furniture = context.Furnitures.Where(f => f.Id == OrderItem.FurnitureId).FirstOrDefault();
                                    if (Furniture != null)
                                    {
                                        OrderItemViewModel.Add(new OrderItemViewModel
                                        {
                                            Name = Furniture.Name,
                                            Price = Furniture.Price,
                                            Quantity = OrderItem.Quantity,
                                            Description = Furniture.Description,
                                            Photo = Furniture.Image
                                        });
                                    }
                                }
                                var OrderState = context.OrderStates.Where(os => os.Id == Order.OrderStateId).FirstOrDefault();
                                var OrderStateName = OrderState != null ? OrderState.Name : "";
                                OrderViewModel.Add(new OrderViewModel
                                {
                                    Id = Order.Id,
                                    DateCreated = Order.DateCreated,
                                    Name = Order.Name,
                                    Address = Order.Address,
                                    Phone = Order.Phone,
                                    Comment = Order.Comment,
                                    Price = Order.TotalPrice,
                                    OrderState = OrderStateName,
                                    OrderItems = OrderItemViewModel
                                });
                            }
                        }
                        return OrderViewModel;
                    }
                }
            }
            return null;
        }

        public async Task<bool> ChangeOrderStateAsync(int id, int newState)
        {
            var user = userManager.FindByNameAsync(httpContextAccessor.HttpContext.User.Identity.Name).Result;
            if (user == null)
            {
                return false;
            }
            var isMaster = await userManager.IsInRoleAsync(user, "Master");
            var isAdmin = await userManager.IsInRoleAsync(user, "Administrator");
            var isUser = await userManager.IsInRoleAsync(user, "User");

            var order = context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return false;
            }

            if (isUser && !CanUserChangeState(order.OrderStateId, newState))
            {
                return false;
            }

            if ((isAdmin || isMaster) && !CanAdminChangeState(order.OrderStateId, newState))
            {
                return false;
            }

            order.OrderStateId = newState;
            context.Orders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        private bool CanUserChangeState(int currentStateId, int newStateId)
        {
            return CheckStatus(currentStateId, newStateId, userRole: true);
        }

        private bool CanAdminChangeState(int currentStateId, int newStateId)
        {
            return CheckStatus(currentStateId, newStateId, userRole: false);
        }

        private bool CheckStatus(int oldId, int newId, bool userRole)
        {
            if (oldId == 2 || oldId == 3) 
            {
                return false;
            }

            if (userRole)
            {
                if ((oldId == 4 && newId == 2) || (oldId == 6 && newId == 7))
                {
                    return true;
                }
                return false;
            }

            if ((oldId == 4 && (newId == 2 || newId == 3)) ||
                (oldId == 5 && (newId == 2 || newId == 3 || newId == 4)))
            {
                return false;
            }

            if (oldId == 6 && newId == 7)
            {
                return true;
            }

            if (oldId == 6 || oldId == 7)
            {
                return false;
            }

            return true;
        }
    }
}
