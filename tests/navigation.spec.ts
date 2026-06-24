import { expect, setupAuth, test } from './helpers'

test.describe('Navigation', () => {
  test('should redirect to login when not authenticated', async ({ page }) => {
    // Mock /me to return 401 (no valid cookie)
    await page.route('**/me', (route) => {
      route.fulfill({ status: 401, contentType: 'application/json', body: '{}' })
    })
    await page.goto('/dashboard')
    await expect(page).toHaveURL(/auth\/login|login/)
  })

  test('should load landing page at root', async ({ page }) => {
    await page.goto('/')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/localhost/)
  })
})

test.describe('Authenticated Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuth(page)
  })

  test('should navigate to dashboard', async ({ page }) => {
    await page.goto('/dashboard')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/dashboard/)
  })

  test('should navigate to companies page', async ({ page }) => {
    await page.goto('/company')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/company/)
  })

  test('should navigate to users page', async ({ page }) => {
    await page.goto('/users')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/users/)
  })

  test('should navigate to pilgrims page', async ({ page }) => {
    await page.goto('/manage-pilgrims')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/manage-pilgrims/)
  })

  test('should navigate to flights page', async ({ page }) => {
    await page.goto('/flights-table')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/flights-table/)
  })

  test('should navigate to notifications page', async ({ page }) => {
    await page.goto('/notifications')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/notifications/)
  })

  test('should navigate to roles page', async ({ page }) => {
    await page.goto('/roles')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/roles/)
  })
})
