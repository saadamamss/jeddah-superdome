// Utilities
import { defineStore } from "pinia";

export const useAppStore = defineStore("app", () => {
  const appBreadcrumb = ref();
  const appPageTitle = ref("");

  const setAppBreadcrumb = (breadcrumb = [], isBaseRoute = false) => {
    const baseHomeRoute = [{ label: $translate("home"), to: "/" }];

    if (isBaseRoute) {
      appBreadcrumb.value = [...baseHomeRoute];
      return;
    }

    appBreadcrumb.value = [...baseHomeRoute, ...breadcrumb];
  };

  const updateAppPageTitle = (pageTitle = "") => {
    appPageTitle.value = pageTitle;
  };
  return {
    appBreadcrumb,
    setAppBreadcrumb,
    appPageTitle,
    updateAppPageTitle,
  };
});
