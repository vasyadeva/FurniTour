using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Guarantee;
using Microsoft.EntityFrameworkCore;

namespace FurniTour.Server.Services
{
    public class GuaranteeService : IGuaranteeService
    {
        private readonly ApplicationDbContext context;
        private readonly IAuthService authService;

        public GuaranteeService(ApplicationDbContext context, IAuthService authService)
        {
            this.context = context;
            this.authService = authService;
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
                    var order = context.Orders.Find(guarantee.OrderId);
                    if (order == null)
                    {
                        return "Order not found";
                    }
                    else
                    {
                        if (!IsGuaranteeValid(guarantee.OrderId)) return "Time for guarantee service has expired";
                        var guaranteeEntity = new Guarantee
                        {
                            UserId = user.Id,
                            OrderId = guarantee.OrderId,
                            Status = "Pending",
                            Comment = guarantee.Comment,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now
                        };
                        context.Guarantees.Add(guaranteeEntity);
                        context.SaveChanges();
                        foreach (var item in guarantee.Items)
                        {
                            var orderItem = context.OrderItems.Find(item);
                            if (orderItem != null)
                            {
                                var guaranteeItem = new GuaranteeItems
                                {
                                    GuaranteeId = guaranteeEntity.Id,
                                    OrderItemId = orderItem.Id
                                };
                                context.GuaranteeItems.Add(guaranteeItem);
                            }
                        }
                        context.SaveChanges();
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
                        model.Photos.Add(Convert.ToBase64String(photo.Photo));
                    }
                    models.Add(model);
                }
                return models;
            }
        }

        public bool IsGuaranteeValid(int guaranteeId)
        {
            var guarantee = context.Guarantees.Find(guaranteeId);
            if (guarantee == null)
            {
                return false;
            }
            else
            {
                return (DateTime.Now - guarantee.DateCreated).TotalDays <= 365;
            }
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
