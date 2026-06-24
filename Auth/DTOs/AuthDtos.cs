using System.ComponentModel.DataAnnotations;

namespace DotnetStarterKit.Auth.DTOs;

// ─── Request DTOs ─────────────────────────────────────────────

public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class ForgotPasswordDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}

// ─── Response DTOs ────────────────────────────────────────────

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? IdentityNumber { get; set; }
    public RoleDto? Role { get; set; }
    public string Status { get; set; } = "active"; // "active" | "inactive"
    public string[] Permissions { get; set; } = [];
    public object[] ParentCompanies { get; set; } = [];
    public CompanyInfoDto? Company { get; set; }
}

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CompanyInfoDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? LicenseNo { get; set; }
    public string? Address { get; set; }
}

public class UpdateCompanyProfileDto
{
    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string LicenseNo { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? Phone { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Address { get; set; }
}
