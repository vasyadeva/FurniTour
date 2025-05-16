using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Guarantee;
using FurniTour.Server.Constants;
using Microsoft.EntityFrameworkCore;

namespace FurniTour.Server.Services
{
    public class GuaranteeService : IGuaranteeService
    {
        private readonly ApplicationDbContext context;
        private readonly IAuthService authService;
        private readonly ILogger<GuaranteeService> logger;

        public GuaranteeService(ApplicationDbContext context, IAuthService authService, ILogger<GuaranteeService> logger)
        {
            this.context = context;
            this.authService = authService;
            this.logger = logger;
        }

        public string AddGuarantee(GuaranteeAddModel guarantee)
        {
            if (guarantee == null)
            {
                return "Query cannot be empty";
            }
            else
            {
                var user = authService.GetUser();
                if (user == null)
                {
                    return "User not found";
                }
                else
                {
                    // Verify order exists
                    var order = context.Orders.Find(guarantee.OrderId);
                    if (order == null)
                    {
                        return "Order not found";
                    }
                    else
                    {
                        if (!IsGuaranteeValid(guarantee.OrderId)) return "Time for guarantee service has expired";
                        
                        // Create guarantee 
                        var guaranteeEntity = new Guarantee
                        {
                            UserId = user.Id,
                            OrderId = guarantee.OrderId,
                            Status = GuaranteeStatusConst.Pending,
                            Comment = guarantee.Comment,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now
                        };
                        context.Guarantees.Add(guaranteeEntity);
                        context.SaveChanges();
                        
                        // Check if Items collection is valid
                        if (guarantee.Items == null || guarantee.Items.Count == 0)
                        {
                            return "No items selected for guarantee";
                        }

                        // Filter out null or invalid items and check if any remain
                        var validItems = guarantee.Items.Where(i => i > 0).ToList();
                        if (validItems.Count == 0)
                        {
                            return "No valid items selected for guarantee";
                        }

                        // Process each valid item
                        int successCount = 0;
                        foreach (var itemId in validItems)
                        {
                            // Explicitly log the orderItem lookup for debugging
                            logger.LogInformation($"Searching for OrderItem with ID: {itemId}");
                            
                            var orderItem = context.OrderItems.Find(itemId);
                            if (orderItem != null)
                            {
                                logger.LogInformation($"Found OrderItem: {orderItem.Id}, FurnitureId: {orderItem.FurnitureId}");
                                
                                var guaranteeItem = new GuaranteeItems
                                {
                                    GuaranteeId = guaranteeEntity.Id,
                                    OrderItemId = orderItem.Id
                                };
                                context.GuaranteeItems.Add(guaranteeItem);
                                successCount++;
                            }
                            else
                            {
                                logger.LogWarning($"OrderItem with ID {itemId} not found");
                            }
                        }
                        
                        if (successCount == 0)
                        {
                            return "Could not find any valid order items with the provided IDs";
                        }
                        
                        context.SaveChanges();
                        
                        // Process photos
                        if (guarantee.Photos != null && guarantee.Photos.Any())
                        {
                            foreach (var photo in guarantee.Photos)
                            {
                                var photoEntity = new GuaranteePhoto
                                {
                                    GuaranteeId = guaranteeEntity.Id,
                                    Photo = Convert.FromBase64String(photo)
                                };
                                context.GuaranteePhotos.Add(photoEntity);
                            }
                            context.SaveChanges();
                        }
                        
                        return string.Empty;
                    }
                }
            }
        }

        public async Task<GuaranteeModel> GetGuarantee(int guaranteeId)
        {
            var guarantee = await context.Guarantees
                .Include(c => c.GuaranteePhotos)
                .Include(g => g.GuaranteeItems)
                .ThenInclude(gi => gi.OrderItem)
                .ThenInclude(oi => oi.Furniture)
                .FirstOrDefaultAsync(g => g.Id == guaranteeId);

            if (guarantee == null)
            {
                return null;
            }
            else
            {
                var user = guarantee.UserId != null ? await authService.GetUserById(guarantee.UserId) : null;
                var model = new GuaranteeModel
                {
                    Id = guarantee.Id,
                    UserName = user == null ? null : user.UserName,
                    OrderId = guarantee.OrderId,
                    Status = guarantee.Status,
                    Comment = guarantee.Comment,
                    DateCreated = guarantee.DateCreated,
                    DateModified = guarantee.DateModified,
                    Items = new List<GuaranteeItemModel>(),
                    Photos = new List<string>()
                };
                if (guarantee.GuaranteeItems != null)
                {
                    foreach (var item in guarantee.GuaranteeItems)
                    {
                        model.Items.Add(new GuaranteeItemModel
                        {
                            Id = item.Id,
                            FurnitureId = item.OrderItem.FurnitureId,
                            FurnitureName = item.OrderItem.Furniture.Name,
                            Quantity = item.OrderItem.Quantity
                        });
                    }
                }
                if (guarantee.GuaranteePhotos != null)
                {
                    foreach (var photo in guarantee.GuaranteePhotos)
                    {
                        model.Photos.Add(Convert.ToBase64String(photo.Photo));
                    }
                }
                return model;
            }
        }

