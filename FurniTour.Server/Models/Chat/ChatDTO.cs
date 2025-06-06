using Microsoft.AspNetCore.Identity;

namespace FurniTour.Server.Models.Chat
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public int? ConversationId { get; set; }
        
        // Photo attachment properties
        public bool HasPhoto { get; set; }
        public string? PhotoContentType { get; set; }
        
        // The actual photo data will be retrieved separately through a dedicated endpoint to avoid
        // large data transfers in regular message lists
        public int? PhotoId => HasPhoto ? Id : null; // Use message ID to retrieve photo
    }

    public class ConversationDTO
    {
        public int Id { get; set; }
        public string User1Id { get; set; } = string.Empty;
        public string User1Name { get; set; } = string.Empty;
        public string User2Id { get; set; } = string.Empty;
        public string User2Name { get; set; } = string.Empty;
        public DateTime LastActivity { get; set; }
        public MessageDTO? LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }

    public class SendMessageDTO
    {
        public string ReceiverId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        
        // Photo data as Base64 string
        public string? PhotoData { get; set; }
        public string? PhotoContentType { get; set; }
        public bool HasPhoto => !string.IsNullOrEmpty(PhotoData);
    }

    public class UserOnlineDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
} 