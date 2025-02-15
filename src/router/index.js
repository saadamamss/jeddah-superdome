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

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  // routes: setupLayouts(routes),
  routes,
  scrollBehavior(to, from, savedPosition) {
    // If a saved position is available (like when using the browser's back button), use it
    if (savedPosition) {
      return savedPosition;
    }
    // Scroll to top for every other navigation
    return { top: 0 };
  },
});

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
