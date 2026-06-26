using DotnetStarterKit.Common.Filters;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Flights.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetStarterKit.Flights;

[ApiController]
[Route("api/v1/dashboard/flights")]
[Authorize]
public class FlightsController : ControllerBase
{
    private readonly FlightsService _flightsService;

    public FlightsController(FlightsService flightsService)
    {
        _flightsService = flightsService;
    }

    private int? GetCompanyId()
    {
        var userType = User.FindFirst("userType")?.Value;
        if (userType != "company") return null;

        var companyIdStr = User.FindFirst("companyId")?.Value;
        return int.TryParse(companyIdStr, out var companyId) ? companyId : null;
    }

    [HttpGet]
    [RequirePermission("flights.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? companyId = null)
    {
        var resolvedCompanyId = GetCompanyId() ?? companyId;
        var result = await _flightsService.GetAllAsync(page, perPage, search, resolvedCompanyId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("flights.view")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyId = GetCompanyId();
        var result = await _flightsService.GetByIdAsync(id, companyId);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Flight not found.", AppCodes.NotFound));

        return Ok(new { data = result });
    }

    [HttpPost]
    [RequirePermission("flights.create")]
    public async Task<IActionResult> Create([FromBody] CreateFlightDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var companyId = GetCompanyId();
        if (companyId.HasValue)
            dto.CompanyId = companyId.Value;

        var (success, message, code, data) = await _flightsService.CreateAsync(dto);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpPut("{id:int}")]
    [RequirePermission("flights.edit")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightDto dto)
    {
        var companyId = GetCompanyId();
        var (success, message, code, data) = await _flightsService.UpdateAsync(id, dto, companyId);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("flights.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var (success, message, code) = await _flightsService.DeleteAsync(id, companyId);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { status = new { success = true, message } });
    }

    [HttpPost("upload")]
    [RequirePermission("flights.create")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var companyId = GetCompanyId();
        var (success, message, code, data) = await _flightsService.UploadAsync(file, companyId);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    // ── Pilgrim Assignment ─────────────────────────────────────

    [HttpPost("{flightId:int}/pilgrims/{pilgrimId:int}")]
    [RequirePermission("flights.edit")]
    public async Task<IActionResult> AssignPilgrim(int flightId, int pilgrimId)
    {
        var companyId = GetCompanyId();
        var (success, message, code) = await _flightsService.AssignPilgrimAsync(flightId, pilgrimId, companyId);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));
        return Ok(new { status = new { success = true, message } });
    }

    [HttpDelete("{flightId:int}/pilgrims/{pilgrimId:int}")]
    [RequirePermission("flights.edit")]
    public async Task<IActionResult> UnassignPilgrim(int flightId, int pilgrimId)
    {
        var companyId = GetCompanyId();
        var (success, message, code) = await _flightsService.UnassignPilgrimAsync(flightId, pilgrimId, companyId);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));
        return Ok(new { status = new { success = true, message } });
    }

    [HttpGet("{flightId:int}/pilgrims")]
    [RequirePermission("flights.view")]
    public async Task<IActionResult> GetAssignedPilgrims(int flightId, [FromQuery] int page = 1, [FromQuery] int perPage = 10)
    {
        var companyId = GetCompanyId();
        var (success, message, code, data) = await _flightsService.GetAssignedPilgrimsAsync(flightId, companyId, page, perPage);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));
        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpGet("template")]
    [AllowAnonymous]
    public IActionResult DownloadTemplate()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "template-flights.csv");
        if (!System.IO.File.Exists(filePath))
            return NotFound(ApiResponse<object>.Fail("Template file not found.", AppCodes.NotFound));

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "text/csv", "template-flights.csv");
    }
}
