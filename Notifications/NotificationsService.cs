using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Data;
using DotnetStarterKit.Mail;
using DotnetStarterKit.Notifications.DTOs;
using DotnetStarterKit.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Notifications;

public class NotificationsService
{
    private readonly AppDbContext _db;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly MailService _mailService;
    private readonly UserManager<User> _userManager;

    public NotificationsService(
        AppDbContext db,
        IHubContext<NotificationHub> hubContext,
        MailService mailService,
        UserManager<User> userManager)
    {
        _db = db;
        _hubContext = hubContext;
        _mailService = mailService;
        _userManager = userManager;
    }

    public async Task<object> GetAllAsync(int page, int perPage, string? search, int? companyId = null)
    {
        var query = _db.Notifications.AsQueryable();

        if (companyId.HasValue)
            query = query.Where(n => n.Target == "شركات");

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
                SentByUserId = n.SentByUserId,
                TargetCompanyId = n.TargetCompanyId,
                CreatedAt = n.CreatedAt,
                RecipientCount = n.UserNotifications.Count,
                ReadCount = n.UserNotifications.Count(un => un.IsRead),
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

    public async Task<(bool Success, string Message, string Code, NotificationResponseDto? Data)> SendAsync(
        SendNotificationDto dto, string? sentByUserId = null)
    {
        // Determine recipients
        List<string> userIdList;
        if (dto.UserIds is { Length: > 0 })
        {
            userIdList = await _userManager.Users
                .Where(u => dto.UserIds.Contains(u.IntId))
                .Select(u => u.Id)
                .ToListAsync();
        }
        else
        {
            userIdList = await ResolveTargetUserIds(dto.Target);
        }

        if (userIdList.Count == 0)
            return (true, "No recipients matched.", AppCodes.Success, null);

        var notification = new Data.Notification
        {
            Title = dto.Title,
            Body = dto.Body,
            Target = dto.Target,
            Type = dto.Type,
            Method = dto.Method,
            SentByUserId = sentByUserId,
            CreatedAt = DateTime.UtcNow,
        };

        var userNotifications = new List<UserNotification>();
        foreach (var userId in userIdList)
        {
            userNotifications.Add(new UserNotification
            {
                Notification = notification,
                UserId = userId,
                DeliveredAt = DateTime.UtcNow,
            });
        }

        _db.Notifications.Add(notification);
        _db.UserNotifications.AddRange(userNotifications);
        await _db.SaveChangesAsync();

        // SignalR push (in-app notification)
        if (dto.Method is "إشعار داخل النظام" or "كلاهما")
        {
            foreach (var un in userNotifications)
            {
                var payload = new
                {
                    id = un.Id,
                    notificationId = notification.Id,
                    title = notification.Title,
                    body = notification.Body,
                    target = notification.Target,
                    type = notification.Type,
                    method = notification.Method,
                    createdAt = notification.CreatedAt,
                };
                await _hubContext.Clients.Group($"user_{un.UserId}").SendAsync("ReceiveNotification", payload);
            }
        }

        // Email delivery if requested
        if (dto.Method is "بريد إلكتروني" or "كلاهما")
        {
            var users = await _userManager.Users.Where(u => userIdList.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    try
                    {
                        await _mailService.SendNotificationEmailAsync(
                            user.Email, user.Name, dto.Title, dto.Body);
                    }
                    catch
                    {
                        // Swallow email failures so notification delivery isn't blocked
                    }
                }
            }
        }

        return (true, "Notification sent successfully.", AppCodes.Created, new NotificationResponseDto
        {
            Id = notification.Id,
            NotificationTitle = notification.Title,
            NotificationText = notification.Body,
            TargetCategory = notification.Target,
            NotificationType = notification.Type,
            TransmissionMethod = notification.Method,
            SentByUserId = notification.SentByUserId,
            TargetCompanyId = notification.TargetCompanyId,
            CreatedAt = notification.CreatedAt,
        });
    }

    public async Task<(bool Success, string Message, string Code, NotificationResponseDto? Data)> ResendAsync(int id, string? sentByUserId = null)
    {
        var original = await _db.Notifications.FindAsync(id);
        if (original is null)
            return (false, "Notification not found.", AppCodes.NotFound, null);

        // Re-send using the same data
        var dto = new SendNotificationDto
        {
            Title = original.Title,
            Body = original.Body,
            Target = original.Target,
            Type = original.Type,
            Method = original.Method,
        };

        return await SendAsync(dto, sentByUserId);
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

    private async Task<List<string>> ResolveTargetUserIds(string target)
    {
        // "all" → everyone
        if (target.Equals("all", StringComparison.OrdinalIgnoreCase))
            return await _userManager.Users.Select(u => u.Id).ToListAsync();

        // "admins" → users without a company
        if (target.Equals("admins", StringComparison.OrdinalIgnoreCase))
            return await _userManager.Users
                .Where(u => !u.UserCompanies.Any())
                .Select(u => u.Id)
                .ToListAsync();

        // "companies" or "شركات" → users with at least one company
        if (target.Equals("companies", StringComparison.OrdinalIgnoreCase) || target == "شركات")
            return await _userManager.Users
                .Where(u => u.UserCompanies.Any())
                .Select(u => u.Id)
                .ToListAsync();

        return new List<string>();
    }
}
