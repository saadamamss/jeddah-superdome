# API Contract

The frontend expects a .NET backend API. Base URL is configured via `VITE_API_BASE_URL`.

## Standard Response Format

### Success

```json
{
  "data": {},
  "status": {
    "success": true,
    "message": "Operation completed"
  }
}
```

### Paginated

```json
{
  "data": [],
  "pagination": {
    "i_current_page": 1,
    "i_per_page": 12,
    "i_total_objects": 100,
    "i_total_pages": 9,
    "i_items_on_page": 12
  },
  "status": {
    "success": true
  }
}
```

### Error (ProblemDetails)

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation Error",
  "status": 422,
  "errors": {
    "email": ["The email field is required."]
  },
  "traceId": "abc123"
}
```

## Endpoints

### Auth

| Method | Endpoint                    | Description               |
| ------ | --------------------------- | ------------------------- |
| POST   | `/login`                    | Login with email/password |
| POST   | `/logout`                   | Logout current user       |
| POST   | `/auth/app/reset-password`  | Request password reset    |
| POST   | `/auth/app/verify-otp`      | Verify OTP code           |
| POST   | `/auth/app/update-password` | Set new password          |
| GET    | `/me`                       | Get current user          |

### Companies

| Method | Endpoint            | Description                |
| ------ | ------------------- | -------------------------- |
| GET    | `/companies`        | List companies (paginated) |
| GET    | `/companies/:id`    | Get company by ID          |
| POST   | `/companies`        | Create company             |
| PUT    | `/companies/:id`    | Update company             |
| DELETE | `/companies/:id`    | Delete company             |
| POST   | `/companies/upload` | Upload companies CSV       |

### Pilgrims

| Method | Endpoint           | Description               |
| ------ | ------------------ | ------------------------- |
| GET    | `/pilgrims`        | List pilgrims (paginated) |
| GET    | `/pilgrims/:id`    | Get pilgrim by ID         |
| POST   | `/pilgrims`        | Create pilgrim            |
| PUT    | `/pilgrims/:id`    | Update pilgrim            |
| DELETE | `/pilgrims/:id`    | Delete pilgrim            |
| POST   | `/pilgrims/upload` | Upload pilgrims CSV       |

### Flights

| Method | Endpoint          | Description              |
| ------ | ----------------- | ------------------------ |
| GET    | `/flights`        | List flights (paginated) |
| GET    | `/flights/:id`    | Get flight by ID         |
| POST   | `/flights`        | Create flight            |
| PUT    | `/flights/:id`    | Update flight            |
| DELETE | `/flights/:id`    | Delete flight            |
| POST   | `/flights/upload` | Upload flights CSV       |

### Users

| Method | Endpoint        | Description                         |
| ------ | --------------- | ----------------------------------- |
| GET    | `/users`        | List users (paginated)              |
| GET    | `/users/:id`    | Get user by ID                      |
| POST   | `/users`        | Create user                         |
| PUT    | `/users/:id`    | Update user                         |
| DELETE | `/users/:id`    | Delete user (optional reassignment) |
| POST   | `/users/upload` | Upload users CSV                    |

### Roles

| Method | Endpoint           | Description            |
| ------ | ------------------ | ---------------------- |
| GET    | `/roles`           | List roles (paginated) |
| GET    | `/roles/:id`       | Get role by ID         |
| POST   | `/add-role`        | Create role            |
| PUT    | `/edit-role/:id`   | Update role            |
| DELETE | `/delete-role/:id` | Delete role            |
| GET    | `/permissions`     | List all permissions   |

### Notifications

| Method | Endpoint                    | Description                    |
| ------ | --------------------------- | ------------------------------ |
| GET    | `/notifications`            | List notifications (paginated) |
| POST   | `/notifications/send`       | Send notification              |
| POST   | `/notifications/:id/resend` | Resend notification            |
| DELETE | `/notifications/:id`        | Delete notification            |

### Dashboard

| Method | Endpoint      | Description              |
| ------ | ------------- | ------------------------ |
| GET    | `/statistics` | Get dashboard statistics |

## Query Parameters

All list endpoints support:

- `page` — Page number (default: 1)
- `per_page` — Items per page (default: 12)
- `search` — Search query string

Additional filters:

- `pilgrims` — `company_id` filter
- `flights` — `company_id` filter
- `users` — `role_id`, `district` filters

## Authentication

- JWT token stored in `localStorage` (`auth_token`)
- Refresh token in `localStorage` (`auth_refresh_token`)
- Token sent via `Authorization: Bearer <token>` header
- 401 response triggers refresh flow → redirect to login on failure

## Permissions

CASL-based permissions loaded from `/me` endpoint. Permission strings follow format:

- `companies.view`
- `users.view`
- `roles.view`
- `admin_notifications.view`

Route-level permission checks via `meta.requiresPermission` in router.
