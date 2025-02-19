import DashboardLayout from "@/layouts/DashboardLayout";
import LoginPage from "@/pages/auth/login";
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
      },
      {
        path: "/sites",
        name: "sites",
        component: SitesPage,
      },
      {
        path: "/sites/housing-details",
        name: "housing-details",
        component: HousingDetailsPage,
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
      },
      {
        path: "/main-sectors",
        name: "main-sectors",
        component: MainSectorsPage,
      },
      {
        path: "/roles",
        name: "roles",
        component: RolesPage,
      },
      {
        path: "/users",
        name: "users",
        component: UsersPage,
      },
      {
        path: "/notes",
        name: "notes",
        component: NotesPage,
      },
      {
        path: "/rating",
        name: "rating",
        component: RatingPage,
      },
      {
        path: "/contracts",
        name: "contracts",
        component: ContractsPage,
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
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: NotFoundPage,
  },
];