        public async Task<List<GuaranteeModel>> GetGuarantees()
        {
            var guarantees = await context.Guarantees
                    .Include(c => c.GuaranteePhotos)
                    .Include(g => g.GuaranteeItems)
                    .ThenInclude(gi => gi.OrderItem)
                    .ThenInclude(oi => oi.Furniture)
                    .ToListAsync();

            var models = new List<GuaranteeModel>();
            foreach (var guarantee in guarantees)
            {
                var user = guarantee.UserId != null ? await authService.GetUserById(guarantee.UserId) : null;
                var model = new GuaranteeModel
                {
                    Id = guarantee.Id,
                    UserName = user == null ? null : user.UserName,
                    OrderId = guarantee.OrderId,
                    Status = guarantee.Status,
                    Comment = guarantee.Comment,
                    DateCreated = guarantee.DateCreated,
                    DateModified = guarantee.DateModified,
                    Items = new List<GuaranteeItemModel>(),
                    Photos = new List<string>()
                };
                foreach (var item in guarantee.GuaranteeItems)
                {
                    model.Items.Add(new GuaranteeItemModel
                    {
                        Id = item.Id,
                        FurnitureId = item.OrderItem.FurnitureId,
                        FurnitureName = item.OrderItem.Furniture.Name,
                        Quantity = item.OrderItem.Quantity
                    });
                }
                foreach (var photo in guarantee.GuaranteePhotos)
                {
                    model.Photos.Add(Convert.ToBase64String(photo.Photo));
                }
                models.Add(model);
            }
            return models;
        }

        public List<GuaranteeModel> GetMyGuarantees()
        {
            var user = authService.GetUser();
            if (user == null)
            {
                return null;
            }
            else
            {
                var guarantees = context.Guarantees.Where(g => g.UserId == user.Id).
                    Include(c => c.GuaranteePhotos).
                    Include(g => g.GuaranteeItems)
                    .ThenInclude(gi => gi.OrderItem)
                    .ThenInclude(oi => oi.Furniture).
                    ToList();
                var models = new List<GuaranteeModel>();
                foreach (var guarantee in guarantees)
                {
                    var model = new GuaranteeModel
                    {
                        Id = guarantee.Id,
                        UserName = guarantee.User?.UserName ?? string.Empty,
                        OrderId = guarantee.OrderId,
                        Status = guarantee.Status,
                        Comment = guarantee.Comment,
                        DateCreated = guarantee.DateCreated,
                        DateModified = guarantee.DateModified,
                        Items = new List<GuaranteeItemModel>(),
                        Photos = new List<string>()
                    };
                    foreach (var item in guarantee.GuaranteeItems)
                    {
                        model.Items.Add(new GuaranteeItemModel
                        {
                            Id = item.Id,
                            FurnitureId = item.OrderItem.FurnitureId,
                            FurnitureName = item.OrderItem.Furniture.Name,
                            Quantity = item.OrderItem.Quantity
                        });
                    }
                    foreach (var photo in guarantee.GuaranteePhotos)
                    {
                    }
                    models.Add(model);
                }
                return models;
            }
        }

        // Fixed method to check order date instead of guarantee date
        public bool IsGuaranteeValid(int orderId)
        {
            // Find the order with the given ID
            var order = context.Orders.Find(orderId);
            
            if (order == null)
            {
                logger.LogWarning($"Order with ID {orderId} not found when checking guarantee validity");
                return false;
            }
            
            // Check if the order is recent enough for guarantee service
            // Orders should be less than 365 days old to be eligible for guarantee
            var daysFromOrder = (DateTime.Now - order.DateCreated).TotalDays;
            
            logger.LogInformation($"Order {orderId} was created {daysFromOrder} days ago");
            
            // Orders should be less than 365 days old
            var isValid = daysFromOrder <= 365;
            
            if (!isValid)
            {
                logger.LogWarning($"Order {orderId} is too old for guarantee service (created {daysFromOrder} days ago)");
            }
            
            return isValid;
        }

        public void UpdateGuarantee(int guaranteeId, string status)
        {
            var guar = context.Guarantees.Find(guaranteeId);
            if (guar != null)
            {
                guar.Status = status;
                guar.DateModified = DateTime.Now;
                context.SaveChanges();
            }
        }
    }
}
