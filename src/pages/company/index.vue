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
              title: 'إدارة الشركات',
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
            <!-- Title - Takes full width on small screens, auto on larger -->
            <h2 class="text-h5 font-weight-bold flex-grow-1 flex-md-grow-0">
              إدارة الشركات
            </h2>

            <!-- Search and Buttons Container - Full width on small screens -->
            <div
              class="d-flex flex-wrap align-center justify-md-end ga-2"
              style="flex-grow: 1; min-width: 0"
            >
              <!-- Search - Full width on small screens, fixed width on medium+ -->
              <v-text-field
                v-model="searchQuery"
                placeholder="بحث برقم الترخيص أو رقم الشركة"
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

              <!-- Buttons Container - Wraps on small screens -->
              <div class="d-flex flex-wrap ga-2" style="min-width: fit-content">
                <!-- Add File Button -->
                <VBtn
                  height="48"
                  elevation="0"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  :prepend-icon="Fileadd"
                  color="primary_2"
                  @click="uploadDialog = true"
                >
                  <span class="d-none d-sm-inline">إضافة ملف شركة جديدة</span>
                  <span class="d-inline d-sm-none">إضافة ملف</span>
                </VBtn>

                <!-- Add Company Button -->
                <VBtn
                  height="48"
                  elevation="0"
                  @click="openAddForm"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                >
                  <span class="d-none d-sm-inline">إضافة شركة جديدة</span>
                  <span class="d-inline d-sm-none">إضافة</span>
                </VBtn>
              </div>
            </div>
          </div>

          <v-row>
            <v-col cols="12">
              <DataTable
                :headers="headers"
                :items="companies"
                :loading="isLoading"
                class="elevation-0"
                :items-per-page="perPage"
                v-model:page="currentPage"
                :hide-default-footer="true"
              >
                <template #[`item.licenseNumber`]="{ item }">
                  <span class="text-nowrap">{{ item.licenseNumber }}</span>
                </template>

                <template #[`item.companyName`]="{ item }">
                  <span class="text-nowrap">{{ item.companyName }}</span>
                </template>
                <template #[`item.contactNumber`]="{ item }">
                  <span class="text-nowrap">{{ item.contactNumber }}</span>
                </template>

                <template #[`item.password`]="{ item, index }">
                  <div class="text-nowrap d-flex gap-3 align-center">
                    <span>
                      {{
                        item.password.show ? item.password.text : "*********"
                      }}
                    </span>
                    <button
                      @click="handleShowPass(item)"
                      class="d-flex gap-3 align-center"
                    >
                      <Eyeoff v-if="item.password.show" />
                      <Eyeicon v-else />
                    </button>
                  </div>
                </template>

                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn
                      size="small"
                      class="text-black text-sm rounded-lg"
                      color="#fff"
                      :prepend-icon="Editicon"
                      @click="handleEdit(item)"
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

      <!-- Add Company Dialog -->
      <VDialog v-model="isAddDialogVisible" max-width="600px">
        <VCard rounded="lg">
          <VCardTitle class="bg-primary_2 py-3 rounded-lg">
            <div class="d-flex align-center justify-space-between">
              <div>
                <h5 class="mb-2">إضافة شركة جديدة</h5>
                <p class="text-sm text-secondary mb-0">
                  يرجى تعبئة بيانات الشركة بدقة. الحقول الإلزامية محددة بعلامة
                  (*).
                </p>
              </div>
            </div>
          </VCardTitle>
          <VCardText>
            <VForm ref="form" @submit.prevent="handleSubmit">
              <VRow>
                <VCol cols="12">
                  <label for="" class="mb-2 d-block">اسم الشركة *</label>
                  <VTextField
                    rounded="lg"
                    v-model="formData.companyName"
                    placeholder="الإسم الكامل الرسمى"
                    required
                  />
                </VCol>

                <VCol cols="12">
                  <label for="" class="mb-2 d-block">رقم الترخيص*</label>
                  <VTextField
                    rounded="lg"
                    v-model="formData.license_no"
                    placeholder="رقم الترخيص"
                    required
                  />
                </VCol>

                <VCol cols="12">
                  <label for="" class="mb-2 d-block">المدينة*</label>
                  <VTextField
                    rounded="lg"
                    v-model="formData.city"
                    placeholder="الرياض، مكة، جدة..."
                    required
                  />
                </VCol>

                <VCol cols="12">
                  <label for="" class="mb-2 d-block">رقم التواصل*</label>
                  <VTextField
                    rounded="lg"
                    v-model="formData.phone"
                    placeholder="رقم التواصل"
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
                  <label for="" class="mb-2 d-block">كلمة المرور*</label>
                  <VTextField
                    rounded="lg"
                    v-model="formData.password"
                    label="كلمة المرور"
                    required
                  />
                </VCol>

                <VCol cols="12">
                  <label for="" class="mb-2 d-block">العنوان</label>
                  <VTextField
                    rounded="lg"
                    v-model="formData.address"
                    label="العنوان التفصيلى إن وجد"
                    required
                  />
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
              {{ isEdit ? "حفظ" : "إضافة" }}
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
                <h5 class="mb-2">رفع ملف الشركات</h5>
                <p class="text-sm text-secondary">
                  يرجى رفع الملفات المطلوبة لإضافة أو تحديث بيانات الشركات، تأكد
                  أن الملفات تحتوي على المعلومات الكاملة.
                </p>
              </div>
              <button @click="uploadDialog = false">
                <v-icon size="sm">mdi-close</v-icon>
              </button>
            </div>
          </VCardTitle>

          <UploadComponent
            title="ملف الشركات"
            btn-text="تحميل نموذج ملف الشركات"
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
        title="تمت إضافة الشركة بنجاح!"
        subtitle="إعادة التوجه إلى إدارة الشركات أو إضافة شركة أخرى."
        cancel-text="إضافة شركة أخرى"
        action-text="التوجه إلى إدارة الشركات"
        v-model="successDialogOpen"
        @cancel="successDialogOpen = false"
        @action="successDialogOpen = false"
      />

      <DeleteDialog
        title="️ هل أنت متأكد من حذف الشركة؟"
        subtitle="سيؤدي حذف هذه الشركة إلى إزالة جميع بياناتها من النظام، وقد لا يمكن استرجاعها لاحقًا."
        cancel-text="إلغاء"
        action-text="نعم، حذف الشركة"
        v-model="deleteDialogOpen"
        @cancel="deleteDialogOpen = false"
        @action="deleteDialogOpen = false"
      />

      <!-- Delete Confirmation Modal -->
      <!-- <VDialog v-model="isDeleteDialogVisible" max-width="400px">
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
    </VDialog> -->
    </v-container>
  </div>
