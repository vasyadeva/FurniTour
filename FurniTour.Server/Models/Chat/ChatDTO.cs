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
    }

    public class UserOnlineDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
} 