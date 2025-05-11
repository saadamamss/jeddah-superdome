<script setup>
import navItems from "@/navigation/vertical";
import { useThemeConfig } from "@core/composable/useThemeConfig";
import { useAppStore } from "@/stores/app";

// Components
import Footer from "@/layouts/components/Footer.vue";
import NavbarThemeSwitcher from "@/layouts/components/NavbarThemeSwitcher.vue";
import UserProfile from "@/layouts/components/UserProfile.vue";
import Notifications from "@/@core/components/Notifications";

// @layouts plugin
import { VerticalNavLayout } from "@layouts";

const appStore = useAppStore();

const { appRouteTransition, isLessThanOverlayNavBreakpoint } = useThemeConfig();
const { width: windowWidth } = useWindowSize();

const notifications = ref([
  {
    id: 1,
    title: "عنوان التنبيه",
    subtitle: "نص وهمي يمكن استبداله",
  },
  {
    id: 2,
    title: "عنوان التنبيه",
    subtitle: "نص وهمي يمكن استبداله نص وهمي يمكن استبداله",
  },
  {
    id: 3,
    title: "عنوان التنبيه",
    subtitle: "نص وهمي يمكن استبداله",
  },
]);

const search = ref("");
</script>

<template>
  <VerticalNavLayout :nav-items="navItems" style="background-color: #f8f9f9">
    <template #navbar="{ toggleVerticalOverlayNavActive }">
      <div class="d-flex h-100 align-center ga-3 text-dark">
        <VBtn
          v-if="isLessThanOverlayNavBreakpoint(windowWidth)"
          icon
          variant="text"
          color="default"
          class="ms-n3"
          size="small"
          @click="toggleVerticalOverlayNavActive(true)"
        >
          <VIcon icon="tabler-menu-2" size="24" />
        </VBtn>

        <!-- search box -->

        <v-btn
          color="default"
          size="small"
          class="btn-icon d-sm-none"
          elevation="0"
        >
          <svg
            width="1.5rem"
            height="1.5rem"
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M19.25 19.25L15.5 15.5M4.75 11C4.75 7.54822 7.54822 4.75 11 4.75C14.4518 4.75 17.25 7.54822 17.25 11C17.25 14.4518 14.4518 17.25 11 17.25C7.54822 17.25 4.75 14.4518 4.75 11Z"
              stroke="#667178"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </v-btn>

        <!-- <span class="text-black font-weight-semibold">{{
          appStore.appPageTitle
        }}</span> -->
        <!-- <NavbarThemeSwitcher /> -->
        <VSpacer />
        <div class="d-flex gap-3">
          <!-- <Notifications :notifications="notifications" /> -->

          <UserProfile />
        </div>
      </div>
    </template>

    <RouterView v-slot="{ Component }">
      <Transition :name="appRouteTransition" mode="out-in">
        <Component :is="Component" />
      </Transition>
    </RouterView>

    <!-- <template #footer>
      <Footer />
    </template> -->

    <!-- <TheCustomizer /> -->
  </VerticalNavLayout>
</template>
