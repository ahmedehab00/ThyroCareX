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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor,Admin")]
    public class AiChatController : AppControllerBase
    {
        private readonly ISubscriptionPlanRepo _subscriptionPlanRepo;
        private readonly IDoctorRepository _doctorRepo;
        private readonly IUserContextService _userContextService;

        public AiChatController(ISubscriptionPlanRepo subscriptionPlanRepo, IDoctorRepository doctorRepo, IUserContextService userContextService)
        {
            _subscriptionPlanRepo = subscriptionPlanRepo;
            _doctorRepo = doctorRepo;
            _userContextService = userContextService;
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
        /// Chats with the ThyraX AI medical assistant. Supports direct file uploads.
        /// </summary>
        /// <param name="command">Chat parameters including query, session ID, and optional image.</param>
        /// <returns>AI response and tools used.</returns>
        [HttpPost("Chat")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat([FromForm] SendChatCommand command)
        {
            if (!await IsPremiumDoctorOrAdmin()) return StatusCode(403, "Active subscription required.");
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}
