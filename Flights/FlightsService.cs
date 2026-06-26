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
            .Include(f => f.FlightPilgrims)
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
                Capacity = f.Capacity,
                Status = f.Status,
                PilgrimsCount = f.FlightPilgrims.Count,
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
            .Include(f => f.FlightPilgrims)
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
            Capacity = f.Capacity,
            Status = f.Status,
            PilgrimsCount = f.FlightPilgrims.Count,
            Transport = f.Transport,
        };
    }

    public async Task<(bool Success, string Message, string Code, FlightResponseDto? Data)> CreateAsync(CreateFlightDto dto)
    {
        if (!dto.CompanyId.HasValue)
            return (false, "Company is required.", AppCodes.ValidationError, null);

        var flight = new Data.Flight
        {
            FlightNumber = dto.FlightNumber,
            CompanyId = dto.CompanyId.Value,
            Date = DateTime.TryParse(dto.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null,
            Time = dto.Time,
            Destination = dto.Destination,
            NoOfHuj = dto.NoOfHuj,
            Capacity = dto.Capacity,
            Status = dto.Status,
            Transport = dto.Transport,
            CreatedAt = DateTime.UtcNow,
        };

        _db.Flights.Add(flight);
        await _db.SaveChangesAsync();

        var company = await _db.Companies.FindAsync(flight.CompanyId);

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
            Capacity = flight.Capacity,
            Status = flight.Status,
            PilgrimsCount = 0,
            Transport = flight.Transport,
        });
    }

    public async Task<(bool Success, string Message, string Code, FlightResponseDto? Data)> UpdateAsync(int id, UpdateFlightDto dto, int? companyId = null)
    {
        var flight = await _db.Flights
            .Include(f => f.FlightPilgrims)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (flight is null)
            return (false, "Flight not found.", AppCodes.NotFound, null);

        if (companyId.HasValue && flight.CompanyId != companyId.Value)
            return (false, "Flight not found.", AppCodes.NotFound, null);

        if (dto.FlightNumber is not null) flight.FlightNumber = dto.FlightNumber;
        if (dto.CompanyId.HasValue && !companyId.HasValue) flight.CompanyId = dto.CompanyId.Value;
        if (dto.Date is not null) flight.Date = DateTime.TryParse(dto.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null;
        if (dto.Time is not null) flight.Time = dto.Time;
        if (dto.Destination is not null) flight.Destination = dto.Destination;
        if (dto.NoOfHuj.HasValue) flight.NoOfHuj = dto.NoOfHuj.Value;
        if (dto.Capacity.HasValue) flight.Capacity = dto.Capacity.Value;
        if (dto.Status is not null) flight.Status = dto.Status;
        if (dto.Transport is not null) flight.Transport = dto.Transport;

        await _db.SaveChangesAsync();

        var company = await _db.Companies.FindAsync(flight.CompanyId);

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
            Capacity = flight.Capacity,
            Status = flight.Status,
            PilgrimsCount = flight.FlightPilgrims.Count,
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

    public async Task<(bool Success, string Message, string Code, object? Data)> UploadAsync(IFormFile file, int? companyId = null)
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

            int? rowCompanyId = null;
            if (!string.IsNullOrWhiteSpace(record.CompanyId))
            {
                if (int.TryParse(record.CompanyId, out var cid) && cid > 0)
                    rowCompanyId = cid;
                else
                {
                    errors.Add($"Row {rowNum}: Invalid CompanyId '{record.CompanyId}'.");
                    continue;
                }
            }
            var resolvedCompanyId = rowCompanyId ?? companyId ?? 0;

            int capacity = 0;
            if (!string.IsNullOrWhiteSpace(record.Capacity))
                int.TryParse(record.Capacity, out capacity);

            var flight = new Data.Flight
            {
                FlightNumber = record.FlightNumber,
                CompanyId = resolvedCompanyId,
                Date = DateTime.TryParse(record.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null,
                Time = record.Time ?? string.Empty,
                Destination = record.Destination ?? string.Empty,
                NoOfHuj = int.TryParse(record.NoOfHuj, out var huj) ? huj : 0,
                Capacity = capacity,
                Status = string.IsNullOrWhiteSpace(record.Status) ? "Scheduled" : record.Status,
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

    // ── Pilgrim Assignment ──────────────────────────────────────

    public async Task<(bool Success, string Message, string Code)> AssignPilgrimAsync(int flightId, int pilgrimId, int? companyId = null)
    {
        var flight = await _db.Flights.FindAsync(flightId);
        if (flight is null)
            return (false, "Flight not found.", AppCodes.NotFound);

        if (companyId.HasValue && flight.CompanyId != companyId.Value)
            return (false, "Flight not found.", AppCodes.NotFound);

        var pilgrim = await _db.Pilgrims.FindAsync(pilgrimId);
        if (pilgrim is null)
            return (false, "Pilgrim not found.", AppCodes.NotFound);

        if (companyId.HasValue && pilgrim.CompanyId != companyId.Value)
            return (false, "Pilgrim not found.", AppCodes.NotFound);

        var existing = await _db.FlightPilgrims.FindAsync(flightId, pilgrimId);
        if (existing is not null)
            return (false, "Pilgrim is already assigned to this flight.", AppCodes.Conflict);

        var currentCount = await _db.FlightPilgrims.CountAsync(fp => fp.FlightId == flightId);
        if (flight.Capacity > 0 && currentCount >= flight.Capacity)
            return (false, "Flight has reached its capacity.", AppCodes.ValidationError);

        _db.FlightPilgrims.Add(new FlightPilgrim
        {
            FlightId = flightId,
            PilgrimId = pilgrimId,
            AssignedAt = DateTime.UtcNow,
        });

        await _db.SaveChangesAsync();
        return (true, "Pilgrim assigned to flight successfully.", AppCodes.Success);
    }

    public async Task<(bool Success, string Message, string Code)> UnassignPilgrimAsync(int flightId, int pilgrimId, int? companyId = null)
    {
        var flight = await _db.Flights.FindAsync(flightId);
        if (flight is null)
            return (false, "Flight not found.", AppCodes.NotFound);

        if (companyId.HasValue && flight.CompanyId != companyId.Value)
            return (false, "Flight not found.", AppCodes.NotFound);

        var fp = await _db.FlightPilgrims.FindAsync(flightId, pilgrimId);
        if (fp is null)
            return (false, "Pilgrim is not assigned to this flight.", AppCodes.NotFound);

        _db.FlightPilgrims.Remove(fp);
        await _db.SaveChangesAsync();
        return (true, "Pilgrim unassigned from flight successfully.", AppCodes.Success);
    }

    public async Task<(bool Success, string Message, string Code, object? Data)> GetAssignedPilgrimsAsync(int flightId, int? companyId = null, int page = 1, int perPage = 10)
    {
        var flight = await _db.Flights.FindAsync(flightId);
        if (flight is null)
            return (false, "Flight not found.", AppCodes.NotFound, null);

        if (companyId.HasValue && flight.CompanyId != companyId.Value)
            return (false, "Flight not found.", AppCodes.NotFound, null);

        var query = _db.FlightPilgrims
            .Where(fp => fp.FlightId == flightId)
            .Include(fp => fp.Pilgrim)
            .AsQueryable();

        var total = await query.CountAsync();

        var pilgrims = await query
            .OrderByDescending(fp => fp.AssignedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(fp => new
            {
                fp.PilgrimId,
                fp.Pilgrim.Name,
                fp.Pilgrim.Identity,
                fp.Pilgrim.Nationality,
                fp.Pilgrim.Gender,
                fp.Pilgrim.Phone,
                fp.Pilgrim.Email,
                fp.AssignedAt,
            })
            .ToListAsync();

        var data = new
        {
            items = pilgrims,
            pagination = new
            {
                currentPage = page,
                perPage = perPage,
                totalObjects = total,
                totalPages = (int)Math.Ceiling((double)total / perPage),
                itemsOnPage = pilgrims.Count,
            }
        };

        return (true, "Operation successful.", AppCodes.Success, data);
    }
}

public class CsvFlightRow
{
    public string FlightNumber { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string NoOfHuj { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Transport { get; set; } = string.Empty;
}
