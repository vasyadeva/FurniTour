using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Hubs;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Notification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FurniTour.Server.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public NotificationService(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IHubContext<NotificationHub> notificationHubContext)
        {
            _context = context;
            _userManager = userManager;
            _notificationHubContext = notificationHubContext;
        }

        public async Task<NotificationDTO> CreateNotificationAsync(CreateNotificationDTO notificationDto)
        {
            var notification = new Notification
            {
                UserId = notificationDto.UserId,
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                NotificationType = notificationDto.NotificationType,
                OrderId = notificationDto.OrderId,
                IndividualOrderId = notificationDto.IndividualOrderId,
                GuaranteeId = notificationDto.GuaranteeId,
                RedirectUrl = notificationDto.RedirectUrl
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var mappedNotificationDto = MapToDTO(notification);

            // Send real-time notification if user is online
            // Update the call to SendNotificationToUser to include the missing 'notificationService' parameter.
            await NotificationHub.SendNotificationToUser(_notificationHubContext, notification.UserId, mappedNotificationDto, this);


            return mappedNotificationDto;
        }

        public async Task<List<NotificationDTO>> CreateNotificationsAsync(List<CreateNotificationDTO> notificationsDto)
        {
            var notifications = new List<Notification>();
            foreach (var dto in notificationsDto)
            {
                notifications.Add(new Notification
                {
                    UserId = dto.UserId,
                    Title = dto.Title,
                    Message = dto.Message,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    NotificationType = dto.NotificationType,
                    OrderId = dto.OrderId,
                    IndividualOrderId = dto.IndividualOrderId,
                    GuaranteeId = dto.GuaranteeId,
                    RedirectUrl = dto.RedirectUrl
                });
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            var result = notifications.Select(MapToDTO).ToList();

            // Send real-time notifications to online users
            foreach (var notification in notifications)
            {
                var dto = MapToDTO(notification);
                await NotificationHub.SendNotificationToUser(_notificationHubContext, notification.UserId, dto, this);
            }

            return result;
        }

        public async Task<List<NotificationDTO>> GetNotificationsAsync(string userId, int page = 1, int pageSize = 20)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return notifications.Select(MapToDTO).ToList();
        }

        public async Task<List<NotificationDTO>> GetUnreadNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(10) // Limit to recent ones
                .ToListAsync();

            return notifications.Select(MapToDTO).ToList();
        }

        public async Task<NotificationDTO> GetNotificationByIdAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            return notification != null ? MapToDTO(notification) : null;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
                return false;

            notification.IsRead = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
                return false;

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
                return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<NotificationCountDTO> GetNotificationCountsAsync(string userId)
        {
            var totalCount = await _context.Notifications
                .Where(n => n.UserId == userId)
                .CountAsync();

            var unreadCount = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return new NotificationCountDTO
            {
                TotalCount = totalCount,
                UnreadCount = unreadCount
            };
        }

        public async Task<Dictionary<NotificationType, int>> GetNotificationCountsByTypeAsync(string userId)
        {
            var counts = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .GroupBy(n => n.NotificationType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToListAsync();

            return counts.ToDictionary(x => x.Type, x => x.Count);
        }

        public async Task NotifyOrderStatusChangedAsync(int orderId, string status)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return;

            var notificationDto = new CreateNotificationDTO
            {
                UserId = order.UserId,
                Title = "Статус замовлення оновлено",
                Message = $"Ваше замовлення №{orderId} змінило статус на: {status}",
                NotificationType = NotificationType.Order,
                OrderId = orderId,
                RedirectUrl = $"/myorders/{orderId}"
            };

            await CreateNotificationAsync(notificationDto);
        }

        public async Task NotifyIndividualOrderStatusChangedAsync(int individualOrderId, string status)
        {
            var order = await _context.IndividualOrders
                .Include(o => o.User)
                .Include(o => o.Master)
                .FirstOrDefaultAsync(o => o.Id == individualOrderId);

            if (order == null) return;

            // Notify the customer
            var customerNotification = new CreateNotificationDTO
            {
                UserId = order.UserId,
                Title = "Статус індивідуального замовлення оновлено",
                Message = $"Ваше індивідуальне замовлення №{individualOrderId} змінило статус на: {status}",
                NotificationType = NotificationType.IndividualOrder,
                IndividualOrderId = individualOrderId,
                RedirectUrl = $"/individual-orders/{individualOrderId}"
            };

            await CreateNotificationAsync(customerNotification);
        }

        public async Task NotifyNewIndividualOrderAsync(int individualOrderId)
        {
            var order = await _context.IndividualOrders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == individualOrderId);

            if (order == null) return;

            // Get all masters
            var masters = await _userManager.GetUsersInRoleAsync("Master");
            var admins = await _userManager.GetUsersInRoleAsync("Administrator");

            // Create notifications for all masters
            var notificationsDto = new List<CreateNotificationDTO>();
            foreach (var master in masters)
            {
                notificationsDto.Add(new CreateNotificationDTO
                {
                    UserId = master.Id,
                    Title = "Нове індивідуальне замовлення",
                    Message = $"Отримано нове індивідуальне замовлення №{individualOrderId} від {order.User.UserName}",
                    NotificationType = NotificationType.IndividualOrder,
                    IndividualOrderId = individualOrderId,
                    RedirectUrl = $"/admin-individual-orders/{individualOrderId}"
                });
            }

            // Notify admins too
            foreach (var admin in admins)
            {
                notificationsDto.Add(new CreateNotificationDTO
                {
                    UserId = admin.Id,
                    Title = "Нове індивідуальне замовлення",
                    Message = $"Отримано нове індивідуальне замовлення №{individualOrderId} від {order.User.UserName}",
                    NotificationType = NotificationType.IndividualOrder,
                    IndividualOrderId = individualOrderId,
                    RedirectUrl = $"/admin-individual-orders/{individualOrderId}"
                });
            }

            await CreateNotificationsAsync(notificationsDto);
        }

        public async Task NotifyGuaranteeStatusChangedAsync(int guaranteeId, string status)
        {
            var guarantee = await _context.Guarantees
                .Include(g => g.User)
                .FirstOrDefaultAsync(g => g.Id == guaranteeId);

            if (guarantee == null) return;

            var notificationDto = new CreateNotificationDTO
            {
                UserId = guarantee.UserId,
                Title = "Статус гарантійної заявки оновлено",
                Message = $"Ваша гарантійна заявка №{guaranteeId} змінила статус на: {status}",
                NotificationType = NotificationType.Guarantee,
                GuaranteeId = guaranteeId,
                RedirectUrl = $"/guarantees/{guaranteeId}"
            };

            await CreateNotificationAsync(notificationDto);
        }

        private NotificationDTO MapToDTO(Notification notification)
        {
            return new NotificationDTO
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                NotificationType = notification.NotificationType.ToString(),
                OrderId = notification.OrderId,
                IndividualOrderId = notification.IndividualOrderId,
                GuaranteeId = notification.GuaranteeId,
                RedirectUrl = notification.RedirectUrl
            };
        }
    }
} 