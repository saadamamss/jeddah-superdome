using DotnetStarterKit.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotnetStarterKit.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Role> BusinessRoles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();
    public DbSet<Pilgrim> Pilgrims => Set<Pilgrim>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<FlightPilgrim> FlightPilgrims => Set<FlightPilgrim>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Rename Identity tables
        builder.Entity<User>().ToTable("users");
        builder.Entity<IdentityRole>().ToTable("identity_roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("user_identity_roles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("user_logins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");

        // Permission
        builder.Entity<Permission>(e =>
        {
            e.ToTable("permissions");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
            e.HasIndex(p => p.Name).IsUnique();
        });

        // Role (business role, not Identity role)
        builder.Entity<Role>(e =>
        {
            e.ToTable("roles");
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).HasMaxLength(50).IsRequired();
            e.HasIndex(r => r.Name).IsUnique();
            e.Property(r => r.Description).HasMaxLength(200);
        });

        // RolePermission (many-to-many)
        builder.Entity<RolePermission>(e =>
        {
            e.ToTable("role_permissions");
            e.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            e.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);
            e.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
        });

        // Company
        builder.Entity<Company>(e =>
        {
            e.ToTable("companies");
            e.HasKey(c => c.Id);
            e.Property(c => c.CompanyName).HasMaxLength(200).IsRequired();
            e.Property(c => c.LicenseNumber).HasMaxLength(100).IsRequired();
            e.Property(c => c.City).HasMaxLength(100).IsRequired();
            e.Property(c => c.ContactNumber).HasMaxLength(20).IsRequired();
            e.Property(c => c.Email).HasMaxLength(200).IsRequired();
            e.Property(c => c.Address).HasMaxLength(500);
            e.Property(c => c.IsActive).IsRequired();
            e.HasIndex(c => c.LicenseNumber).IsUnique();
            e.HasIndex(c => c.Email).IsUnique();
        });

        // UserCompany (many-to-many)
        builder.Entity<UserCompany>(e =>
        {
            e.ToTable("user_companies");
            e.HasKey(uc => new { uc.UserId, uc.CompanyId });
            e.HasOne(uc => uc.User)
                .WithMany(u => u.UserCompanies)
                .HasForeignKey(uc => uc.UserId);
            e.HasOne(uc => uc.Company)
                .WithMany(c => c.UserCompanies)
                .HasForeignKey(uc => uc.CompanyId);
        });

        // Pilgrim
        builder.Entity<Pilgrim>(e =>
        {
            e.ToTable("pilgrims");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(200).IsRequired();
            e.Property(p => p.Identity).HasMaxLength(50).IsRequired();
            e.Property(p => p.Nationality).HasMaxLength(100).IsRequired();
            e.Property(p => p.Gender).HasMaxLength(10).IsRequired();
            e.Property(p => p.BirthDate).HasMaxLength(20).IsRequired();
            e.Property(p => p.Phone).HasMaxLength(20).IsRequired();
            e.Property(p => p.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(p => p.Identity).IsUnique();
            e.HasOne(p => p.Company)
                .WithMany(c => c.Pilgrims)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Flight
        builder.Entity<Flight>(e =>
        {
            e.ToTable("flights");
            e.HasKey(f => f.Id);
            e.Property(f => f.FlightNumber).HasMaxLength(50).IsRequired();
            e.Property(f => f.Date).HasMaxLength(20).IsRequired();
            e.Property(f => f.Time).HasMaxLength(20).IsRequired();
            e.Property(f => f.Destination).HasMaxLength(200).IsRequired();
            e.Property(f => f.Transport).HasMaxLength(50).IsRequired();
            e.Property(f => f.Status).HasMaxLength(20).IsRequired().HasDefaultValue("Scheduled");
            e.Property(f => f.Capacity).HasDefaultValue(0);
            e.HasIndex(f => f.FlightNumber).IsUnique();
            e.HasOne(f => f.Company)
                .WithMany()
                .HasForeignKey(f => f.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasMany(f => f.FlightPilgrims)
                .WithOne(fp => fp.Flight)
                .HasForeignKey(fp => fp.FlightId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FlightPilgrim
        builder.Entity<FlightPilgrim>(e =>
        {
            e.ToTable("flight_pilgrims");
            e.HasKey(fp => new { fp.FlightId, fp.PilgrimId });
            e.Property(fp => fp.AssignedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.HasOne(fp => fp.Flight)
                .WithMany(f => f.FlightPilgrims)
                .HasForeignKey(fp => fp.FlightId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(fp => fp.Pilgrim)
                .WithMany(p => p.FlightPilgrims)
                .HasForeignKey(fp => fp.PilgrimId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Notification
        builder.Entity<Notification>(e =>
        {
            e.ToTable("notifications");
            e.HasKey(n => n.Id);
            e.Property(n => n.Title).HasMaxLength(200).IsRequired();
            e.Property(n => n.Body).HasMaxLength(2000).IsRequired();
            e.Property(n => n.Target).HasMaxLength(100).IsRequired();
            e.Property(n => n.Type).HasMaxLength(50).IsRequired();
            e.Property(n => n.Method).HasMaxLength(50).IsRequired();
            e.Property(n => n.SentByUserId).HasMaxLength(450);
            e.Property(n => n.TargetCompanyId);
            e.HasOne(n => n.SentByUser)
                .WithMany()
                .HasForeignKey(n => n.SentByUserId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(n => n.TargetCompany)
                .WithMany()
                .HasForeignKey(n => n.TargetCompanyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // UserNotification
        builder.Entity<UserNotification>(e =>
        {
            e.ToTable("user_notifications");
            e.HasKey(un => un.Id);
            e.HasOne(un => un.Notification)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(un => un.User)
                .WithMany()
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(un => new { un.UserId, un.NotificationId }).IsUnique();
        });

        // User → Role (optional FK)
        builder.Entity<User>(e =>
        {
            e.Property(u => u.IntId).IsRequired();
            e.HasIndex(u => u.IntId).IsUnique();
            e.Property(u => u.Name).HasMaxLength(100).IsRequired();
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.IdentityNumber).HasMaxLength(20);
            e.Property(u => u.RefreshToken).HasMaxLength(200);
            e.Property(u => u.RefreshTokenExpiry);
            e.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
