# 🏛️ Senior-Level Frontend Guide — Jeddah Superdome

> **Purpose:** This document defines the rules, principles, and patterns every developer (or AI agent) must follow to maintain senior-level code quality in this project.

---

## 1. Architecture Rules

### 1.1 Service Layer Pattern

```
src/services/
├── base.service.ts          # Abstract base — HTTP methods (get, post, put, patch, deleteRequest)
├── auth.service.ts          # Auth: login, logout, register, forgotPassword, verifyOtp
├── companies.service.ts     # CRUD: getAll, getById, create, update, delete
├── users.service.ts
├── pilgrims.service.ts
├── flights.service.ts
├── notifications.service.ts
└── dashboard.service.ts     # Read-only: getStatistics
```

**Rules:**

- ✅ **NEVER** call `axiosIns` directly from pages or stores — always use a service
- ✅ Every service **extends** `BaseService` and uses `this.get/post/put/patch/deleteRequest`
- ✅ The `delete` method on BaseService is named `deleteRequest` to avoid infinite recursion when subclass methods shadow it
- ✅ Services are **singletons** — export instances, not classes
- ❌ **NEVER** hardcode API URLs in components — use the service layer
- ❌ **NEVER** use `any` type in service return types — use `T` generics or defined interfaces

### 1.2 Store Layer (Pinia)

```
src/stores/
├── Auth.js      # Auth state + actions (login, logout, fetchUser)
├── Dashboard.js # Dashboard stats
├── Settings.js  # App-wide settings
└── app.js       # Global app state (snackbar, theme)
```

**Rules:**

- ✅ Stores call **services**, not `axiosIns`
- ✅ Stores expose **actions** (methods) and **getters** (computed)
- ✅ Auth store `logout()` must call `authService.logout()` (server invalidation) then clear state
- ❌ **NEVER** put API calls in components — delegate to stores or services
- ❌ **NEVER** mutate state directly — always through actions

### 1.3 Router

- ✅ All authenticated routes set `meta.requiresAuth: true`
- ✅ Routes with permission checks set `meta.requiresPermission: "resource.action"`
- ✅ Auth guard in `router/index.js` uses `authService.isAuthenticated()` — NOT the store
- ✅ Guest-only routes (login, forgot-password) set `meta.guest: true` and redirect authenticated users to `/dashboard`

---

## 2. Component Rules

### 2.1 Reusable Components

Use these **existing** components — don't recreate them:

| Component         | Purpose                                                        |
| ----------------- | -------------------------------------------------------------- |
| `AppBreadcrumb`   | Breadcrumb navigation — pass `items` prop                      |
| `DataTable`       | Vuetify `v-data-table` wrapper                                 |
| `Pagination`      | Page navigation — binds `currentPage`, `perPage`, `totalItems` |
| `SuccessDialog`   | Success alert dialog                                           |
| `DeleteDialog`    | Confirmation dialog before destructive actions                 |
| `ConfirmDialog`   | Generic confirmation dialog                                    |
| `AppSearchHeader` | Search input with debounce                                     |
| `TelInput`        | Phone number input with country codes                          |
| `UploadComponent` | File upload with preview                                       |

### 2.2 Table Page Pattern

For any page with a data table, use `useTablePage` composable:

```vue
<script setup>
import { useTablePage } from '@/composables/useTablePage'
import { myService } from '@/services/my.service'

const {
  items, // Array of records
  isLoading, // Boolean
  search, // Ref<string> — bound to search input
  currentPage, // Ref<number> — bound to Pagination
  perPage, // Ref<number>
  totalItems, // Ref<number> — bound to Pagination
  totalPages, // Ref<number>
  fetchData, // Function — call after CRUD to refresh
  debouncedFetch, // Function — debounced search
} = useTablePage((params) => myService.getAll(params), { entityName: 'البيانات', defaultPerPage: 12 })
</script>
```

**Rules:**

