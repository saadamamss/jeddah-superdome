using DotnetStarterKit.Common.Filters;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Notifications.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetStarterKit.Notifications;

[ApiController]
[Route("api/v1/dashboard/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationsService _notificationsService;

    public NotificationsController(NotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    [HttpGet]
    [RequirePermission("admin_notifications.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? search = null)
    {
        var result = await _notificationsService.GetAllAsync(page, perPage, search);
        return Ok(result);
    }

    [HttpPost("send")]
    [RequirePermission("admin_notifications.create")]
    public async Task<IActionResult> Send([FromBody] SendNotificationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var (success, message, code, data) = await _notificationsService.SendAsync(dto);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpPost("{id:int}/resend")]
    [RequirePermission("admin_notifications.create")]
    public async Task<IActionResult> Resend(int id)
    {
        var (success, message, code, data) = await _notificationsService.ResendAsync(id);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("admin_notifications.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message, code) = await _notificationsService.DeleteAsync(id);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { status = new { success = true, message } });
    }
}
