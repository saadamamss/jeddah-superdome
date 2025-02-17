<script setup>
defineProps({
  title: "",
  number: 0,
});
</script>

<template>
  <VCard elevation="0" class="rounded-lg">
    <v-card-text class="px-1 py-2">
      <div class="d-flex flex-column gap-6 pa-4">
        <div class="d-flex justify-space-between align-center">
          <div>
            <h2 class="text-h5 text-lg-h4 font-weight-bold text-primary">
              {{ number }}
            </h2>
          </div>
          <div class="d-flex justify-center align-center">
            <slot name="icon" />
          </div>
        </div>
        <div>
          <span class="text-lg text-black">{{ title }}</span>
        </div>
      </div>
    </v-card-text>
  </VCard>
</template>
