const routes = [
  // ... other routes ...

  {
    path: "/main-sectors",
    name: "MainSectors",
    component: () => import("@/pages/main-sectors/index.vue"),
    meta: {
      layout: "default",
      requiresAuth: true,
    },
  },
  {
    path: "/main-sectors/details/:id",
    name: "PhaseDetails",
    component: () => import("@/pages/main-sectors/details.vue"),
    meta: {
      layout: "default",
      requiresAuth: true,
    },
  },

  // ... other routes ...
];
