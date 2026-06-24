import i18n from '@/plugins/i18n'
import Airplane from '@/components/icons/airplane.vue'
import HomeIcon from '@/components/icons/home.vue'
import Notifications from '@/components/icons/notifications.vue'
import Realstate from '@/components/icons/realstate.vue'
import Usergroup from '@/components/icons/usergroup.vue'
import Userstar from '@/components/icons/userstar.vue'

export interface NavItem {
  title: string
  to?: { name: string }
  icon?: ReturnType<typeof import('vue').defineComponent>
  action?: string
  subject?: string
  companyOnly?: boolean
  adminOnly?: boolean
  companyTitle?: string
}

export interface NavSection {
  heading?: string
  divider?: boolean
  children?: NavItem[]
}

// Company users see: home, pilgrims, flights
// Admin users see everything based on their permissions
const navigation: NavSection[] = [
  {
    heading: i18n.global.t('nav.sections.coreOperations'),
    children: [
      {
        title: i18n.global.t('nav.items.home'),
        to: { name: 'home' },
        icon: HomeIcon,
        action: 'read',
        subject: 'Auth',
      },
      {
        title: i18n.global.t('nav.items.companies'),
        to: { name: 'companies' },
        icon: Realstate,
        action: 'view',
        subject: 'companies',
        adminOnly: true,
      },

      {
        title: i18n.global.t('nav.items.pilgrims'),
        to: { name: 'manage-pilgrims' },
        icon: Usergroup,
        action: 'view',
        subject: 'pilgrims',
      },
      {
        title: i18n.global.t('nav.items.flights'),
        to: { name: 'flights-table' },
        icon: Airplane,
        action: 'view',
        subject: 'flights',
      },
    ],
  },
  {
    divider: true,
  },
  {
    heading: i18n.global.t('nav.sections.systemServices'),
    children: [
      {
        title: i18n.global.t('nav.items.users'),
        to: { name: 'users' },
        icon: Userstar,
        action: 'view',
        subject: 'users',
        adminOnly: true,
      },
      {
        title: i18n.global.t('nav.items.notifications'),
        to: { name: 'notifications' },
        icon: Notifications,
        action: 'view',
        subject: 'admin_notifications',
        companyTitle: i18n.global.t('nav.items.notificationsView'),
      },
    ],
  },
]

export default navigation
