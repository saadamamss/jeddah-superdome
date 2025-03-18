<template>
  <div>
    <VCard class="rounded-lg">
      <VCardText>
        <!-- Header Section -->
        <div class="d-flex align-center justify-space-between mb-6">
          <h2 class="text-h5 font-weight-bold">اداره الورديات</h2>
          <div class="d-flex align-center ga-2">
            <!-- Search -->
            <v-text-field
              v-model="searchQuery"
              placeholder="البحث..."
              rounded="lg"
              clearable
              height="48"
              class="bg-white search-field me-3"
              style="width: 320px; max-width: 100%"
              density="compact"
              hide-details
              @update:model-value="handleSearch"
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

            <!-- Add Shift Button -->
            <VBtn
              height="48"
              elevation="0"
              class="rounded-lg"
              prepend-icon="tabler-plus"
              @click="isCreateDialogOpen = true"
            >
              إضافة وردية جديدة
            </VBtn>
          </div>
        </div>

        <!-- Shifts Grid -->
        <VRow>
          <VCol v-for="shift in shifts" :key="shift.id" cols="12" md="6" lg="4">
            <ShiftCard
              :shift="shift"
              @edit="openEditDialog"
              @delete="confirmDelete"
            />
          </VCol>
        </VRow>

        <!-- Pagination -->
        <div class="d-flex justify-center mt-4">
          <VPagination
            v-model="currentPage"
            :length="totalPages"
            :total-visible="7"
            @update:model-value="handlePageChange"
          />
        </div>
      </VCardText>
    </VCard>

    <!-- Create/Edit Dialog -->
    <VDialog v-model="isCreateDialogOpen" max-width="600px">
      <VCard>
        <VCardTitle class="pa-4">
          {{ editedItem.id ? "تعديل الوردية" : "إضافة وردية جديدة" }}
        </VCardTitle>
        <VCardText>
          <VForm ref="form" @submit.prevent="handleSubmit">
            <VRow>
              <VCol cols="12">
                <VTextField
                  v-model="editedItem.name"
                  label="اسم الوردية"
                  required
                  :rules="rules.name"
                />
              </VCol>

              <VCol cols="12" md="6">
                <VTextField
                  v-model="editedItem.start_time"
                  label="وقت البداية"
                  type="time"
                  required
                  :rules="rules.time"
                />
              </VCol>

              <VCol cols="12" md="6">
                <VTextField
                  v-model="editedItem.end_time"
                  label="وقت النهاية"
                  type="time"
                  required
                  :rules="rules.time"
                />
              </VCol>
            </VRow>
          </VForm>
        </VCardText>

        <VCardActions class="pa-4">
          <VSpacer />
          <VBtn
            color="error"
            variant="text"
            @click="isCreateDialogOpen = false"
          >
            إلغاء
          </VBtn>
          <VBtn color="primary" @click="handleSubmit" :loading="isSubmitting">
            حفظ
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>

    <!-- Delete Confirmation Dialog -->
    <VDialog v-model="isDeleteDialogOpen" max-width="400px">
      <VCard>
        <VCardTitle class="text-h6 pa-4">تأكيد الحذف</VCardTitle>
        <VCardText class="pa-4">
          هل أنت متأكد من حذف هذه الوردية؟ لا يمكن التراجع عن هذا الإجراء.
        </VCardText>
        <VCardActions class="pa-4">
          <VSpacer />
          <VBtn
            color="grey-darken-1"
            variant="text"
            @click="isDeleteDialogOpen = false"
          >
            إلغاء
          </VBtn>
          <VBtn
            color="error"
            @click="deleteShiftConfirmed"
            :loading="isDeleting"
          >
            حذف
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { ShiftsService } from "@/services/shifts.service";
import { useAppStore } from "@/stores/app";
import ShiftCard from "@/components/ShiftCard.vue";

const appStore = useAppStore();

// Form validation rules
const rules = {
  name: [
    (v) => !!v || "اسم الوردية مطلوب",
    (v) => (v && v.length >= 3) || "يجب أن يكون اسم الوردية 3 أحرف على الأقل",
  ],
  time: [(v) => !!v || "الوقت مطلوب"],
};

