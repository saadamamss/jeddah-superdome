<script setup>
import RoleCard from "./components/RoleCard.vue";
import AddNewRoleDialog from "./components/AddNewRoleDialog.vue";
import EditRoleDialog from "./components/EditRoleDialog.vue";
import DeleteRoleConfirmationDialog from "./components/DeleteRoleConfirmationDialog.vue";
import { useAppStore } from "@/stores/app";
import { ref, onMounted, watch } from "vue";
import axios from "@/plugins/axios";
import { hasPermission } from "@core/utils/permissions";

const appStore = useAppStore();
const isLoading = ref(true);
const isAddingDialog = ref(false);
const isEditingDialog = ref(false);
const isDeletingDialog = ref(false);
const currentRoleId = ref(null);
const currentRole = ref(null);
const roles = ref([]);
const currentPage = ref(1);
const perPage = ref(10);
const totalItems = ref(0);
const search = ref("");
const debouncedSearch = ref(null);

// Fetch roles with pagination and search
const fetchRoles = async () => {
  try {
    isLoading.value = true;
    const queryParams = new URLSearchParams();

    // Add pagination parameters
    queryParams.append("page", currentPage.value.toString());
    queryParams.append("per_page", perPage.value.toString());

    // Add search parameter if exists
    if (search.value) {
      queryParams.append("search", search.value);
    }

    const response = await axios.get(`/roles?${queryParams.toString()}`);

    if (response.data?.data) {
      roles.value = response.data.data;
      totalItems.value =
        response.data.meta?.total ||
        response.data.pagination?.i_total_objects ||
        0;
    }
  } catch (error) {
    console.error("Error fetching roles:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب الأدوار",
      color: "error",
    });
    roles.value = [];
    totalItems.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Watch for changes in search with debounce
watch(search, (newValue) => {
  if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
  debouncedSearch.value = setTimeout(() => {
    currentPage.value = 1; // Reset to first page when searching
    fetchRoles();
  }, 500); // 500ms debounce
});

// Watch for page changes
watch([currentPage, perPage], () => {
  fetchRoles();
});

const handleDeleteRole = (roleId) => {
  currentRoleId.value = roleId;
  isDeletingDialog.value = true;
};

const handleEditRole = (role) => {
  currentRole.value = role;
  isEditingDialog.value = true;
};

const deleteRole = async (roleId) => {
  if (!roleId) return;

  try {
    await axios.delete(`/delete-role/${roleId}`);

    appStore.showSnackbar({
      message: "تم حذف الدور بنجاح",
      color: "success",
    });

    // Refresh roles list after deletion
    fetchRoles();
  } catch (error) {
    console.error("Error deleting role:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حذف الدور",
      color: "error",
    });
  }
};

const closeDeletingDialog = () => {
  currentRoleId.value = null;
  isDeletingDialog.value = false;
};

const closeEditingDialog = () => {
  currentRole.value = null;
  isEditingDialog.value = false;
};

const handleRoleAdded = () => {
  fetchRoles();
};

const handleRoleUpdated = () => {
  fetchRoles();
};

onMounted(() => {
  fetchRoles();
});
</script>

<template>
  <div>
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
              title: '⁠⁠إدارة الأدوار والصلاحيات',
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
      <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
        <v-card-title class="pt-4 pb-6">
          <div
            class="d-flex flex-column flex-sm-row align-sm-center justify-sm-space-between ga-4 border-b pb-4"
          >
            <h2 class="text-h5 font-weight-bold mb-4">
              إدارة الأدوار والصلاحيات
            </h2>
            <div class="d-flex align-center ga-2">
              <!-- <v-text-field
                v-model="search"
                label="البحث عن دور..."
                rounded="lg"
                clearable
                height="48"
                class="bg-white me-2"
                style="width: 280px; max-width: 100%; border-radius: 1rem"
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
              </v-text-field> -->

              <VBtn
                v-if="hasPermission('permissions.create')"
                height="48"
                elevation="0"
                class="rounded-lg"
                append-icon="mdi-plus"
                @click="isAddingDialog = true"
              >
                إضافة دور جديد
              </VBtn>
            </div>
          </div>

          <v-row v-if="isLoading" class="py-4">
            <v-col v-for="item in 4" :key="item" cols="6" sm="6" md="4">
              <VCard>
                <v-card-text class="px-1 py-2">
                  <v-skeleton-loader
                    class="mx-auto"
                    elevation="0"
                    max-width="400"
                    type="list-item-two-line, table-heading"
                  ></v-skeleton-loader>
                </v-card-text>
              </VCard>
            </v-col>
          </v-row>
          <v-row v-else-if="roles.length > 0" class="mt-3">
            <v-col
              v-for="role in roles"
              :key="role.id"
              cols="12"
              sm="6"
              md="4"
              lg="4"
              class="pa-3"
            >
              <RoleCard
                :role="role"
                @delete-role="handleDeleteRole"
                @edit-role="handleEditRole"
                :can-edit="hasPermission('permissions.edit')"
                :can-delete="hasPermission('permissions.delete')"
              />
            </v-col>
          </v-row>
          <v-row v-else class="py-4">
            <v-col cols="12">
              <div class="text-center py-8">
                <h3 class="text-lg font-weight-medium">لا توجد أدوار متاحة</h3>
                <p class="text-medium-emphasis mt-2">
                  يمكنك إضافة دور جديد بالضغط على زر "إضافة دور جديد".
                </p>
              </div>
            </v-col>
          </v-row>

          <Pagination
            v-if="roles.length > 0"
            :total-items="totalItems"
            v-model:per-page="perPage"
            v-model:page="currentPage"
          />
        </v-card-title>
      </VCard>

      <!-- Add New Role Dialog -->
      <AddNewRoleDialog
        v-model="isAddingDialog"
        @close="isAddingDialog = false"
        @role-added="handleRoleAdded"
      />

      <!-- Edit Role Dialog -->
      <EditRoleDialog
        v-model="isEditingDialog"
        :role="currentRole"
        @close="closeEditingDialog"
        @role-updated="handleRoleUpdated"
      />

      <!-- Delete Role Confirmation Dialog -->
      <DeleteRoleConfirmationDialog
        v-model="isDeletingDialog"
        :current-role-id="currentRoleId"
        @close="closeDeletingDialog"
        @delete="deleteRole"
      />
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
// @use "@core/scss/template/libs/apex-chart.scss";
.grid {
  flex-wrap: wrap;

  &__item {
    // width: 100%;/

    @media (min-width: 960px) {
      width: calc((100% - 4px) / 5);
    }

    @media (min-width: 600px) and (max-width: 959px) {
      width: calc((100% - 4px) / 2);
    }

    @media (max-width: 599px) {
      width: 100%;
    }
  }
}
</style>
