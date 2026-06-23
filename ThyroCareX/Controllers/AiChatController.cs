using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThyroCareX.Bases;
using ThyroCareX.Core.Feature.AiChat.Commands.Models;
using ThyroCareX.Infrastructure.Abstarct;
using ThyroCareX.Service.Abstarct;
using Microsoft.EntityFrameworkCore;

namespace ThyroCareX.Controllers
{
    public class ChatRequestDto
    {
        public string user_message { get; set; }
        public string? session_id { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Admin")]
    public class AiChatController : AppControllerBase
    {
        private readonly ISubscriptionPlanRepo _subscriptionPlanRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUserContextService _userContextService;
        private readonly IAIService _aiService;

        public AiChatController(ISubscriptionPlanRepo subscriptionPlanRepo, IDoctorRepository doctorRepo, IUserContextService userContextService, IAIService aiService)
        {
            _subscriptionPlanRepo = subscriptionPlanRepo;
            _doctorRepo = doctorRepo;
            _userContextService = userContextService;
            _aiService = aiService;
        }

        private async Task<bool> IsPremiumDoctorOrAdmin()
        {
            if (User.IsInRole("Admin")) return true;

            var userIdString = _userContextService.UserId;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                return false;

            var doctor = await _doctorRepo.GetTableNoTracking().FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null) return false;

            var hasActiveSub = await _subscriptionPlanRepo.GetTableNoTracking()
                .AnyAsync(s => s.DoctorId == doctor.DoctorID && s.Status == Data.Enums.SubscriptionStatus.Active && s.EndDate > DateTime.UtcNow);

            return hasActiveSub;
        }

        /// <summary>
        /// Chats with the ThyraX AI medical assistant (Streaming).
        /// </summary>
        /// <param name="command">Chat parameters including query, session ID.</param>
        /// <returns>Streams SSE back to client.</returns>
        [HttpPost("Chat")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task Chat([FromBody] ChatRequestDto command)
        {
            if (!await IsPremiumDoctorOrAdmin())
            {
                Response.StatusCode = 403;
                await Response.WriteAsync("Active subscription required.");
                return;
            }

            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no";

            await foreach (var chunk in _aiService.StreamChatAsync(command.user_message, command.session_id))
            {
                await Response.WriteAsync(chunk);
                await Response.Body.FlushAsync();
            }
        }
    }
}
