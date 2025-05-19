using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurniTour.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IAuthService _authService;

        public ChatController(IChatService chatService, IAuthService authService)
        {
            _chatService = chatService;
            _authService = authService;
        }

        [HttpGet("conversations")]
        public async Task<ActionResult<List<ConversationDTO>>> GetConversations()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversations = await _chatService.GetConversationsAsync(userId);
            return Ok(conversations);
        }

        [HttpGet("conversation/{id}")]
        public async Task<ActionResult<ConversationDTO>> GetConversation(int id)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversation = await _chatService.GetConversationAsync(id);
            if (conversation == null)
                return NotFound();

            if (conversation.User1Id != userId && conversation.User2Id != userId)
                return Forbid();

            return Ok(conversation);
        }

        [HttpGet("conversation/user/{userId}")]
        public async Task<ActionResult<ConversationDTO>> GetConversationWithUser(string userId)
        {
            var currentUserId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            if (!await _chatService.CanCommunicateAsync(currentUserId, userId))
                return Forbid();

            var conversation = await _chatService.GetConversationByUsersAsync(currentUserId, userId);
            if (conversation == null)
                return NotFound();

            return Ok(conversation);
        }

        [HttpGet("messages/{conversationId}")]
        public async Task<ActionResult<List<MessageDTO>>> GetMessages(int conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversation = await _chatService.GetConversationAsync(conversationId);
            if (conversation == null)
                return NotFound();

            if (conversation.User1Id != userId && conversation.User2Id != userId)
                return Forbid();

            var messages = await _chatService.GetMessagesAsync(conversationId, page, pageSize);
            return Ok(messages);
        }

        [HttpPost("markAsRead/{conversationId}")]
        public async Task<IActionResult> MarkAsRead(int conversationId)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var conversation = await _chatService.GetConversationAsync(conversationId);
            if (conversation == null)
                return NotFound();

            if (conversation.User1Id != userId && conversation.User2Id != userId)
                return Forbid();

            var success = await _chatService.MarkMessagesAsReadAsync(conversationId, userId);
            return Ok(new { success });
        }

        [HttpGet("users/online")]
        public async Task<ActionResult<List<UserOnlineDTO>>> GetOnlineUsers()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var users = await _chatService.GetOnlineUsersAsync(userId);
            return Ok(users);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var count = await _chatService.GetUnreadCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("search-users")]
        public async Task<ActionResult<List<UserOnlineDTO>>> SearchUsers([FromQuery] string searchTerm)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return Ok(new List<UserOnlineDTO>());

            var users = await _chatService.SearchUsersAsync(userId, searchTerm);
            return Ok(users);
        }
    }
} 