import DashboardLayout from "@/layouts/DashboardLayout";
import LoginPage from "@/pages/auth/login";
import CreatePasswordPage from "@/pages/auth/create-password";
import ForgotPasswordPage from "@/pages/auth/forgot-password";
import OTPPage from "@/pages/auth/otp";
import DashboardHome from "@/pages/home";
import UsersPage from "@/pages/users/users.vue";
import NotFoundPage from "@/pages/NotFound";
import NotificationsPage from "@/pages/notifications/index.vue";

export default [
  {
    path: "/",
    component: DashboardLayout,
    meta: {
      requiresAuth: true,
      requiresPermission: false,
    },
    children: [
      {
        path: "",
        name: "home",
        component: DashboardHome,
        meta: {
          requiresAuth: true,
          requiresPermission: false,
        },
      },
      {
        path: "/company",
        name: "companies",
        component: () => import("@/pages/company/index.vue"),
        meta: {
          // layout: "default",
          requiresAuth: true,
          requiresPermission: "companies.view",
        },
      },
      {
        path: "/manage-pilgrims",
        name: "manage-pilgrims",
        component: () => import("@/pages/pilgrims/index.vue"),
        meta: {
          // layout: "default",
          requiresAuth: true,
          requiresPermission: "companies.view",
        },
      },
      {
        path: "/flights-table",
        name: "flights-table",
        component: () => import("@/pages/flight-table/index.vue"),
        meta: {
          // layout: "default",
          requiresAuth: true,
          requiresPermission: "companies.view",
        },
      },
      {
        path: "/users",
        name: "users",
        component: UsersPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "users.view",
        },
      },
      {
        path: "/notifications",
        name: "notifications",
        component: NotificationsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "admin_notifications.view",
        },
      },
    ],
  },
  {
    path: "/auth",
    // component: DashboardLayout,
    meta: {
      requiresAuth: false,
      requiresPermission: false,
    },
    children: [
      {
        path: "login",
        name: "login",
        component: LoginPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "create-password",
        name: "create-password",
        component: CreatePasswordPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "forgot-password",
        name: "forgot-password",
        component: ForgotPasswordPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
      {
        path: "otp",
        name: "otp",
        component: OTPPage,
        meta: {
          requiresAuth: false,
          requiresPermission: false,
        },
      },
    ],
  },
  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: NotFoundPage,
  },
];
