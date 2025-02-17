<template>
  <div class="d-flex flex-wrap justify-space-between align-center pa-4">
    <div class="pagination-container d-flex align-center justify-space-between">
      <v-select
        :model-value="perPage"
        :items="perPageOptions"
        variant="outlined"
        density="compact"
        class="per-page-select me-4"
        hide-details
        bg-color="white"
        @update:model-value="$emit('update:perPage', $event)"
      />
      <span class="text-subtitle-2 text-grey-darken-1 ms-4"
        >تم عرض من {{ startItem }} إلى {{ endItem }} من أصل
        {{ totalItems }}</span
      >
    </div>
    <v-pagination
      :model-value="currentPage"
      :length="totalPages"
      :total-visible="5"
      density="comfortable"
      variant="elevated"
      active-color="#B69265"
      @update:model-value="$emit('update:page', $event)"
    />
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  title: {
    type: String,
    required: true,
  },
  totalItems: {
    type: Number,
    required: true,
    default: 0,
  },
  perPage: {
    type: Number,
    required: true,
    default: 6,
  },
  currentPage: {
    type: Number,
    required: true,
    default: 1,
  },
});

const emit = defineEmits(["update:page", "update:perPage"]);

const perPageOptions = [
  { title: "6", value: 6 },
  { title: "12", value: 12 },
  { title: "24", value: 24 },
  { title: "36", value: 36 },
];

const totalPages = computed(
  () => Math.ceil(props.totalItems / props.perPage) || 1
);

const startItem = computed(() => {
  if (props.totalItems === 0) return 0;
  return (Number(props.currentPage) - 1) * Number(props.perPage) + 1;
});

const endItem = computed(() => {
  if (props.totalItems === 0) return 0;
  return Math.min(
    Number(props.currentPage) * Number(props.perPage),
    props.totalItems
  );
});
</script>

<style lang="scss">
.pagination-container {
  .v-field {
    border-radius: 50px !important;
    // min-width: 50px !important;
    // max-width: 50px !important;

    .v-field__input {
      min-height: 32px !important;
      padding: 0 12px !important;
    }

    .v-field__append-inner {
      padding-inline-start: 8px !important;
    }
  }
}

:deep(.v-pagination__item--active) {
  background-color: #b69265 !important;
}

.v-pagination .v-btn {
  border-radius: 50% !important;
  width: 32px;
  height: 32px;
}
</style>
