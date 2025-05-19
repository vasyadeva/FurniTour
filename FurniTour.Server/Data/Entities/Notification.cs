using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FurniTour.Server.Data.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
        
        public string Title { get; set; }
        
        public string Message { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsRead { get; set; } = false;
        
        // Type of notification allows filtering and categorization
        public NotificationType NotificationType { get; set; }
        
        // Optional reference IDs to link to the relevant items
        public int? OrderId { get; set; }
        
        public int? IndividualOrderId { get; set; }
        
        public int? GuaranteeId { get; set; }
        
        // URL to navigate to when clicking the notification
        public string? RedirectUrl { get; set; }
    }

    public enum NotificationType
    {
        Order,              // Regular order updates
        IndividualOrder,    // Individual order updates
        Guarantee,          // Guarantee request updates
        System,             // System notifications
        Other               // Other types
    }
} 