using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThyroCareX.Bases;
using ThyroCareX.Data.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : AppControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("History/{user1Id}/{user2Id}")]
        public async Task<IActionResult> GetHistory(string user1Id, string user2Id)
        {
            var history = await _chatService.GetChatHistoryAsync(user1Id, user2Id);
            return Ok(history);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveMessage([FromBody] Message message)
        {
            if (message == null) return BadRequest();
            await _chatService.SaveMessageAsync(message);
            return Ok();
        }

        [HttpPost("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromQuery] string receiverId, [FromQuery] string senderId)
        {
            await _chatService.MarkAsReadAsync(receiverId, senderId);
            return Ok();
        }

        [HttpGet("UnreadCount/{userId}")]
        public async Task<IActionResult> GetUnreadCount(string userId)
        {
            var count = await _chatService.GetUnreadCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("Notifications/{userId}")]
        public async Task<IActionResult> GetNotifications(string userId)
        {
            var notifications = await _chatService.GetRecentNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image, [FromForm] string senderId, [FromForm] string receiverId)
        {
            if (image == null || image.Length == 0) return BadRequest("No image uploaded");

            var fileName = $"{Guid.NewGuid()}_{image.FileName}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/chat", fileName);
            
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var imageUrl = $"/uploads/chat/{fileName}";
            return Ok(new { imageUrl });
        }
    }
}
