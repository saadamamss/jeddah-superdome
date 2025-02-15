<script setup>
import { useAuthStore } from "@/store/Auth";
import { useThemeConfig } from "@core/composable/useThemeConfig";
import { hexToRgb } from "@layouts/utils";
import { useTheme } from "vuetify";

const authStore = useAuthStore();

const {
  syncInitialLoaderTheme,
  syncVuetifyThemeWithTheme: syncConfigThemeWithVuetifyTheme,
} = useThemeConfig();

const { global } = useTheme();

// ℹ️ Sync current theme with initial loader theme
syncInitialLoaderTheme();
syncConfigThemeWithVuetifyTheme();

onMounted(() => {
  // if(localStorage.getItem("SAR_TOKEN")) {
  //    const user = JSON.parse(localStorage.getItem("SAR_TOKEN"));
  //    authStore.updateUser(user);
  //  }
});
</script>

<template>
  <VLocaleProvider rtl>
    <VApp
      :style="`--v-global-theme-primary: ${hexToRgb(
        global.current.value.colors.primary
      )}`"
    >
      <RouterView />
    </VApp>
  </VLocaleProvider>
</template>

<style>
@import "@layouts/styles/index.scss";

@font-face {
  font-family: "IBMPlexArabic";
  src: url("/fonts/IBMPlexArabic/IBMPlexArabic.ttf") format("truetype");
  font-weight: 400;
}

body {
  block-size: fill-available;
  font-family: "IBMPlexArabic", Tahoma, sans-serif;
}

body pre {
  font-family: "IBMPlexArabic", Tahoma, sans-serif !important;
  line-height: 2;
}

.whitespace-nowrap {
  white-space: nowrap;
}

.v-enter-active,
.v-leave-active {
  transition: opacity 0.5s ease;
}

.v-enter-from,
.v-leave-to {
  opacity: 0;
}

.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.25s ease-out;
}

.slide-up-enter-from {
  opacity: 0;
  transform: translateY(30px);
}

.slide-up-leave-to {
  opacity: 0;
  transform: translateY(-30px);
}
</style>
