using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetStarterKit.Auth.DTOs;
using DotnetStarterKit.Common.Models;
using DotnetStarterKit.Config;
using DotnetStarterKit.Data;
using DotnetStarterKit.Mail;
using DotnetStarterKit.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace DotnetStarterKit.Auth;

public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _db;
    private readonly JwtSettings _jwt;
    private readonly MailService _mailService;
    private readonly AppSettings _appSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        AppDbContext db,
        IOptions<JwtSettings> jwt,
        MailService mailService,
        IOptions<AppSettings> appSettings,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt.Value;
        _mailService = mailService;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    // ── Login ──────────────────────────────────────────────────
    public async Task<(bool Success, string Message, string Code, UserDto? User, string? AccessToken)> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return (false, "Invalid credentials.", AppCodes.InvalidCredentials, null, null);

        // Account locked?
        if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
        {
            var remaining = (int)(user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
            return (false, $"Account locked. Try again in {remaining} minutes.", AppCodes.AccountLocked, null, null);
        }

        // Password check
        var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!validPassword)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockedUntil = DateTime.UtcNow.AddMinutes(30);
                user.FailedLoginAttempts = 0;
            }
            await _userManager.UpdateAsync(user);
            return (false, "Invalid credentials.", AppCodes.InvalidCredentials, null, null);
        }

        // Reset failed attempts
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        await _userManager.UpdateAsync(user);

        // Get Identity roles for JWT claims
        var identityRoles = await _userManager.GetRolesAsync(user);

        // Build UserDto with business role + permissions (needed for both token and response)
        var userDto = await BuildUserDtoAsync(user);

        // Generate tokens
        var accessToken = GenerateAccessToken(user, identityRoles, userDto.Permissions);
        var refreshToken = GenerateSecureToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiryDays);
        await _userManager.UpdateAsync(user);

        return (true, "Login successful.", AppCodes.Success, userDto, accessToken);
    }

    // ── Refresh Token ──────────────────────────────────────────
    public async Task<(bool Success, string Message, string Code, string? AccessToken)> RefreshAsync(string refreshToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null)
            return (false, "Invalid refresh token.", AppCodes.InvalidToken, null);

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(user);
            return (false, "Refresh token expired.", AppCodes.InvalidToken, null);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = user.RoleId.HasValue
            ? await _db.RolePermissions
                .Where(rp => rp.RoleId == user.RoleId)
                .Select(rp => rp.Permission.Name)
                .ToArrayAsync()
            : [];
        var newAccessToken = GenerateAccessToken(user, roles, permissions);
        return (true, "Token refreshed.", AppCodes.Success, newAccessToken);
    }

    // ── Logout ─────────────────────────────────────────────────
    public async Task<(bool Success, string Message)> LogoutAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return (false, "User not found.");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.TokenVersion++;
        await _userManager.UpdateAsync(user);

        return (true, "Logged out successfully.");
    }

    // ── Get Current User ───────────────────────────────────────
    public async Task<UserDto?> GetUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return null;
        return await BuildUserDtoAsync(user);
    }

    // ── Get Current Company ────────────────────────────────────
    public async Task<CompanyInfoDto?> GetCompanyAsync(int companyId)
    {
        var company = await _db.Companies.FindAsync(companyId);
        if (company is null) return null;

        return new CompanyInfoDto
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Email = company.Email,
            Phone = company.ContactNumber,
            City = company.City,
            LicenseNo = company.LicenseNumber,
            Address = company.Address,
        };
    }

    public async Task<(bool Success, string Message, string Code, CompanyInfoDto? Data)> UpdateCompanyProfileAsync(int companyId, UpdateCompanyProfileDto dto)
    {
        var company = await _db.Companies.FindAsync(companyId);
        if (company is null)
            return (false, "Company not found.", AppCodes.NotFound, null);

        if (await _db.Companies.AnyAsync(c => c.Email == dto.Email && c.Id != companyId))
            return (false, "Email already exists.", AppCodes.Conflict, null);

        if (await _db.Companies.AnyAsync(c => c.LicenseNumber == dto.LicenseNo && c.Id != companyId))
            return (false, "License number already exists.", AppCodes.Conflict, null);

        company.CompanyName = dto.CompanyName;
        company.LicenseNumber = dto.LicenseNo;
        company.City = dto.City ?? string.Empty;
        company.ContactNumber = dto.Phone ?? string.Empty;
        company.Email = dto.Email;
        company.Address = dto.Address ?? string.Empty;

        // Update linked user
        var userCompany = await _db.UserCompanies.FirstOrDefaultAsync(uc => uc.CompanyId == companyId);
        if (userCompany is not null)
        {
            var user = await _userManager.FindByIdAsync(userCompany.UserId);
            if (user is not null)
            {
                user.Name = dto.CompanyName;
                user.Email = dto.Email;
                user.UserName = dto.Email;
                user.Phone = dto.Phone;
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
        }

        await _db.SaveChangesAsync();

        return (true, "Company profile updated successfully.", AppCodes.Success, new CompanyInfoDto
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            Email = company.Email,
            Phone = company.ContactNumber,
            City = company.City,
            LicenseNo = company.LicenseNumber,
            Address = company.Address,
        });
    }

    // ── Forgot Password ────────────────────────────────────────
    public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return (true, "If this email exists, a reset link has been sent.");

        user.ResetToken = GenerateSecureToken();
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _userManager.UpdateAsync(user);

        try
        {
            await _mailService.SendPasswordResetAsync(
                dto.Email,
                user.Name,
                user.ResetToken,
                _appSettings.FrontendUrl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send password reset email to {Email}", dto.Email);
        }

        return (true, "If this email exists, a reset link has been sent.");
    }

    // ── Reset Password ─────────────────────────────────────────
    public async Task<(bool Success, string Message, string Code)> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return (false, "Invalid request.", AppCodes.NotFound);

        if (user.ResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
            return (false, "Invalid or expired token.", AppCodes.InvalidToken);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, errors, AppCodes.ValidationError);
        }

        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.TokenVersion++;
        await _userManager.UpdateAsync(user);

        return (true, "Password reset successfully.", AppCodes.Success);
    }

    // ── Helpers ────────────────────────────────────────────────

    private async Task<UserDto> BuildUserDtoAsync(User user)
    {
        // Load business role + permissions from DB
        var role = user.RoleId.HasValue
            ? await _db.BusinessRoles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == user.RoleId)
            : null;

        // Check if user has "company" Identity role — load Company business role if no RoleId set
        var identityRoles = await _userManager.GetRolesAsync(user);
        if (role is null && identityRoles.Contains("company"))
        {
            role = await _db.BusinessRoles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == "Company");

            // Backfill RoleId on the user so future calls don't need this fallback
            if (role is not null)
            {
                user.RoleId = role.Id;
                await _userManager.UpdateAsync(user);
            }
        }

        var permissionNames = role?.RolePermissions
            .Select(rp => rp.Permission.Name)
            .ToArray() ?? [];

        // Load parent companies
        var parentCompanies = await _db.UserCompanies
            .Where(uc => uc.UserId == user.Id)
            .Select(uc => new { uc.CompanyId })
            .ToListAsync();

        CompanyInfoDto? companyInfo = null;

        if (identityRoles.Contains("company"))
        {
            var userCompany = await _db.UserCompanies
                .FirstOrDefaultAsync(uc => uc.UserId == user.Id);

            if (userCompany is not null)
            {
                var company = await _db.Companies.FindAsync(userCompany.CompanyId);
                if (company is not null)
                {
                    companyInfo = new CompanyInfoDto
                    {
                        Id = company.Id,
                        CompanyName = company.CompanyName,
                        Email = company.Email,
                        Phone = company.ContactNumber,
                        City = company.City,
                        LicenseNo = company.LicenseNumber,
                        Address = company.Address,
                    };
                }
            }
        }

        return new UserDto
        {
            Id = user.IntId,
            Name = user.Name,
            Email = user.Email ?? string.Empty,
            Phone = user.Phone,
            IdentityNumber = user.IdentityNumber,
            Role = role is not null ? new RoleDto { Id = role.Id, Name = role.Name } : null,
            Status = user.IsActive ? "active" : "inactive",
            Permissions = permissionNames,
            ParentCompanies = parentCompanies.Select(pc => (object)new { id = pc.CompanyId }).ToArray(),
            Company = companyInfo,
        };
    }

    public string GenerateAccessToken(User user, IList<string> roles, string[] permissions)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.Name),
            new("tokenVersion", user.TokenVersion.ToString())
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(permissions.Select(p => new Claim("permission", p)));

        // Add companyId and userType claims for company users
        if (roles.Contains("company"))
        {
            claims.Add(new("userType", "company"));
            var userCompany = _db.UserCompanies.FirstOrDefault(uc => uc.UserId == user.Id);
            if (userCompany is not null)
            {
                claims.Add(new("companyId", userCompany.CompanyId.ToString()));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateSecureToken() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
}
