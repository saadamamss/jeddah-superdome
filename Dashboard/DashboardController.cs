using DotnetStarterKit.Common.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetStarterKit.Dashboard;

[ApiController]
[Route("api/v1/dashboard/statistics")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    [RequirePermission("dashboard.view")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _dashboardService.GetStatisticsAsync();
        return Ok(new { data = result });
    }
}
