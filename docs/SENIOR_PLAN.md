# 📋 Senior-Level Implementation Plan

> **Status:** In Progress
> **Goal:** Close all remaining gaps to make this project fully senior-level
> **Last Updated:** June 23, 2026

---

## Current Scorecard

| Area                                   | Status         | Grade |
| -------------------------------------- | -------------- | ----- |
| Service layer pattern                  | ✅ Complete    | A     |
| Auth guards enabled                    | ✅ Complete    | A     |
| Hardcoded data removed                 | ✅ Complete    | A     |
| Error handling (try/catch)             | ✅ Complete    | A     |
| Global error handler                   | ✅ Complete    | A     |
| Auth logout (server invalidation)      | ✅ Complete    | A     |
| useTablePage composable                | ✅ All 5 pages | A     |
| TypeScript interfaces                  | ✅ Complete    | A     |
| Request cancellation                   | ✅ Complete    | A     |
| Axios interceptors                     | ✅ Complete    | A     |
| Dead code cleanup                      | ✅ Complete    | A     |
| Breadcrumb component                   | ✅ Complete    | A     |
| Build passes                           | ✅ Complete    | A     |
| Roles service layer                    | ✅ Complete    | A     |
| Request cancellation (AbortController) | ✅ Complete    | A     |

---

## Remaining Gaps (Ordered by Impact)

### 🔴 GAP 1: Wire `useTablePage` to All Table Pages

**Impact:** High — eliminates duplicated pagination/search/loading across 4 pages
**Effort:** Medium (~30 min per page)

**Status:** ✅ COMPLETED — All 5 table pages now use `useTablePage`

**Pages to update:**

- [ ] `src/pages/users/users.vue`
- [ ] `src/pages/pilgrims/index.vue`
- [ ] `src/pages/flight-table/index.vue`
- [ ] `src/pages/notifications/index.vue`

**For each page:**

1. Import `useTablePage` from `@/composables/useTablePage`
2. Replace manual `items`, `isLoading`, `search`, `currentPage`, `perPage`, `totalItems` refs with composable
3. Replace manual `fetchData`/`debouncedSearch` with composable's `fetchData`/`debouncedFetch`
4. Update template bindings to use composable's reactive state
5. Call `fetchData()` after CRUD operations instead of duplicating fetch logic

**Example transformation:**

```javascript
// BEFORE: 40+ lines of manual state management
const items = ref([])
const isLoading = ref(false)
const search = ref('')
const currentPage = ref(1)
const perPage = ref(12)
const totalItems = ref(0)
const totalPages = ref(1)
const fetchData = async (page = 1) => { ... }
const debouncedSearch = useDebounceFn(() => { ... }, 500)
watch(search, () => debouncedSearch())
watch(currentPage, (p) => fetchData(p))
onMounted(() => fetchData())

// AFTER: 5 lines
const { items, isLoading, search, currentPage, perPage, totalItems, totalPages, fetchData } = useTablePage(
  (params) => usersService.getAll(params),
  { entityName: 'المستخدمين' }
)
```

---

### 🟡 GAP 2: Add TypeScript Interfaces for All Entities

**Impact:** Medium — prevents runtime errors, improves IDE support
**Effort:** Low (~15 min)

**Currently defined:**

- ✅ `Company`, `Password`, `ContactPerson`, `Location`, `User`, `Role`, `Permission`
- ✅ `Pilgrim`, `Flight`, `Notification`, `DashboardStatistics`
- ✅ `ProblemDetails`, `PermissionItem`, `AuthUser`, `OtpPayload`, `UpdatePasswordPayload`

**File:** `src/types/index.ts`

---

### 🟡 GAP 3: Replace `any` Types in Auth Service

**Impact:** Medium — type safety for the most critical service
**Effort:** Low (~5 min)

**File:** `src/services/auth.service.ts`

**Status:** ✅ COMPLETED — All types are properly defined (User, AuthUser, etc.)

---

### 🟡 GAP 4: Clean Up Orphaned `companies.vue`

**Impact:** Low — reduces confusion about canonical file
**Effort:** Very low (~5 min)

**File:** `src/pages/companies.vue`

**Action:** Check if this is imported anywhere. If not, delete it. The canonical company page is `src/pages/company/index.vue`.

---

### 🟢 GAP 5: Add Loading Skeletons (Optional Enhancement)

**Impact:** Low — better UX during initial load
**Effort:** Medium (~20 min per page)

**Action:** Replace `v-progress-linear` with Vuetify `v-skeleton-loader` for table pages:

```vue
<v-skeleton-loader v-if="isLoading && items.length === 0" type="table-heading, table-thead, table-tbody@5" />
```

---

### 🟢 GAP 6: Add Optimistic UI Updates (Optional Enhancement)

**Impact:** Low — feels snappier for CRUD operations
**Effort:** Medium

**Action:** After create/update/delete, immediately update the local `items` array before the server responds, then reconcile on success/failure.

---

## Implementation Order

```
Phase 1 (Critical):    GAP 1 → Wire useTablePage to all pages
Phase 2 (Important):   GAP 2 + GAP 3 → TypeScript interfaces + type safety
Phase 3 (Cleanup):     GAP 4 → Remove orphaned file
Phase 4 (Polish):      GAP 5 + GAP 6 → Skeletons + optimistic UI
```

---

## Validation After Each Phase

After completing each phase:

1. Run `npx vite build` — must pass
2. Run code review — no critical issues
3. Test in browser — verify all pages load and CRUD works
4. Check no dead code was introduced

---

## Definition of "Done"

The project is considered **fully senior-level** when:

- [ ] All 5 table pages use `useTablePage` composable
- [ ] All API entities have TypeScript interfaces
- [ ] No `any` types in service layer
- [ ] No orphaned/dead files
- [ ] Build passes with zero warnings
- [ ] Every page has error handling + loading state
- [ ] Every page has breadcrumb navigation
- [ ] Auth flow works end-to-end (login → guard → logout → server invalidation)
