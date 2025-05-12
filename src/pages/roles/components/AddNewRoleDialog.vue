<script setup>
import { useAppStore } from "@/stores/app";
import { ref, reactive, onMounted, watch } from "vue";
import axios from "@/plugins/axios";

const props = defineProps({
  modelValue: Boolean,
});

const emit = defineEmits(["close", "update:modelValue", "role-added"]);

const appStore = useAppStore();
const isLoading = ref(false);
const permissions = ref([]);
const isSubmitting = ref(false);
const dialogVisible = ref(false);

watch(
  () => props.modelValue,
  (val) => {
    dialogVisible.value = val;
    if (val) {
      fetchPermissions();
    }
  }
);

const updateDialog = (val) => {
  dialogVisible.value = val;
  emit("update:modelValue", val);
};

const formState = reactive({
  name: "",
  description: "",
  permissions: [],
});

const fetchPermissions = async () => {
  if (permissions.value.length > 0) return;

  isLoading.value = true;
  try {
    const response = await axios.get("/permissions");
    if (response.data?.data) {
      permissions.value = response.data.data.map((permission) => ({
        title: permission.name,
        value: permission.key,
        checked: false,
      }));
    }
  } catch (error) {
    console.error("Error fetching permissions:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب الصلاحيات",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

const togglePermission = (permissionId) => {
  const index = formState.permissions.indexOf(permissionId);
  if (index === -1) {
    formState.permissions.push(permissionId);
  } else {
    formState.permissions.splice(index, 1);
  }
};

const validateForm = () => {
  if (!formState.name) {
    appStore.showSnackbar({
      message: "يرجى إدخال اسم الدور",
      color: "error",
    });
    return false;
  }

  if (formState.permissions.length === 0) {
    appStore.showSnackbar({
      message: "يرجى اختيار صلاحية واحدة على الأقل",
      color: "error",
    });
    return false;
  }

  return true;
};

const handleSubmit = async () => {
  if (!validateForm()) return;

  isSubmitting.value = true;
  try {
    await axios.post("/add-role", {
      name: formState.name,
      // description: formState.description,
      permissions: formState.permissions,
    });

    appStore.showSnackbar({
      message: "تم إضافة الدور بنجاح",
      color: "success",
    });

    closeDialog();
    emit("role-added");
  } catch (error) {
    console.error("Error creating role:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء إضافة الدور",
      color: "error",
    });
  } finally {
    isSubmitting.value = false;
  }
};

const closeDialog = () => {
  emit("close");
  updateDialog(false);
  // Reset form
  formState.name = "";
  formState.description = "";
  formState.permissions = [];
};

onMounted(() => {
  if (props.modelValue) {
    fetchPermissions();
  }
});
</script>

<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="updateDialog"
    width="625"
  >
    <v-card class="py-4 px-2" rounded="lg">
      <v-card-title class="px-4 border-b pb-4 font-weight-bold">
        <div class="w-100 d-flex align-center justify-space-between">
          <h2 class="mb-0 text-h6 font-weight-bold">إضافة دور جديد</h2>
          <v-btn icon variant="text" color="black" @click="closeDialog">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text class="mb-6 border-b pb-4">
        <v-form @submit.prevent="handleSubmit">
          <v-row>
            <v-col cols="12">
              <label class="d-inline-block mb-1 text-black">اسم الدور</label>
              <v-text-field
                v-model="formState.name"
                variant="text"
                class="mb-1 rounded-lg"
                density="comfortable"
                style="background-color: #f9f9f9"
                placeholder="اسم الدور"
                :error-messages="
                  !formState.name && isSubmitting ? 'هذا الحقل مطلوب' : ''
                "
              ></v-text-field>
            </v-col>
            <v-col cols="12">
              <label class="d-flex ga-1 align-center mb-1 text-black">
                <span>وصف الدور</span>
                <small style="color: #667178">(اختياري)</small>
              </label>
              <v-textarea
                v-model="formState.description"
                variant="text"
                class="mb-1 rounded-lg"
                density="comfortable"
                style="background-color: #ffffff"
                placeholder="وصف الدور..."
              ></v-textarea>
            </v-col>
            <v-col cols="12">
              <label class="d-flex ga-1 align-center mb-1 text-black">
                صلاحيات الدور
              </label>

              <div v-if="isLoading" class="d-flex justify-center pa-4">
                <v-progress-circular
                  indeterminate
                  color="primary"
                ></v-progress-circular>
              </div>

              <div
                v-else
                class="permissions-container py-4 px-3 rounded-lg"
                style="max-height: 300px; overflow-y: auto"
                :class="{
                  'error-border':
                    formState.permissions.length === 0 && isSubmitting,
                }"
              >
                <div
                  v-if="permissions.length === 0"
                  class="text-center py-4 text-medium-emphasis"
                >
                  لا توجد صلاحيات متاحة
                </div>

                <v-row v-else>
                  <v-col
                    v-for="permission in permissions"
                    :key="permission.key"
                    cols="6"
                    class="py-0"
                  >
                    <v-checkbox
                      v-model="permission.checked"
                      :label="permission.title"
                      color="primary"
                      hide-details
                      density="compact"
                      class="square-checkbox mb-1"
                      @update:model-value="togglePermission(permission.value)"
                    ></v-checkbox>
                  </v-col>
                </v-row>
              </div>

              <div
                v-if="formState.permissions.length === 0 && isSubmitting"
                class="text-error text-caption mt-1"
              >
                يرجى اختيار صلاحية واحدة على الأقل
              </div>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>

      <v-card-actions>
        <v-btn
          variant="text"
          rounded="lg"
          color="black"
          style="background-color: #f8f9f9"
          @click="closeDialog"
          :disabled="isSubmitting"
        >
          <span class="d-inline-block px-4">الغاء</span>
        </v-btn>
        <v-spacer></v-spacer>
        <v-btn
          color="primary"
          variant="elevated"
          rounded="lg"
          @click="handleSubmit"
          :loading="isSubmitting"
        >
          <span class="d-inline-block px-4">إضافة</span>
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style scoped>
.error-border {
  border: 1px solid #ff5252;
}
.permissions-container {
  border: 1px solid rgba(0, 0, 0, 0.12);
}
/* Square checkbox styles */
:deep(
    .square-checkbox
      .v-selection-control__input
      .v-selection-control__input--density-default
  ) {
  transform: translateX(2px);
}
:deep(
    .square-checkbox .v-selection-control__input .v-selection-control__control
  ) {
  border-radius: 0 !important;
}
:deep(.square-checkbox .v-selection-control__input .mdi-checkbox-marked) {
  border-radius: 0 !important;
}
:deep(.square-checkbox.v-checkbox .v-selection-control) {
  min-height: 32px;
}
</style>
