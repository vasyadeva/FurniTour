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

                                    OrderItemViewModel.Add(new OrderItemViewModel
                                    {
                                        Name = Furniture.Name,
                                        Price = Furniture.Price,
                                        Quantity = OrderItem.Quantity,
                                        Description = Furniture.Description,
                                        //Photo = Furniture.Image,
                                        Master = Master,
                                        Manufacturer = Manufacturer,
                                        Category = category,
                                        Color = color
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

                                    OrderItemViewModel.Add(new OrderItemViewModel
                                    {
                                        Name = Furniture.Name,
                                        Price = Furniture.Price,
                                        Quantity = OrderItem.Quantity,
                                        Description = Furniture.Description,
                                       // Photo = Furniture.Image,
                                        Master = Master,
                                        Manufacturer = Manufacturer,
                                        Category = category,
                                        Color = color
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
            var isAuth = authService.IsAuthenticated();
            if (isAuth.IsNullOrEmpty())
            {
                var isMaster = authService.IsRole(Roles.Master);
                var isAdmin = authService.IsRole(Roles.Administrator);
                var isUser = authService.IsRole(Roles.User);

                var order = context.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null)
                {
                    return "Замовлення не знайдено.";
                }

                var validationMessage = ValidateStateChange(order.OrderStateId, newState, isUser, isAdmin || isMaster);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    return validationMessage;
                }

                order.OrderStateId = newState;
                context.Orders.Update(order);
                await context.SaveChangesAsync();
                return string.Empty;
            }
            return "Користувач неавторизований.";
        }

        private string ValidateStateChange(int currentStateId, int newStateId, bool isUser, bool isAdminOrMaster)
        {
            if (isUser)
            {
                var userValidationMessage = CheckStatusMessage(currentStateId, newStateId, userRole: true);
                if (!string.IsNullOrEmpty(userValidationMessage))
                {
                    return userValidationMessage;
                }
            }

            if (isAdminOrMaster)
            {
                var adminValidationMessage = CheckStatusMessage(currentStateId, newStateId, userRole: false);
                if (!string.IsNullOrEmpty(adminValidationMessage))
                {
                    return adminValidationMessage;
                }
            }

            return string.Empty;
        }

        private string CheckStatusMessage(int oldId, int newId, bool userRole)
        {
            if (oldId == 2 || oldId == 3)
            {
                return "Неможливо змінити статус, оскільки замовлення вже скасоване.";
            }

            if (userRole)
            {
                if (oldId == 4 && newId == 2)
                {
                    return "Користувач може скасувати замовлення лише після підтвердження.";
                }
                if (oldId == 6 && newId == 7)
                {
                    return string.Empty;
                }
                return "Користувач не має прав для зміни статусу цього замовлення.";
            }

            if (oldId == 4 && (newId == 2 || newId == 3))
            {
                return "Адміністратор або майстер має спершу підтвердити замовлення перед скасуванням.";
            }
            if (oldId == 5 && (newId == 2 || newId == 3 || newId == 4))
            {
                return "Неможливо змінити статус доставки на попередній.";
            }
            if (oldId == 6 && newId == 7)
            {
                return string.Empty;
            }
            if (oldId == 6 || oldId == 7)
            {
                return "Неможливо змінити статус замовлення після підтвердження доставки.";
            }

            return string.Empty;
        }

    }
}
