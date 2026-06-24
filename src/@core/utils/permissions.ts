/**
 * PERMISSION CHECK SYSTEMS
 *
 * This project uses TWO permission check mechanisms:
 *
 * 1. CASL `$can(action, subject)` — for template v-if directives
 *    Example: v-if="$can('create', 'companies')"
 *    Use for: sidebar visibility, button visibility, page-level checks
 *
 * 2. hasPermission(string) — raw string matching
 *    Example: hasPermission('companies.view')
 *    Use for: route guards (meta.requiresPermission)
 *
 * Both derive from the same user.permissions array.
 * CASL is preferred for new code. hasPermission is kept for route guards.
 */
import { useAuthStore } from '@/stores/auth.store'
import type { User } from '@/types'

export const hasPermission = (permissionLabel: string | string[], hasSome = false): boolean => {
  const auth = useAuthStore()
  const userPermissions = auth.user?.permissions || []

  if (!userPermissions.length) return false

  if (Array.isArray(permissionLabel) && permissionLabel.length) {
    if (hasSome) {
      return permissionLabel.some((permission) => userPermissions.includes(permission))
    }

    return permissionLabel.every((permission) => userPermissions.includes(permission))
  }

  return userPermissions.includes(permissionLabel as string)
}

export const canAccessObserverInfo = (): boolean => {
  const auth = useAuthStore()
  const user = auth.user

  return Array.isArray(user?.parentCompanies) ? user.parentCompanies.length === 0 : true
}

export const hasRole = (roleId: string | number): boolean => {
  const auth = useAuthStore()
  const user = auth.user

  return user ? user.role?.id == roleId : false
}
