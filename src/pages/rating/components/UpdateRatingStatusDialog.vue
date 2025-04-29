<script setup>
import { useAppStore } from "@/stores/app";
import { validationRules } from "@/@core/utils/validators";
import axios from "@/plugins/axios";

const props = defineProps({
  rating: Object,
});

const ratingStatus = computed(() => props?.rating);

const emit = defineEmits(["close", "done"]);
const ratingStatusForm = ref(null);
const submitting = ref(false);
const appStore = useAppStore();
const formState = reactive({
  status: null,
  final_rejection_reason: null,
});

const updateStatus = async () => {
  const { valid } = await ratingStatusForm.value.validate();

  if (!valid) {
    appStore.showSnackbar({
      message: "يرجى التحقق من صحة البيانات المدخلة",
      color: "error",
    });
    return;
  }

  if (!props.rating?.id) return;

  try {
    submitting.value = true;
    await axios.post(`/update-rating-final-status/${props.rating.id}`, {
      status: formState.status,
      final_rejection_reason: formState.final_rejection_reason
        ? formState.final_rejection_reason
        : undefined,
    });

    appStore.showSnackbar({
      message: "تم تحديث حالة التقييم بنجاح",
      color: "success",
    });

    emit("done");
  } catch (error) {
    console.error("Error adding user:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ!",
      color: "error",
    });
  } finally {
    submitting.value = false;
  }
};

const closeDialog = () => {
  emit("close");
};
</script>

<template>
  <v-dialog max-width="600px">
    <v-form ref="ratingStatusForm" @submit.prevent="updateStatus">
      <v-card class="pa-4">
        <v-card-title class="text-h5 mb-4">تحديث حالة التقييم</v-card-title>
        <v-card-text>
          <v-row>
            <v-col cols="12">
              <label class="mb-2 d-block">حالة التقييم </label>
              <v-select
                v-model="formState.status"
                :items="[
                  { id: 'APPROVED', title: 'قبول' },
                  { id: 'REJECTED', title: 'رفض' },
                ]"
                item-title="title"
                item-value="id"
                :rules="[validationRules.required]"
                required
              ></v-select>
            </v-col>
            <v-col v-if="formState.status === 'REJECTED'" cols="12">
              <label class="mb-2 d-block">سبب رفض التقييم </label>
              <v-textarea
                v-model="formState.final_rejection_reason"
                :rules="[validationRules.required]"
                required
                rows="3"
              ></v-textarea>
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            type="button"
            class="px-8 py-2 bg-red"
            color="error"
            @click="closeDialog()"
          >
            إلغاء
          </v-btn>
          <v-btn
            type="submit"
            color="primary"
            :loading="submitting"
            class="px-10 py-2 bg-primary text-white"
          >
            تأكيد
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>
