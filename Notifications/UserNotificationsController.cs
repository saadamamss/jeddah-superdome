using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Data;
using DotnetStarterKit.Notifications.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Notifications;

[ApiController]
[Route("api/v1/dashboard/user-notifications")]
[Authorize]
public class UserNotificationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserNotificationsController(AppDbContext db)
    {
        _db = db;
    }

    private string GetUserId()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 20,
        [FromQuery] bool? unreadOnly = null)
    {
        var userId = GetUserId();
        var query = _db.UserNotifications
            .Include(un => un.Notification)
            .Where(un => un.UserId == userId);

        if (unreadOnly == true)
            query = query.Where(un => !un.IsRead);

        var total = await query.CountAsync();
        var unreadCount = await _db.UserNotifications
            .Where(un => un.UserId == userId && !un.IsRead)
            .CountAsync();

        var items = await query
            .OrderByDescending(un => un.DeliveredAt)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(un => new UserNotificationResponseDto
            {
                Id = un.Id,
                NotificationId = un.NotificationId,
                Title = un.Notification.Title,
                Body = un.Notification.Body,
                Target = un.Notification.Target,
                Type = un.Notification.Type,
                IsRead = un.IsRead,
                ReadAt = un.ReadAt,
                DeliveredAt = un.DeliveredAt,
                CreatedAt = un.Notification.CreatedAt,
            })
            .ToListAsync();

        return Ok(new
        {
            data = items,
            unreadCount,
            pagination = new
            {
                currentPage = page,
                perPage,
                totalObjects = total,
                totalPages = (int)Math.Ceiling((double)total / perPage),
                itemsOnPage = items.Count,
            },
            status = new { success = true, message = "Operation successful" },
        });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = GetUserId();
        var count = await _db.UserNotifications
            .Where(un => un.UserId == userId && !un.IsRead)
            .CountAsync();

        return Ok(new
        {
            data = new { unreadCount = count },
            status = new { success = true, message = "Operation successful" },
        });
    }

    [HttpPut("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = GetUserId();
        var userNotification = await _db.UserNotifications
            .FirstOrDefaultAsync(un => un.Id == id && un.UserId == userId);

        if (userNotification is null)
            return BadRequest(ApiResponse<object>.Fail("Notification not found.", AppCodes.NotFound));

        userNotification.IsRead = true;
        userNotification.ReadAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { status = new { success = true, message = "Marked as read." } });
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserId();
        var now = DateTime.UtcNow;
        await _db.UserNotifications
            .Where(un => un.UserId == userId && !un.IsRead)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(un => un.IsRead, true)
                .SetProperty(un => un.ReadAt, now));

        return Ok(new { status = new { success = true, message = "All notifications marked as read." } });
    }
}
