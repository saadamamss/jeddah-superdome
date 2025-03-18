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
import MainSectorsPage from "@/pages/main-sectors";
import AddSectorsPage from "@/pages/main-sectors/addSectors";
import PhaseDetailsPage from "@/pages/main-sectors/details";
import RolesPage from "@/pages/roles";
import NotesPage from "@/pages/notes";
import AddNotePage from "@/pages/notes/addNote";
import EditNotePage from "@/pages/notes/editNote";
import RatingPage from "@/pages/rating";
import CompanyRatingPage from "@/pages/rating/company-rating";
import ContractsPage from "@/pages/contracts";
import ContractDetailsPage from "@/pages/contracts/contract_details";
import UserDetailsPage from "@/pages/users/user-details";
import NotFoundPage from "@/pages/NotFound";
import ShiftsView from "@/views/shifts/ShiftsView.vue";
import QuestionnairesPage from "@/pages/questionnaires/index.vue";
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
          requiresAuth: false,
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
        path: "/rating_details",
        name: "add-rate",
        component: AddRatePage,
      },
      {
        path: "/teams",
        name: "teams",
        component: TeamsPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/teams/team-management/:id",
        name: "team-management",
        component: TeamManagementPage,
      },
      {
        path: "/main-sectors",
        name: "main-sectors",
        component: MainSectorsPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/main-sectors/details/:id",
        name: "phase-details",
        component: PhaseDetailsPage,
      },
      {
        path: "/main-sectors/add-sectors",
        name: "add-sectors",
        component: AddSectorsPage,
      },
      {
        path: "/roles",
        name: "roles",
        component: RolesPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/users",
        name: "users",
        component: UsersPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/users/user-details/:id",
        name: "user-details",
        component: UserDetailsPage,
      },
      {
        path: "/notes",
        name: "notes",
        component: NotesPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/notes/details/:id",
        name: "note-details",
        component: () => import("@/pages/notes/details.vue"),
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: "/notes/add",
        name: "add-note",
        component: AddNotePage,
      },
      {
        path: "/notes/edit/:id",
        name: "edit-note",
        component: EditNotePage,
      },
      {
        path: "/rating",
        name: "rating",
        component: RatingPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/rating/details/:id",
        name: "rating-details",
        component: () => import("@/pages/rating/details.vue"),
      },
      {
        path: "/rating/company-rating/:id",
        name: "company-rating",
        component: CompanyRatingPage,
      },
      {
        path: "/contracts",
        name: "contracts",
        component: ContractsPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/contracts/:id",
        name: "contract-details",
        component: ContractDetailsPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/questionnaires",
        name: "questionnaires",
        component: QuestionnairesPage,
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/questionnaires/details",
        name: "questionnaire-details",
        component: () => import("@/pages/questionnaires/details.vue"),
        meta: {
          // requiresAuth: true,
        },
      },
      {
        path: "/company",
        name: "companies",
        component: () => import("@/pages/company/index.vue"),
        meta: {
          layout: "default",
          requiresAuth: true,
        },
      },
      {
        path: "/company/company_details/:id",
        name: "company-details",
        component: () => import("@/pages/company/company_details.vue"),
        meta: {
          layout: "default",
          requiresAuth: true,
        },
      },
      {
        path: "/shifts",
        name: "shifts",
        component: ShiftsView,
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