- ✅ Always use `useTablePage` for table-based pages — never manually manage pagination/search/loading
- ✅ Pass the service method as the first argument
- ✅ Call `fetchData()` after create/update/delete to refresh the table
- ❌ **NEVER** duplicate pagination/search/loading logic across pages

### 2.3 Form Handling

- ✅ Use Vuetify `v-form` with `ref` and `:rules` validation
- ✅ Use validators from `@core/utils/validators.js` (requiredValidator, emailValidator, phoneValidator, etc.)
- ✅ Wrap form submissions in `try/catch` with `finally { isLoading = false }`
- ✅ Show errors via `appStore.showSnackbar({ message, color: 'error' })` or inline `error` ref
- ❌ **NEVER** submit forms without loading state — users must see feedback

### 2.4 Page Structure Template

Every page should follow this structure:

```vue
<template>
  <div>
    <!-- Breadcrumb -->
    <AppBreadcrumb :items="breadcrumbs" />

    <!-- Page header with title + action button -->
    <div class="d-flex align-center mb-6">
      <h1 class="text-h5 font-weight-bold">Page Title</h1>
      <v-spacer />
      <v-btn color="primary" @click="openDialog">
        <v-icon start>mdi-plus</v-icon>
        Add New
      </v-btn>
    </div>

    <!-- Search + Filters -->
    <v-card class="mb-6">
      <v-card-text>
        <v-text-field
          v-model="search"
          placeholder="بحث..."
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
          density="compact"
          hide-details
        />
      </v-card-text>
    </v-card>

    <!-- Data Table -->
    <v-card>
      <v-data-table :headers="headers" :items="items" :loading="isLoading" :items-per-page="perPage" hover>
        <!-- Custom columns -->
        <template #item.actions="{ item }">
          <v-btn icon size="small" @click="editItem(item)">
            <v-icon>mdi-pencil</v-icon>
          </v-btn>
          <v-btn icon size="small" color="error" @click="confirmDelete(item)">
            <v-icon>mdi-delete</v-icon>
          </v-btn>
        </template>
      </v-data-table>
    </v-card>

    <!-- Pagination -->
    <Pagination v-model:currentPage="currentPage" :total="totalItems" :perPage="perPage" />

    <!-- Dialogs -->
    <AddDialog v-model="showAdd" @save="handleSave" />
    <DeleteDialog v-model="showDelete" @confirm="handleDelete" />
    <SuccessDialog v-model="showSuccess" />
  </div>
</template>
```

---

## 3. TypeScript Rules

### 3.1 Type Definitions

All types live in `src/types/index.ts`:

```typescript
export interface Company {
  id: number
  name: string
  email: string
  phone: string
  password: Password
  // ... etc
}

export interface PaginatedResponse<T> {
  status: { success: boolean; message: string }
  data: T[]
  pagination: {
    i_total_objects: number
    i_total_pages: number
    i_per_page: number
    i_current_page: number
  }
}
```

**Rules:**

- ✅ Define interfaces for **every** API response entity
- ✅ Services return typed responses — e.g., `getAll(): Promise<PaginatedResponse<Company>>`
- ✅ Use `interface` for objects, `type` for unions/intersections
- ❌ **NEVER** use `any` as a return type — use `unknown` or a specific interface
- ❌ **NEVER** type-cast with `as any` — fix the root cause

### 3.2 File Naming

| Pattern     | Example                                   |
| ----------- | ----------------------------------------- |
| Services    | `auth.service.ts`, `companies.service.ts` |
| Types       | `src/types/index.ts`                      |
| Components  | `AppBreadcrumb.vue`, `DataTable.vue`      |
| Composables | `useTablePage.js` (prefix with `use`)     |
| Stores      | `Auth.js`, `Dashboard.js`                 |

---

## 4. Error Handling Rules

### 4.1 Service Layer

```typescript
// BaseService catches nothing — errors propagate to callers
protected async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
  const response = await this.client.get(url, config)
  return response.data
}
```

