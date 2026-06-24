<script setup lang="ts">
import navItems from '@/navigation/vertical'
import { useThemeConfig } from '@core/composable/useThemeConfig'

// Components
import UserProfile from '@/layouts/components/UserProfile.vue'
import Notifications from '@/@core/components/Notifications.vue'

// @layouts plugin
import { VerticalNavLayout } from '@layouts/components'

const { appRouteTransition, isLessThanOverlayNavBreakpoint } = useThemeConfig()
const { width: windowWidth } = useWindowSize()
</script>

<template>
  <VerticalNavLayout :nav-items="navItems" style="background-color: #f8f9f9">
    <template #navbar="{ toggleVerticalOverlayNavActive }">
      <div class="d-flex h-100 align-center ga-3 text-dark">
        <VBtn
          v-if="isLessThanOverlayNavBreakpoint(windowWidth)"
          class="ms-n3"
          color="default"
          icon
          size="small"
          variant="text"
          @click="toggleVerticalOverlayNavActive(true)"
        >
          <VIcon icon="tabler-menu-2" size="24" />
        </VBtn>
        <!-- search box -->
        <v-btn class="btn-icon d-sm-none" color="default" elevation="0" size="small">
          <svg fill="none" height="1.5rem" viewBox="0 0 24 24" width="1.5rem" xmlns="http://www.w3.org/2000/svg">
            <path
              d="M19.25 19.25L15.5 15.5M4.75 11C4.75 7.54822 7.54822 4.75 11 4.75C14.4518 4.75 17.25 7.54822 17.25 11C17.25 14.4518 14.4518 17.25 11 17.25C7.54822 17.25 4.75 14.4518 4.75 11Z"
              stroke="#667178"
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="1.5"
            />
          </svg>
        </v-btn>

        <div class="d-none d-sm-block">
          <h2 class="text-h6">
            <b>مرحبًا، عبدالله 👋</b>
          </h2>
          <p class="text-sm text-secondary">نتمنى لك يومًا موفقًا في إدارة نقطة التجمع.</p>
        </div>
        <!-- <span class="text-black font-weight-semibold">{{
          appStore.appPageTitle
        }}</span> -->
        <!-- <NavbarThemeSwitcher /> -->
        <VSpacer />
        <div class="d-flex gap-3">
          <Notifications :notifications="[]" />
        </div>
        <div class="d-flex gap-3">
          <!-- <Notifications :notifications="notifications" /> -->
          <UserProfile />
        </div>
      </div>
    </template>

    <RouterView v-slot="{ Component }">
      <Transition mode="out-in" :name="appRouteTransition">
        <Component :is="Component" />
      </Transition>
    </RouterView>

    <!-- <template #footer>
      <Footer />
    </template> -->

    <!-- <TheCustomizer /> -->
  </VerticalNavLayout>
</template>
