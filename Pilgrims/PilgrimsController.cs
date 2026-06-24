using DotnetStarterKit.Common.Filters;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Pilgrims.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetStarterKit.Pilgrims;

[ApiController]
[Route("api/v1/dashboard/pilgrims")]
[Authorize]
public class PilgrimsController : ControllerBase
{
    private readonly PilgrimsService _pilgrimsService;

    public PilgrimsController(PilgrimsService pilgrimsService)
    {
        _pilgrimsService = pilgrimsService;
    }

    private int? GetCompanyId()
    {
        var userType = User.FindFirst("userType")?.Value;
        if (userType != "company") return null;

        var companyIdStr = User.FindFirst("companyId")?.Value;
        return int.TryParse(companyIdStr, out var companyId) ? companyId : null;
    }

    [HttpGet]
    [RequirePermission("pilgrims.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? companyId = null)
    {
        var resolvedCompanyId = GetCompanyId() ?? companyId;
        var result = await _pilgrimsService.GetAllAsync(page, perPage, search, resolvedCompanyId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("pilgrims.view")]
    public async Task<IActionResult> GetById(int id)
    {
        var companyId = GetCompanyId();
        var result = await _pilgrimsService.GetByIdAsync(id, companyId);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Pilgrim not found.", AppCodes.NotFound));

        return Ok(new { data = result });
    }

    [HttpPost]
    [RequirePermission("pilgrims.create")]
    public async Task<IActionResult> Create([FromBody] CreatePilgrimDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var companyId = GetCompanyId();
        if (companyId.HasValue)
            dto.CompanyId = companyId.Value;

        var (success, message, code, data) = await _pilgrimsService.CreateAsync(dto);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpPut("{id:int}")]
    [RequirePermission("pilgrims.edit")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePilgrimDto dto)
    {
        var companyId = GetCompanyId();
        var (success, message, code, data) = await _pilgrimsService.UpdateAsync(id, dto, companyId);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("pilgrims.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyId();
        var (success, message, code) = await _pilgrimsService.DeleteAsync(id, companyId);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { status = new { success = true, message } });
    }

    [HttpPost("upload")]
    [RequirePermission("pilgrims.create")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var (success, message, code, data) = await _pilgrimsService.UploadAsync(file);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpGet("template")]
    [AllowAnonymous]
    public IActionResult DownloadTemplate()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "template-pilgrims.csv");
        if (!System.IO.File.Exists(filePath))
            return NotFound(ApiResponse<object>.Fail("Template file not found.", AppCodes.NotFound));

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "text/csv", "template-pilgrims.csv");
    }
}
