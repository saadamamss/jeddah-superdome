/**
 * router/index.ts
 *
 * Automatic routes for `./src/pages/*.vue`
 */

// Composables
import { createRouter, createWebHistory } from "vue-router/auto";
// import { setupLayouts } from "virtual:generated-layouts";
// import { routes } from 'vue-router/auto-routes'

import routes from "./routes";
import { authService } from "@/services/auth.service";
import { hasPermission } from "@core/utils/permissions";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  // routes: setupLayouts(routes),
  routes,
  scrollBehavior(to, from, savedPosition) {
    return savedPosition || { top: 0 };
  },
});

// Global Navigation Guard
router.beforeEach((to, from, next) => {
  const isAuthenticated = authService.isAuthenticated();

  console.log({ isAuthenticated });

  if (
    (typeof to.meta.requiresPermission == "boolean" &&
      to.meta.requiresPermission) ||
    (typeof to.meta.requiresPermission == "string" &&
      !hasPermission(to.meta.requiresPermission))
  ) {
    next({ name: "NotFound" });
    return;
  }

  // if (
  //   typeof to.meta.requiresPermission == "boolean" &&
  //   !to.meta.requiresPermission &&
  //   to.meta.requiresAuth &&
  //   isAuthenticated
  // ) {
  //   console.log({ isAuthenticated });

  //   next();
  //   return;
  // }

  // if (
  //   typeof to.meta.requiresPermission == "string" &&
  //   !hasPermission(to.meta.requiresPermission)
  // ) {
  //   next({ name: "NotFound" });
  //   return;
  // }

  console.log({ check: !isAuthenticated });

  if (to.meta.requiresAuth && !isAuthenticated) {
    return next("/auth/login");
  } else if (to.meta.guest && isAuthenticated) {
    return next("/");
  } else {
    return next();
  }
});

// test@test.com
// password

// Workaround for https://github.com/vitejs/vite/issues/11804
router.onError((err, to) => {
  if (err?.message?.includes?.("Failed to fetch dynamically imported module")) {
    if (!localStorage.getItem("vuetify:dynamic-reload")) {
      console.log("Reloading page to fix dynamic import error");
      localStorage.setItem("vuetify:dynamic-reload", "true");
      location.assign(to.fullPath);
    } else {
      console.error("Dynamic import error, reloading page did not fix it", err);
    }
  } else {
    console.error(err);
  }
});

router.isReady().then(() => {
  localStorage.removeItem("vuetify:dynamic-reload");
});

export default router;
