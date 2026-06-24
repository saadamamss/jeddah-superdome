using DotnetStarterKit.Data;
using DotnetStarterKit.Dashboard.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Dashboard;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<StatisticsResponseDto> GetStatisticsAsync()
    {
        var companiesCount = await _db.Companies.CountAsync();
        var pilgrimsCount = await _db.Pilgrims.CountAsync();
        var flightsCount = await _db.Flights.CountAsync();
        var usersCount = await _db.Users.CountAsync();

        return new StatisticsResponseDto
        {
            Statistics = new DashboardStatisticsDto
            {
                CompaniesCount = companiesCount,
                CampsCount = flightsCount,
                LocationsCount = pilgrimsCount,
                MainCompaniesCount = usersCount,
                PilgrimsCount = pilgrimsCount,
            },
        };
    }
}
