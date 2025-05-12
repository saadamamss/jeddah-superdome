<script setup>
import axios from "@/plugins/axios";
import { useAppStore } from "@/stores/app";
import { format } from "@core/utils/formatters";
import { useDebounceFn } from "@vueuse/core";
import { hasPermission } from "@core/utils/permissions";
import DataTable from "@/components/dataTable.vue";
import Pagination from "@/@core/components/Pagination";
import Repeaticon from "@/components/icons/repeaticon.vue";
import SuccessDialog from "@/components/SuccessDialog.vue";

const appStore = useAppStore();
const isLoading = ref(false);
const isSending = ref(false);
const form = ref(null);
const isValid = ref(false);

// Data
const users = ref([]);
const search = ref("");
const currentPage = ref(1);
const perPage = ref(6);
const dialog = ref(false);
const successDialogOpen = ref(false);
const selectedUsers = ref([]);
const newNotification = ref({
  title: "",
  body: "",
  target: "",
  type: "",
  method: "",
});
const deleteDialog = ref(false);
const isDeleting = ref(false);
const selectedNotification = ref(null);
const totalRecords = ref(0);

const tableHeaders = [
  { title: "عنوان الإشعار", value: "notificationTitle" },
  { title: "نص الإشعار", value: "notificationText" },
  { title: "الفئة المستهدفة", value: "targetCategory" },
  { title: "نوع الإشعار", value: "notificationType" },
  { title: "وسيلة الإرسال", value: "transmissionMethod" },
  { title: "", value: "actions" },
];
const notifications = [
  {
    transmissionMethod: "بريد إلكترونى",
    notificationType: "إشعار داخل النظام",
    targetCategory: "تذكير",
    notificationText:
      "نود إعلامكم بأنه تم تعديل مواعيد بعض الرحلات، الرجاء مراجعة جدول الرحلات المحد...",
    notificationTitle: "تحديث مهم بخصوص مواعيد الرحلات",
  },
  {
    transmissionMethod: "إشعار داخل النظام",
    notificationType: "إشعار داخل النظام",
    targetCategory: "شركات",
    notificationText:
      "نود إعلامكم بأنه تم تعديل مواعيد بعض الرحلات، الرجاء مراجعة جدول الرحلات المحد...",
    notificationTitle: "تحديث مهم بخصوص مواعيد الرحلات",
  },
  {
    transmissionMethod: "بريد إلكترونى",
    notificationType: "بريد إلكتروني",
    targetCategory: "إعلان",
    notificationText:
      "نود إعلامكم بأنه تم تعديل مواعيد بعض الرحلات، الرجاء مراجعة جدول الرحلات المحد...",
    notificationTitle: "تحديث مهم بخصوص مواعيد الرحلات",
  },
  {
    transmissionMethod: "إشعار داخل النظام",
    notificationType: "إشعار داخل النظام",
    targetCategory: "حجاج",
    notificationText:
      "نود إعلامكم بأنه تم تعديل مواعيد بعض الرحلات، الرجاء مراجعة جدول الرحلات المحد...",
    notificationTitle: "تحديث مهم بخصوص مواعيد الرحلات",
  },
  {
    transmissionMethod: "بريد إلكترونى",
    notificationType: "كلاهما",
    targetCategory: "تذكير",
    notificationText:
      "نود إعلامكم بأنه تم تعديل مواعيد بعض الرحلات، الرجاء مراجعة جدول الرحلات المحد...",
    notificationTitle: "تحديث مهم بخصوص مواعيد الرحلات",
  },
  // Add more entries as needed
];

const formatReceiverNames = (receivers) => {
  if (!receivers || receivers.length === 0) return "لا يوجد مستلمين";
  return receivers
    .map((user) => `${user.firstname} ${user.lastname}`)
    .join(", ");
};

const pagination = reactive({
  per_page: 12,
  page: 1,
  total: 0,
  items_on_page: 0,
});

watch(
  () => [pagination.page, pagination.per_page],
  () => {
    fetchNotifications();
  }
);

