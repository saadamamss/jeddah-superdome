using DotnetStarterKit.Auth.DTOs;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Config;
using DotnetStarterKit.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace DotnetStarterKit.Auth;

[ApiController]
[Route("api/v1/dashboard")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private const string RefreshTokenCookie = "refresh_token";

    public AuthController(AuthService authService, UserManager<User> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _authService = authService;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var (success, message, code, userData, accessToken) = await _authService.LoginAsync(dto);

        if (!success)
        {
            var status = code == AppCodes.AccountLocked ? 423 : 401;
            return StatusCode(status, ApiResponse<object>.Fail(message, code));
        }

        var user = await _userManager.FindByEmailAsync(dto.Email) ?? throw new InvalidOperationException("User unexpectedly null after successful login.");
        var refreshToken = user.RefreshToken;

        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

        // Access token → HttpOnly cookie
        Response.Cookies.Append("access_token", accessToken!, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
        });

        if (string.IsNullOrEmpty(refreshToken))
            return StatusCode(500, ApiResponse<object>.Fail("Failed to generate refresh token.", AppCodes.InternalError));

        // Refresh token → HttpOnly cookie
        Response.Cookies.Append(RefreshTokenCookie, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        });

        // Return user data directly (frontend expects { user: UserDto })
        return Ok(new { user = userData });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
        {
            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
            return Unauthorized(ApiResponse<object>.Fail("No refresh token.", AppCodes.InvalidToken));
        }

        var (success, message, code, accessToken) = await _authService.RefreshAsync(refreshToken);
        if (!success)
        {
            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
            Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions { Path = "/" });
            return Unauthorized(ApiResponse<object>.Fail(message, code));
        }

        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

        Response.Cookies.Append("access_token", accessToken!, new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
        });

        return Ok(ApiResponse<object>.Ok(null!, message, code));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
        await _authService.LogoutAsync(userId);

        Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
        Response.Cookies.Delete(RefreshTokenCookie, new CookieOptions { Path = "/" });

        return Ok(ApiResponse<object>.Ok(null!, "Logged out successfully."));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
        var userDto = await _authService.GetUserAsync(userId);
        if (userDto is null)
            return NotFound(ApiResponse<object>.Fail("User not found.", AppCodes.NotFound));

        return Ok(new { data = userDto });
    }

    [HttpPut("company/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateCompanyProfile(UpdateCompanyProfileDto dto)
    {
        // Get company ID from UserCompany relationship
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized(ApiResponse<object>.Fail("User not found.", AppCodes.Unauthorized));

        var userCompany = user.UserCompanies.FirstOrDefault();
        if (userCompany is null)
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation error.", AppCodes.ValidationError));

        var (success, message, code, data) = await _authService.UpdateCompanyProfileAsync(userCompany.CompanyId, dto);
        if (!success)
            return BadRequest(ApiResponse<object>.Fail(message, code));

        return Ok(new { data, status = new { success = true, message } });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var (_, message) = await _authService.ForgotPasswordAsync(dto);
        return Ok(ApiResponse<object>.Ok(null!, message));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var (success, message, code) = await _authService.ResetPasswordAsync(dto);
        if (!success) return BadRequest(ApiResponse<object>.Fail(message, code));
        return Ok(ApiResponse<object>.Ok(null!, message, code));
    }
}
