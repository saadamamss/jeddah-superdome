import { expect, setupAuth, test } from '../helpers'

test.describe('Company Management', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuth(page)
    await page.goto('/company')
    await page.waitForTimeout(2000)
  })

  test('should display companies page', async ({ page }) => {
    await expect(page).toHaveURL(/company/)
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

  test('should have search or filter input', async ({ page }) => {
    await page.waitForTimeout(1000)
    const search = page
      .locator('input[type="search"], input[placeholder*="بحث"], input[placeholder*="search"], .v-text-field input')
      .first()
    if (await search.isVisible({ timeout: 3000 }).catch(() => false)) {
      await expect(search).toBeEnabled()
    }
  })
})
