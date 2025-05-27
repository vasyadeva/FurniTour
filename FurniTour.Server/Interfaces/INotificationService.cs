using FurniTour.Server.Data.Entities;
using FurniTour.Server.Models.Notification;

namespace FurniTour.Server.Interfaces
{
    public interface INotificationService
    {
        // Create a new notification for a user
        Task<NotificationDTO> CreateNotificationAsync(CreateNotificationDTO notificationDto);
        
        // Create notifications for multiple users
        Task<List<NotificationDTO>> CreateNotificationsAsync(List<CreateNotificationDTO> notificationsDto);
        
        // Get all notifications for a user
        Task<List<NotificationDTO>> GetNotificationsAsync(string userId, int page = 1, int pageSize = 20);
        
        // Get unread notifications for a user
        Task<List<NotificationDTO>> GetUnreadNotificationsAsync(string userId);
          // Get notification by id
        Task<NotificationDTO?> GetNotificationByIdAsync(int notificationId);
        
        // Mark a notification as read
        Task<bool> MarkAsReadAsync(int notificationId);
        
        // Mark all notifications as read for a user
        Task<bool> MarkAllAsReadAsync(string userId);
        
        // Delete a notification
        Task<bool> DeleteNotificationAsync(int notificationId);
        
        // Get notification counts (total and unread)
        Task<NotificationCountDTO> GetNotificationCountsAsync(string userId);
        
        // Get notification counts by type
        Task<Dictionary<NotificationType, int>> GetNotificationCountsByTypeAsync(string userId);
        
        // Order changed notification
        Task NotifyOrderStatusChangedAsync(int orderId, string status);
        
        // Individual order changed notification
        Task NotifyIndividualOrderStatusChangedAsync(int individualOrderId, string status);
        
        // New individual order notification
        Task NotifyNewIndividualOrderAsync(int individualOrderId);
          // Guarantee status changed notification
        Task NotifyGuaranteeStatusChangedAsync(int guaranteeId, string status);
        
        // New order notification for admins
        Task NotifyNewOrderAsync(int orderId);
        
        // New guarantee notification for admins
        Task NotifyNewGuaranteeAsync(int guaranteeId);
    }
}