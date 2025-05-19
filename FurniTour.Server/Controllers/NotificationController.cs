using FurniTour.Server.Interfaces;
using FurniTour.Server.Models.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniTour.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;

        public NotificationController(INotificationService notificationService, IAuthService authService)
        {
            _notificationService = notificationService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationDTO>>> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var notifications = await _notificationService.GetNotificationsAsync(userId, page, pageSize);
            return Ok(notifications);
        }

        [HttpGet("unread")]
        public async Task<ActionResult<List<NotificationDTO>>> GetUnreadNotifications()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("counts")]
        public async Task<ActionResult<NotificationCountDTO>> GetNotificationCounts()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var counts = await _notificationService.GetNotificationCountsAsync(userId);
            return Ok(counts);
        }

        [HttpPost("mark-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound();

            // Security check
            if (notification.Id != id)
                return Forbid();

            var success = await _notificationService.MarkAsReadAsync(id);
            return Ok(new { success });
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { success });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = _authService.GetUser().Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
                return NotFound();

            // Security check
            if (notification.Id != id)
                return Forbid();

            var success = await _notificationService.DeleteNotificationAsync(id);
            return Ok(new { success });
        }
    }
} 