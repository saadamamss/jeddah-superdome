using System.ComponentModel.DataAnnotations;

namespace DotnetStarterKit.Users.DTOs;

// ─── Request DTOs ─────────────────────────────────────────────

public class CreateUserDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    public string? Password { get; set; }

    public string? IdentityNumber { get; set; }

    public int? RoleId { get; set; }

    public int? CompanyId { get; set; }

    public string? Status { get; set; }
}

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Password { get; set; }
    public int? RoleId { get; set; }
    public string? Status { get; set; } // "active" | "inactive"
    public string? IdentityNumber { get; set; }
    public int? CompanyId { get; set; }
}

public class DeleteUserDto
{
    public int? NewSupervisorId { get; set; }
}

// ─── Response DTOs ────────────────────────────────────────────

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? IdentityNumber { get; set; }
    public UserRoleDto? Role { get; set; }
    public string Status { get; set; } = "active";
    public string[] Permissions { get; set; } = [];
    public object[] ParentCompanies { get; set; } = [];
}

public class UserRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
