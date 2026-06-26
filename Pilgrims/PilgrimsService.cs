using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Data;
using DotnetStarterKit.Pilgrims.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Pilgrims;

public class PilgrimsService
{
    private readonly AppDbContext _db;

    public PilgrimsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetAllAsync(int page, int perPage, string? search, int? companyId, int? excludeFlightId = null)
    {
        var query = _db.Pilgrims
            .Include(p => p.Company)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Identity.Contains(search) ||
                p.Nationality.Contains(search) ||
                p.Phone.Contains(search) ||
                p.Email.Contains(search));

        if (companyId.HasValue)
            query = query.Where(p => p.CompanyId == companyId.Value);

        if (excludeFlightId.HasValue)
        {
            var assignedIds = _db.FlightPilgrims
                .Where(fp => fp.FlightId == excludeFlightId.Value)
                .Select(fp => fp.PilgrimId);
            query = query.Where(p => !assignedIds.Contains(p.Id));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(p => new PilgrimResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Identity = p.Identity,
                CompanyId = p.CompanyId,
                Company = p.Company != null ? p.Company.CompanyName : null,
                Nationality = p.Nationality,
                Gender = p.Gender,
                BirthDate = p.BirthDate,
                Phone = p.Phone,
                Email = p.Email,
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

    public async Task<PilgrimResponseDto?> GetByIdAsync(int id, int? companyId = null)
    {
        var p = await _db.Pilgrims
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (p is null) return null;

        if (companyId.HasValue && p.CompanyId != companyId.Value)
            return null;

        return new PilgrimResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Identity = p.Identity,
            CompanyId = p.CompanyId,
            Company = p.Company?.CompanyName,
            Nationality = p.Nationality,
            Gender = p.Gender,
            BirthDate = p.BirthDate,
            Phone = p.Phone,
            Email = p.Email,
        };
    }

    public async Task<(bool Success, string Message, string Code, PilgrimResponseDto? Data)> CreateAsync(CreatePilgrimDto dto)
    {
        if (await _db.Pilgrims.AnyAsync(p => p.Identity == dto.Identity))
            return (false, "Identity number already exists.", AppCodes.Conflict, null);

        if (!dto.CompanyId.HasValue)
            return (false, "Company is required.", AppCodes.ValidationError, null);

        var pilgrim = new Data.Pilgrim
        {
            Name = dto.Name,
            Identity = dto.Identity,
            CompanyId = dto.CompanyId.Value,
            Nationality = dto.Nationality,
            Gender = dto.Gender,
            BirthDate = DateTime.TryParse(dto.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate) ? birthDate : null,
            Phone = dto.Phone,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
        };

        _db.Pilgrims.Add(pilgrim);
        await _db.SaveChangesAsync();

        var company = await _db.Companies.FindAsync(pilgrim.CompanyId);

        return (true, "Pilgrim created successfully.", AppCodes.Created, new PilgrimResponseDto
        {
            Id = pilgrim.Id,
            Name = pilgrim.Name,
            Identity = pilgrim.Identity,
            CompanyId = pilgrim.CompanyId,
            Company = company?.CompanyName,
            Nationality = pilgrim.Nationality,
            Gender = pilgrim.Gender,
            BirthDate = pilgrim.BirthDate,
            Phone = pilgrim.Phone,
            Email = pilgrim.Email,
        });
    }

    public async Task<(bool Success, string Message, string Code, PilgrimResponseDto? Data)> UpdateAsync(int id, UpdatePilgrimDto dto, int? companyId = null)
    {
        var pilgrim = await _db.Pilgrims.FindAsync(id);
        if (pilgrim is null)
            return (false, "Pilgrim not found.", AppCodes.NotFound, null);

        if (companyId.HasValue && pilgrim.CompanyId != companyId.Value)
            return (false, "Pilgrim not found.", AppCodes.NotFound, null);

        if (dto.Identity is not null && dto.Identity != pilgrim.Identity)
        {
            if (await _db.Pilgrims.AnyAsync(p => p.Identity == dto.Identity && p.Id != id))
                return (false, "Identity number already exists.", AppCodes.Conflict, null);
            pilgrim.Identity = dto.Identity;
        }

        if (dto.Name is not null) pilgrim.Name = dto.Name;
        if (dto.CompanyId.HasValue && !companyId.HasValue) pilgrim.CompanyId = dto.CompanyId.Value;
        if (dto.Nationality is not null) pilgrim.Nationality = dto.Nationality;
        if (dto.Gender is not null) pilgrim.Gender = dto.Gender;
        if (dto.BirthDate is not null) pilgrim.BirthDate = DateTime.TryParse(dto.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var bd) ? bd : null;
        if (dto.Phone is not null) pilgrim.Phone = dto.Phone;
        if (dto.Email is not null) pilgrim.Email = dto.Email;

        await _db.SaveChangesAsync();

        var company = await _db.Companies.FindAsync(pilgrim.CompanyId);

        return (true, "Pilgrim updated successfully.", AppCodes.Success, new PilgrimResponseDto
        {
            Id = pilgrim.Id,
            Name = pilgrim.Name,
            Identity = pilgrim.Identity,
            CompanyId = pilgrim.CompanyId,
            Company = company?.CompanyName,
            Nationality = pilgrim.Nationality,
            Gender = pilgrim.Gender,
            BirthDate = pilgrim.BirthDate,
            Phone = pilgrim.Phone,
            Email = pilgrim.Email,
        });
    }

    public async Task<(bool Success, string Message, string Code)> DeleteAsync(int id, int? companyId = null)
    {
        var pilgrim = await _db.Pilgrims.FindAsync(id);
        if (pilgrim is null)
            return (false, "Pilgrim not found.", AppCodes.NotFound);

        if (companyId.HasValue && pilgrim.CompanyId != companyId.Value)
            return (false, "Pilgrim not found.", AppCodes.NotFound);

        _db.Pilgrims.Remove(pilgrim);
        await _db.SaveChangesAsync();

        return (true, "Pilgrim deleted successfully.", AppCodes.Success);
    }

    public async Task<(bool Success, string Message, string Code, object? Data)> UploadAsync(IFormFile file, int? companyId = null)
    {
        if (file is null || file.Length == 0)
            return (false, "No file uploaded.", AppCodes.ValidationError, null);

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return (false, "Only CSV files are supported.", AppCodes.ValidationError, null);

        if (!companyId.HasValue)
            return (false, "Company is required for import.", AppCodes.ValidationError, null);

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null,
            PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant(),
        });

