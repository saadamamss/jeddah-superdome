using DotnetStarterKit.Config;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DotnetStarterKit.Mail;

public class MailService
{
    private readonly MailSettings _settings;
    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<MailSettings> settings, ILogger<MailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    // ── Core Send ──────────────────────────────────────────────
    private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_settings.From));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.User, _settings.Password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);

        _logger.LogInformation("Email sent to {Email} — {Subject}", toEmail, subject);
    }

    // ── Welcome Email ──────────────────────────────────────────
    public async Task SendWelcomeAsync(string toEmail, string name)
    {
        var html = $"""
            <div style="font-family:sans-serif;max-width:600px;margin:auto">
                <h2>Welcome, {name}! 👋</h2>
                <p>Your account has been created successfully.</p>
                <p>We're glad to have you on board.</p>
            </div>
            """;

        await SendAsync(toEmail, name, "Welcome to Jeddah Superdome", html);
    }

    // ── Notification Email ─────────────────────────────────────
    public async Task SendNotificationEmailAsync(string toEmail, string name, string subject, string body)
    {
        var html = $"""
            <div style="font-family:sans-serif;max-width:600px;margin:auto">
                <h2>{subject}</h2>
                <p>{body}</p>
                <hr style="margin-top:24px" />
                <p style="color:#6B7280;font-size:13px">
                    This is an automated notification from Jeddah Superdome.
                </p>
            </div>
            """;

        await SendAsync(toEmail, name, subject, html);
    }

    // ── Password Reset ─────────────────────────────────────────
    public async Task SendPasswordResetAsync(string toEmail, string name, string token, string frontendUrl)
    {
        var link = $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(toEmail)}&token={token}";

        var html = $"""
            <div style="font-family:sans-serif;max-width:600px;margin:auto">
                <h2>Reset Your Password</h2>
                <p>Hi {name}, we received a request to reset your password.</p>
                <p>This link expires in <strong>1 hour</strong>.</p>
                <a href="{link}"
                   style="display:inline-block;padding:12px 24px;background:#DC2626;color:#fff;
                          text-decoration:none;border-radius:6px;margin-top:16px">
                    Reset Password
                </a>
                <p style="margin-top:24px;color:#6B7280;font-size:13px">
                    Or copy this link: {link}
                </p>
                <p style="color:#6B7280;font-size:13px">
                    If you didn't request this, ignore this email.
                </p>
            </div>
            """;

        await SendAsync(toEmail, name, "Reset Your Password", html);
    }
}