// Methods
const fetchNotifications = async () => {
  try {
    isLoading.value = true;
    const params = {
      page: pagination.page,
      per_page: pagination.per_page,
      search: search.value || undefined,
    };

    const response = await axios.get("/notifications", { params });
    if (response.data?.status?.success) {
      notifications.value = response.data.data;
      const itemPagination = response.data?.pagination || {};

      pagination.page = itemPagination.i_current_page;
      pagination.per_page = itemPagination.i_per_page;
      pagination.total = itemPagination.i_total_objects;
      pagination.items_on_page = itemPagination.items_on_page;
    }
  } catch (error) {
    console.error("Error fetching notifications:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

const fetchUsers = async () => {
  try {
    const response = await axios.get("/users");
    if (response.data?.status?.success) {
      users.value = response.data.data.map((user) => ({
        ...user,
        fullName: `${user.firstname} ${user.lastname}`,
      }));
    }
  } catch (error) {
    console.error("Error fetching users:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء جلب المستخدمين",
      color: "error",
    });
  }
};
const handleUploadFile = () => {
  dialog.value = false;
  successDialogOpen.value = true;
};
const closeDialog = () => {
  dialog.value = false;
  selectedUsers.value = [];
  newNotification.value = { title: "", body: "" };
  if (form.value) {
    form.value.reset();
  }
};

const handleSubmit = async () => {
  const { valid } = await form.value.validate();

  if (!valid) return;

  try {
    isSending.value = true;
    const response = await axios.post("/notifications/send", {
      user_ids: selectedUsers.value,
      title: newNotification.value.title,
      body: newNotification.value.body,
    });

    if (response.data?.status?.success) {
      appStore.showSnackbar({
        message: "تم إرسال التنبيه بنجاح",
        color: "success",
      });
      closeDialog();
      await fetchNotifications();
    }
  } catch (error) {
    console.error("Error sending notification:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء إرسال التنبيه",
      color: "error",
    });
  } finally {
    isSending.value = false;
  }
};

const deleteNotification = async (id) => {
  try {
    const response = await axios.delete(`/notifications/${id}`);
    if (response.data?.status?.success) {
      appStore.showSnackbar({
        message: "تم حذف التنبيه بنجاح",
        color: "success",
      });
      await fetchNotifications();
    }
  } catch (error) {
    console.error("Error deleting notification:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حذف التنبيه",
      color: "error",
    });
  }
};

const handleTableUpdate = (options) => {
  if (options?.page) {
    currentPage.value = options.page;
  }
  if (options?.itemsPerPage) {
    perPage.value = options.itemsPerPage;
  }
  debouncedSearch();
};

const getRoleIcon = (role) => {
  switch (role) {
    case "SUPERVISOR":
      return "tabler-user-shield";
    case "INSPECTOR":
      return "tabler-user-check";
    default:
      return "tabler-user";
  }
};

const getRoleTitle = (role) => {
  switch (role) {
    case "SUPERVISOR":
      return "مشرف";
    case "INSPECTOR":
      return "مراقب";
    default:
      return "";
  }
};

const getInitials = (firstName, lastName) => {
  return `${firstName?.charAt(0) || ""}${lastName?.charAt(0) || ""}`;
};

const removeUser = (userId) => {
  selectedUsers.value = selectedUsers.value.filter((id) => id !== userId);
};

const openDeleteDialog = (item) => {
  selectedNotification.value = item;
  deleteDialog.value = true;
};

const confirmDelete = async () => {
  if (!selectedNotification.value) return;

  try {
    isDeleting.value = true;
    await deleteNotification(selectedNotification.value.id);
    deleteDialog.value = false;
    selectedNotification.value = null;
  } finally {
    isDeleting.value = false;
  }
};

// Update search handler with debounce
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1; // Reset to first page on search
  fetchNotifications();
}, 500);

// Watch for search changes
watch(search, () => {
  debouncedSearch();
});

// Lifecycle
onMounted(async () => {
  // await Promise.all([fetchNotifications(), fetchUsers()]);
});
</script>

