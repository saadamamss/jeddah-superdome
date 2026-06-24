import { computed, ref } from 'vue'
import type { RouteLocationNormalized, Router } from 'vue-router'

export const openGroups = ref<unknown[]>([])

/**
 * Return nav link props to use
 * @param {Object, String} item navigation routeName or route Object provided in navigation data
 */
export const getComputedNavLinkToProp = computed(
  () => (link: { target?: string; rel?: string; to?: string | Record<string, unknown>; href?: string }) => {
    const props: { target?: string; rel?: string; to?: string | Record<string, unknown>; href?: string } = {
      target: link.target,
      rel: link.rel,
    }

    // If route is string => it assumes string is route name => Create route object from route name
    // If route is not string => It assumes it's route object => returns passed route object
    if (link.to) props.to = typeof link.to === 'string' ? { name: link.to } : link.to
    else props.href = link.href

    return props
  },
)

/**
 * Return route name for navigation link
 * If link is string then it will assume it is route-name
 * IF link is object it will resolve the object and will return the link
 * @param {Object, String} link navigation link object/string
 */
export const resolveNavLinkRouteName = (
  link: { to?: string | Record<string, unknown> },
  router: Router,
): string | null | undefined => {
  if (!link.to) return null
  if (typeof link.to === 'string') return link.to

  const resolved = router.resolve(link.to)
  return typeof resolved.name === 'string' ? resolved.name : null
}

/**
 * Check if nav-link is active
 * @param {Object} link nav-link object
 */
export const isNavLinkActive = (
  link: { to?: string | Record<string, unknown>; href?: string },
  router: Router,
): boolean => {
  // Matched routes array of current route
  const matchedRoutes = router.currentRoute.value.matched

  // Check if provided route matches route's matched route
  const resolveRoutedName = resolveNavLinkRouteName(link, router)
  if (!resolveRoutedName) return false

  return matchedRoutes.some((route) => {
    return route.name === resolveRoutedName || route.meta.navActiveLink === resolveRoutedName
  })
}

/**
 * Check if nav group is active
 * @param {Array} children Group children
 */
export const isNavGroupActive = (
  children: Array<{ to?: string; href?: string; children?: unknown[] }>,
  router: Router,
): boolean =>
  children.some((child) => {
    // If child have children => It's group => Go deeper(recursive)
    if ('children' in child && child.children) {
      return isNavGroupActive(child.children as Array<{ to?: string; href?: string; children?: unknown[] }>, router)
    }

    // else it's link => Check for matched Route
    return isNavLinkActive(child as { to?: string | Record<string, unknown>; href?: string }, router)
  })

/**
 * Convert Hex color to rgb
 * @param hex
 */
export const hexToRgb = (hex: string): string | null => {
  // Expand shorthand form (e.g. "03F") to full form (e.g. "0033FF")
  const shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i

  hex = hex.replace(shorthandRegex, (m, r, g, b) => {
    return r + r + g + g + b + b
  })

  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)

  return result ? `${parseInt(result[1], 16)},${parseInt(result[2], 16)},${parseInt(result[3], 16)}` : null
}
