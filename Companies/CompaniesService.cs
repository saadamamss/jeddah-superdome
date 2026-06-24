using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DotnetStarterKit.Companies.DTOs;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Data;
using DotnetStarterKit.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Companies;

public class CompaniesService
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public CompaniesService(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<object> GetAllAsync(int page, int perPage, string? search)
    {
        var query = _db.Companies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.CompanyName.Contains(search) ||
                c.LicenseNumber.Contains(search) ||
                c.City.Contains(search) ||
                c.ContactNumber.Contains(search) ||
                c.Email.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.Id)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(c => new CompanyResponseDto
            {
                Id = c.Id,
                CompanyName = c.CompanyName,
                LicenseNumber = c.LicenseNumber,
                City = c.City,
                ContactNumber = c.ContactNumber,
                Email = c.Email,
                Address = c.Address,
                ConnectedHujjaj = c.ConnectedHujjaj,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
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

    public async Task<CompanyResponseDto?> GetByIdAsync(int id)
    {
        var c = await _db.Companies.FindAsync(id);
        if (c is null) return null;

        return new CompanyResponseDto
        {
            Id = c.Id,
            CompanyName = c.CompanyName,
            LicenseNumber = c.LicenseNumber,
            City = c.City,
            ContactNumber = c.ContactNumber,
            Email = c.Email,
            Address = c.Address,
            ConnectedHujjaj = c.ConnectedHujjaj,
            Latitude = c.Latitude,
            Longitude = c.Longitude,
        };
    }

    public async Task<(bool Success, string Message, string Code, CompanyResponseDto? Data)> CreateAsync(CreateCompanyDto dto)
    {
        if (await _db.Companies.AnyAsync(c => c.Email == dto.Email))
            return (false, "Company email already exists.", AppCodes.Conflict, null);

        if (await _db.Companies.AnyAsync(c => c.LicenseNumber == dto.LicenseNo))
            return (false, "License number already exists.", AppCodes.Conflict, null);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            // Create company record
            var company = new Data.Company
            {
                CompanyName = dto.CompanyName,
                LicenseNumber = dto.LicenseNo,
                City = dto.City,
                ContactNumber = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
            };

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            // Create Identity User with role "company"
            var companyBusinessRole = await _db.BusinessRoles.FirstOrDefaultAsync(r => r.Name == "Company");

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.CompanyName,
                Phone = dto.Phone,
                IsActive = true,
                RoleId = companyBusinessRole?.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors, AppCodes.ValidationError, null);
            }

            // Assign "company" role
            await _userManager.AddToRoleAsync(user, "company");

            // Link user to company
            _db.UserCompanies.Add(new UserCompany
            {
                UserId = user.Id,
                CompanyId = company.Id,
            });
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return (true, "Company created successfully.", AppCodes.Created, new CompanyResponseDto
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                LicenseNumber = company.LicenseNumber,
                City = company.City,
                ContactNumber = company.ContactNumber,
                Email = company.Email,
                Address = company.Address,
                ConnectedHujjaj = company.ConnectedHujjaj,
                Latitude = company.Latitude,
                Longitude = company.Longitude,
            });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<(bool Success, string Message, string Code, CompanyResponseDto? Data)> UpdateAsync(int id, UpdateCompanyDto dto)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company is null)
            return (false, "Company not found.", AppCodes.NotFound, null);

        if (await _db.Companies.AnyAsync(c => c.Email == dto.Email && c.Id != id))
            return (false, "Company email already exists.", AppCodes.Conflict, null);

        if (await _db.Companies.AnyAsync(c => c.LicenseNumber == dto.LicenseNo && c.Id != id))
            return (false, "License number already exists.", AppCodes.Conflict, null);

        company.CompanyName = dto.CompanyName;
        company.LicenseNumber = dto.LicenseNo;
        company.City = dto.City;
        company.ContactNumber = dto.Phone;
        company.Email = dto.Email;
        company.Address = dto.Address;

        // Update linked user if exists
        var userCompany = await _db.UserCompanies.FirstOrDefaultAsync(uc => uc.CompanyId == id);
        if (userCompany is not null)
        {
            var user = await _userManager.FindByIdAsync(userCompany.UserId);
            if (user is not null)
            {
                user.Name = dto.CompanyName;
                user.Email = dto.Email;
                user.UserName = dto.Email;
                user.Phone = dto.Phone;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
        }

        await _db.SaveChangesAsync();

        return (true, "Company updated successfully.", AppCodes.Success, new CompanyResponseDto
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            LicenseNumber = company.LicenseNumber,
            City = company.City,
            ContactNumber = company.ContactNumber,
            Email = company.Email,
            Address = company.Address,
            ConnectedHujjaj = company.ConnectedHujjaj,
            Latitude = company.Latitude,
            Longitude = company.Longitude,
        });
    }

    public async Task<(bool Success, string Message, string Code)> DeleteAsync(int id)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company is null)
            return (false, "Company not found.", AppCodes.NotFound);

        // Delete linked user
        var userCompany = await _db.UserCompanies.FirstOrDefaultAsync(uc => uc.CompanyId == id);
        if (userCompany is not null)
        {
            var user = await _userManager.FindByIdAsync(userCompany.UserId);
            if (user is not null)
                await _userManager.DeleteAsync(user);
        }

        _db.Companies.Remove(company);
        await _db.SaveChangesAsync();

        return (true, "Company deleted successfully.", AppCodes.Success);
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

        var records = csv.GetRecords<CsvCompanyRow>().ToList();

        if (records.Count == 0)
            return (false, "CSV file is empty.", AppCodes.ValidationError, null);

        var created = 0;
        var errors = new List<string>();

        foreach (var (record, index) in records.Select((r, i) => (r, i)))
        {
            var rowNum = index + 2;

            if (string.IsNullOrWhiteSpace(record.CompanyName))
            {
                errors.Add($"Row {rowNum}: Company name is required.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.Email))
            {
                errors.Add($"Row {rowNum}: Email is required.");
                continue;
            }

            if (await _db.Companies.AnyAsync(c => c.Email == record.Email))
            {
                errors.Add($"Row {rowNum}: Email '{record.Email}' already exists.");
                continue;
            }

            var company = new Data.Company
            {
                CompanyName = record.CompanyName,
                LicenseNumber = record.LicenseNumber ?? string.Empty,
                City = record.City ?? string.Empty,
                ContactNumber = record.Phone ?? string.Empty,
                Email = record.Email,
                Address = record.Address,
                CreatedAt = DateTime.UtcNow,
            };

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            // Create Identity User
            var password = string.IsNullOrWhiteSpace(record.Password) ? "12345678" : record.Password;
            var companyBusinessRole = await _db.BusinessRoles.FirstOrDefaultAsync(r => r.Name == "Company");
            var user = new User
            {
                UserName = record.Email,
                Email = record.Email,
                Name = record.CompanyName,
                Phone = record.Phone ?? string.Empty,
                IsActive = true,
                RoleId = companyBusinessRole?.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var userErrors = string.Join(", ", result.Errors.Select(e => e.Description));
                errors.Add($"Row {rowNum}: {userErrors}");
                continue;
            }

            await _userManager.AddToRoleAsync(user, "company");

            _db.UserCompanies.Add(new UserCompany
            {
                UserId = user.Id,
                CompanyId = company.Id,
            });

            created++;
        }

        if (created > 0)
            await _db.SaveChangesAsync();

        return (true, $"Import completed. {created} companies created, {errors.Count} errors.", AppCodes.Success, new
        {
            created,
            errors,
        });
    }
}

public class CsvCompanyRow
{
    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Address { get; set; }
}
