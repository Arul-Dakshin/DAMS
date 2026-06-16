using DAMS.API.DTOs;

namespace DAMS.API.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
