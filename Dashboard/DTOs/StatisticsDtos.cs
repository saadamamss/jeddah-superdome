namespace DotnetStarterKit.Dashboard.DTOs;

public class StatisticsResponseDto
{
    public DashboardStatisticsDto Statistics { get; set; } = new();
}

public class DashboardStatisticsDto
{
    public int CompaniesCount { get; set; }
    public int FlightsCount { get; set; }
    public int PilgrimsCount { get; set; }
    public int UsersCount { get; set; }
}
