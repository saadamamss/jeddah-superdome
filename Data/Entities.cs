namespace DotnetStarterKit.Data;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. "companies.view"

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<Users.User> Users { get; set; } = [];
}

public class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

public class Company
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int ConnectedHujjaj { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserCompany> UserCompanies { get; set; } = [];
}

public class UserCompany
{
    public string UserId { get; set; } = string.Empty;
    public int CompanyId { get; set; }

    public Users.User User { get; set; } = null!;
    public Company Company { get; set; } = null!;
}

public class Pilgrim
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Identity { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Company? Company { get; set; }
}

public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public int? CompanyId { get; set; }
    public DateTime? Date { get; set; }
    public string Time { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public int NoOfHuj { get; set; }
    public string Transport { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Company? Company { get; set; }
}

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
