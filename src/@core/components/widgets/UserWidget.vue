<script setup>
defineProps({
  user: {},
  noShadow: {
    type: Boolean,
    default: false,
  },
  noPadding: {
    type: Boolean,
    default: false,
  },
});
</script>

<template>
  <VCard
    class="rounded-lg"
    :style="
      noShadow
        ? 'box-shadow: none'
        : 'box-shadow: 0px 4px 44px 4px rgba(0, 0, 0, 0.04)'
    "
  >
    <v-card-text :class="noPadding ? 'pa-0' : 'px-3 py-4'">
      <div class="d-flex ga-3">
        <div>
          <span
            class="d-flex justify-center align-center"
            style="width: 2.5rem; height: 2.5rem"
          >
            <img
              src="/imgs/person.svg"
              width="2.5rem"
              height="2.5rem"
              class="avatar-img"
              style="width: 2.5rem; height: 2.5rem"
            />
          </span>
        </div>
        <div>
          <h2 class="text-subtitle-1 text-black mb-1">
            {{ user?.name }}
          </h2>
          <p class="mb-0 text-body-2" style="color: #75797c">
            {{ user?.job }}
          </p>
        </div>
      </div>
    </v-card-text>
  </VCard>
</template>
