using FurniTour.Server.Data.Entities;

namespace FurniTour.Server.Models.Notification
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Message { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public bool IsRead { get; set; }
        
        public string NotificationType { get; set; }
        
        public int? OrderId { get; set; }
        
        public int? IndividualOrderId { get; set; }
        
        public int? GuaranteeId { get; set; }
        
        public string? RedirectUrl { get; set; }
    }

    public class NotificationCountDTO
    {
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }

    public class CreateNotificationDTO
    {
        public string UserId { get; set; }
        
        public string Title { get; set; }
        
        public string Message { get; set; }
        
        public NotificationType NotificationType { get; set; }
        
        public int? OrderId { get; set; }
        
        public int? IndividualOrderId { get; set; }
        
        public int? GuaranteeId { get; set; }
        
        public string? RedirectUrl { get; set; }
    }
} 