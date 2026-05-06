using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ThyroCareX.Bases;
using ThyroCareX.Core.Feature.Dashboard.Queries.Models;

namespace ThyroCareX.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : AppControllerBase
    {
        [HttpGet("Stats")]
        public async Task<IActionResult> GetPlatformStats()
        {
            var response = await Mediator.Send(new GetPlatformStatsQuery());
            return Ok(response);
        }
    }
}
