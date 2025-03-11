<script setup>
import { ref, onMounted, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAppStore } from "@/stores/app";
// import axios from "@/plugins/axios";
import axiosIns from "@/plugins/axios";
import Datepicker from "@vuepic/vue-datepicker";
import "@vuepic/vue-datepicker/dist/main.css";

const route = useRoute();
const router = useRouter();
const appStore = useAppStore();

const contract = ref(null);
const isLoading = ref(true);
const isSaving = ref(false);

const editForm = ref({
  name: "",
  created_at: null,
  updated_at: null,
});

// Validate route and redirect if no ID
const validateRoute = () => {
  if (!route.params.id) {
    appStore.showSnackbar({
      message: "معرف الباقة غير موجود",
      color: "error",
    });
    router.push("/contracts");
    return false;
  }
  return true;
};

// Watch for route changes
watch(
  () => route.params.id,
  (newId) => {
    if (newId) {
      fetchContractDetails();
    }
  }
);

// Fetch contract details
const fetchContractDetails = async () => {
  if (!validateRoute()) return;

  try {
    isLoading.value = true;
    // const response = await axios.get(`contracts/${route.params.id}`);
    const response = await axiosIns.get(`contracts/${route.params.id}`);

    if (response.data?.status?.success) {
      contract.value = response.data.data;
      // Initialize edit form with current values
      editForm.value = {
        name: contract.value.name || "",
        created_at: contract.value.created_at
          ? formatDateForInput(contract.value.created_at)
          : null,
        updated_at: contract.value.updated_at
          ? formatDateForInput(contract.value.updated_at)
          : null,
      };
    }
  } catch (error) {
    console.error("Error fetching contract:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    router.push("/contracts");
  } finally {
    isLoading.value = false;
  }
};

// Update contract
const updateContract = async () => {
  if (!validateRoute()) return;

  try {
    isSaving.value = true;

    const formData = {
      name: editForm.value.name,
      created_at: formatDateForAPI(editForm.value.created_at),
      updated_at: formatDateForAPI(editForm.value.updated_at),
    };

    const response = await axiosIns.put(
      `contracts/${route.params.id}`,
      formData
    );

    if (response.data?.status?.success) {
      appStore.showSnackbar({
        message: "تم تحديث الباقة بنجاح",
        color: "success",
      });
      contract.value = response.data.data;
      editForm.value = {
        name: contract.value.name || "",
        created_at: formatDateForInput(contract.value.created_at),
        updated_at: formatDateForInput(contract.value.updated_at),
      };
    }
  } catch (error) {
    console.error("Error updating contract:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء تحديث البيانات",
      color: "error",
    });
  } finally {
    isSaving.value = false;
  }
};

const formatDate = (date) => {
  if (!date) return "";
  try {
    const dateObj = new Date(date);
    if (isNaN(dateObj.getTime())) return date;

    const formatter = new Intl.DateTimeFormat("ar", {
      calendar: "gregory",
      day: "numeric",
      month: "short",
      year: "numeric",
      hour: "numeric",
      minute: "numeric",
      hour12: true,
    });

    return formatter.format(dateObj);
  } catch (error) {
    return date;
  }
};

const formatDateForInput = (date) => {
  if (!date) return null;
  try {
    const dateObj = new Date(date);
    return isNaN(dateObj.getTime()) ? null : dateObj;
  } catch (error) {
    console.error("Error formatting date for input:", error);
    return null;
  }
};

const formatDateForAPI = (date) => {
  if (!date) return null;
  try {
    return date instanceof Date && !isNaN(date.getTime())
      ? date.toISOString()
      : null;
  } catch (error) {
    console.error("Error formatting date for API:", error);
    return null;
  }
};

const goBack = () => {
  router.push("/contracts");
};

