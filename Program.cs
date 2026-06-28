using DotnetStarterKit.Config;
using Asp.Versioning;
using DotnetStarterKit.Common.Filters;
using DotnetStarterKit.Common.Middleware;
using DotnetStarterKit.Data;
using DotnetStarterKit.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotnetStarterKit.Auth;
using DotnetStarterKit.Mail;
using DotnetStarterKit.Upload;
using DotnetStarterKit.Roles;
using DotnetStarterKit.Companies;
using DotnetStarterKit.Pilgrims;
using DotnetStarterKit.Flights;
using DotnetStarterKit.Notifications;
using DotnetStarterKit.Dashboard;
using Serilog;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ── PORT binding (Railway / cloud fallback) ────────────────────
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (string.IsNullOrEmpty(urls))
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// ── Strongly-Typed Config ──────────────────────────────────────
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("App"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<RateLimitingSettings>(builder.Configuration.GetSection("RateLimiting"));

// ── Rate Limiting ──────────────────────────────────────────────
var rateLimitSettings = builder.Configuration.GetSection("RateLimiting").Get<RateLimitingSettings>()!;

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", o =>
    {
        o.PermitLimit = rateLimitSettings.PermitLimit;
        o.Window = TimeSpan.FromMinutes(rateLimitSettings.WindowMinutes);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("login", o =>
    {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Too many requests. Please try again later.",
            code = "TOO_MANY_REQUESTS",
            data = (object?)null
        }, token);
    };
});

// ── CORS ───────────────────────────────────────────────────────
var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>()!;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsSettings.GetOrigins())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ── Database (SQLite) ──────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ── Identity ───────────────────────────────────────────────────
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── JWT Authentication ─────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;
            if (path.StartsWithSegments("/hubs"))
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                else
                {
                    var cookieToken = context.Request.Cookies["access_token"];
                    if (!string.IsNullOrEmpty(cookieToken))
                        context.Token = cookieToken;
                }
            }
            return Task.CompletedTask;
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var response = new
            {
                success = false,
                message = "Unauthorized. Please login first.",
                code = "UNAUTHORIZED",
                data = (object?)null
            };
            await context.Response.WriteAsJsonAsync(response);
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var response = new
            {
                success = false,
                message = "You do not have permission to access this resource.",
                code = "FORBIDDEN",
                data = (object?)null
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    };
});

// ── Services ───────────────────────────────────────────────────
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RolesService>();
builder.Services.AddScoped<CompaniesService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<PilgrimsService>();
builder.Services.AddScoped<FlightsService>();
builder.Services.AddScoped<NotificationsService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<UploadService>();

// ── SignalR ─────────────────────────────────────────────────────
builder.Services.AddSignalR();

// ── Controllers ────────────────────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// ── API Versioning ─────────────────────────────────────────────
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

// ── Swagger (Dev only) ─────────────────────────────────────────
#if DEBUG
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endif

// ── HTTP Context ───────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ── Pipeline ───────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseStaticFiles();

// ── Security Headers ───────────────────────────────────────────
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<CookieToHeaderMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// ── Auto-migrate + Seed ────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var conn = db.Database.GetDbConnection();
    await conn.OpenAsync();

    // Check if any tables exist
    await using (var checkCmd = conn.CreateCommand())
    {
        checkCmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_type = 'BASE TABLE'";
        var tableCount = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());

        if (tableCount == 0)
        {
            // Fresh database — run all migrations
            logger.LogInformation("No tables found. Running all migrations...");
            await db.Database.MigrateAsync();
        }
        else
        {
            // Existing database — ensure migration history table exists
            await using (var createCmd = conn.CreateCommand())
            {
                createCmd.CommandText = """
                    CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
                        `MigrationId` varchar(150) NOT NULL,
                        `ProductVersion` varchar(32) NOT NULL,
                        PRIMARY KEY (`MigrationId`)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
                    """;
                await createCmd.ExecuteNonQueryAsync();
            }

            // Record all pending migrations as applied
            var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
            if (pending.Count != 0)
            {
                await using (var insertCmd = conn.CreateCommand())
                {
                    foreach (var migration in pending)
                    {
                        insertCmd.CommandText = $"INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) VALUES ('{migration.Replace("'", "''")}', '8.0.2')";
                        await insertCmd.ExecuteNonQueryAsync();
                    }
                }

                logger.LogInformation("Migration history synced ({Count} pending migration(s) recorded).", pending.Count);
            }
        }
    }

    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
