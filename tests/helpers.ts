import { test as base, expect, type Page } from '@playwright/test'

export const USER_KEY = 'neolt_storage_key_user'

export const adminUser = {
  id: 1,
  email: 'admin@test.com',
  name: 'Admin User',
  role: { id: 1, name: 'admin' },
  permissions: [
    'companies.view',
    'companies.create',
    'companies.edit',
    'companies.delete',
    'users.view',
    'users.create',
    'users.edit',
    'users.delete',
    'roles.view',
    'roles.create',
    'roles.edit',
    'roles.delete',
    'admin_notifications.view',
    'admin_notifications.create',
    'admin_notifications.edit',
    'admin_notifications.delete',
  ],
}

export const companyUser = {
  id: 2,
  email: 'company@test.com',
  name: 'Company User',
  role: { id: 2, name: 'company' },
  parentCompanies: [1],
  permissions: ['companies.view', 'companies.create', 'companies.edit'],
}

/**
 * Setup auth for E2E tests by intercepting /me endpoint and setting user data in localStorage.
 * With cookie-based auth, the frontend checks auth via GET /me — we mock that response.
 */
export async function setupAuth(page: Page, user = adminUser): Promise<void> {
  // Store user data in localStorage for CASL permissions
  await page.addInitScript(
    ({ userKey, userData }) => {
      localStorage.setItem(userKey, JSON.stringify(userData))
    },
    { userKey: USER_KEY, userData: user },
  )

  // Intercept /me API to return the user (simulates valid auth cookie)
  await page.route('**/me', (route) => {
    route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({ data: user }),
    })
  })
}

export const test = base.extend<{ authenticatedPage: Page }>({
  authenticatedPage: async ({ page }, use) => {
    await setupAuth(page)
    await use(page)
  },
})

export { expect }
