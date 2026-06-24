<script setup lang="ts">
import { useThemeConfig } from '@core/composable/useThemeConfig'
import { computedWithControl, useCycleList } from '@vueuse/core'

const props = defineProps<{
  themes: { name: string; icon: string }[]
}>()

const { theme } = useThemeConfig()

const {
  state: currentTheme,
  next: getNextThemeName,
  index: currentThemeIndex,
} = useCycleList(
  props.themes.map((t: { name: string; icon: string }) => t.name),
  { initialValue: theme.value },
)

const changeTheme = () => {
  theme.value = getNextThemeName()
}

const getThemeIcon = computedWithControl(theme, (): string => {
  const nextThemeIndex = currentThemeIndex.value + 1 === props.themes.length ? 0 : currentThemeIndex.value + 1

  return props.themes[nextThemeIndex].icon
})

watch(theme, (val: string) => {
  currentTheme.value = val
})
</script>

<template>
  <VBtn color="default" icon size="small" variant="text" @click="changeTheme">
    <VIcon :icon="getThemeIcon" size="24" />
  </VBtn>
</template>
