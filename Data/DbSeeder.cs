using DotnetStarterKit.Data;
using DotnetStarterKit.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var db = services.GetRequiredService<AppDbContext>();

        // ── Seed Permissions ─────────────────────────────────────
        string[] permissionNames =
        [
            "companies.view", "companies.create", "companies.edit", "companies.delete",
            "pilgrims.view", "pilgrims.create", "pilgrims.edit", "pilgrims.delete",
            "flights.view", "flights.create", "flights.edit", "flights.delete",
            "users.view", "users.create", "users.edit", "users.delete",
            "roles.view", "roles.create", "roles.edit", "roles.delete",
            "admin_notifications.view", "admin_notifications.create", "admin_notifications.edit", "admin_notifications.delete"
        ];

        foreach (var name in permissionNames)
        {
            if (!await db.Permissions.AnyAsync(p => p.Name == name))
                db.Permissions.Add(new Permission { Name = name });
        }
        await db.SaveChangesAsync();

        // ── Seed Roles ───────────────────────────────────────────
        var allPermissions = await db.Permissions.ToListAsync();

        // Admin role — all permissions
        var adminRole = await db.BusinessRoles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole is null)
        {
            adminRole = new Role { Name = "Admin", Description = "Full access" };
            db.BusinessRoles.Add(adminRole);
            await db.SaveChangesAsync();

            foreach (var perm in allPermissions)
                db.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = perm.Id });
            await db.SaveChangesAsync();
        }

        // User role — view only
        var userRole = await db.BusinessRoles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Name == "User");
        if (userRole is null)
        {
            userRole = new Role { Name = "User", Description = "View only" };
            db.BusinessRoles.Add(userRole);
            await db.SaveChangesAsync();

            var viewPerms = allPermissions.Where(p => p.Name.EndsWith(".view")).ToList();
            foreach (var perm in viewPerms)
                db.RolePermissions.Add(new RolePermission { RoleId = userRole.Id, PermissionId = perm.Id });
            await db.SaveChangesAsync();
        }

        // Moderator role — create/edit + users view
        var modRole = await db.BusinessRoles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Name == "Moderator");
        if (modRole is null)
        {
            modRole = new Role { Name = "Moderator", Description = "Create/Edit access" };
            db.BusinessRoles.Add(modRole);
            await db.SaveChangesAsync();

            var modPerms = allPermissions.Where(p =>
                p.Name.EndsWith(".view") || p.Name.EndsWith(".create") || p.Name.EndsWith(".edit") ||
                p.Name == "users.view").ToList();
            foreach (var perm in modPerms)
                db.RolePermissions.Add(new RolePermission { RoleId = modRole.Id, PermissionId = perm.Id });
            await db.SaveChangesAsync();
        }

        // Company role — view own pilgrims, flights, and notifications
        var companyRole = await db.BusinessRoles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Name == "Company");
        if (companyRole is null)
        {
            companyRole = new Role { Name = "Company", Description = "View own pilgrims, flights, notifications" };
            db.BusinessRoles.Add(companyRole);
            await db.SaveChangesAsync();

            var companyPerms = allPermissions.Where(p =>
                p.Name == "pilgrims.view" ||
                p.Name == "flights.view" ||
                p.Name == "admin_notifications.view").ToList();
            foreach (var perm in companyPerms)
                db.RolePermissions.Add(new RolePermission { RoleId = companyRole.Id, PermissionId = perm.Id });
            await db.SaveChangesAsync();
        }

        // ── Seed Identity Roles (for JWT claims) ─────────────────
        string[] identityRoles = ["Admin", "User", "Moderator", "company"];
        foreach (var role in identityRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ── Seed Admin User ──────────────────────────────────────
        const string adminEmail = "admin@test.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var maxIntId = db.Users.Any() ? db.Users.Max(u => u.IntId) : 0;
            var admin = new User
            {
                IntId = maxIntId + 1,
                Name = "Admin",
                Email = adminEmail,
                UserName = adminEmail,
                Phone = "+966500000000",
                IdentityNumber = "1234567890",
                EmailVerified = true,
                IsActive = true,
                RoleId = adminRole.Id
            };

            var result = await userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── Seed Test Company ──────────────────────────────────
        if (!await db.Companies.AnyAsync())
        {
            var company = new Company
            {
                CompanyName = "شركة الاختبار",
                LicenseNumber = "LIC-001",
                Email = "test@company.com",
                ContactNumber = "+966501234567",
                City = "جدة",
                Address = "شارع الملك فهد",
            };

            db.Companies.Add(company);
            await db.SaveChangesAsync();

            // Create Identity User for company login
            var companyEmail = "test@company.com";
            if (await userManager.FindByEmailAsync(companyEmail) is null)
            {
                var maxIntId = db.Users.Any() ? db.Users.Max(u => u.IntId) : 0;
                var companyUser = new User
                {
                    IntId = maxIntId + 1,
                    Name = "شركة الاختبار",
                    Email = companyEmail,
                    UserName = companyEmail,
                    Phone = "+966501234567",
                    EmailVerified = true,
                    IsActive = true,
                    RoleId = companyRole.Id,
                };

                var result = await userManager.CreateAsync(companyUser, "Test@1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(companyUser, "company");
                    db.UserCompanies.Add(new UserCompany
                    {
                        UserId = companyUser.Id,
                        CompanyId = company.Id,
                    });
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
