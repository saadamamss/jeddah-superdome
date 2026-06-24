using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Data;
using DotnetStarterKit.Notifications.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Notifications;

public class NotificationsService
{
    private readonly AppDbContext _db;

    public NotificationsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetAllAsync(int page, int perPage, string? search)
    {
        var query = _db.Notifications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(n =>
                n.Title.Contains(search) ||
                n.Body.Contains(search) ||
                n.Target.Contains(search) ||
                n.Type.Contains(search) ||
                n.Method.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                NotificationTitle = n.Title,
                NotificationText = n.Body,
                TargetCategory = n.Target,
                NotificationType = n.Type,
                TransmissionMethod = n.Method,
                CreatedAt = n.CreatedAt,
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

    public async Task<(bool Success, string Message, string Code, NotificationResponseDto? Data)> SendAsync(SendNotificationDto dto)
    {
        var notification = new Data.Notification
        {
            Title = dto.Title,
            Body = dto.Body,
            Target = dto.Target,
            Type = dto.Type,
            Method = dto.Method,
            CreatedAt = DateTime.UtcNow,
        };

        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();

        return (true, "Notification sent successfully.", AppCodes.Created, new NotificationResponseDto
        {
            Id = notification.Id,
            NotificationTitle = notification.Title,
            NotificationText = notification.Body,
            TargetCategory = notification.Target,
            NotificationType = notification.Type,
            TransmissionMethod = notification.Method,
            CreatedAt = notification.CreatedAt,
        });
    }

    public async Task<(bool Success, string Message, string Code, NotificationResponseDto? Data)> ResendAsync(int id)
    {
        var notification = await _db.Notifications.FindAsync(id);
        if (notification is null)
            return (false, "Notification not found.", AppCodes.NotFound, null);

        // Create a new copy with the same data
        var resent = new Data.Notification
        {
            Title = notification.Title,
            Body = notification.Body,
            Target = notification.Target,
            Type = notification.Type,
            Method = notification.Method,
            CreatedAt = DateTime.UtcNow,
        };

        _db.Notifications.Add(resent);
        await _db.SaveChangesAsync();

        return (true, "Notification resent successfully.", AppCodes.Created, new NotificationResponseDto
        {
            Id = resent.Id,
            NotificationTitle = resent.Title,
            NotificationText = resent.Body,
            TargetCategory = resent.Target,
            NotificationType = resent.Type,
            TransmissionMethod = resent.Method,
            CreatedAt = resent.CreatedAt,
        });
    }

    public async Task<(bool Success, string Message, string Code)> DeleteAsync(int id)
    {
        var notification = await _db.Notifications.FindAsync(id);
        if (notification is null)
            return (false, "Notification not found.", AppCodes.NotFound);

        _db.Notifications.Remove(notification);
        await _db.SaveChangesAsync();

        return (true, "Notification deleted successfully.", AppCodes.Success);
    }
}
