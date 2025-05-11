<template>
  <div>
    <VCard class="rounded-lg">
      <VCardText>
        <!-- Header Section -->
        <div class="d-flex align-center justify-space-between mb-6">
          <h2 class="text-h5 font-weight-bold">جدول الرحلات</h2>
          <div class="d-flex align-center ga-2">
            <!-- Search -->
            <v-text-field
              v-model="searchQuery"
              placeholder="بحث برقم الرحلة   "
              rounded="lg"
              clearable
              height="48"
              class="bg-primary_2 me-3 border-none"
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
                    fill="#667178"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M9.16663 1.04169C4.67931 1.04169 1.04163 4.67937 1.04163 9.16669C1.04163 13.654 4.67931 17.2917 9.16663 17.2917C11.1853 17.2917 13.032 16.5555 14.453 15.337L17.8914 18.7753C18.1354 19.0194 18.5312 19.0194 18.7752 18.7753C19.0193 18.5312 19.0193 18.1355 18.7752 17.8914L15.3369 14.4531C16.5554 13.0321 17.2916 11.1854 17.2916 9.16669C17.2916 4.67937 13.6539 1.04169 9.16663 1.04169ZM2.29163 9.16669C2.29163 5.36973 5.36967 2.29169 9.16663 2.29169C12.9636 2.29169 16.0416 5.36973 16.0416 9.16669C16.0416 12.9636 12.9636 16.0417 9.16663 16.0417C5.36967 16.0417 2.29163 12.9636 2.29163 9.16669Z"
                    fill="#667178"
                  />
                </svg>
              </template>
            </v-text-field>

            <!-- Add Company Button -->
            <VBtn
              height="48"
              elevation="0"
              class="rounded-lg"
              prepend-icon="tabler-file-plus"
              color="primary_2"
            >
              إضافة ملف الرحلات
            </VBtn>
            <VBtn
              height="48"
              elevation="0"
              @click="isAddDialogVisible = true"
              class="rounded-lg"
            >
              إضافة رحلة جديدة
            </VBtn>
          </div>
        </div>

        <v-row>
          <v-col cols="12">
            <dataTable
              :headers="headers"
              :items="companies"
              :loading="isLoading"
              class="elevation-0"
              :items-per-page="perPage"
              v-model:page="currentPage"
              :hide-default-footer="true"
            >
              <template #[`item.actions`]="{ item }">
                <div class="d-flex gap-2">
                  <v-btn
                    size="small"
                    class="text-sm text-black rounded-lg"
                    color="#fff"
                    :prepend-icon="Editicon"
                  >
                    تعديل
                  </v-btn>
                  <v-btn
                    size="small"
                    class="text-sm text-error rounded-lg"
                    color="#fff"
                    :prepend-icon="Trash"
                  >
                    حذف
                  </v-btn>
                </div>
              </template>
            </dataTable>

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

              <VCol cols="12">
                <label class="text-subtitle-2 mb-2 d-block"
                  >الموقع على الخريطة</label
                >
                <LocationPicker
                  v-model="selectedLocation"
                  class="rounded-lg overflow-hidden"
                  @update:model-value="handleLocationChange"
                />
                <!-- Coordinates display -->
                <div class="d-flex gap-4 mt-2">
                  <VTextField
                    v-model="formData.latitude"
                    label="خط العرض"
                    readonly
                    variant="outlined"
                    density="comfortable"
                    class="rounded-lg"
                    hide-details
                  />
                  <VTextField
                    v-model="formData.longitude"
                    label="خط الطول"
                    readonly
                    variant="outlined"
                    density="comfortable"
                    class="rounded-lg"
                    hide-details
                  />
                </div>
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

    <!-- Delete Confirmation Modal -->
    <VDialog v-model="isDeleteDialogVisible" max-width="400px">
      <VCard>
        <VCardTitle class="text-h6 pa-4">تأكيد الحذف</VCardTitle>
        <VCardText class="pa-4">
          هل أنت متأكد من حذف هذه الشركة؟ لا يمكن التراجع عن هذا الإجراء.
        </VCardText>
        <VCardActions class="pa-4">
          <VSpacer />
          <VBtn
            color="grey-darken-1"
            variant="text"
            @click="isDeleteDialogVisible = false"
          >
            إلغاء
          </VBtn>
          <VBtn color="error" @click="confirmDelete" :loading="isDeleting">
            حذف
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>
  </div>
</template>

<script setup>
import { ref } from "vue";
import CompanyCard from "@/components/CompanyCard.vue";
import LocationPicker from "@/components/maps/LocationPicker.vue";
import axios from "@axios";
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination.vue";
import dataTable from "@/components/dataTable.vue";
import Editicon from "@/components/icons/editicon.vue";
import Trash from "@/components/icons/trash.vue";
import Eyeicon from "@/components/icons/eyeicon.vue";
const appStore = useAppStore();
const headers = ref([
  { title: "رقم الرحلة", value: "flight_number" },
  { title: "الشركة المشغلة", value: "company" },
  { title: "التاريخ", value: "date" },
  { title: "الوقت", value: "time" },
  { title: "الوجهة", value: "distnation" },
  { title: "عدد الحجاج", value: "no_of_huj" },
  { title: "وسبلة النقل", value: "transport" },
  { title: "", value: "actions" },
]);

// State
const companies = ref([
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
  {
    flight_number: 112,
    company: "شركة النور",
    date: "10/05/2024",
    time: "08:00 ص",
    distnation: "مكة المكرمة",
    no_of_huj: "20 حاج",
    transport: "باص",
  },
]);
const searchQuery = ref("");
const currentPage = ref(1);
const totalItems = ref(0);
const itemsPerPage = ref(12);
const totalPages = ref(1);
const isAddDialogVisible = ref(false);
const isSubmitting = ref(false);
const form = ref(null);

// Delete confirmation state
const isDeleteDialogVisible = ref(false);
const isDeleting = ref(false);
const companyToDelete = ref(null);

// Default location (Makkah coordinates)
const selectedLocation = ref({ lat: 21.4225, lng: 39.8262 });

const formData = ref({
  name: "",
  license_no: "",
  center_no: "",
  phone: "",
  email: "",
  contact_name: "",
  pligrims_nationality: "",
  latitude: "21.4225",
  longitude: "39.8262",
  address: "",
  created_by_id: 1,
});

// Handle location change from map
const handleLocationChange = (newLocation) => {
  if (newLocation) {
    formData.value.latitude = newLocation.lat.toString();
    formData.value.longitude = newLocation.lng.toString();
  }
};

// Methods
const fetchCompanies = async (page = 1) => {
  try {
    let url = "/companies?page=" + page + "&per_page=" + itemsPerPage.value;
    if (searchQuery.value) {
      url += "&search=" + searchQuery.value;
    }

    const response = await axios.get(url);

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
      latitude: "21.4225",
      longitude: "39.8262",
      address: "",
      created_by_id: 1,
    };
    selectedLocation.value = { lat: 21.4225, lng: 39.8262 }; // Reset map location
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
  companyToDelete.value = id;
  isDeleteDialogVisible.value = true;
};

const confirmDelete = async () => {
  if (!companyToDelete.value) return;

  try {
    isDeleting.value = true;
    await axios.delete("/companies/" + companyToDelete.value);
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
  } finally {
    isDeleting.value = false;
    isDeleteDialogVisible.value = false;
    companyToDelete.value = null;
  }
};

// Lifecycle
// fetchCompanies();
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
