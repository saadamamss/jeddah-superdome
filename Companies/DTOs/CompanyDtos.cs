using System.ComponentModel.DataAnnotations;

namespace DotnetStarterKit.Companies.DTOs;

// ─── Request DTOs ─────────────────────────────────────────────

public class CreateCompanyDto
{
    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string LicenseNo { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? Address { get; set; }
}

public class UpdateCompanyDto
{
    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string LicenseNo { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Address { get; set; }
}

// ─── Response DTOs ────────────────────────────────────────────

public class CompanyResponseDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int ConnectedHujjaj { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
