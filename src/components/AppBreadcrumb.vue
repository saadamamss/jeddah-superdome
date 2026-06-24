<template>
  <div class="mb-2">
    <v-breadcrumbs divider=">" :items="computedItems">
      <template #prepend>
        <svg fill="none" height="20" viewBox="0 0 20 20" width="20" xmlns="http://www.w3.org/2000/svg">
          <path
            d="M5.62519 16.0419H14.3752C15.2957 16.0419 16.0419 15.2957 16.0419 14.3752V8.12519L10.0002 3.95853L3.95853 8.12519V14.3752C3.95853 15.2957 4.70472 16.0419 5.62519 16.0419Z"
            stroke="#25343E"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="1.5"
          />
          <path
            v-if="showHomePath"
            d="M8.12468 13.1244C8.12468 12.2039 8.87088 11.4577 9.79135 11.4577H10.208C11.1285 11.4577 11.8747 12.2039 11.8747 13.1244V16.0411H8.12468V13.1244Z"
            stroke="#25343E"
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="1.5"
          />
        </svg>
      </template>
    </v-breadcrumbs>
  </div>
</template>

<script setup lang="ts">
interface BreadcrumbItem {
  title: string
  disabled?: boolean
  to?: string
}

const props = defineProps<{
  items: BreadcrumbItem[]
  showHomePath?: boolean
}>()

const computedItems = computed(() => {
  return props.items.map((item: BreadcrumbItem, index: number) => ({
    title: item.title,
    disabled: item.disabled ?? index === props.items.length - 1,
    to: item.to,
  }))
})
</script>
