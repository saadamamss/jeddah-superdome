import DashboardLayout from '@/layouts/DashboardLayout.vue'

export default [
  {
    path: '/dashboard',
    component: DashboardLayout,
    meta: {
      requiresAuth: true,
      requiresPermission: false,
    },
    children: [
      {
        path: '',
        name: 'home',
        component: () => import('@/pages/home/index.vue'),
        meta: {
          requiresAuth: true,
          requiresPermission: false,
        },
      },
      {
        path: '/company',
        name: 'companies',
        component: () => import('@/pages/company/index.vue'),
        meta: {
          requiresAuth: true,
          requiresPermission: 'companies.view',
        },
      },

      {
        path: '/manage-pilgrims',
        name: 'manage-pilgrims',
        component: () => import('@/pages/pilgrims/index.vue'),
        meta: {
          requiresAuth: true,
          requiresPermission: 'pilgrims.view',
        },
      },
      {
        path: '/flights-table',
        name: 'flights-table',
        component: () => import('@/pages/flight-table/index.vue'),
        meta: {
          requiresAuth: true,
          requiresPermission: 'flights.view',
        },
      },
      {
        path: '/users',
        name: 'users',
        component: () => import('@/pages/users/users.vue'),
        meta: {
          requiresAuth: true,
          requiresPermission: 'users.view',
        },
      },
      {
        path: '/notifications',
        name: 'notifications',
        component: () => import('@/pages/notifications/index.vue'),
        meta: {
          requiresAuth: true,
          requiresPermission: false,
        },
      },
      {
        path: '/email-template',
        name: 'email-template',
        component: () => import('@/pages/emailtemp.vue'),
        meta: {
          requiresAuth: true,
          requiresPermission: 'admin_notifications.view',
        },
      },
      {
        path: '/roles',
        name: 'roles',
        component: () => import('@/pages/roles/index.vue'),
        meta: {
          requiresAuth: true,
          requiresPermission: 'roles.view',
        },
      },
    ],
  },
  {
    path: '/',
    component: () => import('@/pages/LandingPage.vue'),
    meta: {
      requiresAuth: false,
      requiresPermission: false,
    },
  },
  {
    path: '/auth',
    meta: {
      requiresAuth: false,
      requiresPermission: false,
    },
    children: [
      {
        path: 'login',
        name: 'login',
        component: () => import('@/pages/auth/login.vue'),
        meta: {
          guest: true,
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: 'create-password',
        name: 'create-password',
        component: () => import('@/pages/auth/create-password.vue'),
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: 'reset-password',
        name: 'reset-password',
        component: () => import('@/pages/auth/reset-password.vue'),
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: 'forgot-password',
        name: 'forgot-password',
        component: () => import('@/pages/auth/forgot-password.vue'),
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: 'otp',
        name: 'otp',
        component: () => import('@/pages/auth/otp.vue'),
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
    ],
  },
  {
    path: '/permission-denied',
    name: 'PermissionDenied',
    component: () => import('@/pages/PermissionDenied.vue'),
    meta: {
      requiresAuth: true,
      requiresPermission: false,
    },
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/pages/NotFound.vue'),
  },
]
