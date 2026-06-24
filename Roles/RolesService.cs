using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Data;
using DotnetStarterKit.Roles.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Roles;

public class RolesService
{
    private readonly AppDbContext _db;

    public RolesService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetAllAsync(int page, int perPage, string? search)
    {
        var query = _db.BusinessRoles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Name.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(r => r.Id)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(r => new RoleResponseDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Permissions = r.RolePermissions.Select(rp => new RolePermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                }).ToList(),
            })
            .ToListAsync();

        return new
        {
            data = items,
            pagination = new PaginationResult
            {
                CurrentPage = page,
                PerPage = perPage,
                TotalObjects = total,
                TotalPages = (int)Math.Ceiling((double)total / perPage),
                ItemsOnPage = items.Count,
            },
            status = new { success = true, message = "Operation successful" },
        };
    }

    public async Task<object?> GetByIdAsync(int id)
    {
        var role = await _db.BusinessRoles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role is null) return null;

        return new
        {
            data = new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.RolePermissions.Select(rp => new RolePermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                }).ToList(),
            },
            status = new { success = true, message = "Operation successful" },
        };
    }

    public async Task<RoleOperationResult> CreateAsync(CreateRoleDto dto)
    {
        if (await _db.BusinessRoles.AnyAsync(r => r.Name == dto.Name))
            return new RoleOperationResult(false, "Role name already exists.", AppCodes.Conflict);

        var role = new Data.Role
        {
            Name = dto.Name,
            Description = dto.Description,
        };

        _db.BusinessRoles.Add(role);
        await _db.SaveChangesAsync();

        if (dto.Permissions.Count > 0)
        {
            var permissionNames = dto.Permissions;
            var permissions = await _db.Permissions
                .Where(p => permissionNames.Contains(p.Name))
                .ToListAsync();

            role.RolePermissions = permissions.Select(p => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = p.Id,
            }).ToList();

            await _db.SaveChangesAsync();
        }

        var createdRole = await _db.BusinessRoles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstAsync(r => r.Id == role.Id);

        return new RoleOperationResult(true, "Role created successfully.", AppCodes.Created, new RoleResponseDto
        {
            Id = createdRole.Id,
            Name = createdRole.Name,
            Description = createdRole.Description,
            Permissions = createdRole.RolePermissions.Select(rp => new RolePermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
            }).ToList(),
        });
    }

    public async Task<RoleOperationResult> UpdateAsync(int id, CreateRoleDto dto)
    {
        var role = await _db.BusinessRoles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role is null)
            return new RoleOperationResult(false, "Role not found.", AppCodes.NotFound);

        if (await _db.BusinessRoles.AnyAsync(r => r.Name == dto.Name && r.Id != id))
            return new RoleOperationResult(false, "Role name already exists.", AppCodes.Conflict);

        role.Name = dto.Name;
        role.Description = dto.Description;

        role.RolePermissions.Clear();

        if (dto.Permissions.Count > 0)
        {
            var permissionNames = dto.Permissions;
            var permissions = await _db.Permissions
                .Where(p => permissionNames.Contains(p.Name))
                .ToListAsync();

            role.RolePermissions = permissions.Select(p => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = p.Id,
            }).ToList();
        }

        await _db.SaveChangesAsync();

        var updatedRole = await _db.BusinessRoles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstAsync(r => r.Id == id);

        return new RoleOperationResult(true, "Role updated successfully.", AppCodes.Success, new RoleResponseDto
        {
            Id = updatedRole.Id,
            Name = updatedRole.Name,
            Description = updatedRole.Description,
            Permissions = updatedRole.RolePermissions.Select(rp => new RolePermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
            }).ToList(),
        });
    }

    public async Task<RoleOperationResult> DeleteAsync(int id)
    {
        var role = await _db.BusinessRoles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role is null)
            return new RoleOperationResult(false, "Role not found.", AppCodes.NotFound);

        var hasUsers = await _db.Users.AnyAsync(u => u.RoleId == id);
        if (hasUsers)
            return new RoleOperationResult(false, "Cannot delete role with assigned users.", AppCodes.Conflict);

        _db.RolePermissions.RemoveRange(role.RolePermissions);
        _db.BusinessRoles.Remove(role);
        await _db.SaveChangesAsync();

        return new RoleOperationResult(true, "Role deleted successfully.", AppCodes.Success);
    }

    public async Task<object> GetPermissionsAsync()
    {
        var permissions = await _db.Permissions
            .OrderBy(p => p.Id)
            .Select(p => new PermissionResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Key = p.Name,
            })
            .ToListAsync();

        return new { data = permissions };
    }
}

public record RoleOperationResult(bool Success, string Message, string Code, RoleResponseDto? Data = null)
{
    public RoleOperationResult(bool success, string message, string code) : this(success, message, code, null) { }
}

public class PaginationResult
{
    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int TotalObjects { get; set; }
    public int TotalPages { get; set; }
    public int ItemsOnPage { get; set; }
}
