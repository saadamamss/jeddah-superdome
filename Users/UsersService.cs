using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Data;
using DotnetStarterKit.Users.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Users;

public class UsersService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _db;

    public UsersService(UserManager<User> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<object> GetAllAsync(int page, int perPage, string? search, int? roleId)
    {
        var query = _db.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserCompanies)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u =>
                u.Name.Contains(search) ||
                u.Email!.Contains(search) ||
                (u.Phone != null && u.Phone.Contains(search)));

        if (roleId.HasValue)
            query = query.Where(u => u.RoleId == roleId.Value);

        var total = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        var items = users.Select(MapToDto).ToList();

        return new
        {
            data = items,
            pagination = new
            {
                currentPage = page,
                perPage,
                totalObjects = total,
                totalPages = (int)Math.Ceiling((double)total / perPage),
                itemsOnPage = items.Count,
            },
            status = new { success = true, message = "Operation successful" },
        };
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _db.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserCompanies)
            .FirstOrDefaultAsync(u => u.IntId == id);

        if (user is null) return null;
        return MapToDto(user);
    }

    public async Task<(bool Success, string Message, string Code, UserResponseDto? Data)> CreateAsync(CreateUserDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return (false, "Email already exists.", AppCodes.Conflict, null);

        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            Phone = dto.Phone,
            IdentityNumber = dto.IdentityNumber,
            RoleId = dto.RoleId,
            IsActive = dto.Status != "inactive",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var password = string.IsNullOrWhiteSpace(dto.Password) ? "12345678" : dto.Password;

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors, AppCodes.ValidationError, null);
            }

            var created = await _db.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r!.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .Include(u => u.UserCompanies)
                .FirstAsync(u => u.Id == user.Id);

            await transaction.CommitAsync();

            return (true, "User created successfully.", AppCodes.Created, MapToDto(created));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<(bool Success, string Message, string Code, UserResponseDto? Data)> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _db.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserCompanies)
            .FirstOrDefaultAsync(u => u.IntId == id);

        if (user is null)
            return (false, "User not found.", AppCodes.NotFound, null);

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email && u.IntId != id))
                return (false, "Email already exists.", AppCodes.Conflict, null);
            user.Email = dto.Email;
            user.UserName = dto.Email;
        }

        if (dto.Name is not null) user.Name = dto.Name;
        if (dto.Phone is not null) user.Phone = dto.Phone;
        if (dto.IdentityNumber is not null) user.IdentityNumber = dto.IdentityNumber;
        if (dto.RoleId.HasValue) user.RoleId = dto.RoleId;
        if (dto.Status is not null) user.IsActive = dto.Status != "inactive";
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, dto.Password);
        }

        await _db.SaveChangesAsync();

        var updated = await _db.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserCompanies)
            .FirstAsync(u => u.IntId == id);

        return (true, "User updated successfully.", AppCodes.Success, MapToDto(updated!));
    }

    public async Task<(bool Success, string Message, string Code)> DeleteAsync(int id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.IntId == id);
        if (user is null)
            return (false, "User not found.", AppCodes.NotFound);

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, errors, AppCodes.ValidationError);
        }

        return (true, "User deleted successfully.", AppCodes.Success);
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

        var records = csv.GetRecords<CsvUserRow>().ToList();
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

            if (string.IsNullOrWhiteSpace(record.Email))
            {
                errors.Add($"Row {rowNum}: Email is required.");
                continue;
            }

            if (await _db.Users.AnyAsync(u => u.Email == record.Email))
            {
                errors.Add($"Row {rowNum}: Email '{record.Email}' already exists.");
                continue;
            }

            var user = new User
            {
                UserName = record.Email,
                Email = record.Email,
                Name = record.Name,
                Phone = record.Phone ?? string.Empty,
                IsActive = string.IsNullOrWhiteSpace(record.Status) || record.Status.ToLower() != "inactive",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var password = string.IsNullOrWhiteSpace(record.Password) ? "12345678" : record.Password;
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var userErrors = string.Join(", ", result.Errors.Select(e => e.Description));
                errors.Add($"Row {rowNum}: {userErrors}");
                continue;
            }
            created++;
        }

        return (true, $"Import completed. {created} users created, {errors.Count} errors.", AppCodes.Success, new
        {
            created,
            errors,
        });
    }

    private static UserResponseDto MapToDto(User user) => new()
    {
        Id = user.IntId,
        Name = user.Name,
        Email = user.Email ?? string.Empty,
        Phone = user.Phone,
        IdentityNumber = user.IdentityNumber,
        Role = user.Role is not null ? new UserRoleDto
        {
            Id = user.Role.Id,
            Name = user.Role.Name,
        } : null,
        Status = user.IsActive ? "active" : "inactive",
        Permissions = user.Role?.RolePermissions
            .Select(rp => rp.Permission.Name)
            .ToArray() ?? [],
        ParentCompanies = user.UserCompanies
            .Select(uc => (object)new { id = uc.CompanyId })
            .ToArray(),
    };
}

public class CsvUserRow
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Status { get; set; }
}
