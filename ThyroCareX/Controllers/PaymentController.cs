using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ThyroCareX.Bases;
using ThyroCareX.Core.Feature.Payment.Commands.Models;
using ThyroCareX.Core.Feature.Payment.Queries.Models;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : AppControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(CreatePaymentCommandRequest command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);

        }

        [HttpGet("return")]
        public async Task<IActionResult> Return([FromQuery] Dictionary<string, string> query, [FromServices] IPaymentService paymentService)
        {
            var success = query.ContainsKey("success") && query["success"] == "true";
            
            // For localhost development, webhook won't be reachable by Paymob servers.
            // So we manually pick up order & transaction ids from the redirect query string.
            if (query.TryGetValue("order", out var orderId) && query.TryGetValue("id", out var transactionId))
            {
                await paymentService.UpdateSubscriptionStatus(orderId, success, transactionId);
            }

            // Determine the frontend domain. Both the new domain and Vercel domain are supported.
            var frontendDomain = query.TryGetValue("source", out var source) && source == "vercel"
                ? "https://thyro-care-x-6jdn.vercel.app"
                : "https://thyrax.site";

            return success
                ? Redirect($"{frontendDomain}/payment/success")
                : Redirect($"{frontendDomain}/payment/failure");
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPaymentHistory()
        {
            var response = await Mediator.Send(new GetSubscriptionHistoryQuery());
            return Ok(response);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetAdminStats()
        {
            var response = await Mediator.Send(new GetAdminDashboardStatsQuery());
            return Ok(response);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement body, [FromQuery] string hmac)
        {
            await Mediator.Send(new PaymobCallbackCommand
            {
                Body = body,
                Hmac = hmac
            });

            return Ok();
        }
    }
}
