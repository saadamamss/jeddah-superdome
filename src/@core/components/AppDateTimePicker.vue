<script setup>
import VueDatePicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";
import { useTheme } from "vuetify";

const props = defineProps({
  modelValue: Object,
  height: String,
  timePicker: Boolean,
  inline: Boolean,
});

const emit = defineEmits(["update:modelValue", "click:clear"]);

const onClear = (el) => {
  el.stopPropagation();
  nextTick(() => {
    emit("update:modelValue", "");
    emit("click:clear", el);
  });
};

const vuetifyTheme = useTheme();

const emitModelValue = (val) => {
  emit("update:modelValue", val);
};
</script>

<template>
  <VueDatePicker
    class="app-date-picker"
    :class="{ 'is-time-picker': timePicker, inline: inline }"
    :model-value="modelValue"
    :disabled="isReadonly.value"
    :inline="inline"
    :time-picker="timePicker"
    @update:model-value="emitModelValue"
    select-text="اختيار"
    cancel-text="إلغاء"
    placeholder="حدد التاريخ"
    auto-apply
  >
    <!-- locale="ar" -->
    <template #input-icon>
      <VIcon icon="formkit:date" class="text-dark" size="lg" />
    </template>
  </VueDatePicker>
</template>
<style lang="scss">
:root {
  --dp-direction: rtl;
  --dp-input-padding: 13px 40px 13px 20px;
  --dp-border-radius: 8px;
  --dp-border-color: rgb(var(--v-theme-gray));
  --dp-font-family: "IBMPlexArabic", Tahoma, sans-serif;
}

.dp__theme_light {
  --dp-primary-color: rgb(var(--v-theme-primary));
}

.app-date-picker {
  .dp__input {
    line-height: 1;
    letter-spacing: -0.8px;
  }

  .dp__input_icon {
    inset-inline-start: 8px;
    color: var(--dp-icon-color);
  }

  .dp__arrow_top {
    transform: translate(-50%, -50%) rotate(50deg);
  }
}
</style>
