using System.ComponentModel.DataAnnotations;

namespace DotnetStarterKit.Roles.DTOs;

// ─── Request DTOs ─────────────────────────────────────────────

public class CreateRoleDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<string> Permissions { get; set; } = [];
}

// ─── Response DTOs ────────────────────────────────────────────

public class RoleResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<RolePermissionDto> Permissions { get; set; } = [];
}

public class RolePermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PermissionResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