### 4.2 Axios Interceptors (`src/plugins/axios.js`)

- ✅ **Request interceptor:** Attaches `Authorization: Bearer <token>` header
- ✅ **Response interceptor:**
  - 401 → logout + redirect to `/auth/login`
  - 422 → show validation errors via toast
  - Other errors → propagate to caller

### 4.3 Page-Level Error Handling

```javascript
const fetchData = async () => {
  try {
    isLoading.value = true
    const response = await companiesService.getAll(params)
    // process response
  } catch (error) {
    console.error('Error fetching data:', error)
    error.value = 'حدث خطأ أثناء تحميل البيانات'
  } finally {
    isLoading.value = false
  }
}
```

**Rules:**

- ✅ Every API call must be wrapped in `try/catch/finally`
- ✅ `finally` must set `isLoading.value = false`
- ✅ Show user-friendly Arabic error messages
- ✅ Global error handler in `main.js` catches unhandled rejections
- ❌ **NEVER** let errors fail silently — always show feedback

---

## 5. Authentication Rules

### 5.1 Token Management

```
localStorage keys:
  auth_token        → JWT token
  neolt_storage_key_user  → User object (JSON)
```

**Rules:**

- ✅ `authService.getToken()` reads the token
- ✅ `authService.isAuthenticated()` checks token presence
- ✅ `authService.setToken(token)` stores the token
- ✅ `authService.clearAuth()` removes all auth data
- ❌ **NEVER** read `localStorage` directly for auth — use `authService`

### 5.2 Auth Flow

```
Login → authService.login() → setToken() → setUser() → redirect to /dashboard
Logout → authService.logout() → POST /logout → clearAuth() → redirect to /auth/login
Guard → authService.isAuthenticated() → redirect if needed
```

**Rules:**

- ✅ Auth store `logout()` calls `authService.logout()` (server invalidation + local clear)
- ✅ 401 response triggers automatic logout via interceptor
- ✅ Guest-only pages redirect authenticated users to `/dashboard`
- ❌ **NEVER** disable auth guards — they are the security boundary

---

## 6. Code Quality Rules

### 6.1 DRY (Don't Repeat Yourself)

- ✅ Reuse components — don't copy-paste dialogs or cards
- ✅ Reuse composables — don't duplicate pagination/search logic
- ✅ Reuse services — don't call `axiosIns` from multiple places
- ❌ **NEVER** copy a page to create a new one — extract shared logic

### 6.2 File Organization

```
src/
├── @core/           # Shared utilities, components, styles
├── @layouts/        # Layout wrappers (DashboardLayout, default)
├── components/      # Global reusable components
├── composables/     # Shared composable functions
├── config/          # App configuration
├── navigation/      # Sidebar navigation items
├── pages/           # Route-level page components
├── plugins/         # Vuetify, Axios, Router config
├── router/          # Route definitions
├── services/        # API service layer
├── stores/          # Pinia stores
├── styles/          # Global SCSS
└── types/           # TypeScript type definitions
```

**Rules:**

- ✅ Pages only import from `@/services`, `@/stores`, `@/components`, `@/composables`
- ✅ Services only import from `@/plugins/axios` and `@/types`
- ✅ Stores import from `@/services` — never from `@/plugins/axios` directly
- ❌ **NEVER** import from deep relative paths — use `@/` alias

### 6.3 Dead Code

- ✅ Remove unused imports, variables, and functions
- ✅ Remove unused files — check with `grep -rn` before deleting
- ✅ Every created file must be used somewhere — or document why it exists
- ❌ **NEVER** leave dead code "just in case" — version control exists

### 6.4 Naming Conventions

| Element          | Convention                               | Example                        |
| ---------------- | ---------------------------------------- | ------------------------------ |
| Vue components   | PascalCase                               | `AppBreadcrumb.vue`            |
| Composables      | camelCase with `use` prefix              | `useTablePage.js`              |
| Services         | camelCase with `Service` suffix          | `companiesService`             |
| Stores           | camelCase with `Store` suffix (implicit) | `useAuthStore`                 |
| Types/Interfaces | PascalCase                               | `Company`, `PaginatedResponse` |
| CSS classes      | kebab-case with `bg-` prefix             | `bg-light-white`               |

