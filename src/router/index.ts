/**
 * router/index.ts
 *
 * Automatic routes for `./src/pages/*.vue`
 */

// Composables
import { createRouter, createWebHistory, type RouteLocationNormalized } from 'vue-router/auto'

import routes from './routes'
import { hasPermission } from '@core/utils/permissions'
import { useAuthStore } from '@/stores/auth.store'
import { logger } from '@/@core/utils/logger'

const router = createRouter({
  history: createWebHistory('/'),
  routes,
  scrollBehavior(to: RouteLocationNormalized, from: RouteLocationNormalized, savedPosition: { top: number } | null) {
    return savedPosition || ({ top: 0 } as { top: number })
  },
})

// Global Navigation Guard
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()

  // 1. Guest routes — redirect away if already authenticated
  if (to.meta.guest && authStore.isAuthenticated) {
    return next({ name: 'Dashboard' })
  }

  // 2. Public routes — no auth needed
  if (!to.meta.requiresAuth) {
    return next()
  }

  // 3. Protected routes — ensure auth is initialized
  if (!authStore.initialized) {
    await authStore.init()
  }

  // 4. Still not authenticated? → login with redirect back
  if (!authStore.isAuthenticated) {
    return next({
      path: '/auth/login',
      query: to.fullPath !== '/' ? { redirect: to.fullPath } : undefined,
    })
  }

  // 5. Permission check
  const permission = to.meta.requiresPermission
  if (permission) {
    const hasAccess = typeof permission === 'string' ? hasPermission(permission) : true
    if (!hasAccess) {
      return next({ name: 'PermissionDenied' })
    }
  }

  next()
})

// Workaround for https://github.com/vitejs/vite/issues/11804
router.onError((err: Error & { message?: string }, to: RouteLocationNormalized) => {
  if (err?.message?.includes?.('Failed to fetch dynamically imported module')) {
    if (!localStorage.getItem('vuetify:dynamic-reload')) {
      logger.log('Reloading page to fix dynamic import error')
      localStorage.setItem('vuetify:dynamic-reload', 'true')
      location.assign(to.fullPath)
    } else {
      logger.error('Dynamic import error, reloading page did not fix it', err)
    }
  } else {
    logger.error(err)
  }
})

router.isReady().then(() => {
  localStorage.removeItem('vuetify:dynamic-reload')
})

export default router