onMounted(() => {
  if (validateRoute()) {
    fetchContractDetails();
  }
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
      <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
        <v-card-text class="pa-4">
          <!-- Header -->
          <div
            class="d-flex align-center justify-space-between border-b pb-4 mb-6"
          >
            <div class="d-flex align-center ga-4">
              <v-btn icon variant="text" color="default" @click="goBack">
                <v-icon>mdi-arrow-right</v-icon>
              </v-btn>
              <h2 class="text-h5 font-weight-bold">تفاصيل الباقة</h2>
            </div>
            <div class="d-flex align-center ga-2">
              <v-btn
                color="primary"
                class="rounded-lg"
                :loading="isSaving"
                @click="updateContract"
              >
                حفظ التغييرات
              </v-btn>
            </div>
          </div>

          <!-- Loading State -->
          <div v-if="isLoading" class="d-flex justify-center align-center py-8">
            <v-progress-circular
              indeterminate
              color="primary"
            ></v-progress-circular>
          </div>

          <!-- Content -->
          <div v-else-if="contract" class="contract-details">
            <v-row>
              <v-col cols="12" md="8">
                <!-- Basic Information -->
                <div class="mb-6">
                  <h3 class="text-h6 font-weight-bold mb-4">
                    المعلومات الأساسية
                  </h3>
                  <v-card variant="outlined" class="rounded-lg">
                    <v-card-text>
                      <v-form @submit.prevent="updateContract">
                        <div class="d-flex flex-column ga-4">
                          <!-- Contract Name -->
                          <div class="form-group">
                            <label class="text-subtitle-2 mb-2 d-block"
                              >اسم الباقة</label
                            >
                            <v-text-field
                              v-model="editForm.name"
                              variant="outlined"
                              density="comfortable"
                              placeholder="اسم الباقة"
                              hide-details
                              class="rounded-lg bg-grey-lighten-4"
                              required
                            />
                          </div>

                          <!-- Created At -->
                          <div class="form-group">
                            <label class="text-subtitle-2 mb-2 d-block"
                              >تاريخ الإنشاء</label
                            >
                            <Datepicker
                              v-model="editForm.created_at"
                              :max-date="new Date()"
                              format="dd MMM yyyy"
                              locale="ar"
                              :enable-time-picker="false"
                              auto-apply
                              text-input
                              :placeholder="'اختر التاريخ'"
                              :clearable="false"
                              class="date-picker-custom"
                              :rtl="true"
                            />
                          </div>

                          <!-- Updated At -->
                          <div class="form-group">
                            <label class="text-subtitle-2 mb-2 d-block"
                              >آخر تحديث</label
                            >
                            <Datepicker
                              v-model="editForm.updated_at"
                              :max-date="new Date()"
                              format="dd MMM yyyy"
                              locale="ar"
                              :enable-time-picker="false"
                              auto-apply
                              text-input
                              :placeholder="'اختر التاريخ'"
                              :clearable="false"
                              class="date-picker-custom"
                              :rtl="true"
                            />
                          </div>
                        </div>
                      </v-form>
                    </v-card-text>
                  </v-card>
                </div>
              </v-col>
            </v-row>
          </div>

          <!-- Error State -->
          <div v-else class="d-flex justify-center align-center py-8">
            <p class="text-body-1 text-medium-emphasis">لا توجد بيانات متاحة</p>
          </div>
        </v-card-text>
      </VCard>
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.ga-2 {
  gap: 0.5rem;
}

.ga-4 {
  gap: 1rem;
}

.border-b {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}

:deep(.v-btn) {
  text-transform: none;
  letter-spacing: 0;
}

.form-group {
  margin-bottom: 1.5rem;

  &:last-child {
    margin-bottom: 0;
  }

  label {
    color: rgba(0, 0, 0, 0.6);
    margin-bottom: 0.5rem;
  }
}

:deep(.v-text-field) {
  .v-field__input {
    padding: 8px 16px;
  }

  .v-field__outline__start,
  .v-field__outline__end {
    border-color: rgba(0, 0, 0, 0.12);
  }

  &:hover,
  &:focus-within {
    .v-field__outline__start,
    .v-field__outline__end {
      border-color: var(--v-primary-base);
    }
  }
}

:deep(.date-picker-custom) {
  width: 100%;

  .dp__input {
    padding: 8px 16px;
    background-color: rgb(250, 250, 250);
    border: 1px solid rgba(0, 0, 0, 0.12);
    border-radius: 8px;
    font-size: 14px;
    height: 40px;
    color: rgba(0, 0, 0, 0.87);
    text-align: right;
  }

  .dp__main {
    border: 1px solid rgba(0, 0, 0, 0.12);
    border-radius: 8px;
    font-family: inherit;
    direction: rtl;
  }

  .dp__calendar_header,
  .dp__calendar {
    direction: rtl;
    width: 100%;
  }

  .dp__calendar_header {
    padding: 8px;
    background-color: rgb(250, 250, 250);
  }

  .dp__active_date {
    background-color: var(--v-primary-base) !important;
    color: white !important;
  }

  .dp__today {
    border-color: var(--v-primary-base);
  }

  .dp__action_buttons {
    padding: 0 8px 8px;
  }

  .dp__month_year_row {
    direction: rtl;
    margin: 8px;
  }

  .dp__overlay {
    direction: rtl;
  }

  .dp__instance_calendar {
    width: 100%;
    direction: rtl;
  }

  .dp__menu {
    direction: rtl;
    min-width: 300px;
  }
}
</style>
