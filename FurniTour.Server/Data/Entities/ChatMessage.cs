using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FurniTour.Server.Data.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        public bool IsRead { get; set; } = false;
        
        [Required]
        public string SenderId { get; set; } = string.Empty;
        
        [ForeignKey(nameof(SenderId))]
        public IdentityUser Sender { get; set; } = null!;
        
        [Required]
        public string ReceiverId { get; set; } = string.Empty;
        
        [ForeignKey(nameof(ReceiverId))]
        public IdentityUser Receiver { get; set; } = null!;
        
        public int? ConversationId { get; set; }
        
        [ForeignKey(nameof(ConversationId))]
        public Conversation? Conversation { get; set; }
    }
} 