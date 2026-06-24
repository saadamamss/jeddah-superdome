<script setup lang="ts">
defineProps<{
  title?: string
  number?: number
}>()
</script>

<template>
  <VCard class="rounded-lg" elevation="0">
    <v-card-text class="px-1 py-2 pb-0">
      <div class="d-flex flex-column gap-4 pa-4">
        <div class="d-flex flex-column">
          <div class="d-flex justify-start align-start">
            <slot name="icon" />
          </div>
          <div>
            <h2 class="text-h5 text-lg-h4 font-weight-bold text-primary">
              {{ number }}
            </h2>
          </div>
        </div>
        <div>
          <span class="text-lg text-black">{{ title }}</span>
        </div>
      </div>
    </v-card-text>
  </VCard>
</template>