<template>
  <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2 pt-0">
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
            title: 'إدارة الإشعارات',
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
          </svg>
        </template>
      </v-breadcrumbs>
    </div>

    <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
      <v-card-title class="pt-4 pb-2">
        <div class="">
          <div class="d-flex align-center gap-2 mb-2">
            <div class="d-flex align-center justify-space-between w-100">
              <h2 class="text-h6 font-weight-bold">إدارة الإشعارات</h2>

              <VBtn
                height="48"
                color="primary"
                @click="dialog = true"
                class="rounded-lg"
              >
                إرسال إشعار جديد
              </VBtn>
            </div>
          </div>
        </div>
      </v-card-title>

      <v-row v-if="isLoading">
        <v-col v-for="item in 9" :key="item" cols="6" sm="6" lg="4">
          <v-card elevation="0">
            <v-card-text class="px-1 py-2">
              <v-skeleton-loader
                class="mx-auto"
                elevation="0"
                max-width="400"
                type="list-item-two-line, table-heading"
              ></v-skeleton-loader>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
      <v-card v-else elevation="0" rounded="lg">
        <DataTable
          :items="notifications"
          :loading="isLoading"
          :page="currentPage"
          :items-per-page="perPage"
          :total-items="totalRecords"
          :headers="tableHeaders"
          class="notifications"
        >
        <template #[`item.notificationTitle`]="{ item }">
          <p style="white-space: wrap">
            {{ item.notificationTitle }}
          </p>
        </template>
          <template #[`item.notificationText`]="{ item }">
            <p style="white-space: wrap">
              {{ item.notificationText }}
            </p>
          </template>
          <template #[`item.actions`]="{ item }">
            <v-btn :prepend-icon="Repeaticon" color="white" @click="">
              إعادة الإرسال
            </v-btn>
          </template>
        </DataTable>
        <Pagination
          :total-items="pagination.total"
          v-model:per-page="pagination.per_page"
          v-model:page="pagination.page"
        />

        <div
          v-if="!isLoading && !notifications.length"
          class="d-flex flex-column align-center justify-center py-16"
        >
          <div class="text-h6 font-weight-medium mt-6 text-center">
            لا توجد تنبيهات
          </div>
          <div class="text-body-1 text-medium-emphasis text-center mt-2">
            يمكنك إرسال تنبيه جديد عن طريق الضغط على زر "إرسال تنبيه"
          </div>

          <VBtn color="primary" class="mt-6" @click="dialog = true">
            إرسال تنبيه
          </VBtn>
        </div>
      </v-card>
    </VCard>

    <!-- Send Notification Dialog -->
    <VDialog v-model="dialog" max-width="600" persistent>
      <VCard>
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">إرسال إشعار جديد</h5>
              <p class="text-sm text-secondary mb-0">
                إرسال إشعار فوري لمستخدم واحد أو مجموعة مستخدمين لإبلاغهم
                بمعلومة مهمة أو إجراء عاجل.
              </p>
            </div>
            <button @click="dialog = false">
              <v-icon size="sm">mdi-close</v-icon>
            </button>
          </div>
        </VCardTitle>
        <VDivider />
        <VForm ref="form" @submit.prevent="handleSubmit" v-model="isValid">
          <VCardText class="pa-4">
            <VRow>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">عنوان الإشعار*</label>
                <VTextField
                  v-model="newNotification.title"
                  placeholder="عنوان الإشعار"
                  :rules="[(v) => !!v || 'عنوان الإشعار مطلوب']"
                />
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">نص الإشعار*</label>
                <VTextarea
                  v-model="newNotification.body"
                  placeholder="نص الإشعار"
                  :rules="[(v) => !!v || 'نص الإشعار مطلوب']"
                />
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">الفئة المستهدفة*</label>
                <v-select
                  v-model="newNotification.target"
                  :items="[]"
                  label="الفئة المستهدفة"
                  :rules="[(v) => !!v || 'الفئة المستهدفة مطلوب']"
                ></v-select>
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">نوع الإشعار*</label>
                <v-select
                  v-model="newNotification.type"
                  :items="[]"
                  placeholder="نوع الإشعار"
                  :rules="[(v) => !!v || 'نوع الإشعار مطلوب']"
                ></v-select>
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">وسيلة الإرسال*</label>
                <v-select
                  v-model="newNotification.method"
                  :items="[]"
                  placeholder="وسيلة الإرسال"
                  :rules="[(v) => !!v || 'وسيلة الإرسال مطلوب']"
                ></v-select>
              </VCol>
            </VRow>
          </VCardText>

          <VCardActions
            class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between"
          >
            <VBtn
              variant="outlined"
              class="text-black rounded-xl bg-white"
              @click="dialog = false"
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
        </VForm>
      </VCard>
    </VDialog>

    <SuccessDialog
      title="تمت إرسال الإشعار بنجاح!"
      subtitle="إعادة التوجه إلى إدارة الإشعارات أو أرسال إشعار آخر."
      cancel-text="إرسال إشعار آخر"
      action-text="التوجه إلى إدارة الإشعارات"
      v-model="successDialogOpen"
      @cancel="successDialogOpen = false"
      @action="successDialogOpen = false"
    />
    <!-- Delete Confirmation Dialog -->
    <!-- <v-dialog v-model="deleteDialog" max-width="400">
      <v-card>
        <v-card-title class="text-center pa-4"> تأكيد الحذف </v-card-title>
        <v-card-text class="text-center pa-4">
          هل أنت متأكد من حذف هذا التنبيه؟
        </v-card-text>
        <v-card-actions class="d-flex justify-center pa-4 gap-4">
          <v-btn
            color="primary"
            variant="outlined"
            @click="deleteDialog = false"
          >
            إلغاء
          </v-btn>
          <v-btn color="error" :loading="isDeleting" @click="confirmDelete">
            حذف
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    -->
  </v-container>
</template>

<style lang="scss" scoped></style>
