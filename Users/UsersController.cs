using DotnetStarterKit.Common.Filters;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Users.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetStarterKit.Users;

[ApiController]
[Route("api/v1/dashboard/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    [RequirePermission("users.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? roleId = null)
    {
        var result = await _usersService.GetAllAsync(page, perPage, search, roleId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [RequirePermission("users.view")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _usersService.GetByIdAsync(id);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("User not found.", AppCodes.NotFound));

        return Ok(new { data = result });
    }

    [HttpPost]
    [RequirePermission("users.create")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var (success, message, code, data) = await _usersService.CreateAsync(dto);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpPut("{id:int}")]
    [RequirePermission("users.edit")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var (success, message, code, data) = await _usersService.UpdateAsync(id, dto);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpDelete("{id:int}")]
    [RequirePermission("users.delete")]
    public async Task<IActionResult> Delete(int id, [FromBody] DeleteUserDto? dto = null)
    {
        var (success, message, code) = await _usersService.DeleteAsync(id);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { status = new { success = true, message } });
    }

    [HttpPost("upload")]
    [RequirePermission("users.create")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var (success, message, code, data) = await _usersService.UploadAsync(file);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpGet("template")]
    [AllowAnonymous]
    public IActionResult DownloadTemplate()
    {
        var csv = "name,email,phone,password,status\n";
        var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
        return File(bytes, "text/csv", "template-users.csv");
    }
}
