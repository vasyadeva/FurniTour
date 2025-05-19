using FurniTour.Server.Models.Chat;

namespace FurniTour.Server.Interfaces
{
    public interface IChatService
    {
        Task<MessageDTO> SendMessageAsync(string senderId, SendMessageDTO messageDto);
        Task<List<MessageDTO>> GetMessagesAsync(int conversationId, int page = 1, int pageSize = 20);
        Task<List<ConversationDTO>> GetConversationsAsync(string userId);
        Task<ConversationDTO?> GetConversationAsync(int conversationId);
        Task<ConversationDTO?> GetConversationByUsersAsync(string user1Id, string user2Id);
        Task<bool> MarkMessagesAsReadAsync(int conversationId, string userId);
        Task<bool> CanCommunicateAsync(string senderId, string receiverId);
        Task<List<UserOnlineDTO>> GetOnlineUsersAsync(string currentUserId);
        Task<List<UserOnlineDTO>> GetAllCommunicableUsersAsync(string currentUserId);
        Task<List<UserOnlineDTO>> SearchUsersAsync(string currentUserId, string searchTerm);
        Task<int> GetUnreadCountAsync(string userId);
        void UpdateUserActivity(string userId);
    }
} 