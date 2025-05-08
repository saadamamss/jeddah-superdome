<script setup>
const props = defineProps({
  modelValue: Boolean,
  attachmentUrl: String,
});

const emit = defineEmits(["close", "update:modelValue"]);
const dialogVisible = ref(false);

watch(
  () => props.modelValue,
  (val) => {
    dialogVisible.value = val;
  }
);

const updateDialog = (val) => {
  dialogVisible.value = val;
  emit("update:modelValue", val);
};

const closeDialog = () => {
  emit("close");
  updateDialog(false);
};
</script>

<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="updateDialog"
    width="625"
    persistent
  >
    <v-card class="py-4 px-2" rounded="lg">
      <v-card-title class="px-4 border-b pb-4 font-weight-bold">
        <div class="w-100 d-flex align-center justify-space-between">
          <h2 class="mb-0 text-h6 font-weight-bold">عرض المرفق</h2>
          <v-btn icon variant="text" color="black" @click="closeDialog">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text class="mb-6 border-b pb-4">
        <v-row>
          <v-col cols="12" class="py-8 text-center">
            <img
              style="
                min-width: 500px;
                max-width: 100%;
                min-height: 300px;
                max-height: 400px;
              "
              :src="attachmentUrl"
              alt=""
            />
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions>
        <v-row>
          <v-col cols="6">
            <v-btn
              @click="closeDialog"
              color="primary"
              size="large"
              variant="elevated"
              rounded="pill"
              block
            >
              <span class="d-inline-block px-4">إلغاء </span>
            </v-btn>
          </v-col>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
