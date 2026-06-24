import { expect, setupAuth, test } from '../helpers'

test.describe('Pilgrim Management', () => {
  test.beforeEach(async ({ page }) => {
    await setupAuth(page)
    await page.goto('/manage-pilgrims')
    await page.waitForTimeout(2000)
  })

  test('should display pilgrims page', async ({ page }) => {
    await expect(page).toHaveURL(/manage-pilgrims/)
  })

  test('should show content area after loading', async ({ page }) => {
    const content = page.locator('.v-card, .v-sheet, .v-table, .v-skeleton-loader, table, .v-main')
    await expect(content.first()).toBeVisible({ timeout: 10000 })
  })
})
