using FurniTour.Server.Constants;
using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Order;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FurniTour.Server.Services
{
    public class OrderFurnitureService : IOrderFurnitureService
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IAuthService authService;

        public OrderFurnitureService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IAuthService authService) {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.authService = authService;
        }


        public List<OrderViewModel> MyOrders()
        {
            var user = authService.GetUser();
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

        public async Task<string> Order(OrderModel order)
        {
            if (order.Name != null && order.Address != null && order.Phone != null)
            {
                var user = authService.GetUser();
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
                            return "Some error occurred while making order";
                        }
                        return "Cart is empty";
                    }
                    return "Cart is empty";
                }
                return "You are not logged in";
            }
            return "The form information is not valid";
        }

        public List<OrderViewModel> AdminOrders()
        {
            var check = authService.CheckRoleMasterOrAdmin();
            if (check.IsNullOrEmpty())
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
            return null;
        }

        public async Task<string> ChangeOrderStateAsync(int id, int newState)
        {
            var isAuth = authService.CheckRoleMasterOrAdmin();
            if (isAuth.IsNullOrEmpty())
            {


                var isMaster = authService.IsRole(Roles.Master);
                var isAdmin = authService.IsRole(Roles.Administrator);
                var isUser = authService.IsRole(Roles.User);

                var order = context.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null)
                {
                    return "Order not found";
                }

                if (isUser && !CanUserChangeState(order.OrderStateId, newState))
                {
                    return "Can't change state of the order from the previous one";
                }

                if ((isAdmin || isMaster) && !CanAdminChangeState(order.OrderStateId, newState))
                {
                    return "Can't change state of the order from the previous one";
                }

                order.OrderStateId = newState;
                context.Orders.Update(order);
                await context.SaveChangesAsync();
                return string.Empty;
            }
            return isAuth;
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
