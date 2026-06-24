# Services

All API calls go through the service layer. Services extend `BaseService` which wraps axios.

## Usage

```typescript
import { companiesService } from '@/services/companies.service'

// List (paginated)
const response = await companiesService.getAll({ page: 1, per_page: 10 })

// Get by ID
const company = await companiesService.getById(1)

// Create
const result = await companiesService.create({ companyName: 'ACME' })

// Update
const result = await companiesService.update(1, { companyName: 'ACME Corp' })

// Delete
await companiesService.delete(1)
```

## Services

| Service                | File                       | Description                                          |
| ---------------------- | -------------------------- | ---------------------------------------------------- |
| `BaseService`          | `base.service.ts`          | Abstract HTTP wrapper (GET, POST, PUT, DELETE)       |
| `AuthService`          | `auth.service.ts`          | Login, logout, OTP, password reset, token management |
| `CompaniesService`     | `companies.service.ts`     | Company CRUD + CSV upload                            |
| `PilgrimsService`      | `pilgrims.service.ts`      | Pilgrim CRUD + CSV upload                            |
| `FlightsService`       | `flights.service.ts`       | Flight CRUD + CSV upload                             |
| `UsersService`         | `users.service.ts`         | User CRUD + CSV upload                               |
| `RolesService`         | `roles.service.ts`         | Role CRUD + permissions                              |
| `NotificationsService` | `notifications.service.ts` | Notification send/list/delete                        |
| `DashboardService`     | `dashboard.service.ts`     | Dashboard statistics                                 |

## Adding a New Service

1. Create `src/services/foo.service.ts`:

```typescript
import BaseService from './base.service'
import type { PaginatedResponse } from '@/types'

interface Foo {
  id: number
  name: string
}

class FooService extends BaseService {
  private readonly BASE = '/foos'

  async getAll(params?: { page?: number; per_page?: number }): Promise<PaginatedResponse<Foo>> {
    return this.get<PaginatedResponse<Foo>>(this.BASE, { params })
  }

  async getById(id: number): Promise<{ data: Foo }> {
    return this.get<{ data: Foo }>(`${this.BASE}/${id}`)
  }

  async create(data: Partial<Foo>): Promise<{ data: Foo; status: { success: boolean; message: string } }> {
    return this.post(this.BASE, data)
  }

  async update(id: number, data: Partial<Foo>): Promise<{ data: Foo; status: { success: boolean; message: string } }> {
    return this.put(`${this.BASE}/${id}`, data)
  }

  async delete(id: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.deleteRequest(`${this.BASE}/${id}`)
  }
}

export const fooService = new FooService()
```

2. Add types to `src/types/index.ts`
3. Add tests in `src/services/__tests__/foo.service.spec.ts`
