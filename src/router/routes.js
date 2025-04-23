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
import TeamManagementPage from "@/pages/teams/team-management";
import AddRatePage from "@/pages/sites/addRate";
import TeamsPage from "@/pages/teams";
import MainSectorsPage from "@/pages/phases";
import AddSectorsPage from "@/pages/phases/addSectors";
import PhaseDetailsPage from "@/pages/phases/details";
import RolesPage from "@/pages/roles";
import NotesPage from "@/pages/notes";
import AddNotePage from "@/pages/notes/addNote";
import EditNotePage from "@/pages/notes/editNote";
import RatingPage from "@/pages/rating";
import ContractsPage from "@/pages/contracts";
import ContractDetailsPage from "@/pages/contracts/contract_details";
import NotFoundPage from "@/pages/NotFound";
import ShiftsView from "@/views/shifts/ShiftsView.vue";
import QuestionnairesPage from "@/pages/questionnaires/index.vue";
import AttendancePage from "@/pages/attendance/index.vue";
import IntegrationsPage from "@/pages/integrations/index.vue";
import NotificationsPage from "@/pages/notifications/index.vue";
// import CompanyRatingPage from "@/pages/rating/company-rating";
// import TempPage from "@/pages/temp";

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
        path: "/sites",
        name: "sites",
        component: SitesPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "locations.view",
        },
      },
      {
        path: "/sites/housing-details",
        name: "housing-details",
        component: HousingDetailsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "locations.view",
        },
      },
      {
        path: "/sites/company-details",
        name: "company-details",
        component: CompanyDetailsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "locations.view",
        },
      },

      {
        path: "/teams",
        name: "teams",
        component: TeamsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "teams.view",
        },
      },
      {
        path: "/teams/team-management/:id",
        name: "team-management",
        component: TeamManagementPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "teams.view",
        },
      },
      {
        path: "/phases",
        name: "phases",
        component: MainSectorsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "imtithal_phases.view",
        },
      },
      {
        path: "/phases/details/:id",
        name: "phase-details",
        component: PhaseDetailsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "imtithal_phases.view",
        },
      },
      {
        path: "/phases/add-sectors",
        name: "add-sectors",
        component: AddSectorsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "imtithal_phases.view",
        },
      },
      {
        path: "/roles",
        name: "roles",
        component: RolesPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "permissions.view",
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
        path: "/notes",
        name: "notes",
        component: NotesPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "notes.view",
        },
      },
      {
        path: "/notes/details/:id",
        name: "note-details",
        component: () => import("@/pages/notes/details.vue"),
        meta: {
          requiresAuth: true,
          requiresPermission: "notes.view",
        },
      },
      {
        path: "/notes/add",
        name: "add-note",
        component: AddNotePage,
        meta: {
          requiresAuth: true,
          requiresPermission: "notes.view",
        },
      },
      {
        path: "/notes/edit/:id",
        name: "edit-note",
        component: EditNotePage,
        meta: {
          requiresAuth: true,
          requiresPermission: "notes.view",
        },
      },
      {
        path: "/rating",
        name: "rating",
        component: RatingPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "ratings.view",
        },
      },
      {
        path: "/rating/details/:id",
        name: "rating-details",
        component: () => import("@/pages/rating/details.vue"),
        meta: {
          requiresAuth: true,
          requiresPermission: "ratings.view",
        },
      },
      // {
      //   path: "/rating/company-rating/:id",
      //   name: "company-rating",
      //   component: CompanyRatingPage,
      //   meta: {
      //     requiresAuth: true,
      //     requiresPermission: "ratings.view",
      //   },
      // },
      // {
      //   path: "/rating_details",
      //   name: "add-rate",
      //   component: AddRatePage,
      //   meta: {
      //     requiresAuth: true,
      //   },
      // },
      {
        path: "/contracts",
        name: "contracts",
        component: ContractsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "contracts.view",
        },
      },
      {
        path: "/contracts/:id",
        name: "contract-details",
        component: ContractDetailsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "contracts.view",
        },
      },
      {
        path: "/questionnaires",
        name: "questionnaires",
        component: QuestionnairesPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "questionnaires.view",
        },
      },
      {
        path: "/questionnaires/details",
        name: "questionnaire-details",
        component: () => import("@/pages/questionnaires/details.vue"),
        meta: {
          requiresAuth: true,
          requiresPermission: "questionnaires.view",
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
        path: "/company/company_details/:id",
        name: "company-details",
        component: () => import("@/pages/company/company_details.vue"),
        meta: {
          // layout: "default",
          requiresAuth: true,
          requiresPermission: "companies.view",
        },
      },
      {
        path: "/shifts",
        name: "shifts",
        component: ShiftsView,
        meta: {
          requiresAuth: true,
          requiresPermission: "shifts.view",
        },
      },
      {
        path: "/attendance",
        name: "attendance",
        component: AttendancePage,
        meta: {
          requiresAuth: true,
          requiresPermission: "attendances.view",
        },
      },
      {
        path: "/integrations",
        name: "integrations",
        component: IntegrationsPage,
        meta: {
          requiresAuth: true,
          requiresPermission: "integrations.view",
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
