namespace DotnetStarterKit.Dashboard.DTOs;

public class StatisticsResponseDto
{
    public DashboardStatisticsDto Statistics { get; set; } = new();
}

public class DashboardStatisticsDto
{
    public int CompaniesCount { get; set; }
    public int CampsCount { get; set; }
    public int LocationsCount { get; set; }
    public int MainCompaniesCount { get; set; }
    public int PilgrimsCount { get; set; }
}
