<template>
  <div>
    <VCard title="إدارة الشركات">
      <VCardText>
        <div class="d-flex align-center flex-wrap gap-4 mb-4">
          <!-- Search -->
          <div class="me-3" style="width: 250px">
            <VTextField
              v-model="searchQuery"
              placeholder="البحث..."
              density="compact"
              prepend-inner-icon="tabler-search"
              single-line
              hide-details
              @update:model-value="handleSearch"
            />
          </div>

          <!-- Add Company Button -->
          <VBtn prepend-icon="tabler-plus" @click="isAddDialogVisible = true">
            إضافة شركة جديدة
          </VBtn>
        </div>

        <!-- Companies Grid -->
        <VRow>
          <VCol
            v-for="company in companies"
            :key="company.id"
            cols="12"
            md="6"
            lg="4"
          >
            <CompanyCard :company="company" @delete="handleDeleteCompany" />
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

    <!-- Add Company Dialog -->
    <VDialog v-model="isAddDialogVisible" max-width="600px">
      <VCard>
        <VCardTitle>إضافة شركة جديدة</VCardTitle>
        <VCardText>
          <VForm ref="form" @submit.prevent="handleSubmit">
            <VRow>
              <VCol cols="12">
                <VTextField
                  v-model="formData.name"
                  label="اسم الشركة"
                  required
                />
              </VCol>

              <VCol cols="12" md="6">
                <VTextField
                  v-model="formData.license_no"
                  label="رقم الترخيص"
                  required
                />
              </VCol>

              <VCol cols="12" md="6">
                <VTextField
                  v-model="formData.center_no"
                  label="رقم المركز"
                  required
                />
              </VCol>

              <VCol cols="12" md="6">
                <VTextField
                  v-model="formData.phone"
                  label="رقم الهاتف"
                  required
                />
              </VCol>

              <VCol cols="12" md="6">
                <VTextField
                  v-model="formData.email"
                  label="البريد الإلكتروني"
                  type="email"
                  required
                />
              </VCol>

              <VCol cols="12">
                <VTextField
                  v-model="formData.contact_name"
                  label="اسم جهة الاتصال"
                  required
                />
              </VCol>

              <VCol cols="12">
                <VTextField
                  v-model="formData.pligrims_nationality"
                  label="جنسية الحجاج"
                  required
                />
              </VCol>

              <VCol cols="12" md="6">
                <VTextField
                  v-model="formData.latitude"
                  label="خط العرض"
                  type="number"
                  step="any"
                  required
                />
              </VCol>

              <VCol cols="12" md="6">
                <VTextField
                  v-model="formData.longitude"
                  label="خط الطول"
                  type="number"
                  step="any"
                  required
                />
              </VCol>

              <VCol cols="12">
                <VTextField
                  v-model="formData.address"
                  label="العنوان"
                  required
                />
              </VCol>
            </VRow>
          </VForm>
        </VCardText>

        <VCardActions>
          <VSpacer />
          <VBtn
            color="error"
            variant="text"
            @click="isAddDialogVisible = false"
          >
            إلغاء
          </VBtn>
          <VBtn color="primary" @click="handleSubmit" :loading="isSubmitting">
            حفظ
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import CompanyCard from "@/components/CompanyCard.vue";
import axios from "@axios";
import { useAppStore } from "@/stores/app";

const appStore = useAppStore();

// State
const companies = ref([]);
const searchQuery = ref("");
const currentPage = ref(1);
const totalItems = ref(0);
const itemsPerPage = ref(12);
const totalPages = ref(1);
const isAddDialogVisible = ref(false);
const isSubmitting = ref(false);
const form = ref(null);

const formData = ref({
  name: "",
  license_no: "",
  center_no: "",
  phone: "",
  email: "",
  contact_name: "",
  pligrims_nationality: "",
  latitude: "",
  longitude: "",
  address: "",
});

// Methods
const fetchCompanies = async (page = 1) => {
  try {
    const response = await axios.get(
      `/companies?page=${page}&per_page=${itemsPerPage.value}${
        searchQuery.value ? `&search=${searchQuery.value}` : ""
      }`
    );

    if (response.data.status.success) {
      companies.value = response.data.data;
      totalItems.value = response.data.pagination.i_total_objects;
      totalPages.value = response.data.pagination.i_total_pages;
      itemsPerPage.value = response.data.pagination.i_per_page;
      currentPage.value = response.data.pagination.i_current_page;
    }
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
  fetchCompanies(1);
};

const handlePageChange = (page) => {
  fetchCompanies(page);
};

const handleSubmit = async () => {
  try {
    isSubmitting.value = true;
    await axios.post("/companies", formData.value);

    appStore.showSnackbar({
      message: "تم إضافة الشركة بنجاح",
      color: "success",
    });
    isAddDialogVisible.value = false;
    await fetchCompanies(currentPage.value);

    // Reset form
    formData.value = {
      name: "",
      license_no: "",
      center_no: "",
      phone: "",
      email: "",
      contact_name: "",
      pligrims_nationality: "",
      latitude: "",
      longitude: "",
      address: "",
    };
  } catch (error) {
    appStore.showSnackbar({
      message: "حدث خطأ أثناء إضافة الشركة",
      color: "error",
    });
    console.error(error);
  } finally {
    isSubmitting.value = false;
  }
};

const handleDeleteCompany = async (id) => {
  if (!confirm("هل أنت متأكد من حذف هذه الشركة؟")) return;

  try {
    await axios.delete("/companies/" + id);
    appStore.showSnackbar({
      message: "تم حذف الشركة بنجاح",
      color: "success",
    });
    await fetchCompanies(currentPage.value);
  } catch (error) {
    appStore.showSnackbar({
      message: "حدث خطأ أثناء حذف الشركة",
      color: "error",
    });
    console.error(error);
  }
};

// Lifecycle
fetchCompanies();
</script>
