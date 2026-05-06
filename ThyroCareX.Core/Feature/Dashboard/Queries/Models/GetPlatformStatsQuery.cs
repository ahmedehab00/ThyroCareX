using MediatR;
using ThyroCareX.Core.Bases;
using ThyroCareX.Data.Responses;

namespace ThyroCareX.Core.Feature.Dashboard.Queries.Models
{
    public class GetPlatformStatsQuery : IRequest<Response<GetPlatformStatsResponse>>
    {
    }
}
