using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FurniTour.Server.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;
        private static readonly ConcurrentDictionary<string, string> _userConnectionMap = new();

        public NotificationHub(IAuthService authService, INotificationService notificationService)
        {
            _authService = authService;
            _notificationService = notificationService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _authService.GetUser().Id;
            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"User {userId} connected to NotificationHub with connection ID {Context.ConnectionId}");
                
                // Store the connection
                _userConnectionMap[userId] = Context.ConnectionId;
                
                // Send initial unread count
                var counts = await _notificationService.GetNotificationCountsAsync(userId);
                await Clients.Caller.SendAsync("NotificationCounts", counts);
                
                // Get and send recent unread notifications
                var unreadNotifications = await _notificationService.GetUnreadNotificationsAsync(userId);
                await Clients.Caller.SendAsync("UnreadNotifications", unreadNotifications);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _authService.GetUser().Id;
            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"User {userId} disconnected from NotificationHub");
                _userConnectionMap.TryRemove(userId, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetNotifications(int page = 1, int pageSize = 20)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            var notifications = await _notificationService.GetNotificationsAsync(userId, page, pageSize);
            await Clients.Caller.SendAsync("ReceiveNotifications", notifications);
        }

        public async Task MarkAsRead(int notificationId)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            var success = await _notificationService.MarkAsReadAsync(notificationId);
            if (success)
            {
                // Update notification counts
                var counts = await _notificationService.GetNotificationCountsAsync(userId);
                await Clients.Caller.SendAsync("NotificationCounts", counts);
            }
        }

        public async Task MarkAllAsRead()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            var success = await _notificationService.MarkAllAsReadAsync(userId);
            if (success)
            {
                // Update notification counts
                var counts = await _notificationService.GetNotificationCountsAsync(userId);
                await Clients.Caller.SendAsync("NotificationCounts", counts);
            }
        }

        // Static method to send notification to a specific user
        public static async Task SendNotificationToUser(IHubContext<NotificationHub> hubContext, string userId, NotificationDTO notification, INotificationService notificationService)
        {
            if (_userConnectionMap.TryGetValue(userId, out var connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("NewNotification", notification);

                // Also update counts
                if (notificationService != null)
                {
                    var counts = await notificationService.GetNotificationCountsAsync(userId);
                    await hubContext.Clients.Client(connectionId).SendAsync("NotificationCounts", counts);
                }
            }
        }
    }
} 