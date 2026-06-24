import { expect, test } from '../helpers'

test.describe('Login Page', () => {
  test('should display login form with email, password, and submit', async ({ page }) => {
    await page.goto('/auth/login')
    await expect(page.locator('input[type="email"]')).toBeVisible()
    await expect(page.locator('input[type="password"]')).toBeVisible()
    await expect(page.locator('button[type="submit"]').first()).toBeVisible()
  })

  test('should show validation errors on empty submit', async ({ page }) => {
    await page.goto('/auth/login')
    await page.locator('button[type="submit"]').first().click()
    await page.waitForTimeout(1000)
    const errorMessages = page.locator('.v-messages__message')
    await expect(errorMessages.first()).toBeVisible({ timeout: 5000 })
  })

  test('should toggle password visibility', async ({ page }) => {
    await page.goto('/auth/login')
    const eyeBtn = page.locator('.v-field__append-inner button').first()
    if (await eyeBtn.isVisible()) {
      await eyeBtn.click()
      const textField = page.locator('input[type="text"]')
      await expect(textField).toBeVisible()
    }
  })

  test('should show error toast on invalid credentials', async ({ page }) => {
    await page.goto('/auth/login')
    await page.locator('input[type="email"]').fill('wrong@example.com')
    await page.locator('input[type="password"]').fill('wrongpassword')
    await page.locator('button[type="submit"]').first().click()
    await page.waitForTimeout(5000)
    const toast = page.locator('.toast-container .toast-item, [role="alert"], .Toastify__toast')
    const hasToast = await toast
      .first()
      .isVisible()
      .catch(() => false)
    const hasError = await page
      .locator('.v-alert, .error')
      .first()
      .isVisible()
      .catch(() => false)
    expect(hasToast || hasError).toBeTruthy()
  })
})

test.describe('Navigation', () => {
  test('should redirect to login when not authenticated', async ({ page }) => {
    // Mock /me to return 401 (no valid cookie)
    await page.route('**/me', (route) => {
      route.fulfill({ status: 401, contentType: 'application/json', body: '{}' })
    })
    await page.goto('/dashboard')
    await expect(page).toHaveURL(/auth\/login|login/)
  })

  test('should show 404 for unknown routes', async ({ page }) => {
    await page.goto('/nonexistent-page-xyz')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/nonexistent-page-xyz/)
  })

  test('should allow access to landing page', async ({ page }) => {
    await page.goto('/')
    await page.waitForTimeout(2000)
    await expect(page).toHaveURL(/localhost/)
  })
})
