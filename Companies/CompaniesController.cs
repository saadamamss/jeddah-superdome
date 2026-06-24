using DotnetStarterKit.Common.Filters;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Companies.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetStarterKit.Companies;

[ApiController]
[Route("api/v1/dashboard/companies")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly CompaniesService _companiesService;

    public CompaniesController(CompaniesService companiesService)
    {
        _companiesService = companiesService;
    }

    [HttpGet]
    [RequirePermission("companies.view")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? search = null)
    {
        var result = await _companiesService.GetAllAsync(page, perPage, search);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("companies.view")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _companiesService.GetByIdAsync(id);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Company not found.", AppCodes.NotFound));

        return Ok(new { data = result });
    }

    [HttpPost]
    [RequirePermission("companies.create")]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var (success, message, code, data) = await _companiesService.CreateAsync(dto);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpPut("{id:int}")]
    [RequirePermission("companies.edit")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var (success, message, code, data) = await _companiesService.UpdateAsync(id, dto);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("companies.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message, code) = await _companiesService.DeleteAsync(id);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { status = new { success = true, message } });
    }

    [HttpPost("upload")]
    [RequirePermission("companies.create")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var (success, message, code, data) = await _companiesService.UploadAsync(file);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpGet("template")]
    [AllowAnonymous]
    public IActionResult DownloadTemplate()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "template-companies.csv");
        if (!System.IO.File.Exists(filePath))
            return NotFound(ApiResponse<object>.Fail("Template file not found.", AppCodes.NotFound));

        var bytes = System.IO.File.ReadAllBytes(filePath);
        return File(bytes, "text/csv", "template-companies.csv");
    }
}
