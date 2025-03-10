<script setup>
import { useAppStore } from "@/stores/app";
import { useRoute, useRouter } from "vue-router";
import { ref, onMounted } from "vue";
import axios from "@/plugins/axios";
import AppDateTimePicker from "@/@core/components/AppDateTimePicker";

const appStore = useAppStore();
const route = useRoute();
const router = useRouter();

const links = ref([
  { title: "إدارة المراحل", href: "/main-sectors", disabled: false },
  { title: "تفاصيل المرحلة", href: "/main-sectors/details", disabled: true },
]);

const phase = ref(null);
const isLoading = ref(true);
const loading = ref(false);
const deleteDialog = ref(false);

// Form validation rules
const formRules = {
  title: [(v) => !!v || "عنوان المرحلة مطلوب"],
  description: [(v) => !!v || "وصف المرحلة مطلوب"],
  start_date: [(v) => !!v || "تاريخ البداية مطلوب"],
  end_date: [
    (v) => !!v || "تاريخ النهاية مطلوب",
    (v, form) =>
      !form.start_date ||
      v >= form.start_date ||
      "تاريخ النهاية يجب أن يكون بعد تاريخ البداية",
  ],
};

const form = ref(null);

// Fetch phase details
const fetchPhase = async () => {
  try {
    isLoading.value = true;
    const response = await axios.get(`/phases/${route.params.id}`);
    if (response.data?.status?.success) {
      phase.value = response.data.data;
    }
  } catch (error) {
    console.error("Error fetching phase:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    router.push("/main-sectors");
  } finally {
    isLoading.value = false;
  }
};

// Update phase
const updatePhase = async () => {
  const { valid } = await form.value?.validate();

  if (!valid) {
    appStore.showSnackbar({
      message: "يرجى التحقق من صحة جميع الحقول",
      color: "error",
    });
    return;
  }

  try {
    loading.value = true;

    // Format dates to match API expectations
    const formattedData = {
      ...phase.value,
      start_date: phase.value.start_date
        ? new Date(phase.value.start_date).toISOString().split("T")[0]
        : null,
      end_date: phase.value.end_date
        ? new Date(phase.value.end_date).toISOString().split("T")[0]
        : null,
    };

    const response = await axios.put(
      `/phases/${route.params.id}`,
      formattedData
    );

    if (response.data?.status?.success) {
      appStore.showSnackbar({
        message: "تم تحديث المرحلة بنجاح",
        color: "success",
      });
      await fetchPhase();
    }
  } catch (error) {
    console.error("Error updating phase:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء تحديث المرحلة",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Delete phase
const deletePhase = async () => {
  try {
    loading.value = true;
    const response = await axios.delete(`/phases/${route.params.id}`);

    if (response.data?.status?.success) {
      appStore.showSnackbar({
        message: "تم حذف المرحلة بنجاح",
        color: "success",
      });
      router.push("/main-sectors");
    }
  } catch (error) {
    console.error("Error deleting phase:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حذف المرحلة",
      color: "error",
    });
  } finally {
    loading.value = false;
    deleteDialog.value = false;
  }
};

onMounted(() => {
  fetchPhase();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
      <!-- Add breadcrumbs -->
      <v-breadcrumbs
        :items="links"
        class="px-0 custom-breadcrumbs mb-4"
        active-class="primary--text"
      >
        <template v-slot:divider>
          <svg
            width="16"
            height="16"
            viewBox="0 0 16 16"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M6.85352 11.8536C6.65825 11.6583 6.65825 11.3417 6.85352 11.1465L9.50016 8.49982L6.85352 5.85318C6.65825 5.65792 6.65825 5.34134 6.85352 5.14607C7.04878 4.95081 7.36536 4.95081 7.56063 5.14607L10.5606 8.14607C10.7559 8.34134 10.7559 8.65792 10.5606 8.85318L7.56063 11.8532C7.36536 12.0484 7.04878 12.0484 6.85352 11.8536Z"
              fill="#929A9F"
            />
          </svg>
        </template>
      </v-breadcrumbs>

      <v-row v-if="isLoading">
        <v-col cols="12">
          <v-card elevation="0">
            <v-card-text class="px-1 py-2">
              <v-skeleton-loader
                class="mx-auto"
                elevation="0"
                max-width="600"
                type="article"
              ></v-skeleton-loader>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <div v-else>
        <v-container fluid class="px-0 rounded-lg">
          <v-card elevation="0" class="rounded-lg px-4 bg-light-white">
            <v-card-title class="pt-4 pb-6">
              <div class="d-flex align-center justify-space-between ga-4">
                <h2 class="text-h5 font-weight-bold mb-4">تفاصيل المرحلة</h2>
                <v-btn
                  color="error"
                  variant="outlined"
                  @click="deleteDialog = true"
                  :loading="loading"
                >
                  حذف المرحلة
                </v-btn>
              </div>
              <v-divider class="mt-6" />
            </v-card-title>

            <v-card-text class="pa-6">
              <v-form ref="form" @submit.prevent="updatePhase">
                <div class="d-flex flex-column ga-6">
                  <div class="input-group">
                    <label class="text-subtitle-2 mb-2 d-block"
                      >عنوان المرحلة</label
                    >
                    <v-text-field
                      v-model="phase.title"
                      placeholder="أدخل عنوان المرحلة"
                      variant="outlined"
                      density="comfortable"
                      bg-color="rgb(250, 250, 250)"
                      hide-details="auto"
                      class="input-field"
                      :rules="formRules.title"
                    ></v-text-field>
                  </div>

                  <div class="input-group">
                    <label class="text-subtitle-2 mb-2 d-block"
                      >وصف المرحلة</label
                    >
                    <v-textarea
                      v-model="phase.description"
                      placeholder="أدخل وصف المرحلة"
                      variant="outlined"
                      density="comfortable"
                      bg-color="rgb(250, 250, 250)"
                      hide-details="auto"
                      class="input-field"
                      rows="3"
                      :rules="formRules.description"
                    ></v-textarea>
                  </div>

                  <div class="d-flex gap-4">
                    <div class="input-group flex-1">
                      <label class="text-subtitle-2 mb-2 d-block"
                        >تاريخ البداية</label
                      >
                      <AppDateTimePicker
                        v-model="phase.start_date"
                        class="date-picker-field"
                        :enable-time-picker="false"
                        auto-apply
                        :clearable="false"
                        placeholder="اختر تاريخ البداية"
                        :error-messages="
                          !phase.start_date ? ['تاريخ البداية مطلوب'] : []
                        "
                      />
                    </div>

                    <div class="input-group flex-1">
                      <label class="text-subtitle-2 mb-2 d-block"
                        >تاريخ النهاية</label
                      >
                      <AppDateTimePicker
                        v-model="phase.end_date"
                        class="date-picker-field"
                        :enable-time-picker="false"
                        auto-apply
                        :clearable="false"
                        placeholder="اختر تاريخ النهاية"
                        :error-messages="
                          !phase.end_date ? ['تاريخ النهاية مطلوب'] : []
                        "
                        :min-date="phase.start_date"
                      />
                    </div>
                  </div>
                </div>

                <v-card-actions class="pa-0 mt-6">
                  <v-spacer></v-spacer>
                  <v-btn
                    color="primary"
                    type="submit"
                    :loading="loading"
                    min-width="140"
                    height="48"
                    class="rounded-lg"
                  >
                    حفظ التغييرات
                  </v-btn>
                  <v-btn
                    color="grey-darken-1"
                    variant="outlined"
                    @click="router.push('/main-sectors')"
                    min-width="140"
                    height="48"
                    class="rounded-lg me-3"
                  >
                    إلغاء
                  </v-btn>
                </v-card-actions>
              </v-form>
            </v-card-text>
          </v-card>
        </v-container>
      </div>
    </v-container>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400px">
      <v-card class="rounded-lg">
        <v-card-title class="text-h6 pa-6">تأكيد الحذف</v-card-title>
        <v-card-text class="pa-6">
          هل أنت متأكد من حذف هذه المرحلة؟ لا يمكن التراجع عن هذا الإجراء.
        </v-card-text>
        <v-card-actions class="pa-6">
          <v-spacer></v-spacer>
          <v-btn
            color="error"
            variant="outlined"
            @click="deletePhase"
            :loading="loading"
          >
            حذف
          </v-btn>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="deleteDialog = false"
          >
            إلغاء
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.input-group {
  .input-field {
    :deep(.v-field) {
      border-radius: 8px;
      background-color: rgb(250, 250, 250);
      border: 1px solid rgba(0, 0, 0, 0.12);

      &.v-field--focused {
        background-color: white;
        border-color: var(--v-primary-base);
      }

      .v-field__input {
        min-height: 44px;
        padding-top: 0;
        padding-bottom: 0;
      }

      .v-field__outline {
        display: none;
      }
    }
  }
}

.ga-6 {
  gap: 1.5rem;
}

.flex-1 {
  flex: 1;
}

.date-picker-field {
  :deep(.dp__input) {
    padding: 13px 16px;
    background-color: rgb(250, 250, 250);
    border: 1px solid rgba(0, 0, 0, 0.12);
    border-radius: 8px;
    font-size: 14px;
    height: 44px;
    color: rgba(0, 0, 0, 0.87);
    text-align: right;
    padding-left: 40px;

    &:focus {
      background-color: white;
      border-color: var(--v-primary-base);
    }
  }

  :deep(.dp__input_icon) {
    position: absolute;
    left: 12px;
    top: 50%;
    transform: translateY(-50%);
    color: rgba(0, 0, 0, 0.54);
    opacity: 1 !important;
    pointer-events: none;
    width: 20px;
    height: 20px;
    display: none;
  }

  :deep(.dp__input_wrap) {
    position: relative;
  }

  :deep(.dp__main) {
    border: 1px solid rgba(0, 0, 0, 0.12);
    border-radius: 8px;
    font-family: inherit;
    direction: rtl;
  }

  :deep(.dp__calendar_header),
  :deep(.dp__calendar) {
    direction: rtl;
    width: 100%;
  }

  :deep(.dp__calendar_header) {
    padding: 8px;
    background-color: rgb(250, 250, 250);
  }

  :deep(.dp__active_date) {
    background-color: var(--v-primary-base) !important;
    color: white !important;
  }

  :deep(.dp__today) {
    border-color: var(--v-primary-base);
  }
}

.custom-breadcrumbs {
  :deep(.v-breadcrumbs-item--disabled) {
    opacity: 1;
  }

  :deep(.v-breadcrumbs__item) {
    color: rgba(0, 0, 0, 0.6);
    font-size: 13px;
    padding: 0 8px;
  }

  :deep(.v-breadcrumbs-item--disabled) {
    color: rgba(0, 0, 0, 0.6);
  }
}
</style>
