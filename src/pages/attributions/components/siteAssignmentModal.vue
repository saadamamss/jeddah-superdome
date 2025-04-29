<script setup>
import { ref, watch } from "vue";

const props = defineProps({
  modelValue: {
    type: Boolean,
    required: true,
  },
});

const emit = defineEmits(["update:modelValue", "save"]);

const selectedSite = ref(null);
const selectedServices = ref([]);
const isOpen = ref(props.modelValue);

// Watch for changes in modelValue prop
watch(
  () => props.modelValue,
  (newValue) => {
    isOpen.value = newValue;
  }
);

// Watch for changes in local isOpen
watch(
  () => isOpen.value,
  (newValue) => {
    emit("update:modelValue", newValue);
  }
);

// Static data for selects
const siteOptions = [
  { title: "إسناد موقع الرافعي في قباء", value: 1 },
  { title: "إسناد موقع في قباء", value: 2 },
  { title: "إسناد موقع الجعفري", value: 3 },
  { title: "إسناد موقع محمد عبدالله", value: 4 },
];

const serviceOptions = [
  { title: "خدمة السكن", value: 1 },
  { title: "خدمة الطعام", value: 2 },
  { title: "خدمة النقل", value: 3 },
  { title: "خدمة الإرشاد", value: 4 },
];

const handleSave = () => {
  emit("save", {
    site: selectedSite.value,
    services: selectedServices.value,
  });
  // Reset form
  selectedSite.value = null;
  selectedServices.value = [];
  isOpen.value = false;
};

const closeModal = () => {
  // Reset form
  selectedSite.value = null;
  selectedServices.value = [];
  isOpen.value = false;
};
</script>

<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="500px"
  >
    <v-card class="rounded-lg">
      <v-card-title class="text-h6 pa-4">إسناد موقع</v-card-title>

      <v-card-text class="pa-4">
        <div class="mb-4">
          <label class="text-body-2 font-weight-medium mb-2 d-block"
            >اسم الموقع</label
          >
          <v-select
            v-model="selectedSite"
            :items="siteOptions"
            item-title="title"
            item-value="value"
            placeholder="إسناد موقع الرافعي في قباء"
            variant="outlined"
            density="comfortable"
            class="mb-4"
            hide-details
          />
        </div>

        <div class="mb-4">
          <label class="text-body-2 font-weight-medium mb-2 d-block"
            >اسم مركز الخدمه</label
          >
          <v-select
            v-model="selectedServices"
            :items="serviceOptions"
            item-title="title"
            item-value="value"
            placeholder="اختر مركز الخدمه"
            variant="outlined"
            density="comfortable"
            multiple
            chips
            hide-details
          />
        </div>
      </v-card-text>

      <v-card-actions class="pa-4">
        <v-spacer></v-spacer>
        <v-btn color="default" variant="text" @click="closeModal">
          إلغاء
        </v-btn>
        <v-btn color="primary" @click="handleSave" :disabled="!selectedSite">
          إضافة
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style lang="scss" scoped>
.v-card-title {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}

:deep(.v-select) {
  .v-field__input {
    padding-top: 5px;
    min-height: 44px;
  }
}
</style>
