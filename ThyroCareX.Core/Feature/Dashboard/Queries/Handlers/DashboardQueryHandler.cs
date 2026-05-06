using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ThyroCareX.Core.Bases;
using ThyroCareX.Core.Feature.Dashboard.Queries.Models;
using ThyroCareX.Data.Responses;
using ThyroCareX.Service.Abstarct;

namespace ThyroCareX.Core.Feature.Dashboard.Queries.Handlers
{
    public class DashboardQueryHandler : ResponseHandler,
        IRequestHandler<GetPlatformStatsQuery, Response<GetPlatformStatsResponse>>
    {
        private readonly IDashboardService _dashboardService;

        public DashboardQueryHandler(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<Response<GetPlatformStatsResponse>> Handle(GetPlatformStatsQuery request, CancellationToken cancellationToken)
        {
            var stats = await _dashboardService.GetPlatformStatsAsync();
            return Success(stats);
        }
    }
}
