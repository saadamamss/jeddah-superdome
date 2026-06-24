import { initialAbility } from '@/plugins/casl/ability'

/**
 * Returns ability result if ACL is configured or else just return true
 * Default allow when no explicit rule matches.
 *
 * @param {String} action CASL Actions
 * @param {String} subject CASL Subject
 */
export const can = (action?: string, subject?: string): boolean => {
  if (!action || !subject) return true

  // If no rules loaded yet, deny everything (before auth init)
  if (initialAbility.rules.length === 0) return false

  return initialAbility.can(action, subject)
}

/**
 * Check if user can view item based on it's ability
 */
export const canViewNavMenuGroup = (item: {
  action: string
  subject: string
  children: Array<{ action: string; subject: string }>
}): boolean => {
  const hasAnyVisibleChild = item.children.some((i) => can(i.action, i.subject))

  if (!(item.action && item.subject)) return hasAnyVisibleChild

  return can(item.action, item.subject) && hasAnyVisibleChild
}
