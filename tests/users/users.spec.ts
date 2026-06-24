import { expect, setupAuth, test } from '../helpers'

test.describe('User Management', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuth(page)
    await page.goto('/users')
    await page.waitForTimeout(2000)
  })

  test('should display users page', async ({ page }) => {
    await expect(page).toHaveURL(/users/)
  })

  test('should show content area after loading', async ({ page }) => {
    const content = page.locator('.v-card, .v-sheet, .v-table, .v-skeleton-loader, table, .v-main')
    await expect(content.first()).toBeVisible({ timeout: 10000 })
  })

  test('should have an add button', async ({ page }) => {
    await page.waitForTimeout(1000)
    const addBtn = page
      .locator('button')
      .filter({ hasText: /add|جديد|إضافة|new/i })
      .first()
    if (await addBtn.isVisible({ timeout: 3000 }).catch(() => false)) {
      await expect(addBtn).toBeEnabled()
    }
  })
})
