<template>
  <div>
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
            title: 'إدارة الرحلات',
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
          <h2 class="text-h5 font-weight-bold">جدول الرحلات</h2>

          <!-- Search -->
          <div
            class="d-flex flex-wrap align-center justify-md-end ga-2"
            style="flex-grow: 1; min-width: 0"
          >
            <v-text-field
              v-model="searchQuery"
              placeholder="بحث برقم الرحلة"
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
                إضافة ملف الرحلات
              </VBtn>
              <VBtn
                height="48"
                elevation="0"
                @click="isAddDialogVisible = true"
                class="rounded-lg flex-grow-1 flex-md-grow-0"
              >
                إضافة رحلة جديدة
              </VBtn>
            </div>
          </div>
        </div>

        <v-row>
          <v-col cols="12">
            <dataTable
              :headers="headers"
              :items="flights"
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
                    @click="deleteDialogOpen = true"
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
      <VCard rounded="lg">
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">إضافة رحلة جديدة</h5>
              <p class="text-sm text-secondary mb-0">
                يرجى تعبئة بيانات الرحلة بدقة. الحقول الإلزامية محددة بعلامة
                (*).
              </p>
            </div>
            <button @click="isAddDialogVisible = false">
              <v-icon size="sm">mdi-close</v-icon>
            </button>
          </div>
        </VCardTitle>

        <VCardText class="">
          <VForm ref="form" @submit.prevent="handleSubmit">
            <VRow>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">رقم الرحلة*</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.tripNumber"
                  placeholder="رقم الرحلة"
                  required
                />
              </VCol>
              <VCol cols="12" md="6">
                <label for="" class="mb-2 d-block">الوقت</label>
                <v-select
                  variant="solo"
                  rounded="lg"
                  v-model="formData.time"
                  placeholder="اختيار"
                  required
                ></v-select>
              </VCol>
              <VCol cols="12" md="6">
                <label for="" class="mb-2 d-block">التاريخ</label>
                <v-select
                  variant="solo"
                  rounded="lg"
                  v-model="formData.date"
                  placeholder="اختيار"
                  required
                ></v-select>
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">الشركة التابع لها*</label>
                <v-select
                  rounded="lg"
                  v-model="formData.comapy"
                  placeholder="اختيار"
                  required
                ></v-select>
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">الوجهة*</label>
                <v-select
                  rounded="lg"
                  v-model="formData.distnation"
                  placeholder="اختيار"
                  required
                ></v-select>
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">وسيلة النقل*</label>
                <v-select
                  rounded="lg"
                  v-model="formData.transport"
                  placeholder="اختيار"
                  required
                ></v-select>
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">عدد الحجاج المتوقع*</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.pilgrim_no"
                  placeholder="عدد الحجاج المتوقع"
                  required
                ></VTextField>
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
              <h5 class="mb-2">رفع ملف الرحلات</h5>
              <p class="text-sm text-secondary">
                رفع ملف بيانات الرحلات لإضافتها أو تحديثها دفعة واحدة، تأكد أن
                الملفات تحتوي على المعلومات الكاملة.
              </p>
            </div>
            <button @click="uploadDialog = false">
              <v-icon size="sm">mdi-close</v-icon>
            </button>
          </div>
        </VCardTitle>

        <UploadComponent
          title="ملف الرحلات"
          btn-text="تحميل نموذج ملف الرحلات"
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
      title="تمت إضافة الحاج بنجاح!"
      subtitle="إعادة التوجه إلى إدارة الشركات أو إضافة حاج آخر."
      cancel-text="إضافة حاج آخر"
      action-text="التوجه إلى إدارة الحجاج"
      v-model="successDialogOpen"
      @cancel="successDialogOpen = false"
      @action="successDialogOpen = false"
    />

    <DeleteDialog
      title="️ هل أنت متأكد من حذف الحاج"
      subtitle="سيؤدي حذف هذا الحاج إلى إزالة جميع بياناته من النظام، وقد لا يمكن استرجاعها لاحقًا."
      cancel-text="إلغاء"
      action-text="نعم، حذف الحاج"
      v-model="deleteDialogOpen"
      @cancel="deleteDialogOpen = false"
      @action="deleteDialogOpen = false"
    />
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
import Fileadd from "@/components/icons/fileadd.vue";
import UploadComponent from "@/components/UploadComponent.vue";
import SuccessDialog from "@/components/SuccessDialog.vue";
import DeleteDialog from "@/components/DeleteDialog.vue";
import Magnifier from "@/components/icons/magnifier.vue";
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
const flights = ref([
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
const totalItems = ref(flights.value.length);
const itemsPerPage = ref(12);
const totalPages = ref(1);
const detailsDialog = ref(false);
const isAddDialogVisible = ref(false);
const uploadDialog = ref(false);
const successDialogOpen = ref(false);
const deleteDialogOpen = ref(false);
const isSubmitting = ref(false);
const form = ref(null);

// Delete confirmation state
const isDeleteDialogVisible = ref(false);
const isDeleting = ref(false);
const companyToDelete = ref(null);

// Default location (Makkah coordinates)
const selectedLocation = ref({ lat: 21.4225, lng: 39.8262 });

const formData = ref({
  tripNumber: "",
  time: "",
  date: "",
  comapy: "",
  distnation: "",
  transport: "",
  pilgrim_no: "",
});

// Methods
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