        var records = csv.GetRecords<CsvPilgrimRow>().ToList();
        if (records.Count == 0)
            return (false, "CSV file is empty.", AppCodes.ValidationError, null);

        var created = 0;
        var errors = new List<string>();

        foreach (var (record, index) in records.Select((r, i) => (r, i)))
        {
            var rowNum = index + 2;

            if (string.IsNullOrWhiteSpace(record.Name))
            {
                errors.Add($"Row {rowNum}: Name is required.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.Identity))
            {
                errors.Add($"Row {rowNum}: Identity is required.");
                continue;
            }

            if (await _db.Pilgrims.AnyAsync(p => p.Identity == record.Identity))
            {
                errors.Add($"Row {rowNum}: Identity '{record.Identity}' already exists.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.Nationality))
            {
                errors.Add($"Row {rowNum}: Nationality is required.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.Gender))
            {
                errors.Add($"Row {rowNum}: Gender is required.");
                continue;
            }

            var normalizedGender = record.Gender.Trim().ToLowerInvariant();
            if (normalizedGender != "male" && normalizedGender != "female"
                && normalizedGender != "ذكر" && normalizedGender != "أنثى")
            {
                errors.Add($"Row {rowNum}: Gender must be Male/Female (ذكر/أنثى).");
                continue;
            }

            if (!DateTime.TryParse(record.BirthDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
            {
                errors.Add($"Row {rowNum}: Invalid date format for BirthDate.");
                continue;
            }

            var pilgrim = new Data.Pilgrim
            {
                Name = record.Name,
                Identity = record.Identity,
                CompanyId = companyId.Value,
                Nationality = record.Nationality,
                Gender = record.Gender,
                BirthDate = birthDate,
                Phone = record.Phone ?? string.Empty,
                Email = record.Email ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
            };

            _db.Pilgrims.Add(pilgrim);
            created++;
        }

        if (created > 0)
            await _db.SaveChangesAsync();

        return (true, $"Import completed. {created} pilgrims created, {errors.Count} errors.", AppCodes.Success, new
        {
            created,
            errors,
        });
    }
}

public class CsvPilgrimRow
{
    public string Name { get; set; } = string.Empty;
    public string Identity { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
