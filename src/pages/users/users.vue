<script setup>
import { useAppStore } from "@/stores/app";
import axios from "@/plugins/axios";
import { hasPermission } from "@core/utils/permissions";
import UserCard from "@/@core/components/widgets/userCard";
import DeleteObserverConfirmationDialog from "./components/DeleteObserverConfirmationDialog";
import DeleteOperatorConfirmationDialog from "./components/DeleteOperatorConfirmationDialog";
import DeleteConfirmationDialog from "./components/DeleteConfirmationDialog";
import AddNewUserDialog from "./components/AddNewUserDialog.vue";
import UpdateUserDialog from "./components/UpdateUserDialog.vue";
import Magnifier from "@/components/icons/magnifier.vue";
import DataTable from "@/components/dataTable.vue";
import Pagination from "@/@core/components/Pagination.vue";
import CustomeSwitch from "@/components/CustomeSwitch.vue";
import Editicon from "@/components/icons/editicon.vue";
import Trash from "@/components/icons/trash.vue";
import Fileadd from "@/components/icons/fileadd.vue";
import UploadComponent from "@/components/UploadComponent.vue";
import SuccessDialog from "@/components/SuccessDialog.vue";
import DeleteDialog from "@/components/DeleteDialog.vue";
// import UserWidget from "@/@core/components/widgets/UserWidget.vue";
const headers = [
  { title: "رقم الهوية", value: "identityNumber", align: "start" },
  { title: "الاسم", value: "name" },
  { title: "الدور", value: "role" },
  { title: "رقم الجوال", value: "phone" },
  { title: "الحالة", value: "status" },
  { title: "", value: "actions" },
];
const users = ref([
  {
    identityNumber: "0987654321",
    name: "مجمد الفحطاني عبدالله",
    role: "مشرف ميداني",
    phone: "966 098 8889",
    status: "active",
  },
  {
    identityNumber: "1234567890",
    name: "علي السعيد",
    role: "مشرف ميداني",
    phone: "966 123 4567",
    status: "active",
  },
  {
    identityNumber: "2345678901",
    name: "فاطمة الرهواء",
    role: "مشرف ميداني",
    phone: "966 234 5678",
    status: "inactive",
  },
  {
    identityNumber: "3456789012",
    name: "يوسف الشمري",
    role: "مشرف ميداني",
    phone: "966 345 6789",
    status: "active",
  },
  {
    identityNumber: "4567890123",
    name: "سارة الفحطاني",
    role: "مشرف ميداني",
    phone: "966 456 7890",
    status: "inactive",
  },
  {
    identityNumber: "5678901234",
    name: "أحمد العلي",
    role: "مشرف ميداني",
    phone: "966 567 8901",
    status: "active",
  },
  {
    identityNumber: "6789012345",
    name: "مع الفرجان",
    role: "مشرف ميداني",
    phone: "966 678 9012",
    status: "active",
  },
  {
    identityNumber: "7890123456",
    name: "طارق اجلهي",
    role: "مشرف ميداني",
    phone: "966 789 0123",
    status: "inactive",
  },
]);
const appStore = useAppStore();
const isLoading = ref(false);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");

// Data for users

const totalUsers = ref(0);
const userDialog = ref(false);
const isUpdatingDialog = ref(false);
const userIdToDelete = ref(null);
const filterDialog = ref(false);
const isAddDialogVisible = ref(false);
const uploadDialog = ref(false);
const successDialogOpen = ref(false);
const deleteDialogOpen = ref(false);
const deleteDialog = ref(false);
const isDeletingObserver = ref(false);
const isDeletingOperator = ref(false);
const isDeleting = ref(false);

const formData = ref({
  name: "",
  email: "",
  phone: "",
  status: "",
  role: "",
});
// Filter form data
const filterForm = ref({
  role_id: null,
  district: null,
});

// Available roles for selection
const roles = ref([
  { id: 1, name: "مدير النظام" },
  { id: 4, name: "المشرف العام" },
  { id: 2, name: "المشرف" },
  { id: 3, name: "المراقب" },
]);

// Available districts
const districts = [
  "العزيزية",
  "الشوقية",
  "النوارية",
  "السفلة",
  "بطحاء قريش",
  "العوالي",
  "الرصيفة",
  "الزاهر",
];

