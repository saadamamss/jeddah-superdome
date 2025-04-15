<template>
  <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
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
            title: 'التنبيهات',
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
      <v-card-title class="pt-4 pb-6">
        <div class="border-b pb-6">
          <div class="d-flex align-center gap-2 mb-6">
            <div class="d-flex align-center justify-space-between w-100">
              <h2 class="text-h6 font-weight-bold">التنبيهات</h2>

              <div class="align-center ga-4 flex-wrap d-flex">
                <v-text-field
                  v-model="search"
                  label="البحث"
                  rounded="lg"
                  clearable
                  height="46"
                  class="bg-white"
                  style="width: 280px; max-width: 100%"
                >
                  <template v-slot:prepend-inner>
                    <svg
                      width="1.3rem"
                      height="1.3rem"
                      viewBox="0 0 20 20"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <path
                        opacity="0.4"
                        d="M16.6666 9.16669C16.6666 5.02455 13.3088 1.66669 9.16663 1.66669C5.02449 1.66669 1.66663 5.02455 1.66663 9.16669C1.66663 13.3088 5.02449 16.6667 9.16663 16.6667C13.3088 16.6667 16.6666 13.3088 16.6666 9.16669Z"
                        fill="#B69265"
                      />
                      <path
                        fill-rule="evenodd"
                        clip-rule="evenodd"
                        d="M9.16663 1.04169C4.67931 1.04169 1.04163 4.67937 1.04163 9.16669C1.04163 13.654 4.67931 17.2917 9.16663 17.2917C11.1853 17.2917 13.032 16.5555 14.453 15.337L17.8914 18.7753C18.1354 19.0194 18.5312 19.0194 18.7752 18.7753C19.0193 18.5312 19.0193 18.1355 18.7752 17.8914L15.3369 14.4531C16.5554 13.0321 17.2916 11.1854 17.2916 9.16669C17.2916 4.67937 13.6539 1.04169 9.16663 1.04169ZM2.29163 9.16669C2.29163 5.36973 5.36967 2.29169 9.16663 2.29169C12.9636 2.29169 16.0416 5.36973 16.0416 9.16669C16.0416 12.9636 12.9636 16.0417 9.16663 16.0417C5.36967 16.0417 2.29163 12.9636 2.29163 9.16669Z"
                        fill="#B69265"
                      />
                    </svg>
                  </template>
                </v-text-field>

                <VBtn
                  prepend-icon="tabler-plus"
                  color="primary"
                  @click="dialog = true"
                  class="rounded-lg"
                >
                  إرسال تنبيه
                </VBtn>
              </div>
            </div>
          </div>
        </div>
      </v-card-title>

      <v-card elevation="0" rounded="lg">
        <dataTable
          v-if="notifications.length"
          :items="notifications"
          :loading="isLoading"
          :page="currentPage"
          :items-per-page="perPage"
          :total-items="totalRecords"
          :headers="tableHeaders"
          @update:options="handleTableUpdate"
        >
          <template #[`item.title`]="{ item }">
            <div class="d-flex align-center">
              <svg
                width="33"
                height="32"
                viewBox="0 0 33 32"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M12.4997 22.333C12.4997 22.333 12.4997 25.6663 16.4997 25.6663C20.4997 25.6663 20.4997 22.333 20.4997 22.333M23.4997 13.333V15.9997L26.1663 21.6663H6.83301L9.49967 15.9997V13.333C9.49967 9.46701 12.6337 6.33301 16.4997 6.33301C20.3657 6.33301 23.4997 9.46701 23.4997 13.333Z"
                  stroke="#C4AB79"
                  stroke-width="2.13333"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
              </svg>
              <span>{{ item.title }}</span>
            </div>
          </template>

          <template #[`item.admin`]="{ item }">
            <div class="d-flex align-center gap-2">
              <v-avatar size="32" color="grey-lighten-3">
                <v-img
                  v-if="item.admin?.avatar_url"
                  :src="item.admin.avatar_url"
                  :alt="item.admin.firstname"
                />
                <span v-else class="text-subtitle-2">
                  {{ getInitials(item.admin?.firstname, item.admin?.lastname) }}
                </span>
              </v-avatar>
              <div class="d-flex flex-column">
                <span class="text-subtitle-2 font-weight-medium">
                  {{ item.admin?.firstname }} {{ item.admin?.lastname }}
                </span>
                <span class="text-caption text-medium-emphasis">
                  {{ getRoleTitle(item.admin?.role?.title) }}
                </span>
              </div>
            </div>
          </template>

          <template #[`item.users`]="{ item }">
            <div class="d-flex align-center gap-2">
              <v-avatar size="32" color="grey-lighten-3" class="elevation-1">
                <v-img
                  v-if="item.users[0]?.avatar_url"
                  :src="item.users[0].avatar_url"
                  :alt="item.users[0].firstname"
                />
                <span v-else class="text-subtitle-2">
                  {{
                    getInitials(
                      item.users[0]?.firstname,
                      item.users[0]?.lastname
                    )
                  }}
                </span>
              </v-avatar>
              <span class="text-subtitle-2"
                >{{ item.users[0]?.firstname }}
                {{ item.users[0]?.lastname }}</span
              >
            </div>
          </template>

          <template #[`item.body`]="{ item }">
            <div class="text-truncate" style="max-width: 250px">
              {{ item.body }}
            </div>
          </template>

          <template #[`item.created_at`]="{ item }">
            <div class="d-flex align-center text-medium-emphasis">
              <span class="text-body-2">{{ formatDate(item.created_at) }}</span>
            </div>
          </template>

          <template #[`item.actions`]="{ item }">
            <v-menu location="bottom end">
              <template v-slot:activator="{ props }">
                <v-btn icon variant="text" size="small" v-bind="props">
                  <v-icon>tabler-dots-vertical</v-icon>
                </v-btn>
              </template>

              <v-list density="compact" class="rounded-lg" min-width="150">
                <v-list-item
                  prepend-icon="tabler-eye"
                  title="عرض"
                  @click="viewNotification(item)"
                />
                <v-list-item
                  prepend-icon="tabler-edit"
                  title="تعديل"
                  @click="editNotification(item)"
                />
                <v-list-item
                  prepend-icon="tabler-trash"
                  title="حذف"
                  color="error"
                  @click="openDeleteDialog(item)"
                />
              </v-list>
            </v-menu>
          </template>

          <template #pagination>
            <div class="d-flex align-center justify-space-between px-4 py-2">
              <span class="text-caption text-medium-emphasis">
                تم عرض {{ notifications.length }} من {{ totalRecords }} تنبيه
              </span>
              <Pagination
                v-model:page="currentPage"
                v-model:per-page="perPage"
                :total-items="totalRecords"
                :total-pages="totalPages"
                @update:page="handleTableUpdate"
              />
            </div>
          </template>
        </dataTable>

        <div
          v-else-if="!isLoading"
          class="d-flex flex-column align-center justify-center py-16"
        >
          <div class="text-h6 font-weight-medium mt-6 text-center">
            لا توجد تنبيهات
          </div>
          <div class="text-body-1 text-medium-emphasis text-center mt-2">
            يمكنك إرسال تنبيه جديد عن طريق الضغط على زر "إرسال تنبيه"
          </div>

          <VBtn
            color="primary"
            class="mt-6"
            prepend-icon="tabler-plus"
            @click="dialog = true"
          >
            إرسال تنبيه
          </VBtn>
        </div>
      </v-card>
    </VCard>

    <!-- Send Notification Dialog -->
    <VDialog v-model="dialog" max-width="600" persistent>
      <VCard>
        <VCardTitle class="d-flex justify-space-between pa-4">
          <span>إرسال تنبيه جديد</span>
          <VBtn icon variant="text" @click="closeDialog">
            <VIcon icon="tabler-x" />
          </VBtn>
        </VCardTitle>
        <VDivider />
        <VCardText class="pa-4">
          <VForm ref="form" @submit.prevent="handleSubmit" v-model="isValid">
            <VRow>
              <VCol cols="12">
                <VSelect
                  v-model="selectedUsers"
                  :items="users"
                  item-title="fullName"
                  item-value="id"
                  label="المستخدمين"
                  multiple
                  chips
                  :rules="[
                    (v) =>
                      (!!v && v.length > 0) ||
                      'يرجى اختيار مستخدم واحد على الأقل',
                  ]"
                >
                  <template v-slot:selection="{ item }">
                    <v-chip
                      class="ma-1"
                      closable
                      @click:close="removeUser(item.value)"
                    >
                      <template v-slot:prepend>
                        <v-avatar size="24" color="primary" class="text-white">
                          <v-img
                            v-if="item.raw.personal_image"
                            :src="item.raw.personal_image"
                            :alt="item.raw.firstname"
                            @error="onImageError"
                          >
                            <template v-slot:error>
                              <span class="text-caption">
                                {{
                                  getInitials(
                                    item.raw.firstname,
                                    item.raw.lastname
                                  )
                                }}
                              </span>
                            </template>
                          </v-img>
                          <span v-else class="text-caption">
                            {{
                              getInitials(item.raw.firstname, item.raw.lastname)
                            }}
                          </span>
                        </v-avatar>
                      </template>
                      {{ item.raw.firstname }} {{ item.raw.lastname }}
                      <v-icon size="16" class="ms-2" color="primary">
                        {{ getRoleIcon(item.raw.role?.title) }}
                      </v-icon>
                    </v-chip>
                  </template>
                  <template v-slot:item="{ item, props }">
                    <v-list-item
                      v-bind="props"
                      :title="item.raw.firstname + ' ' + item.raw.lastname"
                      :subtitle="getRoleTitle(item.raw.role?.title)"
                    >
                      <template v-slot:prepend>
                        <v-avatar size="32" color="primary" class="text-white">
                          <v-img
                            v-if="item.raw.personal_image"
                            :src="item.raw.personal_image"
                            :alt="item.raw.firstname"
                            @error="onImageError"
                          >
                            <template v-slot:error>
                              <span class="text-subtitle-2">
                                {{
                                  getInitials(
                                    item.raw.firstname,
                                    item.raw.lastname
                                  )
                                }}
                              </span>
                            </template>
                          </v-img>
                          <span v-else class="text-subtitle-2">
                            {{
                              getInitials(item.raw.firstname, item.raw.lastname)
                            }}
                          </span>
                        </v-avatar>
                      </template>
                    </v-list-item>
                  </template>
                </VSelect>
              </VCol>
              <VCol cols="12">
                <VTextField
                  v-model="newNotification.title"
                  label="عنوان التنبيه"
                  :rules="[(v) => !!v || 'عنوان التنبيه مطلوب']"
                />
              </VCol>
              <VCol cols="12">
                <VTextarea
                  v-model="newNotification.body"
                  label="نص التنبيه"
                  :rules="[(v) => !!v || 'نص التنبيه مطلوب']"
                />
              </VCol>
            </VRow>
            <div class="d-flex gap-4 justify-end mt-4">
              <VBtn color="secondary" variant="outlined" @click="closeDialog">
                إلغاء
              </VBtn>
              <VBtn
                color="primary"
                type="submit"
                :loading="isSending"
                :disabled="!isValid || isSending"
              >
                إرسال
              </VBtn>
            </div>
          </VForm>
        </VCardText>
      </VCard>
    </VDialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400">
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
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import axios from "@/plugins/axios";
import { useAppStore } from "@/stores/app";
import dayjs from "dayjs";
import Pagination from "@/@core/components/Pagination";
import dataTable from "@/components/dataTable.vue";
import { useDebounceFn } from "@vueuse/core";

