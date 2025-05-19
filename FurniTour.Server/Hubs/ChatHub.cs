using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Chat;
using FurniTour.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace FurniTour.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IAuthService _authService;
        private static readonly ConcurrentDictionary<string, string> _userConnectionMap = new();

        public ChatHub(IChatService chatService, IAuthService authService)
        {
            _chatService = chatService;
            _authService = authService;

            // Share the connection dictionary with the chat service if possible
            if (chatService is ChatService concreteChatService)
            {
                concreteChatService.SetConnectionMap(_userConnectionMap);
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userId = _authService.GetUser().Id;
            if (!string.IsNullOrEmpty(userId))
            {
                // Log for debugging
                Console.WriteLine($"User {userId} connected with connection ID {Context.ConnectionId}");
                
                // Store the connection
                _userConnectionMap[userId] = Context.ConnectionId;
                
                // Update user online status
                if (_chatService is ChatService chatService)
                {
                    chatService.UpdateUserActivity(userId);
                }

                // Get all users who can communicate with this user
                var allUsers = await _chatService.GetAllCommunicableUsersAsync(userId);
                
                // Notify them that this user is online
                foreach (var user in allUsers)
                {
                    if (_userConnectionMap.TryGetValue(user.UserId, out var connectionId))
                    {
                        Console.WriteLine($"Notifying user {user.UserId} that user {userId} is online");
                        await Clients.Client(connectionId).SendAsync("UserOnline", userId);
                    }
                }
                
                // Refresh online users for current user
                var onlineUsers = await _chatService.GetOnlineUsersAsync(userId);
                Console.WriteLine($"Sending {onlineUsers.Count} online users to {userId}");
                await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _authService.GetUser().Id;
            if (!string.IsNullOrEmpty(userId))
            {
                // Log for debugging
                Console.WriteLine($"User {userId} disconnected");
                
                // Remove the connection
                _userConnectionMap.TryRemove(userId, out _);

                // Get all users who can communicate with this user
                var allUsers = await _chatService.GetAllCommunicableUsersAsync(userId);
                
                // Notify them that this user is offline
                foreach (var user in allUsers)
                {
                    if (_userConnectionMap.TryGetValue(user.UserId, out var connectionId))
                    {
                        Console.WriteLine($"Notifying user {user.UserId} that user {userId} is offline");
                        await Clients.Client(connectionId).SendAsync("UserOffline", userId);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageDTO message)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            try
            {
                // Update user online status
                if (_chatService is ChatService chatService)
                {
                    chatService.UpdateUserActivity(userId);
                }

                var sentMessage = await _chatService.SendMessageAsync(userId, message);

                // Send to sender
                await Clients.Caller.SendAsync("ReceiveMessage", sentMessage);

                // Send to receiver if online
                if (_userConnectionMap.TryGetValue(message.ReceiverId, out var receiverConnectionId))
                {
                    await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", sentMessage);
                    await Clients.Client(receiverConnectionId).SendAsync("UpdateUnreadCount");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new HubException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new HubException($"Error sending message: {ex.Message}");
            }
        }

        public async Task MarkAsRead(int conversationId)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            try
            {
                // Update user online status
                if (_chatService is ChatService chatService)
                {
                    chatService.UpdateUserActivity(userId);
                }

                var success = await _chatService.MarkMessagesAsReadAsync(conversationId, userId);
                if (success)
                {
                    var conversation = await _chatService.GetConversationAsync(conversationId);
                    if (conversation != null)
                    {
                        var otherUserId = conversation.User1Id == userId ? conversation.User2Id : conversation.User1Id;
                        if (_userConnectionMap.TryGetValue(otherUserId, out var otherConnectionId))
                        {
                            await Clients.Client(otherConnectionId).SendAsync("MessagesRead", conversationId, userId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new HubException($"Error marking messages as read: {ex.Message}");
            }
        }

        public async Task JoinConversation(int conversationId)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            var conversation = await _chatService.GetConversationAsync(conversationId);
            if (conversation == null)
                throw new HubException("Conversation not found");

            if (conversation.User1Id != userId && conversation.User2Id != userId)
                throw new HubException("You are not part of this conversation");

            // Update user online status
            if (_chatService is ChatService chatService)
            {
                chatService.UpdateUserActivity(userId);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
            
            // Notify the other user that this user is online
            var otherUserId = conversation.User1Id == userId ? conversation.User2Id : conversation.User1Id;
            if (_userConnectionMap.TryGetValue(otherUserId, out var otherConnectionId))
            {
                await Clients.Client(otherConnectionId).SendAsync("UserOnline", userId);
            }
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }

        public async Task GetOnlineUsers()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            // Update user online status
            if (_chatService is ChatService chatService)
            {
                chatService.UpdateUserActivity(userId);
            }

            // Log current connection map for debugging
            Console.WriteLine($"Current connection map has {_userConnectionMap.Count} users online");
            foreach (var kvp in _userConnectionMap)
            {
                Console.WriteLine($"User {kvp.Key} is online with connection {kvp.Value}");
            }

            var onlineUsers = await _chatService.GetOnlineUsersAsync(userId);
            Console.WriteLine($"Sending {onlineUsers.Count} online users to {userId}");
            await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
        }

        public async Task Heartbeat()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return;

            // Log heartbeat for debugging
            Console.WriteLine($"Received heartbeat from user {userId}");
            
            // Make sure connection is in the dictionary
            if (!_userConnectionMap.ContainsKey(userId) || _userConnectionMap[userId] != Context.ConnectionId)
            {
                Console.WriteLine($"Updating connection for user {userId} to {Context.ConnectionId}");
                _userConnectionMap[userId] = Context.ConnectionId;
                
                // Since this is a reconnection, notify other users
                var allUsers = await _chatService.GetAllCommunicableUsersAsync(userId);
                foreach (var user in allUsers)
                {
                    if (_userConnectionMap.TryGetValue(user.UserId, out var connectionId))
                    {
                        await Clients.Client(connectionId).SendAsync("UserOnline", userId);
                    }
                }
            }
            
            // Update user activity timestamp
            if (_chatService is ChatService chatService)
            {
                chatService.UpdateUserActivity(userId);
            }
            
            // Send ping response to confirm the connection is working
            await Clients.Caller.SendAsync("HeartbeatResponse");
        }
    }
} 