const fileToUpload = ref(null);
const uploadError = ref({
  message: "فشل التحميل",
});
const handleFileSelection = (file) => {
  console.log(file);
  fileToUpload.value = file;
};
const handleUploadFile = () => {
  isAddDialogVisible.value = false;
  successDialogOpen.value = true;
};
const currentUserId = ref(null);
const handleUpdateUserDialog = (userId) => {
  currentUserId.value = userId;
  isUpdatingDialog.value = true;
};

// Fetch users with pagination and search
const fetchUsers = async () => {
  try {
    isLoading.value = true;
    const queryParams = new URLSearchParams({
      page: currentPage.value.toString(),
      per_page: perPage.value.toString(),
      search: search.value || "",
      district: filterForm.value.district,
    });

    // Only add role_id to params if it's not null
    if (filterForm.value.role_id !== null) {
      queryParams.append("role_id", filterForm.value.role_id.toString());
    }

    const response = await axios.get(`/users?${queryParams.toString()}`);
    if (response.data?.data) {
      users.value = response.data.data;
      // Update pagination data
      if (response.data.pagination) {
        totalUsers.value = response.data.pagination.i_total_objects;
        perPage.value = response.data.pagination.i_per_page;
        currentPage.value = response.data.pagination.i_current_page;
      }
    }
  } catch (error) {
    console.error("Error fetching users:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    users.value = [];
    totalUsers.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Delete user confirmation
const confirmDelete = (user) => {
  userIdToDelete.value = user.id;
  userIdToDelete.value = user.id;

  // delete observer => role.id == 3
  if (user.role?.id == 3) {
    isDeletingObserver.value = true;
    return;
  }
  // delete Operator (supervisor) => role.id == 2
  if (user.role?.id == 2) {
    isDeletingOperator.value = true;
    return;
  }

  deleteDialog.value = true;
};

// Delete user
const deleteUser = async (newSupervisorId = null) => {
  if (!userIdToDelete.value) return;

  try {
    isDeleting.value = true;
    const response = await axios.delete(`/users/${userIdToDelete.value}`, {
      data: newSupervisorId
        ? {
            new_supervisor_id: newSupervisorId,
          }
        : {},
    });

    if (response.data) {
      appStore.showSnackbar({
        message: "تم حذف المستخدم بنجاح",
        color: "success",
      });
      closeDeletingDialog();
      await fetchUsers();
    }
  } catch (error) {
    console.error("Error deleting user:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حذف المستخدم",
      color: "error",
    });
  } finally {
    isDeleting.value = false;
  }
};

const closeDeletingDialog = () => {
  isDeletingObserver.value = false;
  isDeletingOperator.value = false;
  deleteDialog.value = false;

  userIdToDelete.value = null;
};

// Watch for changes in search with debounce
const debouncedSearch = ref(null);
watch(search, (newValue) => {
  if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
  debouncedSearch.value = setTimeout(() => {
    currentPage.value = 1;
    fetchUsers();
  }, 500);
});

// Watch for page changes
watch(currentPage, () => {
  fetchUsers();
});

// Add watcher for perPage changes
watch(perPage, () => {
  currentPage.value = 1; // Reset to first page when changing page size
  fetchUsers();
});

// Add applyFilter function before the onMounted
const applyFilter = async () => {
  filterDialog.value = false;
  currentPage.value = 1;
  await fetchUsers();
};

// Add reset filter function
const resetFilter = () => {
  filterForm.value = {
    role_id: null,
    district: "العزيزية",
  };
  filterDialog.value = false;
  fetchUsers();
};

const handleUserAdded = () => {
  userDialog.value = false;
  fetchUsers();
};

const handleUserUpdated = () => {
  isUpdatingDialog.value = false;
  currentUserId.value = null;
  fetchUsers();
};

onMounted(() => {
  // fetchUsers();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-0">
      <div class="mb-2">
        <v-breadcrumbs
          divider=">"
          :items="[
            {
              title: 'الرئيسية',
              disabled: false,
              to: '/',
            },
            {
              title: 'إدارة المستخدمين',
              disabled: true,
            },
          ]"
        >
          <template v-slot:prepend>
            <svg
              width="20"
              height="20"
              viewBox="0 0 20 20"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M5.62519 16.0419H14.3752C15.2957 16.0419 16.0419 15.2957 16.0419 14.3752V8.12519L10.0002 3.95853L3.95853 8.12519V14.3752C3.95853 15.2957 4.70472 16.0419 5.62519 16.0419Z"
                stroke="#25343E"
                stroke-width="1.5"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
              <path
                d="M8.12468 13.1244C8.12468 12.2039 8.87088 11.4577 9.79135 11.4577H10.208C11.1285 11.4577 11.8747 12.2039 11.8747 13.1244V16.0411H8.12468V13.1244Z"
                stroke="#25343E"
                stroke-width="1.5"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
          </template>
        </v-breadcrumbs>
      </div>

      <VCard class="rounded-lg">
        <VCardText>
          <!-- Header Section -->
          <div
            class="d-flex flex-wrap align-center justify-space-between mb-6 ga-4"
          >
            <h2 class="text-h5 font-weight-bold">إدارة المستخدمين</h2>

            <!-- Search -->
            <div
              class="d-flex flex-wrap align-center justify-md-end ga-2"
              style="flex-grow: 1; min-width: 0"
            >
              <v-text-field
                v-model="searchQuery"
                placeholder="بحث برقم الهوية او الإسم"
                rounded="lg"
                clearable
                height="48"
                class="bg-primary_2 border-none flex-grow-1"
                :style="{
                  'min-width': '200px',
                  'max-width': '300px',
                  'margin-inline-end': '12px',
                }"
                density="compact"
                hide-details
                @update:model-value="handleSearch"
              >
                <template v-slot:prepend-inner>
                  <Magnifier />
                </template>
              </v-text-field>

              <!-- Add Company Button -->
              <div class="d-flex flex-wrap ga-2" style="min-width: fit-content">
                <VBtn
                  height="48"
                  elevation="0"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  :prepend-icon="Fileadd"
                  color="primary_2"
                  @click="uploadDialog = true"
                >
                  إضافة ملف المستخدمين
                </VBtn>
                <VBtn
                  height="48"
                  elevation="0"
                  @click="isAddDialogVisible = true"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                >
                  إضافة مستخدم جديد
                </VBtn>
              </div>
            </div>
          </div>

          <v-row>
            <v-col cols="12">
              <DataTable
                :headers="headers"
                :items="users"
                :loading="isLoading"
                class="elevation-0"
                :items-per-page="perPage"
                v-model:page="currentPage"
                :hide-default-footer="true"
              >
                <template #[`item.status`]="{ item }">
                  <CustomeSwitch
                    :_key="item.identityNumber"
                    v-model="item.status"
                  />
                </template>

                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn
                      size="small"
                      class="text-black text-sm rounded-lg"
                      color="#fff"
                      :prepend-icon="Editicon"
                    >
                      تعديل
                    </v-btn>
                    <v-btn
                      size="small"
                      class="text-error text-sm rounded-lg"
                      color="#fff"
                      :prepend-icon="Trash"
                      @click="deleteDialogOpen = true"
                    >
                      حذف
                    </v-btn>
                  </div>
                </template>
              </DataTable>

              <!-- Pagination -->
              <Pagination
                :total-items="totalItems"
                v-model:per-page="perPage"
                v-model:page="currentPage"
              />
            </v-col>
          </v-row>
        </VCardText>
      </VCard>
    </v-container>
    <!-- Add Company Dialog -->
    <VDialog v-model="isAddDialogVisible" max-width="600px">
      <VCard rounded="lg">
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">إضافة مستخدم</h5>
              <p class="text-sm text-secondary mb-0">
                يرجى تعبئة بيانات المستخدم بدقة. الحقول الإلزامية محددة بعلامة
                (*).
              </p>
            </div>
          </div>
        </VCardTitle>
        <VCardText>
          <VForm ref="form">
            <VRow>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">اسم الكامل *</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.name"
                  placeholder="الإسم الكامل الرسمى"
                  required
                />
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">البريد الإلكتروني*</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.email"
                  placeholder="البريد الإلكتروني"
                  type="email"
                  required
                />
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">رقم الجوال*</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.phone"
                  placeholder="رقم الجوال"
                  required
                />
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">الحالة *</label>
                <v-select
                  rounded="lg"
                  v-model="formData.status"
                  label="اختيار"
                  required
                ></v-select>
              </VCol>

              <VCol cols="12">
                <label for="" class="mb-2 d-block">الدور</label>
                <v-select
                  rounded="lg"
                  v-model="formData.role"
                  label="اختيار"
                  required
                ></v-select>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>

        <VCardActions
          class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between"
        >
          <VBtn
            variant="outlined"
            class="text-black rounded-xl bg-white"
            @click="isAddDialogVisible = false"
          >
            إلغاء
          </VBtn>
          <VBtn
            color="primary"
            variant="flat"
            class="rounded-xl px-8"
            @click="handleUploadFile"
          >
            إضافة
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>

    <!-- upload company file Dialog -->
    <VDialog v-model="uploadDialog" max-width="700px">
      <VCard rounded="lg" class="rounded-lg">
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">رفع ملف المستخدمين</h5>
              <p class="text-sm text-secondary">
                يرجى رفع الملفات المطلوبة لإضافة أو تحديث بيانات المستخدمين،
                تأكد أن الملفات تحتوي على المعلومات الكاملة.
              </p>
            </div>
            <button @click="uploadDialog = false">
              <v-icon size="sm">mdi-close</v-icon>
            </button>
          </div>
        </VCardTitle>

        <UploadComponent
          title="ملف المستخدمين"
          btn-text="تحميل نموذج ملف المستخدمين"
          @file-selected="handleFileSelection"
          :upload-error="uploadError"
          @reupload-file="handleFileSelection"
        />

        <VCardActions
          class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between"
        >
          <VBtn
            variant="outlined"
            class="text-black rounded-xl bg-white"
            @click="uploadDialog = false"
          >
            إلغاء
          </VBtn>
          <VBtn
            color="primary"
            variant="flat"
            class="rounded-xl"
            :disabled="!fileToUpload"
          >
            إضافة
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>

    <!-- Success Dialog -->
    <SuccessDialog
      title="تمت إضافة المستخدم بنجاح!"
      subtitle="إعادة التوجه إلى إدارة المستخدمين أو إضافة مستخدم أخرى."
      cancel-text="إضافة مستخدم أخرى"
      action-text="التوجه إلى إدارة المستخدمين"
      v-model="successDialogOpen"
      @cancel="successDialogOpen = false"
      @action="successDialogOpen = false"
    />

    <DeleteDialog
      title="️ هل أنت متأكد من حذف المستخدم؟"
      subtitle="سيؤدي حذف هذه المستخدم إلى إزالة جميع بياناتها من النظام، وقد لا يمكن استرجاعها لاحقًا."
      cancel-text="إلغاء"
      action-text="نعم، حذف المستخدم"
      v-model="deleteDialogOpen"
      @cancel="deleteDialogOpen = false"
      @action="deleteDialogOpen = false"
    />
  </div>
</template>

<style lang="scss">
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.v-text-field {
  :deep(.v-field) {
    border-radius: 8px;
    background-color: rgb(250, 250, 250);

    &.v-field--focused {
      background-color: white;
    }
  }
}

.v-btn {
  text-transform: none;
  letter-spacing: 0;
  border-radius: 8px;
}

.v-radio-group {
  .v-selection-control-group {
    gap: 8px;
  }
}

.v-selection-control {
  &.v-selection-control--dirty {
    .v-selection-control__input {
      border-color: #c5a572 !important;

      &::before {
        opacity: 1 !important;
        background-color: #c5a572 !important;
        transform: scale(0.55) !important;
      }
    }
  }
}

.v-selection-control__input {
  width: 18px !important;
  height: 18px !important;
  border: 2px solid rgba(190, 194, 197, 1) !important;
  border-radius: 50% !important;
  margin: 0 !important;
  padding: 0 !important;
  opacity: 1 !important;
  background: transparent !important;

  &::before {
    opacity: 0 !important;
    background-color: transparent !important;
    transition: all 0.2s ease-in-out !important;
  }
}

.v-label {
  font-size: 14px;
  color: rgba(0, 0, 0, 0.87);
  margin-inline-start: 4px;
}

// Remove ripple effect
:deep(.v-ripple__container) {
  display: none !important;
}

.bg-gray-1 {
  background-color: #f8f8f8;
  border-radius: 8px;
  padding: 16px;
}

.statistics-bar {
  background-color: #f8f8f8;
  border-radius: 8px;
  margin-top: 24px;
}

.v-dialog {
  .v-card {
    border-radius: 12px;
  }

  label {
    font-size: 14px;
    color: rgba(0, 0, 0, 0.87);
  }

  .v-field {
    border-radius: 8px;
    background-color: rgb(250, 250, 250) !important;

    &.v-field--focused {
      background-color: white !important;
    }
  }

  .v-btn {
    height: 48px;
    border-radius: 8px;
    text-transform: none;
    letter-spacing: 0;
    font-weight: 500;
  }
}

.v-dialog > .v-overlay__content {
  overflow-y: auto !important;
  padding-inline-end: 0px !important;
}
</style>
