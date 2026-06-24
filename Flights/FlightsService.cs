using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Data;
using DotnetStarterKit.Flights.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Flights;

public class FlightsService
{
    private readonly AppDbContext _db;

    public FlightsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetAllAsync(int page, int perPage, string? search, int? companyId)
    {
        var query = _db.Flights
            .Include(f => f.Company)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(f =>
                f.FlightNumber.Contains(search) ||
                f.Destination.Contains(search) ||
                f.Transport.Contains(search));

        if (companyId.HasValue)
            query = query.Where(f => f.CompanyId == companyId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(f => new FlightResponseDto
            {
                Id = f.Id,
                FlightNumber = f.FlightNumber,
                CompanyId = f.CompanyId,
                Company = f.Company != null ? f.Company.CompanyName : null,
                Date = f.Date,
                Time = f.Time,
                Destination = f.Destination,
                NoOfHuj = f.NoOfHuj,
                Transport = f.Transport,
            })
            .ToListAsync();

        return new
        {
            data = items,
            pagination = new
            {
                currentPage = page,
                perPage = perPage,
                totalObjects = total,
                totalPages = (int)Math.Ceiling((double)total / perPage),
                itemsOnPage = items.Count,
            },
            status = new { success = true, message = "Operation successful" },
        };
    }

    public async Task<FlightResponseDto?> GetByIdAsync(int id, int? companyId = null)
    {
        var f = await _db.Flights
            .Include(f => f.Company)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (f is null) return null;

        if (companyId.HasValue && f.CompanyId != companyId.Value)
            return null;

        return new FlightResponseDto
        {
            Id = f.Id,
            FlightNumber = f.FlightNumber,
            CompanyId = f.CompanyId,
            Company = f.Company?.CompanyName,
            Date = f.Date,
            Time = f.Time,
            Destination = f.Destination,
            NoOfHuj = f.NoOfHuj,
            Transport = f.Transport,
        };
    }

    public async Task<(bool Success, string Message, string Code, FlightResponseDto? Data)> CreateAsync(CreateFlightDto dto)
    {
        var flight = new Data.Flight
        {
            FlightNumber = dto.FlightNumber,
            CompanyId = dto.CompanyId,
            Date = DateTime.TryParse(dto.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null,
            Time = dto.Time,
            Destination = dto.Destination,
            NoOfHuj = dto.NoOfHuj,
            Transport = dto.Transport,
            CreatedAt = DateTime.UtcNow,
        };

        _db.Flights.Add(flight);
        await _db.SaveChangesAsync();

        var company = flight.CompanyId.HasValue
            ? await _db.Companies.FindAsync(flight.CompanyId.Value)
            : null;

        return (true, "Flight created successfully.", AppCodes.Created, new FlightResponseDto
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            CompanyId = flight.CompanyId,
            Company = company?.CompanyName,
            Date = flight.Date,
            Time = flight.Time,
            Destination = flight.Destination,
            NoOfHuj = flight.NoOfHuj,
            Transport = flight.Transport,
        });
    }

    public async Task<(bool Success, string Message, string Code, FlightResponseDto? Data)> UpdateAsync(int id, UpdateFlightDto dto, int? companyId = null)
    {
        var flight = await _db.Flights.FindAsync(id);
        if (flight is null)
            return (false, "Flight not found.", AppCodes.NotFound, null);

        if (companyId.HasValue && flight.CompanyId != companyId.Value)
            return (false, "Flight not found.", AppCodes.NotFound, null);

        if (dto.FlightNumber is not null) flight.FlightNumber = dto.FlightNumber;
        if (dto.CompanyId.HasValue) flight.CompanyId = dto.CompanyId;
        if (dto.Date is not null) flight.Date = DateTime.TryParse(dto.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null;
        if (dto.Time is not null) flight.Time = dto.Time;
        if (dto.Destination is not null) flight.Destination = dto.Destination;
        if (dto.NoOfHuj.HasValue) flight.NoOfHuj = dto.NoOfHuj.Value;
        if (dto.Transport is not null) flight.Transport = dto.Transport;

        await _db.SaveChangesAsync();

        var company = flight.CompanyId.HasValue
            ? await _db.Companies.FindAsync(flight.CompanyId.Value)
            : null;

        return (true, "Flight updated successfully.", AppCodes.Success, new FlightResponseDto
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            CompanyId = flight.CompanyId,
            Company = company?.CompanyName,
            Date = flight.Date,
            Time = flight.Time,
            Destination = flight.Destination,
            NoOfHuj = flight.NoOfHuj,
            Transport = flight.Transport,
        });
    }

    public async Task<(bool Success, string Message, string Code)> DeleteAsync(int id, int? companyId = null)
    {
        var flight = await _db.Flights.FindAsync(id);
        if (flight is null)
            return (false, "Flight not found.", AppCodes.NotFound);

        if (companyId.HasValue && flight.CompanyId != companyId.Value)
            return (false, "Flight not found.", AppCodes.NotFound);

        _db.Flights.Remove(flight);
        await _db.SaveChangesAsync();

        return (true, "Flight deleted successfully.", AppCodes.Success);
    }

    public async Task<(bool Success, string Message, string Code, object? Data)> UploadAsync(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return (false, "No file uploaded.", AppCodes.ValidationError, null);

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return (false, "Only CSV files are supported.", AppCodes.ValidationError, null);

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null,
            PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant(),
        });

        var records = csv.GetRecords<CsvFlightRow>().ToList();
        if (records.Count == 0)
            return (false, "CSV file is empty.", AppCodes.ValidationError, null);

        var created = 0;
        var errors = new List<string>();

        foreach (var (record, index) in records.Select((r, i) => (r, i)))
        {
            var rowNum = index + 2;

            if (string.IsNullOrWhiteSpace(record.FlightNumber))
            {
                errors.Add($"Row {rowNum}: Flight number is required.");
                continue;
            }

            var flight = new Data.Flight
            {
                FlightNumber = record.FlightNumber,
                CompanyId = null,
                Date = DateTime.TryParse(record.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null,
                Time = record.Time ?? string.Empty,
                Destination = record.Destination ?? string.Empty,
                NoOfHuj = int.TryParse(record.NoOfHuj, out var huj) ? huj : 0,
                Transport = record.Transport ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
            };

            _db.Flights.Add(flight);
            created++;
        }

        if (created > 0)
            await _db.SaveChangesAsync();

        return (true, $"Import completed. {created} flights created, {errors.Count} errors.", AppCodes.Success, new
        {
            created,
            errors,
        });
    }
}

public class CsvFlightRow
{
    public string FlightNumber { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string NoOfHuj { get; set; } = string.Empty;
    public string Transport { get; set; } = string.Empty;
}
