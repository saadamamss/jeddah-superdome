<script setup lang="ts">
import { useSkins } from '@core/composable/useSkins'
import { useThemeConfig } from '@core/composable/useThemeConfig'

// @layouts plugin
import { useSettingsStore } from '@/stores/settings.store'

const DefaultLayoutWithVerticalNav = defineAsyncComponent(() => import('./components/DefaultLayoutWithVerticalNav.vue'))
const { width: windowWidth } = useWindowSize()
const { switchToVerticalNavOnLtOverlayNavBreakpoint } = useThemeConfig()

const settings = useSettingsStore()

// Remove below composable usage if you are not using horizontal nav layout in your app
switchToVerticalNavOnLtOverlayNavBreakpoint(windowWidth)

const { layoutAttrs, injectSkinClasses } = useSkins()

injectSkinClasses()
</script>

<template>
  <div>
    <div class="position-fixed" style="z-index: 99; top: 10px; left: 10px">
      <Alert
        :color="settings.alertColor"
        :is-show="settings.isAlertShow"
        :message="settings.alertMessage"
        @change-status="settings.isAlertShow = !settings.isAlertShow"
      />
    </div>
    <DefaultLayoutWithVerticalNav v-bind="layoutAttrs" />
  </div>
</template>

<style lang="scss">
// As we are using `layouts` plugin we need its styles to be imported
@use '@layouts/styles/default-layout';
</style>
