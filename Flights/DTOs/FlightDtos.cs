using System.ComponentModel.DataAnnotations;

namespace DotnetStarterKit.Flights.DTOs;

// ─── Request DTOs ─────────────────────────────────────────────

public class CreateFlightDto
{
    [Required]
    public string FlightNumber { get; set; } = string.Empty;

    public int? CompanyId { get; set; }

    [Required]
    public string Date { get; set; } = string.Empty;

    [Required]
    public string Time { get; set; } = string.Empty;

    [Required]
    public string Destination { get; set; } = string.Empty;

    [Required]
    public int NoOfHuj { get; set; }

    public int Capacity { get; set; } = 0;

    public string Status { get; set; } = "Scheduled";

    [Required]
    public string Transport { get; set; } = string.Empty;
}

public class UpdateFlightDto
{
    public string? FlightNumber { get; set; }
    public int? CompanyId { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Destination { get; set; }
    public int? NoOfHuj { get; set; }
    public int? Capacity { get; set; }
    public string? Status { get; set; }
    public string? Transport { get; set; }
}

// ─── Response DTOs ────────────────────────────────────────────

public class FlightResponseDto
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public string? Company { get; set; }
    public DateTime? Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public int NoOfHuj { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PilgrimsCount { get; set; }
    public string Transport { get; set; } = string.Empty;
}
