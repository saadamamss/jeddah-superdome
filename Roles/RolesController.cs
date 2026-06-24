using DotnetStarterKit.Common.Filters;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Roles.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetStarterKit.Roles;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly RolesService _rolesService;

    public RolesController(RolesService rolesService)
    {
        _rolesService = rolesService;
    }

    [HttpGet("roles")]
    [RequirePermission("roles.view")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? search = null)
    {
        var result = await _rolesService.GetAllAsync(page, perPage, search);
        return Ok(result);
    }

    [HttpGet("roles/{id:int}")]
    [RequirePermission("roles.view")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _rolesService.GetByIdAsync(id);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Role not found.", AppCodes.NotFound));

        return Ok(result);
    }

    [HttpPost("add-role")]
    [RequirePermission("roles.create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var result = await _rolesService.CreateAsync(dto);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message, result.Code));

        return Ok(result);
    }

    [HttpPut("edit-role/{id:int}")]
    [RequirePermission("roles.edit")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateRoleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var result = await _rolesService.UpdateAsync(id, dto);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message, result.Code));

        return Ok(result);
    }

    [HttpDelete("delete-role/{id:int}")]
    [RequirePermission("roles.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _rolesService.DeleteAsync(id);
        if (!result.Success)
            return BadRequest(ApiResponse<object>.Fail(result.Message, result.Code));

        return Ok(result);
    }

    [HttpGet("permissions")]
    [RequirePermission("roles.view")]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await _rolesService.GetPermissionsAsync();
        return Ok(result);
    }
}
