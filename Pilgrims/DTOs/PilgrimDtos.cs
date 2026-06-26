using System.ComponentModel.DataAnnotations;

namespace DotnetStarterKit.Pilgrims.DTOs;

// ─── Request DTOs ─────────────────────────────────────────────

public class CreatePilgrimDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Identity { get; set; } = string.Empty;

    public int? CompanyId { get; set; }

    [Required]
    public string Nationality { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string BirthDate { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class UpdatePilgrimDto
{
    public string? Name { get; set; }
    public string? Identity { get; set; }
    public int? CompanyId { get; set; }
    public string? Nationality { get; set; }
    public string? Gender { get; set; }
    public string? BirthDate { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

// ─── Response DTOs ────────────────────────────────────────────

public class PilgrimResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Identity { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string? Company { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
