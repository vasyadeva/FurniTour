using FurniTour.Server.Data;
using FurniTour.Server.Data.Entities;
using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Chat;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace FurniTour.Server.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Dictionary<string, DateTime> _onlineUsers = new();
        private ConcurrentDictionary<string, string>? _connectionMap;

        public ChatService(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SetConnectionMap(ConcurrentDictionary<string, string> connectionMap)
        {
            _connectionMap = connectionMap;
        }

        private bool IsUserOnline(string userId)
        {
            // Use connection map as the primary way to determine online status
            // A user is online if they have an active SignalR connection
            return _connectionMap != null && _connectionMap.ContainsKey(userId);
        }

        public async Task<bool> CanCommunicateAsync(string senderId, string receiverId)
        {
            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
                return false;

            var sender = await _userManager.FindByIdAsync(senderId);
            var receiver = await _userManager.FindByIdAsync(receiverId);

            if (sender == null || receiver == null)
                return false;

            var senderRoles = await _userManager.GetRolesAsync(sender);
            var receiverRoles = await _userManager.GetRolesAsync(receiver);

            // Users can only message Masters and Administrators
            if (senderRoles.Any(r => r.Equals("User", StringComparison.OrdinalIgnoreCase)))
            {
                return receiverRoles.Any(r => r.Equals("Master", StringComparison.OrdinalIgnoreCase) || 
                                             r.Equals("Administrator", StringComparison.OrdinalIgnoreCase));
            }
            
            // Administrators can message Users and Masters
            if (senderRoles.Any(r => r.Equals("Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                return receiverRoles.Any(r => r.Equals("User", StringComparison.OrdinalIgnoreCase) || 
                                             r.Equals("Master", StringComparison.OrdinalIgnoreCase));
            }
            
            // Masters can message Administrators and Users
            if (senderRoles.Any(r => r.Equals("Master", StringComparison.OrdinalIgnoreCase)))
            {
                return receiverRoles.Any(r => r.Equals("Administrator", StringComparison.OrdinalIgnoreCase) || 
                                             r.Equals("User", StringComparison.OrdinalIgnoreCase));
            }

            // If no specific role rules match, allow communication by default
            return true;
        }

        public async Task<ConversationDTO?> GetConversationAsync(int conversationId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
                return null;

            var lastMessage = conversation.Messages.FirstOrDefault();
            var user1Roles = await _userManager.GetRolesAsync(conversation.User1);
            var user2Roles = await _userManager.GetRolesAsync(conversation.User2);

            var unreadCount = await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId && !m.IsRead)
                .CountAsync();

            return new ConversationDTO
            {
                Id = conversation.Id,
                User1Id = conversation.User1Id,
                User1Name = conversation.User1.UserName ?? "",
                User2Id = conversation.User2Id,
                User2Name = conversation.User2.UserName ?? "",
                LastActivity = conversation.LastActivity,
                LastMessage = lastMessage != null ? new MessageDTO
                {
                    Id = lastMessage.Id,
                    Content = lastMessage.Content,
                    SentAt = lastMessage.SentAt,
                    IsRead = lastMessage.IsRead,
                    SenderId = lastMessage.SenderId,
                    SenderName = (await _userManager.FindByIdAsync(lastMessage.SenderId))?.UserName ?? "",
                    ReceiverId = lastMessage.ReceiverId,
                    ReceiverName = (await _userManager.FindByIdAsync(lastMessage.ReceiverId))?.UserName ?? "",
                    ConversationId = lastMessage.ConversationId,
                    HasPhoto = lastMessage.HasPhoto,
                    PhotoContentType = lastMessage.PhotoContentType
                } : null,
                UnreadCount = unreadCount
            };
        }

        public async Task<ConversationDTO?> GetConversationByUsersAsync(string user1Id, string user2Id)
        {
            var conversation = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == user1Id && c.User2Id == user2Id) ||
                    (c.User1Id == user2Id && c.User2Id == user1Id));

            if (conversation == null)
                return null;

            var lastMessage = conversation.Messages.FirstOrDefault();
            var unreadCount = await _context.ChatMessages
                .Where(m => m.ConversationId == conversation.Id && !m.IsRead && m.ReceiverId == user1Id)
                .CountAsync();

            return new ConversationDTO
            {
                Id = conversation.Id,
                User1Id = conversation.User1Id,
                User1Name = conversation.User1.UserName ?? "",
                User2Id = conversation.User2Id,
                User2Name = conversation.User2.UserName ?? "",
                LastActivity = conversation.LastActivity,
                LastMessage = lastMessage != null ? new MessageDTO
                {
                    Id = lastMessage.Id,
                    Content = lastMessage.Content,
                    SentAt = lastMessage.SentAt,
                    IsRead = lastMessage.IsRead,
                    SenderId = lastMessage.SenderId,
                    SenderName = (await _userManager.FindByIdAsync(lastMessage.SenderId))?.UserName ?? "",
                    ReceiverId = lastMessage.ReceiverId,
                    ReceiverName = (await _userManager.FindByIdAsync(lastMessage.ReceiverId))?.UserName ?? "",
                    ConversationId = lastMessage.ConversationId,
                    HasPhoto = lastMessage.HasPhoto,
                    PhotoContentType = lastMessage.PhotoContentType
                } : null,
                UnreadCount = unreadCount
            };
        }

        public async Task<List<ConversationDTO>> GetConversationsAsync(string userId)
        {
            var conversations = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .OrderByDescending(c => c.LastActivity)
                .ToListAsync();

            var result = new List<ConversationDTO>();

            foreach (var conversation in conversations)
            {
                var lastMessage = conversation.Messages.FirstOrDefault();
                var unreadCount = await _context.ChatMessages
                    .Where(m => m.ConversationId == conversation.Id && !m.IsRead && m.ReceiverId == userId)
                    .CountAsync();

                result.Add(new ConversationDTO
                {
                    Id = conversation.Id,
                    User1Id = conversation.User1Id,
                    User1Name = conversation.User1.UserName ?? "",
                    User2Id = conversation.User2Id,
                    User2Name = conversation.User2.UserName ?? "",
                    LastActivity = conversation.LastActivity,
                    LastMessage = lastMessage != null ? new MessageDTO
                    {
                        Id = lastMessage.Id,
                        Content = lastMessage.Content,
                        SentAt = lastMessage.SentAt,
                        IsRead = lastMessage.IsRead,
                        SenderId = lastMessage.SenderId,
                        SenderName = (await _userManager.FindByIdAsync(lastMessage.SenderId))?.UserName ?? "",
                        ReceiverId = lastMessage.ReceiverId,
                        ReceiverName = (await _userManager.FindByIdAsync(lastMessage.ReceiverId))?.UserName ?? "",
                        ConversationId = lastMessage.ConversationId,
                        HasPhoto = lastMessage.HasPhoto,
                        PhotoContentType = lastMessage.PhotoContentType
                    } : null,
                    UnreadCount = unreadCount
                });
            }

            return result;
        }

        public async Task<List<MessageDTO>> GetMessagesAsync(int conversationId, int page = 1, int pageSize = 20)
        {
            // First, get the conversation to verify it exists
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                return new List<MessageDTO>();
            }

            // Now get messages that belong ONLY to this specific conversation
            var messages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<MessageDTO>();

            foreach (var message in messages)
            {
                // Additional security check - ensure the message is between the users in this conversation
                if (message.SenderId != conversation.User1Id && message.SenderId != conversation.User2Id ||
                    message.ReceiverId != conversation.User1Id && message.ReceiverId != conversation.User2Id)
                {
                    continue; // Skip messages that don't belong to this conversation's users
                }

                result.Add(new MessageDTO
                {
                    Id = message.Id,
                    Content = message.Content,
                    SentAt = message.SentAt,
                    IsRead = message.IsRead,
                    SenderId = message.SenderId,
                    SenderName = message.Sender.UserName ?? "",
                    ReceiverId = message.ReceiverId,
                    ReceiverName = message.Receiver.UserName ?? "",
                    ConversationId = message.ConversationId,
                    HasPhoto = message.HasPhoto,
                    PhotoContentType = message.PhotoContentType
                });
            }

            return result.OrderBy(m => m.SentAt).ToList();
        }

        public async Task<List<UserOnlineDTO>> GetAllCommunicableUsersAsync(string currentUserId)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
                return new List<UserOnlineDTO>();

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var allUsers = new List<IdentityUser>();

            if (currentUserRoles.Any(r => r.Equals("User", StringComparison.OrdinalIgnoreCase)))
            {
                var masterRole = await _roleManager.FindByNameAsync("Master");
                var adminRole = await _roleManager.FindByNameAsync("Administrator");
                
                if (masterRole != null)
                {
                    var masters = await _userManager.GetUsersInRoleAsync(masterRole.Name);
                    allUsers.AddRange(masters);
                }
                
                if (adminRole != null)
                {
                    var admins = await _userManager.GetUsersInRoleAsync(adminRole.Name);
                    allUsers.AddRange(admins);
                }
            }
            else if (currentUserRoles.Any(r => r.Equals("Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                var userRole = await _roleManager.FindByNameAsync("User");
                var masterRole = await _roleManager.FindByNameAsync("Master");
                
                if (userRole != null)
                {
                    var users = await _userManager.GetUsersInRoleAsync(userRole.Name);
                    allUsers.AddRange(users);
                }
                
                if (masterRole != null)
                {
                    var masters = await _userManager.GetUsersInRoleAsync(masterRole.Name);
                    allUsers.AddRange(masters);
                }
            }
            else if (currentUserRoles.Any(r => r.Equals("Master", StringComparison.OrdinalIgnoreCase)))
            {
                var userRole = await _roleManager.FindByNameAsync("User");
                var adminRole = await _roleManager.FindByNameAsync("Administrator");
                
                if (userRole != null)
                {
                    var users = await _userManager.GetUsersInRoleAsync(userRole.Name);
                    allUsers.AddRange(users);
                }
                
                if (adminRole != null)
                {
                    var admins = await _userManager.GetUsersInRoleAsync(adminRole.Name);
                    allUsers.AddRange(admins);
                }
            }

            if (!allUsers.Any())
            {
                allUsers = await _userManager.Users
                    .Where(u => u.Id != currentUserId)
                    .ToListAsync();
            }

            var result = new List<UserOnlineDTO>();

            foreach (var user in allUsers.Distinct())
            {
                if (user.Id == currentUserId)
                    continue;

                var roles = await _userManager.GetRolesAsync(user);
                var isOnline = IsUserOnline(user.Id);

                result.Add(new UserOnlineDTO
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? "",
                    Role = roles.FirstOrDefault() ?? "",
                    IsOnline = isOnline
                });
            }

            return result;
        }

        public async Task<List<UserOnlineDTO>> GetOnlineUsersAsync(string currentUserId)
        {
            var allUsers = await GetAllCommunicableUsersAsync(currentUserId);
            return allUsers.Where(u => u.IsOnline).ToList();
        }

        public async Task<bool> MarkMessagesAsReadAsync(int conversationId, string userId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId && m.ReceiverId == userId && !m.IsRead)
                .ToListAsync();

            if (!messages.Any())
                return false;

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MessageDTO> SendMessageAsync(string senderId, SendMessageDTO messageDto)
        {
            if (!await CanCommunicateAsync(senderId, messageDto.ReceiverId))
                throw new UnauthorizedAccessException("You cannot send messages to this user based on role restrictions.");

            var sender = await _userManager.FindByIdAsync(senderId);
            var receiver = await _userManager.FindByIdAsync(messageDto.ReceiverId);

            if (sender == null || receiver == null)
                throw new ArgumentException("Sender or receiver not found");

            // Get or create conversation
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == senderId && c.User2Id == messageDto.ReceiverId) ||
                    (c.User1Id == messageDto.ReceiverId && c.User2Id == senderId));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1Id = senderId,
                    User2Id = messageDto.ReceiverId,
                    LastActivity = DateTime.UtcNow
                };

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }
            else
            {
                conversation.LastActivity = DateTime.UtcNow;
                _context.Conversations.Update(conversation);
            }

            // Create message
            var message = new ChatMessage
            {
                Content = messageDto.Content,
                SenderId = senderId,
                ReceiverId = messageDto.ReceiverId,
                SentAt = DateTime.UtcNow,
                ConversationId = conversation.Id
            };

            // Add photo if provided as base64 string
            if (messageDto.HasPhoto && !string.IsNullOrEmpty(messageDto.PhotoData))
            {
                try
                {
                    message.PhotoData = Convert.FromBase64String(messageDto.PhotoData);
                    message.PhotoContentType = messageDto.PhotoContentType ?? "image/jpeg";
                    Console.WriteLine($"Photo received and processed. Size: {message.PhotoData.Length} bytes.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing photo: {ex.Message}");
                }
            }

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return new MessageDTO
            {
                Id = message.Id,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                SenderId = message.SenderId,
                SenderName = sender.UserName ?? "",
                ReceiverId = message.ReceiverId,
                ReceiverName = receiver.UserName ?? "",
                ConversationId = message.ConversationId,
                HasPhoto = message.HasPhoto,
                PhotoContentType = message.PhotoContentType
            };
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.ChatMessages
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .CountAsync();
        }

        public async Task<List<UserOnlineDTO>> SearchUsersAsync(string currentUserId, string searchTerm)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
                return new List<UserOnlineDTO>();

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var allUsers = new List<IdentityUser>();

            if (currentUserRoles.Any(r => r.Equals("User", StringComparison.OrdinalIgnoreCase)))
            {
                var masterRole = await _roleManager.FindByNameAsync("Master");
                var adminRole = await _roleManager.FindByNameAsync("Administrator");
                
                if (masterRole != null)
                {
                    var masters = await _userManager.GetUsersInRoleAsync(masterRole.Name);
                    allUsers.AddRange(masters);
                }
                
                if (adminRole != null)
                {
                    var admins = await _userManager.GetUsersInRoleAsync(adminRole.Name);
                    allUsers.AddRange(admins);
                }
            }
            else if (currentUserRoles.Any(r => r.Equals("Administrator", StringComparison.OrdinalIgnoreCase)))
            {
                var userRole = await _roleManager.FindByNameAsync("User");
                var masterRole = await _roleManager.FindByNameAsync("Master");
                
                if (userRole != null)
                {
                    var users = await _userManager.GetUsersInRoleAsync(userRole.Name);
                    allUsers.AddRange(users);
                }
                
                if (masterRole != null)
                {
                    var masters = await _userManager.GetUsersInRoleAsync(masterRole.Name);
                    allUsers.AddRange(masters);
                }
            }
            else if (currentUserRoles.Any(r => r.Equals("Master", StringComparison.OrdinalIgnoreCase)))
            {
                var userRole = await _roleManager.FindByNameAsync("User");
                var adminRole = await _roleManager.FindByNameAsync("Administrator");
                
                if (userRole != null)
                {
                    var users = await _userManager.GetUsersInRoleAsync(userRole.Name);
                    allUsers.AddRange(users);
                }
                
                if (adminRole != null)
                {
                    var admins = await _userManager.GetUsersInRoleAsync(adminRole.Name);
                    allUsers.AddRange(admins);
                }
            }

            if (!allUsers.Any())
            {
                allUsers = await _userManager.Users
                    .Where(u => u.Id != currentUserId)
                    .ToListAsync();
            }

            var filteredUsers = allUsers.Distinct()
                .Where(u => u.Id != currentUserId && 
                       (u.UserName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

            var result = new List<UserOnlineDTO>();

            foreach (var user in filteredUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isOnline = IsUserOnline(user.Id);

                result.Add(new UserOnlineDTO
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? "",
                    Role = roles.FirstOrDefault() ?? "",
                    IsOnline = isOnline
                });
            }

            return result;
        }

        public void UpdateUserActivity(string userId)
        {
            // Keep time-based tracking for backward compatibility or additional metrics
            // but don't rely on this for online status determination
            _onlineUsers[userId] = DateTime.UtcNow;
            
            // We primarily rely on _connectionMap which is directly updated by ChatHub
            // through OnConnectedAsync and OnDisconnectedAsync, not this method
        }

        public async Task<(bool HasPhoto, byte[]? PhotoData, string? PhotoContentType)> GetMessagePhotoAsync(int messageId)
        {
            var message = await _context.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null || !message.HasPhoto || message.PhotoData == null)
            {
                return (HasPhoto: false, PhotoData: null, PhotoContentType: null);
            }

            return (HasPhoto: true, PhotoData: message.PhotoData, PhotoContentType: message.PhotoContentType);
        }
        
        public async Task<MessageDTO> SendPhotoMessageAsync(string senderId, SendMessageDTO messageDto, byte[] photoData)
        {
            if (!await CanCommunicateAsync(senderId, messageDto.ReceiverId))
                throw new UnauthorizedAccessException("You cannot send messages to this user based on role restrictions.");

            var sender = await _userManager.FindByIdAsync(senderId);
            var receiver = await _userManager.FindByIdAsync(messageDto.ReceiverId);

            if (sender == null || receiver == null)
                throw new ArgumentException("Sender or receiver not found");

            // Get or create conversation
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c =>
                    (c.User1Id == senderId && c.User2Id == messageDto.ReceiverId) ||
                    (c.User1Id == messageDto.ReceiverId && c.User2Id == senderId));

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    User1Id = senderId,
                    User2Id = messageDto.ReceiverId,
                    LastActivity = DateTime.UtcNow
                };

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }
            else
            {
                conversation.LastActivity = DateTime.UtcNow;
                _context.Conversations.Update(conversation);
            }

            // Create message
            var message = new ChatMessage
            {
                Content = messageDto.Content,
                SenderId = senderId,
                ReceiverId = messageDto.ReceiverId,
                SentAt = DateTime.UtcNow,
                ConversationId = conversation.Id,
                PhotoData = photoData,
                PhotoContentType = messageDto.PhotoContentType ?? "image/jpeg"
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Photo message saved. Photo size: {photoData.Length} bytes");

            var result = new MessageDTO
            {
                Id = message.Id,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                SenderId = message.SenderId,
                SenderName = sender.UserName ?? "",
                ReceiverId = message.ReceiverId,
                ReceiverName = receiver.UserName ?? "",
                ConversationId = message.ConversationId,
                HasPhoto = message.HasPhoto,
                PhotoContentType = message.PhotoContentType
            };

            return result;
        }
    }
} 