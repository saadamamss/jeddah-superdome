# Jeddah Superdome — Backend API

.NET 8 REST API for the Jeddah Superdome Hajj management system.

## Tech Stack

- **Framework** — ASP.NET Core 8
- **Database** — MySQL 8 via EF Core (Pomelo)
- **Auth** — ASP.NET Identity + JWT (HttpOnly cookies)
- **Real-time** — SignalR
- **Mail** — MailKit
- **Logging** — Serilog
- **Docs** — Swagger (dev only)

## Project Structure

```
backend/
├── Auth/               Login, refresh, logout
├── Common/             Filters, middleware, models
├── Companies/          Company CRUD
├── Config/             Strongly-typed settings
├── Dashboard/          Statistics endpoint
├── Data/               DbContext, entities, seeder
├── Flights/            Flight CRUD
├── Hubs/               SignalR notification hub
├── Mail/               Email service
├── Migrations/         EF Core migrations
├── Notifications/      Send, list, mark-read, delivery
├── Pilgrims/           Pilgrim CRUD
├── Roles/              Role & permission management
├── Services/           Background services
├── Upload/             File upload
├── Users/              User management
└── Program.cs          App entry point
```

## API Endpoints

All endpoints are prefixed with `/api/v1/dashboard`.

### Auth

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/login` | Public | Login, sets HttpOnly cookies |
| POST | `/refresh` | Cookie | Refresh access token |
| POST | `/logout` | Required | Clear cookies |
| GET | `/me` | Required | Current user |
| POST | `/forgot-password` | Public | Send reset email |
| POST | `/reset-password` | Public | Reset password |

### Notifications

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/notifications` | Admin | List all sent |
| POST | `/notifications/send` | Admin | Send notification |
| GET | `/notifications/{id}/recipients` | Admin | View recipients |
| POST | `/notifications/{id}/mark-read` | Required | Mark as read |
| GET | `/me/notifications` | Required | My notifications |
| GET | `/me/notifications/unread-count` | Required | Unread count |
| POST | `/notifications/{id}/resend` | Admin | Resend |

### SignalR

| Hub | Path | Description |
|-----|------|-------------|
| NotificationHub | `/hubs/notifications` | Real-time notification push |

Events: `ReceiveNotification`, `UnreadCount`

### Other Resources

| Resource | Path | Description |
|----------|------|-------------|
| Companies | `/companies` | CRUD with pagination |
| Pilgrims | `/pilgrims` | CRUD with pagination, CSV export |
| Flights | `/flights` | CRUD with pagination |
| Users | `/users` | Admin CRUD |
| Roles | `/roles` | Role & permission management |
| Dashboard | `/dashboard` | Statistics |
| Upload | `/upload` | File upload (images + files) |
| Health | `/health` | Health check |

## Configuration

Key settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=jeddah;User=root;Password=;"
  },
  "JwtSettings": {
    "Secret": "32-char-min secret",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "CorsSettings": {
    "AllowedOrigins": "http://localhost:5173"
  }
}
```

## Local Email Testing

Use [Mailpit](https://github.com/axllent/mailpit):

```bash
mailpit
# SMTP: localhost:1025, UI: http://localhost:8025
```

## Default Admin

| Field | Value |
|-------|-------|
| Email | admin@starter.com |
| Password | Admin@123456 |