const appStore = useAppStore();
const isLoading = ref(false);
const isSending = ref(false);
const form = ref(null);
const isValid = ref(false);

// Data
const notifications = ref([]);
const users = ref([]);
const search = ref("");
const currentPage = ref(1);
const perPage = ref(6);
const dialog = ref(false);
const selectedUsers = ref([]);
const newNotification = ref({
  title: "",
  body: "",
});
const deleteDialog = ref(false);
const isDeleting = ref(false);
const selectedNotification = ref(null);
const totalRecords = ref(0);
const totalPages = ref(1);

const tableHeaders = [
  { title: "عنوان التنبيه", key: "title", sortable: true },
  { title: "وصف التنبيه", key: "body", sortable: true },
  { title: "مرسل إلى", key: "users", sortable: false },
  { title: "تاريخ الارسال", key: "created_at", sortable: true },
  { title: "الإجراءات", key: "actions", sortable: false, align: "center" },
];

// Computed
const filteredData = computed(() => {
  return notifications.value.filter((item) => {
    return (
      search.value === "" ||
      item.title.toLowerCase().includes(search.value.toLowerCase()) ||
      item.body.toLowerCase().includes(search.value.toLowerCase())
    );
  });
});

const pageCount = computed(() => Math.ceil(totalRecords.value / perPage.value));

