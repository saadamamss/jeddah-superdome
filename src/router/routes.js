import DashboardLayout from "@/layouts/DashboardLayout";
import LoginPage from "@/pages/auth/login";
import CreatePasswordPage from "@/pages/auth/create-password";
import ForgotPasswordPage from "@/pages/auth/forgot-password";
import OTPPage from "@/pages/auth/otp";

import DashboardHome from "@/pages/home";
import UsersPage from "@/pages/users/users.vue";
import SitesPage from "@/pages/sites";
import CompanyDetailsPage from "@/pages/sites/company-details";
import HousingDetailsPage from "@/pages/sites/housing-details";
import TeamsPage from "@/pages/teams";
import MainSectorsPage from "@/pages/main-sectors";
import RolesPage from "@/pages/roles";
import NotesPage from "@/pages/notes";
import RatingPage from "@/pages/rating";
import ContractsPage from "@/pages/contracts";

import NotFoundPage from "@/pages/NotFound";
// import TempPage from "@/pages/temp";

export default [
  {
    path: "/",
    component: DashboardLayout,
    children: [
      {
        path: "",
        name: "home",
        component: DashboardHome,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/sites",
        name: "sites",
        component: SitesPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/sites/housing-details",
        name: "housing-details",
        component: HousingDetailsPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/sites/company-details",
        name: "company-details",
        component: CompanyDetailsPage,
      },
      {
        path: "/teams",
        name: "teams",
        component: TeamsPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/main-sectors",
        name: "main-sectors",
        component: MainSectorsPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/roles",
        name: "roles",
        component: RolesPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/users",
        name: "users",
        component: UsersPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/notes",
        name: "notes",
        component: NotesPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/rating",
        name: "rating",
        component: RatingPage,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/contracts",
        name: "contracts",
        component: ContractsPage,
        meta: {
          requiresAuth: true,
        },
      },

      // {
      //   path: "/temp",
      //   name: "temp",
      //   component: TempPage,
      // },

      // {
      //   path: "/settings",
      //   component: SettingsIndexPage,
      //   children: [
      //     {
      //       path: "",
      //       name: "settings",
      //       component: SettingsPage,
      //     },
      //     {
      //       path: ":id",
      //       name: "setting",
      //       component: SettingPage,
      //     },
      //   ],
      // },
    ],
  },
  {
    path: "/auth/login",
    name: "login",
    component: LoginPage,
  },
  {
    path: "/auth/create-password",
    name: "create-password",
    component: CreatePasswordPage,
  },
  {
    path: "/auth/forgot-password",
    name: "forgot-password",
    component: ForgotPasswordPage,
  },
  {
    path: "/auth/otp",
    name: "otp",
    component: OTPPage,
  },
  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: NotFoundPage,
  },
];
