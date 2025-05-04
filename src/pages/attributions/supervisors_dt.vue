<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import dayjs from "dayjs";
import CustomVSelect from "@core/components/global/CustomVSelect.vue";
import dataTable from "@/components/dataTable.vue";
import axios from "@/plugins/axios";
import CustomVAutocomplete from "@/@core/components/global/CustomVAutocomplete.vue";
import { ref, computed, reactive, onMounted, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import AddSiteModal from "./components/siteAssignmentModal.vue";

const appStore = useAppStore();
const router = useRouter();
const route = useRoute();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const dateFilter = ref(null);
const totalItems = ref(20);
const isExporting = ref(false);
const isGettingFilters = ref(false);
const isDatepickerMenu = ref(false);
const attendanceDataa = ref([]);
const debouncedSearch = ref(null);
const searchQuery = ref("");
const showRemoveAssignmentModal = ref(false);
const showNewAssignmentModal = ref(false);
const selectedItemForRemoval = ref(null);
const inspectorData = ref({
  id: null,
  name: "",
  locations: [],
});
const availableTypes = ref([
  { title: "مخيم", value: "all" },
  { title: "مسكن", value: "attendance" },
  // { title: "متأخر", value: "late" },
]);
const type_render = ref("all");

const filters = reactive({
  user_id: undefined,
  attendance_by_user_id: undefined,
  location_type_id: "",
  search: "",
  status: undefined,
  created_at: undefined,
  user_id: undefined,
  attendance_by_user_id: undefined,
  tap: "daily",
});

// New state variables for locations and companies
const locations = ref([]);
const companies = ref([]);
const isLoadingLocations = ref(false);
const isLoadingCompanies = ref(false);

// Helper function to format date for API
const formatDate = (date) => {
  if (!date) return "";

  // Create a date object with tiintegrationsmezone handling
  const dateObj = new Date(date);

  // Format to YYYY-MM-DD without adding extra day
  const year = dateObj.getFullYear();
  const month = String(dateObj.getMonth() + 1).padStart(2, "0");
  const day = String(dateObj.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}`;
};

// Function to display formatted date in UI (doesn't need day adjustment)
const displayFormattedDate = (date) => {
  if (!date) return "";
  return dayjs(date).format("YYYY-MM-DD");
};

const onClearDateFilter = () => {
  filters.created_at = undefined;
  fetchInspectorLocations(); // Trigger data refresh when clearing the date filter
};

const handleTableUpdate = (options) => {
  currentPage.value = options.page;
  perPage.value = options.itemsPerPage;
  // You can add additional logic here for sorting or filtering
};

const tableHeaders = [
  { title: "اسم الموقع", key: "siteName", sortable: false },
  { title: "نوع الموقع", key: "locationType" },
  { title: "رقم الرخصة/المخيم", key: "licenseNumber" },
  { title: "جنسية الحجاج", key: "pilgrimsNationality" },
  { title: "شركة تقديم الخدمة", key: "serviceCompany" },
  { title: "الشركة الأم", key: "parentCompany" },
  { title: "الإجراءات", key: "actions" },
];

// Watch for changes in currentPage or perPage
watch([currentPage, perPage], () => {
  fetchInspectorLocations();
});

// Watch for filter changes
watch(
  () => [
    filters.status,
    filters.user_id,
    filters.attendance_by_user_id,
    filters.tap,
    filters.search,
    filters.created_at,
  ],
  () => {
    fetchInspectorLocations();
  },
  { deep: true }
);

// Separate watch for date filter to handle date-specific logic
watch(
  () => dateFilter.value,
  (newValue) => {
    if (newValue) {
      fetchInspectorLocations();
    }
  }
);

const fetchInspectorLocations = async () => {
  try {
    isLoading.value = true;

    // Get inspector_id from route params
    const inspectorId = route.params.id;

    if (!inspectorId) {
      console.warn("Inspector ID is missing from route params");
      locations.value = [];
      totalItems.value = 0;
      return;
    }

    // Prepare query parameters
    const params = {
      inspector_id: inspectorId,
      per_page: perPage.value,
      page: currentPage.value,
    };

    // Remove undefined values
    Object.keys(params).forEach((key) => {
      if (params[key] === undefined) {
        delete params[key];
      }
    });

    const response = await axios.get("inspector-locations/details", { params });

    if (response.data?.status?.success) {
      // Extract data from response
      const locationsData = response.data.data || [];

      // Get inspector info from the first location item if available
      if (locationsData.length > 0 && locationsData[0].inspector) {
        const inspector = locationsData[0].inspector;
        inspectorData.value = {
          id: inspector.id,
          name:
            inspector.fullname ||
            `${inspector.firstname || ""} ${inspector.lastname || ""}`,
          locations: locationsData,
        };
      } else if (route.query?.inspector_name) {
        // If no locations found but we have inspector name in query params
        inspectorData.value = {
          id: inspectorId,
          name: route.query.inspector_name,
          locations: [],
        };
      }

      // Map API response to expected format for the table
      locations.value = locationsData.map((item) => ({
        id: item.id,
        siteName: item.location?.name || "غير محدد",
        locationType: item.location?.location_type?.title || "-",
        licenseNumber:
          item.location?.license_no || item.location?.camp_number || "-",
        serviceCompany: item.company?.name || "-",
        parentCompany: item.company?.parent_company_name || "-",
        pilgrimsNationality: item.company?.pligrims_nationality || "-",
        raw: item, // Store the raw data for reference
      }));

      // Handle pagination data
      if (response.data.pagination) {
        totalItems.value = response.data.pagination.i_total_objects || 0;
        perPage.value = response.data.pagination.i_per_page || perPage.value;
        currentPage.value =
          response.data.pagination.i_current_page || currentPage.value;
      } else {
        totalItems.value = locationsData.length;
      }
    } else if (response.data?.message) {
      // Only show error toast if there's an actual error message from the API
      appStore.showSnackbar({
        message: response.data.message,
        color: "error",
      });
    }
  } catch (error) {
    console.error("Error fetching inspector locations:", error);
    // Only show error toast for actual errors, not for empty data
    if (error.response?.status !== 404) {
      appStore.showSnackbar({
        message:
          error.response?.data?.message ||
          error.message ||
          "حدث خطأ أثناء جلب بيانات المواقع",
        color: "error",
      });
    }

    // Clear data on error
    locations.value = [];
    totalItems.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Helper function to format time from datetime
const formatTime = (datetime) => {
  if (!datetime) return "-";

  const date = new Date(datetime);
  const hours = date.getHours();
  const minutes = date.getMinutes();
  const formattedMinutes = minutes < 10 ? `0${minutes}` : minutes;

  // Arabic format with AM/PM
  const suffix = hours >= 12 ? "م" : "ص";
  const displayHours = hours % 12 || 12;

  return `${displayHours}:${formattedMinutes} ${suffix}`;
};

// Handle search input with debounce
watch(
  () => filters.search,
  (newValue) => {
    if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
    debouncedSearch.value = setTimeout(() => {
      currentPage.value = 1; // Reset to first page when searching
      fetchInspectorLocations();
    }, 500); // 500ms debounce
  }
);

const exportData = async () => {
  try {
    isExporting.value = true;

    const response = await axios.post(
      `/attendance-export?tap=${filters.tap}`,
      {},
      {
        responseType: "blob", // Important to receive binary data
        ...filters,
      }
    );

    const url = window.URL.createObjectURL(new Blob([response.data]));

    const link = document.createElement("a");
    link.href = url;

    // Optional: Get file name from headers
    const disposition = response.headers["content-disposition"];
    let fileName = "سجل-الحضور-والانصراف.xlsx";
    if (disposition && disposition.includes("filename=")) {
      fileName = disposition.split("filename=")[1].replace(/['"]/g, "").trim();
    }

    link.setAttribute("download", fileName);
    document.body.appendChild(link);
    link.click();

    // Clean up
    link.remove();
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error("Download failed:", error);
    appStore.showSnackbar({
      message: "فشل تصدير البيانات",
      color: "error",
    });
  } finally {
    isExporting.value = false;
  }
};

const isGettingUtilities = ref(false);
const userObservers = ref([]);
const userOperators = ref([]);
const getUserObservers = async () => {
  return await axios.get(`/users?role_id=0&per_page=1000`);
};
const getUserOperators = async () => {
  return await axios.get(`/users?role_id=3&per_page=1000`);
};
const getUtilities = async () => {
  try {
    isGettingUtilities.value = true;
    const [observers, operators] = await Promise.all([
      getUserObservers(),
      getUserOperators(),
    ]);

    userObservers.value = (observers?.data?.data || []).map((item) => ({
      id: item.id,
      name: `${item.firstname} ${item.lastname}`,
    }));
    userOperators.value = (operators?.data?.data || []).map((item) => ({
      id: item.id,
      name: `${item.firstname} ${item.lastname}`,
    }));
  } catch (error) {
    console.error("Error fetching ratings:", error);
  } finally {
    isGettingUtilities.value = false;
  }
};

// Fetch locations data
const fetchLocations = async () => {
  try {
    isLoadingLocations.value = true;
    const response = await axios.get("/locations", {
      params: {
        per_page: 1000,
      },
    });

    if (response.data?.status?.success) {
      locations.value = response.data.data.map((location) => ({
        id: location.id,
        name: location.name,
        location_type: location.location_type,
        license_no: location.license_no,
        camp_number: location.camp_number,
        delegate_name: location.delegate_name,
        delegate_phone: location.delegate_phone,
        latitude: location.latitude,
        longitude: location.longitude,
      }));
    }
  } catch (error) {
    console.error("Error fetching locations:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء جلب بيانات المواقع",
      color: "error",
    });
  } finally {
    isLoadingLocations.value = false;
  }
};

// Fetch companies data
const fetchCompanies = async () => {
  try {
    isLoadingCompanies.value = true;
    const response = await axios.get("/companies", {
      params: {
        per_page: 1000,
      },
    });

    if (response.data?.status?.success) {
      companies.value = response.data.data.map((company) => ({
        id: company.id,
        name: company.name,
        parent_company_name: company.parent_company_name,
        pligrims_nationality: company.pligrims_nationality,
        contact_name: company.contact_name,
        phone: company.phone,
        email: company.email,
      }));
    }
  } catch (error) {
    console.error("Error fetching companies:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء جلب بيانات الشركات",
      color: "error",
    });
  } finally {
    isLoadingCompanies.value = false;
  }
};

onMounted(() => {
  fetchInspectorLocations();
  getUtilities();
  fetchLocations();
  fetchCompanies();
});

// New dummy data to match screenshot
const dummyData = [
  {
    id: 1,
    siteName: "حسن سالم الجعفري",
    street: "قباء مدينة الزائر",
    address: "مسجد ذو القبلتين",
    district: "قباء",
    serviceCompany: "إكرام الضيف للسياحة",
    ekramCompany: "إكرام الضيف للسياحة",
    permitNumber: "87 998",
    technical: "التقني",
  },
  {
    id: 2,
    siteName: "حسن سالم الجعفري",
    street: "قباء مدينة الزائر",
    address: "مسجد ذو القبلتين",
    district: "قباء",
    serviceCompany: "إكرام الضيف للسياحة",
    ekramCompany: "إكرام الضيف للسياحة",
    permitNumber: "87 998",
    technical: "التقني",
  },
  // Add more dummy items as needed
];

// Computed
const filteredData = computed(() => {
  let data = [...locations.value];

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    data = data.filter(
      (item) =>
        item.siteName.toLowerCase().includes(query) ||
        item.street.toLowerCase().includes(query) ||
        item.address.toLowerCase().includes(query) ||
        item.district.toLowerCase().includes(query) ||
        item.serviceCompany.toLowerCase().includes(query) ||
        item.permitNumber.toLowerCase().includes(query)
    );
  }

  return data;
});

const totalPages = computed(() => {
  return Math.ceil(locations.value.length / perPage.value);
});

const paginationInfo = computed(() => {
  const start = (currentPage.value - 1) * perPage.value + 1;
  const end = Math.min(start + perPage.value - 1, locations.value.length);
  return `${start} إلى ${end} من أصل ${locations.value.length}`;
});

// Methods
const viewDetails = (item) => {
  console.log("View details for:", item);
  // Will implement navigation to details page later
  // router.push(`/attributions/${item.id}`)
};

const handleAction = (action, item) => {
  if (action === "remove") {
    selectedItemForRemoval.value = item;
    showRemoveAssignmentModal.value = true;
  }
};

const handleRemoveAssignment = async () => {
  try {
    // Get inspector_id from route params
    const inspectorId = route.params.id;

    if (!inspectorId || !selectedItemForRemoval.value?.id) {
      throw new Error("Missing required data for unassignment");
    }

    // Prepare request body
    const requestData = {
      assign_id: selectedItemForRemoval.value.id,
      inspector_id: parseInt(inspectorId),
    };

    // Call API to remove the assignment
    const response = await axios.post(
      "/un-assign-inspector-location",
      requestData
    );

    // Close the modal first to prevent UI issues
    showRemoveAssignmentModal.value = false;

    if (response.data?.status?.success) {
      appStore.showSnackbar({
        message: "تم إلغاء الإسناد بنجاح",
        color: "success",
      });

      // Store the name for reference in case all items are removed
      const inspectorName = inspectorData.value.name;

      // Add a small delay to ensure the backend has processed the change
      setTimeout(async () => {
        try {
          // Pass inspector name in query params in case all items are removed
          if (!route.query.inspector_name) {
            router.replace({
              path: route.path,
              query: { ...route.query, inspector_name: inspectorName },
            });
          }

          // Refresh the data
          await fetchInspectorLocations();

          // Clear selection after successful data refresh
          selectedItemForRemoval.value = null;
        } catch (refreshError) {
          console.error(
            "Error refreshing data after unassignment:",
            refreshError
          );
        }
      }, 500);
    } else {
      throw new Error(response.data?.message || "حدث خطأ أثناء إلغاء الإسناد");
    }
  } catch (error) {
    console.error("Error removing assignment:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message ||
        error.message ||
        "حدث خطأ أثناء إلغاء الإسناد",
      color: "error",
    });
  }
};

const handleNewAssignment = async (formData) => {
  try {
    // Get inspector_id from route params
    const inspectorId = route.params.id;

    // Create assignment data
    const assignmentData = {
      inspector_ids: [parseInt(inspectorId)], // Changed to inspector_ids as an array
      location_id: formData.location_id?.id || formData.location_id,
      company_id: formData.company_id?.id || formData.company_id,
    };

    // Call API to create the assignment
    const response = await axios.post("/inspector-locations", assignmentData);

    if (response.data?.status?.success) {
      appStore.showSnackbar({
        message: "تم إسناد الموقع بنجاح",
        color: "success",
      });

      // Refresh the data
      await fetchInspectorLocations();
    } else {
      throw new Error(response.data?.message || "حدث خطأ أثناء إسناد الموقع");
    }

    // Reset form
    showNewAssignmentModal.value = false;
  } catch (error) {
    console.error("Error creating assignment:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message ||
        error.message ||
        "حدث خطأ أثناء إسناد الموقع",
      color: "error",
    });
  }
};

// Update breadcrumb title with inspector name
const breadcrumbItems = computed(() => [
  {
    title: "الرئيسية",
    disabled: false,
    to: "/",
  },
  {
    title: "الإسنادات",
    disabled: false,
    to: "/attributions",
  },
  {
    title: `المواقع المسندة للمراقب ${inspectorData.value.name}`,
    disabled: true,
  },
]);

const openAssignmentModal = () => {
  showNewAssignmentModal.value = true;
};

// Add these computed properties after other computed properties
const formattedLocations = computed(() => {
  return locations.value.map((location) => ({
    value: location.id,
    title: location.name,
    raw: {
      ...location,
      type: location.location_type?.title || "-",
      license: location.license_no || location.camp_number || "-",
      delegate: location.delegate_name || "-",
      phone: location.delegate_phone || "-",
    },
  }));
});

const formattedCompanies = computed(() => {
  return companies.value.map((company) => ({
    value: company.id,
    title: company.name,
    raw: {
      ...company,
      parentCompany: company.parent_company_name || "-",
      nationality: company.pligrims_nationality || "-",
      contact: company.contact_name || "-",
      phone: company.phone || "-",
    },
  }));
});

// Add these methods after other methods
const getLocationDetails = (locationId) => {
  const location = formattedLocations.value.find(
    (loc) => loc.value === locationId
  );
  return location?.raw || null;
};

const getCompanyDetails = (companyId) => {
  const company = formattedCompanies.value.find(
    (comp) => comp.value === companyId
  );
  return company?.raw || null;
};

const formatLocationDisplay = (location) => {
  if (!location) return "-";
  return `${location.name} (${location.location_type?.title || "-"})`;
};

const formatCompanyDisplay = (company) => {
  if (!company) return "-";
  return `${company.name} ${
    company.parent_company_name ? `- ${company.parent_company_name}` : ""
  }`;
};
</script>

<template>
  <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2 pt-0">
    <div class="mb-2">
      <v-breadcrumbs divider=">" :items="breadcrumbItems">
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

    <VCard elevation="0" class="rounded-lg px-2 pb-4 px-sm-4 bg-light-white">
      <v-card-title class="px-2 pt-4 pb-6">
        <div class="border-b pb-6">
          <div class="d-flex align-center gap-4 mb-6">
            <div
              class="d-flex align-start justify-space-between gap-4 flex-wrap w-100"
            >
              <h2 class="text-h6 font-weight-bold">
                المواقع المسندة للمراقب {{ inspectorData.name }}
              </h2>

              <div class="align-center ga-2 flex-wrap d-flex gap-4">
                <v-text-field
                  v-model="searchQuery"
                  label="البحث..."
                  rounded="lg"
                  clearable
                  height="46"
                  class="bg-white"
                  style="width: 260px; max-width: 100%"
                  prepend-inner-icon="mdi-magnify"
                  density="compact"
                  hide-details
                />
                <v-btn
                  color="primary"
                  @click="openAssignmentModal"
                  prepend-icon="mdi-plus"
                >
                  إسناد موقع
                </v-btn>
              </div>
            </div>
          </div>
        </div>
      </v-card-title>

      <v-card elevation="0" rounded="lg">
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
        <template v-else>
          <dataTable
            v-if="filteredData.length"
            :items="filteredData"
            :loading="isLoading"
            :page="currentPage"
            :items-per-page="perPage"
            :total-items="totalItems"
            :headers="tableHeaders"
            @update:options="handleTableUpdate"
          >
            <template #[`item.siteName`]="{ item }">
              <div class="d-flex align-center gap-2">
                <div>
                  <div class="font-weight-medium">{{ item.siteName }}</div>
                  <div class="text-caption text-medium-emphasis">
                    {{ item.delegateName }} - {{ item.delegatePhone }}
                  </div>
                </div>
              </div>
            </template>

            <template #[`item.serviceCompany`]="{ item }">
              <div>
                <div class="font-weight-medium">{{ item.serviceCompany }}</div>
                <div class="text-caption text-medium-emphasis">
                  {{ item.parentCompany }}
                </div>
              </div>
            </template>

            <template #[`item.locationType`]="{ item }">
              <v-chip
                size="small"
                :color="item.locationType === 'مخيم' ? 'primary' : 'success'"
                variant="flat"
              >
                {{ item.locationType }}
              </v-chip>
            </template>

            <template #[`item.licenseNumber`]="{ item }">
              <div class="font-weight-medium">{{ item.licenseNumber }}</div>
            </template>

            <template #pagination>
              <Pagination
                :total-items="totalItems"
                v-model:per-page="perPage"
                v-model:page="currentPage"
              />
            </template>

            <template #[`item.actions`]="{ item }">
              <v-btn
                color="error"
                variant="text"
                @click="handleAction('remove', item)"
                size="small"
                class="rounded-pill"
                style="background-color: #faeeeb; color: #c62800"
              >
                إلغاء الإسناد
                <svg
                  class="ms-1"
                  width="17"
                  height="16"
                  viewBox="0 0 17 16"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    d="M1.7207 7.99935C1.7207 11.9574 4.92933 15.166 8.88737 15.166C12.8454 15.166 16.054 11.9574 16.054 7.99935C16.054 4.04131 12.8454 0.832682 8.88737 0.832682C4.92933 0.832682 1.7207 4.04131 1.7207 7.99935Z"
                    fill="#C62800"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M11.3588 5.52829C11.6191 5.78864 11.6191 6.21075 11.3588 6.47109L9.83008 7.99967L11.3588 9.52825C11.6191 9.7886 11.6191 10.2107 11.3588 10.4711C11.0984 10.7314 10.6763 10.7314 10.416 10.4711L8.88724 8.94245L7.35876 10.4708C7.0984 10.7312 6.67629 10.7312 6.41595 10.4708C6.15561 10.2104 6.15562 9.78833 6.41598 9.52799L7.9444 7.99967L6.41598 6.47136C6.15562 6.21102 6.15561 5.78891 6.41595 5.52855C6.67629 5.26819 7.0984 5.26818 7.35876 5.52852L8.88724 7.0569L10.416 5.52825C10.6763 5.26791 11.0984 5.26793 11.3588 5.52829Z"
                    fill="#C62800"
                  />
                </svg>
              </v-btn>
            </template>
          </dataTable>

          <div
            v-else
            class="d-flex flex-column align-center justify-center py-16 min-h-500"
          >
            <svg
              width="128"
              height="106"
              viewBox="0 0 128 106"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <!-- ... SVG path data ... -->
            </svg>
            <div class="text-h6 font-weight-medium mt-4">
              لم يتم العثور على أي إسنادات.
            </div>
            <div class="text-body-1 text-medium-emphasis">
              لا توجد إسنادات متاحة في الوقت الحالي.
            </div>
          </div>
        </template>
      </v-card>
    </VCard>

    <!-- New Assignment Modal -->
    <AddSiteModal
      :model-value="showNewAssignmentModal"
      @update:model-value="showNewAssignmentModal = $event"
      :inspector-id="route.params.id"
      :locations="locations"
      :companies="companies"
      :loading-locations="isLoadingLocations"
      :loading-companies="isLoadingCompanies"
      @save="handleNewAssignment"
    />

    <!-- Existing Remove Assignment Modal -->
    <v-dialog v-model="showRemoveAssignmentModal" max-width="500px">
      <v-card class="rounded-lg">
        <v-card-title class="text-h5 pa-4"> تأكيد إلغاء الإسناد </v-card-title>

        <v-card-text class="text-center pa-4">
          <div class="mb-4">
            <svg
              width="140"
              height="145"
              viewBox="0 0 140 145"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M130 0H10C4.47715 0 0 4.47715 0 10V130C0 135.523 4.47715 140 10 140H130C135.523 140 140 135.523 140 130V10C140 4.47715 135.523 0 130 0Z"
                fill="#FFF0F0"
              />
              <path
                d="M115 145H25V48C29.242 47.9952 33.3089 46.308 36.3084 43.3085C39.308 40.3089 40.9952 36.242 41 32H99C98.9955 34.1014 99.4078 36.1828 100.213 38.1238C101.018 40.0648 102.2 41.8268 103.691 43.308C105.172 44.7991 106.934 45.9816 108.875 46.787C110.817 47.5924 112.898 48.0047 115 48V145Z"
                fill="white"
              />
              <path
                d="M70 97C83.2548 97 94 86.2548 94 73C94 59.7452 83.2548 49 70 49C56.7452 49 46 59.7452 46 73C46 86.2548 56.7452 97 70 97Z"
                fill="#C05E5E"
              />
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M70.0883 59.168H70.0029C69.3292 59.168 68.7577 59.1679 68.2834 59.2111C67.7802 59.2569 67.3114 59.3562 66.8581 59.5972C66.6792 59.6923 66.5085 59.8021 66.3477 59.9254C65.9404 60.2379 65.6556 60.6233 65.4052 61.0623C65.1692 61.4759 64.9322 61.9959 64.6528 62.6089L64.0941 63.8346H60.0018C60.0012 63.8346 60.0007 63.8346 60.0002 63.8346H58C57.4477 63.8346 57 64.2824 57 64.8346C57 65.3869 57.4477 65.8346 58 65.8346H59.0586L59.8111 78.3001C59.9117 79.9663 59.9915 81.2885 60.1575 82.3447C60.3278 83.4278 60.6016 84.3298 61.1508 85.119C61.6532 85.8409 62.3005 86.4503 63.0514 86.9083C63.8722 87.4089 64.7891 87.6279 65.8805 87.7326C66.9448 87.8347 68.2693 87.8347 69.9385 87.8347H70.042C71.709 87.8347 73.032 87.8347 74.0949 87.7328C75.1851 87.6283 76.101 87.4097 76.9212 86.91C77.6717 86.4529 78.3187 85.8446 78.8213 85.1238C79.3707 84.3359 79.6453 83.4352 79.8169 82.3536C79.9842 81.2989 80.0658 79.9786 80.1686 78.3148L80.9401 65.8346H82C82.5523 65.8346 83 65.3869 83 64.8346C83 64.2824 82.5523 63.8346 82 63.8346H80.0009C79.9997 63.8346 79.9985 63.8346 79.9973 63.8346H76.0342L75.3788 62.4828C75.0922 61.8915 74.8489 61.3895 74.6094 60.9905C74.3551 60.5669 74.0693 60.1956 73.6668 59.8954C73.5078 59.7768 73.3393 59.6713 73.1632 59.5799C72.7175 59.3487 72.2588 59.2534 71.7667 59.2094C71.3031 59.1679 70.7453 59.168 70.0883 59.168ZM78.9363 65.8346H75.4297C75.4154 65.8349 75.4012 65.8349 75.387 65.8346H64.7587C64.744 65.835 64.7293 65.835 64.7146 65.8346H61.0622L61.8044 78.1279C61.9088 79.8573 61.9839 81.0837 62.1333 82.034C62.2802 82.9683 62.4859 83.5361 62.7924 83.9766C63.1362 84.4705 63.579 84.8875 64.0928 85.2008C64.551 85.4803 65.1301 85.6514 66.0715 85.7417C67.0291 85.8336 68.2578 85.8347 69.9904 85.8347C71.7206 85.8347 72.9476 85.8336 73.9041 85.7419C74.8443 85.6518 75.4228 85.481 75.8807 85.202C76.3941 84.8892 76.8369 84.473 77.1808 83.9799C77.4874 83.54 77.6936 82.9732 77.8416 82.0403C77.9921 81.0914 78.0689 79.8667 78.1756 78.1398L78.9363 65.8346ZM73.8116 63.8346H66.2921L66.4547 63.4778C66.7569 62.8149 66.9568 62.3786 67.1424 62.0534C67.3185 61.7447 67.4449 61.6045 67.5651 61.5123C67.6381 61.4562 67.7157 61.4063 67.7971 61.3631C67.9308 61.2919 68.1108 61.2351 68.4648 61.2029C68.8377 61.1689 69.3176 61.168 70.046 61.168C70.7567 61.168 71.2246 61.1689 71.5885 61.2014C71.9339 61.2323 72.1104 61.2868 72.2422 61.3552C72.3222 61.3968 72.3988 61.4447 72.4711 61.4986C72.5901 61.5874 72.7162 61.7225 72.8946 62.0198C73.0826 62.3331 73.2876 62.7537 73.5976 63.3931L73.8116 63.8346Z"
                fill="white"
              />
              <path
                d="M83 103H57C55.3431 103 54 104.343 54 106C54 107.657 55.3431 109 57 109H83C84.6569 109 86 107.657 86 106C86 104.343 84.6569 103 83 103Z"
                fill="#FFF0F0"
              />
              <path
                d="M92 115H48C46.3431 115 45 116.343 45 118C45 119.657 46.3431 121 48 121H92C93.6569 121 95 119.657 95 118C95 116.343 93.6569 115 92 115Z"
                fill="#FFF0F0"
              />
            </svg>
          </div>
          <div class="text-h6 mb-2">
            هل أنت متأكد من إلغاء إسناد هذا الموقع للمراقب؟
          </div>
          <div v-if="selectedItemForRemoval" class="text-subtitle-1 mb-2">
            <strong>اسم الموقع:</strong> {{ selectedItemForRemoval.siteName }}
          </div>
          <div v-if="selectedItemForRemoval" class="text-subtitle-1 mb-2">
            <strong>شركة تقديم الخدمة:</strong>
            {{ selectedItemForRemoval.serviceCompany }}
          </div>
          <div v-if="selectedItemForRemoval" class="text-subtitle-1 mb-2">
            <strong>نوع الموقع:</strong>
            {{ selectedItemForRemoval.locationType }}
          </div>
          <div v-if="selectedItemForRemoval" class="text-subtitle-1 mb-2">
            <strong>جنسية الحجاج:</strong>
            {{ selectedItemForRemoval.pilgrimsNationality }}
          </div>
          <div class="text-body-1 text-medium-emphasis">
            بمجرد تأكيدك، سيتم إزالة ارتباط المراقب بهذا الموقع ولن يكون مسؤولاً
            عنه بعد الآن.
          </div>
        </v-card-text>

        <v-card-actions class="pa-4">
          <v-spacer></v-spacer>
          <v-btn
            color="default"
            variant="text"
            @click="showRemoveAssignmentModal = false"
          >
            إلغاء
          </v-btn>
          <v-btn
            color="error"
            @click="handleRemoveAssignment"
            :loading="isLoading"
          >
            تأكيد الإلغاء
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.bg-light-gray {
  background-color: rgba(248, 249, 249, 1) !important;
}

.text-gray-1 {
  color: rgba(124, 133, 139, 1) !important;
}

.border {
  border: 1px solid rgba(0, 0, 0, 0.12) !important;
}

.responsive-text {
  font-size: 14px;
  @media (max-width: 600px) {
    font-size: 12px;
  }
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
    text-align: center !important;

    &:first-child {
      border-top-right-radius: 8px;
      border-bottom-right-radius: 8px;
    }

    &:last-child {
      border-top-left-radius: 8px;
      border-bottom-left-radius: 8px;
    }
  }

  .v-data-table__th {
    text-align: center !important;
  }
}

:deep(.v-table > .v-table__wrapper > table) {
  border-spacing: 0 16px !important;
  border-collapse: separate !important;
}

.v-table {
  --v-table-header-height: 50px;

  th {
    font-size: 0.875rem;
    color: rgba(var(--v-theme-on-surface), var(--v-medium-emphasis-opacity));
    font-weight: 500;
    white-space: nowrap;
  }
}

.text-error {
  color: rgb(var(--v-theme-error)) !important;
}

.bg-error-lighten-5 {
  background-color: rgb(var(--v-theme-error), 0.05) !important;
}

.v-list-item {
  &.text-error {
    .v-list-item-title {
      color: rgb(var(--v-theme-error)) !important;
    }
  }
}

// Add new styles for the dialog
.v-dialog {
  .v-card {
    border-radius: 16px;

    .v-card-title {
      border-bottom: 1px solid rgba(0, 0, 0, 0.12);
      font-size: 1.25rem;
      font-weight: 600;
    }
  }
}

// Add new styles for the SVG icon
.action-svg {
  width: 24px;
  height: 24px;
  margin-right: 8px;
}
</style>