const from = computed(() => (currentPage.value - 1) * perPage.value + 1);
const to = computed(() =>
  Math.min(currentPage.value * perPage.value, totalRecords.value)
);

const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * perPage.value;
  const end = start + perPage.value;
  return filteredData.value.slice(start, end);
});

// Format date using dayjs
const formatDate = (date) => {
  if (!date) return "";
  return dayjs(date).format("YYYY-MM-DD HH:mm");
};

// Methods
const fetchNotifications = async () => {
  try {
    isLoading.value = true;
    const params = {
      page: currentPage.value,
      per_page: perPage.value,
      search: search.value || undefined,
    };

    const response = await axios.get("/notifications", { params });
    if (response.data?.status?.success) {
      notifications.value = response.data.data;
      totalRecords.value = response.data.pagination.i_total_objects;
      totalPages.value = response.data.pagination.i_total_pages;
      currentPage.value = response.data.pagination.i_current_page;
      perPage.value = response.data.pagination.i_per_page;
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
  fetchNotifications();
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

const viewNotification = (item) => {
  // Implement view action
  console.log("View notification:", item);
};

const editNotification = (item) => {
  // Implement edit action
  console.log("Edit notification:", item);
};

const onImageError = (event) => {
  // The error template will automatically show instead
  console.log("Image failed to load:", event);
};

// Update search handler with debounce
const debouncedSearch = useDebounceFn(() => {
  currentPage.value = 1; // Reset to first page on search
  fetchNotifications();
}, 300);

// Watch for search changes
watch(search, () => {
  debouncedSearch();
});

// Lifecycle
onMounted(async () => {
  await Promise.all([fetchNotifications(), fetchUsers()]);
});
</script>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

:deep(.v-data-table) {
  .v-table {
    background: transparent;
  }

  .v-data-table-footer {
    background: transparent;
  }

  .v-data-table__tr {
    margin-bottom: 16px;
    filter: drop-shadow(0px 0px 8px rgba(0, 0, 0, 0.05));
    border-radius: 8px;
  }

  .v-data-table__td {
    background: white !important;
    height: 72px !important;

    &:first-child {
      border-top-right-radius: 8px;
      border-bottom-right-radius: 8px;
    }

    &:last-child {
      border-top-left-radius: 8px;
      border-bottom-left-radius: 8px;
    }
  }
}

:deep(.v-table > .v-table__wrapper > table) {
  border-spacing: 0 16px !important;
  border-collapse: separate !important;
}

:deep(.v-list-item) {
  min-height: 40px;
}

:deep(.v-list) {
  background-color: white;
  box-shadow: 0px 4px 16px rgba(0, 0, 0, 0.08);
}
</style>