---

## 7. Git & Deployment Rules

### 7.1 Commit Messages

```
feat: add new service layer pattern
fix: resolve infinite recursion in delete methods
refactor: extract useTablePage composable
chore: remove dead code files
```

### 7.2 Pre-Commit Checklist

- [ ] `npx vite build` passes
- [ ] No `console.log` left in production code (only `console.error`)
- [ ] No unused imports or variables
- [ ] New components are in `src/components/` or scoped to page
- [ ] New services extend `BaseService`
- [ ] Auth guards are enabled in router

---

## 8. Performance Rules

- ✅ Use lazy loading for route components: `component: () => import("@/pages/...")`
- ✅ Use `v-if` over `v-show` for rarely-shown elements
- ✅ Use `watch` with `{ deep: false }` unless nested reactivity is needed
- ✅ Debounce search inputs (500ms default via `useDebounceFn`)
- ❌ **NEVER** fetch data in `created`/`onMounted` without loading state
- ❌ **NEVER** use `v-for` without `:key`

---

## 9. Security Rules

- ✅ Never expose API keys in frontend code — use `.env` variables
- ✅ Never store sensitive data in `localStorage` beyond auth tokens
- ✅ Always validate forms before submission
- ✅ Use HTTPS in production
- ❌ **NEVER** trust user input — validate on frontend AND backend
- ❌ **NEVER** disable ESLint rules project-wide

---

## 10. Checklist Before Merging Any PR

```markdown
## Code Review Checklist

- [ ] Service layer used (no direct axiosIns calls from pages/stores)
- [ ] Error handling: try/catch/finally on all API calls
- [ ] Loading state shown during async operations
- [ ] Form validation using Vuetify rules
- [ ] Arabic text for user-facing messages
- [ ] Reusable components used (no copy-paste)
- [ ] TypeScript types defined for new entities
- [ ] No dead code left behind
- [ ] Auth guards respected
- [ ] Build passes (`npx vite build`)
- [ ] No `any` types in new code
- [ ] Breadcrumb added to new pages
```

---

## Current Architecture Summary

```
┌─────────────────────────────────────────────────────────┐
│                    VUE COMPONENTS                        │
│  Pages (company, users, pilgrims, flights, etc.)        │
│  └── uses: useTablePage composable                      │
│  └── uses: reusable components (AppBreadcrumb, etc.)    │
└───────────────────────┬─────────────────────────────────┘
                        │ calls
┌───────────────────────▼─────────────────────────────────┐
│                   PINIA STORES                           │
│  Auth store, Dashboard store, App store                  │
│  └── calls: services                                    │
└───────────────────────┬─────────────────────────────────┘
                        │ calls
┌───────────────────────▼─────────────────────────────────┐
│                 SERVICE LAYER                            │
│  BaseService (get/post/put/patch/deleteRequest)         │
│  ├── AuthService                                        │
│  ├── CompaniesService                                   │
│  ├── UsersService                                       │
│  ├── PilgrimsService                                    │
│  ├── FlightsService                                     │
│  ├── NotificationsService                               │
│  └── DashboardService                                   │
└───────────────────────┬─────────────────────────────────┘
                        │ uses
┌───────────────────────▼─────────────────────────────────┐
│                 AXIOS + INTERCEPTORS                     │
│  Request:  attach Bearer token                          │
│  Response: 401 → logout, 422 → show errors              │
└───────────────────────┬─────────────────────────────────┘
                        │ sends
┌───────────────────────▼─────────────────────────────────┐
│                    BACKEND API                           │
│  Base URL from API_BASE_URL env variable                │
└─────────────────────────────────────────────────────────┘
```
