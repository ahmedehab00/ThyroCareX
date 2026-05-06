using System.Threading.Tasks;
using ThyroCareX.Data.Responses;

namespace ThyroCareX.Service.Abstarct
{
    public interface IDashboardService
    {
        Task<GetPlatformStatsResponse> GetPlatformStatsAsync();
    }
}