// State
const shifts = ref([]);
const searchQuery = ref("");
const currentPage = ref(1);
const totalItems = ref(0);
const itemsPerPage = ref(12);
const totalPages = ref(1);
const isCreateDialogOpen = ref(false);
const isSubmitting = ref(false);
const form = ref(null);

// Delete confirmation state
const isDeleteDialogOpen = ref(false);
const isDeleting = ref(false);
const deleteItemId = ref(null);

// Edited item
const editedItem = ref({
  id: null,
  name: "",
  start_time: "",
  end_time: "",
});

// Methods
const fetchShifts = async (page = 1) => {
  try {
    const params = {
      page,
      per_page: itemsPerPage.value,
      search: searchQuery.value,
    };

    const response = await ShiftsService.getShifts(params);
    shifts.value = response.data.data;
    totalItems.value = response.data.pagination.i_total_objects;
    totalPages.value = response.data.pagination.i_total_pages;
    itemsPerPage.value = response.data.pagination.i_per_page;
    currentPage.value = response.data.pagination.i_current_page;
  } catch (error) {
    appStore.showSnackbar({
      message: "حدث خطأ أثناء تحميل البيانات",
      color: "error",
    });
    console.error(error);
  }
};

const handleSearch = () => {
  currentPage.value = 1;
  fetchShifts(1);
};

const handlePageChange = (page) => {
  fetchShifts(page);
};

const openEditDialog = (shift) => {
  editedItem.value = { ...shift };
  isCreateDialogOpen.value = true;
};

const handleSubmit = async () => {
  const { valid } = await form.value.validate();

  if (!valid) {
    appStore.showSnackbar({
      message: "يرجى التحقق من صحة جميع الحقول",
      color: "error",
    });
    return;
  }

  try {
    isSubmitting.value = true;
    if (editedItem.value.id) {
      await ShiftsService.updateShift(editedItem.value.id, editedItem.value);
      appStore.showSnackbar({
        message: "تم تحديث الوردية بنجاح",
        color: "success",
      });
    } else {
      await ShiftsService.createShift(editedItem.value);
      appStore.showSnackbar({
        message: "تم إنشاء الوردية بنجاح",
        color: "success",
      });
    }

    isCreateDialogOpen.value = false;
    await fetchShifts(currentPage.value);
    resetForm();
  } catch (error) {
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حفظ البيانات",
      color: "error",
    });
    console.error(error);
  } finally {
    isSubmitting.value = false;
  }
};

const confirmDelete = (id) => {
  deleteItemId.value = id;
  isDeleteDialogOpen.value = true;
};

const deleteShiftConfirmed = async () => {
  if (!deleteItemId.value) return;

  try {
    isDeleting.value = true;
    await ShiftsService.deleteShift(deleteItemId.value);
    appStore.showSnackbar({
      message: "تم حذف الوردية بنجاح",
      color: "success",
    });
    await fetchShifts(currentPage.value);
  } catch (error) {
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حذف الوردية",
      color: "error",
    });
    console.error(error);
  } finally {
    isDeleting.value = false;
    isDeleteDialogOpen.value = false;
    deleteItemId.value = null;
  }
};

// Reset form
const resetForm = () => {
  editedItem.value = {
    id: null,
    name: "",
    start_time: "",
    end_time: "",
  };
  if (form.value) {
    form.value.reset();
  }
};

// Initial load
onMounted(() => {
  fetchShifts();
});
</script>

<style lang="scss" scoped>
.search-field {
  :deep(.v-field) {
    border-radius: 8px;
    background-color: rgb(250, 250, 250);

    .v-field__input {
      min-height: 44px;
      padding-top: 0;
      padding-bottom: 0;
    }

    .v-field__prepend-inner {
      padding-inline-start: 12px;
      color: rgba(0, 0, 0, 0.38);
    }
  }
}

.ga-2 {
  gap: 0.5rem;
}
</style>