</template>

<script setup>
import { ref } from "vue";
import CompanyCard from "@/components/CompanyCard.vue";
import LocationPicker from "@/components/maps/LocationPicker.vue";
import UploadComponent from "@/components/UploadComponent.vue";
import SuccessDialog from "@/components/SuccessDialog.vue";
import DeleteDialog from "@/components/DeleteDialog.vue";
import axios from "@axios";
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination.vue";
import DataTable from "@/components/dataTable.vue";
import Editicon from "@/components/icons/editicon.vue";
import Trash from "@/components/icons/trash.vue";
import Fileadd from "@/components/icons/fileadd.vue";
import Magnifier from "@/components/icons/magnifier.vue";
import Eyeicon from "@/components/icons/eyeicon.vue";
import Eyeoff from "@/components/icons/eyeoff.vue";
import TelInput from "@/components/TelInput.vue";
const appStore = useAppStore();
const headers = ref([
  { title: "رقم الترخيص", value: "licenseNumber" },
  { title: "اسم الشركة", value: "companyName" },
  { title: "عدد الحجاج المرتبطين", value: "connectedHujjaj", align: "start" },
  { title: "وسيلة الاتصال", value: "contactNumber" },
  { title: "كلمة المرور", value: "password" },
  { title: "", value: "actions" },
]);

// State
const companies = ref([
  {
    connectedHujjaj: 34,
    licenseNumber: "1966 555 555 555",
    companyName: "34 حاج",
    contactNumber: "2378903242",
    password: {
      text: "msn2115",
      show: true,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 20,
    licenseNumber: "1966 500 123 456",
    companyName: "20 حاج",
    contactNumber: "1234567890",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 15,
    licenseNumber: "1966 511 234 567",
    companyName: "15 حاج",
    contactNumber: "0987654321",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 50,
    licenseNumber: "1966 522 345 678",
    companyName: "50 حاج",
    contactNumber: "1357924680",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 25,
    licenseNumber: "1966 533 456 789",
    companyName: "25 حاج",
    contactNumber: "2468135790",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 40,
    licenseNumber: "1966 544 567 890",
    companyName: "40 حاج",
    contactNumber: "9876543210",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 40,
    licenseNumber: "1966 544 567 890",
    companyName: "40 حاج",
    contactNumber: "9876543210",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 40,
    licenseNumber: "1966 544 567 890",
    companyName: "40 حاج",
    contactNumber: "9876543210",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 40,
    licenseNumber: "1966 544 567 890",
    companyName: "40 حاج",
    contactNumber: "9876543210",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 40,
    licenseNumber: "1966 544 567 890",
    companyName: "40 حاج",
    contactNumber: "9876543210",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 40,
    licenseNumber: "1966 544 567 890",
    companyName: "40 حاج",
    contactNumber: "9876543210",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
  {
    connectedHujjaj: 40,
    licenseNumber: "1966 544 567 890",
    companyName: "40 حاج",
    contactNumber: "9876543210",
    password: {
      text: "msn2115",
      show: false,
    },
    actions: { edit: true },
  },
]);
const searchQuery = ref("");
const currentPage = ref(1);
const totalItems = ref(companies.value.length);
const itemsPerPage = ref(12);
const totalPages = ref(1);
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
  companyName: "",
  license_no: "",
  city: "",
  phone: "",
  email: "",
  password: "",
  address: "",
});
const fileToUpload = ref(null);
const uploadError = ref(null);
const handleFileSelection = (file) => {
  console.log(file);
  fileToUpload.value = file;
};
const handleUploadFile = () => {
  isAddDialogVisible.value = false;
  successDialogOpen.value = true;
};
const handleShowPass = (item) => {
  item.password.show = !item.password.show;
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

const isEdit = ref(false);
const openAddForm = () => {
  isEdit.value = false;
  isAddDialogVisible.value = true;
  formData.value = {};
};
const handleEdit = (item) => {
  isEdit.value = true;
  const data = {
    companyName: item.companyName,
    license_no: item.licenseNumber,
    city: item.city,
    phone: item.contactNumber,
    email: item.email,
    password: item.password.text,
    address: item.address,
  };
  formData.value = { ...data };
  isAddDialogVisible.value = true;
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
.text-nowrap {
  white-space: nowrap;
}
.ga-2 {
  gap: 0.5rem;
}
</style>
