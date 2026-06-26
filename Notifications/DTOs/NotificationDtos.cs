using System.ComponentModel.DataAnnotations;

namespace DotnetStarterKit.Notifications.DTOs;

// ─── Request DTOs ─────────────────────────────────────────────

public class SendNotificationDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [Required]
    public string Target { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Method { get; set; } = string.Empty;

    public int[]? UserIds { get; set; }
}

// ─── Response DTOs ────────────────────────────────────────────

public class NotificationResponseDto
{
    public int Id { get; set; }
    public string NotificationTitle { get; set; } = string.Empty;
    public string NotificationText { get; set; } = string.Empty;
    public string TargetCategory { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string TransmissionMethod { get; set; } = string.Empty;
    public string? SentByUserId { get; set; }
    public int? TargetCompanyId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int RecipientCount { get; set; }
    public int ReadCount { get; set; }
}

public class UserNotificationResponseDto
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime DeliveredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
