<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import dayjs from "dayjs";
// import CustomVSelect from "@core/components/global/CustomVSelect.vue";
import dataTable from "@/components/dataTable.vue";
import axios from "@/plugins/axios";
// import CustomVAutocomplete from "@/@core/components/global/CustomVAutocomplete.vue";
import { ref, computed, reactive, onMounted } from "vue";
import { useRouter } from "vue-router";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const dateFilter = ref(null);
const totalItems = ref(20);
const totalPages = ref(1);
const isExporting = ref(false);
const isGettingFilters = ref(false);
const isDatepickerMenu = ref(false);
const attendanceData = ref([]);
const debouncedSearch = ref(null);
const router = useRouter();
const activeTab = ref("assigned");
const searchQuery = ref("");
const showRemoveAssignmentModal = ref(false);
const selectedItemForRemoval = ref(null);
const availableTypes = ref([
  { title: "الكل", value: "all" },
  { title: "مخيم", value: "1" },
  { title: "مسكن", value: "2" },
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
  // tap: "daily",
});

watch(currentPage, () => {
  fetchAttendance();
});

// Add watcher for perPage changes
watch(perPage, () => {
  currentPage.value = 1; // Reset to first page when changing page size
  fetchAttendance();
});

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
  fetchAttendance(); // Trigger data refresh when clearing the date filter
};

// const handleTableUpdate = (options) => {
//   // Update pagination values and trigger data reload
//   if (options.page !== currentPage.value) {
//     currentPage.value = options.page;
//   }

//   if (options.itemsPerPage !== perPage.value) {
//     perPage.value = options.itemsPerPage;
//   }

//   // Don't trigger fetchAttendance here - the watchers will handle it
// };

const tableHeaders = [
  { title: "اسم الموقع", key: "siteName", sortable: false },
  { title: "نوع الموقع", key: "locationType" },
  { title: "رقم المخيم", key: "campNumber" },
  { title: "رقم البوابة", key: "gateNumber" },
  { title: "اسم المراقب", key: "observerName" },
  { title: "رقم جوال المراقب", key: "observerPhone" },
  { title: "اسم الشركة الرئيسية", key: "mainCompany" },
  { title: "جنسية الحجاج", key: "pilgrimsNationality" },
  { title: "عدد التقييمات", key: "ratingsCount" },
  { title: "الإجراءات", key: "actions" },
];

// Watch for changes in activeTab
watch(
  () => activeTab.value,
  () => {
    currentPage.value = 1; // Reset to first page when switching tabs
    fetchAttendance();
  }
);

// Watch for changes in currentPage or perPage
watch(
  [currentPage, perPage],
  () => {
    fetchAttendance();
  },
  { immediate: false }
);

// Watch for filter changes
watch(
  () => [
    filters.status,
    filters.user_id,
    filters.attendance_by_user_id,
    filters.tap,
    filters.created_at,
    filters.location_type_id,
  ],
  () => {
    currentPage.value = 1; // Reset to first page when filters change
    fetchAttendance();
  },
  { deep: true }
);

// Separate watch for search filter with debounce
watch(
  () => searchQuery.value,
  (newValue) => {
    if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
    debouncedSearch.value = setTimeout(() => {
      filters.search = newValue;
      currentPage.value = 1; // Reset to first page when searching
      fetchAttendance();
    }, 500); // 500ms debounce
  }
);

// Watch for type filter changes
watch(
  () => type_render.value,
  (newValue) => {
    if (newValue === "all") {
      filters.location_type_id = "";
    } else {
      filters.location_type_id = newValue;
    }
    currentPage.value = 1; // Reset to first page when type changes
    fetchAttendance();
  }
);

const fetchAttendance = async () => {
  try {
    isLoading.value = true;

    // Prepare query parameters with formatted date
    const params = {
      per_page: perPage.value,
      page: currentPage.value,
      ...filters,
    };

    // Format the created_at date properly if it exists
    if (params.created_at) {
      params.created_at = formatDate(params.created_at);
    }

    // Remove undefined values and empty strings
    Object.keys(params).forEach((key) => {
      if (params[key] === undefined || params[key] === "") {
        delete params[key];
      }
    });

    // Use different endpoints based on the active tab
    const endpoint =
      activeTab.value === "assigned"
        ? "assign-locations"
        : "un-assign-locations";

    const response = await axios.get(endpoint, { params });

    if (response.data?.status?.success) {
      // Map API response to expected format for the table
      attendanceData.value = response.data.data.map((item) => {
        // Get location type from the location_type object if available
        const locationType =
          item.location?.location_type?.title ||
          (item.location?.location_type_id === 1
            ? "مخيم"
            : item.location?.location_type_id === 2
            ? "مسكن"
            : "");

        // Common properties for both assigned and unassigned
        const mappedItem = {
          id: item.id,
          siteName: item.location?.name || "",
          locationType: locationType,
          mainCompany:
            item.company?.parent_company?.name ||
            item.company?.parent_company_name ||
            "",
          permitNumber: item.location?.license_no || "",
          pilgrimsNationality: item.company?.pligrims_nationality || "",
          campNumber: item.location?.camp_number || "",
          squareNumber: item.location?.square_number || "",
          gateNumber: item.location?.gate_number || "",
          delegateName: item.location?.delegate_name || "",
          delegatePhone: item.location?.delegate_phone || "",
          serviceCenterNo: item.location?.service_center_no || "",
          serviceCenterPhone: item.location?.service_center_phone || "",
          ratingsCount: item.total_ratings || item.company?.ratings_count || 0,
          raw: item, // Store the raw data for reference if needed
        };

        // Add observer (inspector) data if available (for assigned locations)
        if (item.inspector) {
          mappedItem.observerName = `${item.inspector.firstname} ${item.inspector.lastname}`;
          mappedItem.observerAvatar =
            item.inspector.avatar_url || "/imgs/person.svg";
          mappedItem.observerPhone = item.inspector.phone || "";
          mappedItem.observerId = item.inspector.id;
        } else {
          mappedItem.observerName = "";
          mappedItem.observerAvatar = "/imgs/person.svg";
          mappedItem.observerPhone = "";
          mappedItem.observerId = null;
        }

        return mappedItem;
      });

      console.log(attendanceData.value);

      // Handle pagination data from API response
      if (response.data.pagination) {
        totalItems.value = response.data.pagination.i_total_objects || 0;
        totalPages.value = response.data.pagination.i_total_pages || 1;
        perPage.value = response.data.pagination.i_per_page || perPage.value;
        currentPage.value =
          response.data.pagination.i_current_page || currentPage.value;
      }
    }
  } catch (error) {
    console.error("Error fetching locations:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    // Reset data on error
    attendanceData.value = [];
    totalItems.value = 0;
    totalPages.value = 1;
  } finally {
    isLoading.value = false;
  }
};

// Helper function to format time from datetime
// const formatTime = (datetime) => {
//   if (!datetime) return "-";

//   const date = new Date(datetime);
//   const hours = date.getHours();
//   const minutes = date.getMinutes();
//   const formattedMinutes = minutes < 10 ? `0${minutes}` : minutes;

//   // Arabic format with AM/PM
//   const suffix = hours >= 12 ? "م" : "ص";
//   const displayHours = hours % 12 || 12;

//   return `${displayHours}:${formattedMinutes} ${suffix}`;
// };

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

onMounted(() => {
  fetchAttendance();
  getUtilities();
});

// // Dummy data
// const dummyData = [
//   {
//     id: 1,
//     siteName: "حسن سالم الجعفري",
//     observerName: "سعود الغامدي",
//     observerAvatar: "/imgs/person.svg",
//     observerPhone: "+996 542 223 33",
//     mainCompany: "شركة روافد مكة",
//     permitNumber: "87 998",
//     pilgrimsNationality: "مصرية",
//     pilgrimsCount: "87 998",
//     ratingsCount: 45,
//   },
//   {
//     id: 2,
//     siteName: "حسن سالم الجعفري",
//     observerName: "سعود الغامدي",
//     observerAvatar: "/imgs/person.svg",
//     observerPhone: "+996 542 223 33",
//     mainCompany: "الرفادة لخدمات الحجاج",
//     permitNumber: "87 998",
//     pilgrimsNationality: "مصرية",
//     pilgrimsCount: "87 998",
//     ratingsCount: 23,
//   },
//   {
//     id: 3,
//     siteName: "حسن سالم الجعفري",
//     observerName: "سعود الغامدي",
//     observerAvatar: "/imgs/person.svg",
//     observerPhone: "+996 542 223 33",
//     mainCompany: "إكرام الضيف للسياحة",
//     permitNumber: "87 998",
//     pilgrimsNationality: "مصرية",
//     pilgrimsCount: "87 998",
//     ratingsCount: 76,
//   },
// ];

// Update the filteredData computed property to simplify it since we're doing server-side filtering
const filteredData = computed(() => {
  if (attendanceData.value && attendanceData.value.length > 0) {
    return attendanceData.value;
  }

  // // Fallback to dummy data if API data is not available
  // return dummyData.filter((item) => {
  //   // Apply filtering based on active tab
  //   if (activeTab.value === "assigned") {
  //     return Boolean(item.observerName); // Has observer assigned
  //   } else {
  //     return !Boolean(item.observerName); // No observer assigned
  //   }
  // });
  return [];
});

// const paginationInfo = computed(() => {
//   const start = (currentPage.value - 1) * perPage.value + 1;
//   const end = Math.min(start + perPage.value - 1, dummyData.length);
//   return `${start} إلى ${end} من أصل ${dummyData.length}`;
// });

// // Methods
// const viewDetails = (item) => {
//   console.log("View details for:", item);
//   // Will implement navigation to details page later
//   // router.push(`/attributions/${item.id}`)
// };

const handleViewDt = (action, item) => {
  // Use the raw item id from the API data
  const locationId = item.raw?.location_id || item.raw?.location?.id || item.id;
  const companyId = item.raw?.company_id || item.raw?.company?.id;
  router.push({
    path: `/attributions/residence/${locationId}`,
    query: {
      company_id: companyId,
      location_id: locationId,
    },
  });
};

const handleRemoveAssignment = async () => {
  try {
    // Call API to remove the assignment
    await axios.delete(`/attributions/${selectedItemForRemoval.value.id}`);
    showRemoveAssignmentModal.value = false;
    selectedItemForRemoval.value = null;
    // Refresh the data
    await fetchAttendance();
    appStore.showSnackbar({
      message: "تم إلغاء الإسناد بنجاح",
      color: "success",
    });
  } catch (error) {
    console.error("Error removing assignment:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء إلغاء الإسناد",
      color: "error",
    });
  }
};
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
            title: 'الإسنادات',
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

    <VCard elevation="0" class="rounded-lg px-2 px-sm-4 bg-light-white">
      <v-card-title class="px-2 pt-4 pb-6">
        <div class="border-b pb-6">
          <div class="d-flex align-center gap-4 mb-6">
            <div
              class="d-flex align-start justify-space-between gap-4 flex-wrap w-100"
            >
              <h2 class="text-h6 font-weight-bold">الإسنادات</h2>

              <div class="align-center ga-2 flex-wrap d-flex">
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
              </div>
            </div>
          </div>

          <div class="d-flex justify-space-between align-center">
            <div
              class="d-flex gap-2 border px-2 py-1"
              style="border-radius: 16px; max-width: fit-content"
            >
              <v-btn
                v-for="tab in [
                  { id: 'assigned', label: 'المواقع المسندة' },
                  { id: 'unassigned', label: 'المواقع غير المسندة' },
                ]"
                :key="tab.id"
                elevation="0"
                class="rounded-lg font-weight-bold"
                :class="{
                  'bg-light-gray text-primary': activeTab === tab.id,
                  'bg-white text-gray-1': activeTab !== tab.id,
                }"
                @click="activeTab = tab.id"
              >
                <span class="responsive-text">{{ tab.label }}</span>
              </v-btn>
            </div>

            <div>
              <v-select
                v-model="type_render"
                :items="availableTypes"
                variant="outlined"
                density="comfortable"
                placeholder="نوع الموقع"
                hide-details
                class="rounded-lg bg-grey-lighten-4 px-2"
                style="min-width: 150px"
              />
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
          <div
            v-if="filteredData.length === 0 && !isLoading"
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
              {{
                searchQuery
                  ? "لم يتم العثور على أي مواقع تطابق معايير البحث."
                  : "لم يتم العثور على أي إسنادات."
              }}
            </div>
            <div class="text-body-1 text-medium-emphasis">
              {{
                activeTab === "assigned"
                  ? "لا توجد مواقع مسندة متاحة في الوقت الحالي."
                  : "لا توجد مواقع غير مسندة متاحة في الوقت الحالي."
              }}
            </div>
          </div>

          <dataTable
            v-else
            :items="filteredData"
            :loading="isLoading"
            :headers="tableHeaders"
          >
            <!-- :page="currentPage"
            :items-per-page="perPage"
            :total-items="totalItems" -->
            <!-- @update:options="handleTableUpdate" -->
            <template #[`item.siteName`]="{ item }">
              <div class="d-flex align-center gap-2">
                <div class="font-weight-medium">{{ item.siteName }}</div>
              </div>
            </template>

            <template #[`item.locationType`]="{ item }">
              <v-chip
                size="small"
                :color="item.locationType === 'مخيم' ? 'primary' : 'secondary'"
                variant="outlined"
                class="font-weight-medium"
              >
                {{ item.locationType }}
              </v-chip>
            </template>

            <template #[`item.campNumber`]="{ item }">
              <div>{{ item.campNumber || "-" }}</div>
            </template>

            <template #[`item.gateNumber`]="{ item }">
              <div>{{ item.gateNumber || "-" }}</div>
            </template>

            <template #[`item.observerName`]="{ item }">
              <div class="d-flex align-center gap-2" v-if="item.observerName">
                <!-- <v-avatar size="32" rounded="lg">
                  <v-img :src="item.observerAvatar || '/imgs/person.svg'" />
                </v-avatar> -->
                <div>{{ item.observerName }}</div>
              </div>
              <div v-else class="text-medium-emphasis">غير مسند</div>
            </template>

            <template #[`item.observerPhone`]="{ item }">
              <div v-if="item.observerPhone">{{ item.observerPhone }}</div>
              <div v-else class="text-medium-emphasis">-</div>
            </template>

            <template #[`item.mainCompany`]="{ item }">
              <div>{{ item.mainCompany || "-" }}</div>
            </template>

            <template #[`item.pilgrimsNationality`]="{ item }">
              <div>{{ item.pilgrimsNationality || "-" }}</div>
            </template>

            <template #[`item.ratingsCount`]="{ item }">
              <div class="d-flex align-center">
                {{ item.ratingsCount }}
              </div>
            </template>

            <template #pagination>
              <div class="d-flex justify-end py-3">
                <Pagination
                  :total-items="totalItems"
                  :total-pages="totalPages"
                  v-model:per-page="perPage"
                  v-model:page="currentPage"
                />
                <!-- <Pagination
                  :total-items="totalItems"
                  v-model:per-page="perPage"
                  v-model:page="currentPage"
                /> -->
              </div>
            </template>

            <template #[`item.actions`]="{ item }">
              <div class="d-flex align-center">
                <v-tooltip text="عرض التفاصيل" location="top">
                  <template v-slot:activator="{ props }">
                    <div
                      v-bind="props"
                      @click="handleViewDt('view', item)"
                      class="cursor-pointer mx-1"
                    >
                      <svg
                        width="32"
                        height="32"
                        viewBox="0 0 40 40"
                        fill="none"
                        xmlns="http://www.w3.org/2000/svg"
                      >
                        <path
                          d="M10.667 0.333008H29.333C35.0399 0.333008 39.667 4.96005 39.667 10.667V29.333C39.667 35.0399 35.0399 39.667 29.333 39.667H10.667C4.96005 39.667 0.333008 35.0399 0.333008 29.333V10.667C0.333008 4.96005 4.96005 0.333008 10.667 0.333008Z"
                          stroke="#E9EBEC"
                          stroke-width="0.666667"
                        />
                        <path
                          opacity="0.4"
                          fill-rule="evenodd"
                          clip-rule="evenodd"
                          d="M26.6673 20.0007C26.6673 19.7903 26.566 19.6482 26.3633 19.364C25.4526 18.0869 23.1268 15.334 20.0007 15.334C16.8745 15.334 14.5487 18.0869 13.638 19.364C13.4353 19.6482 13.334 19.7903 13.334 20.0007C13.334 20.211 13.4353 20.3531 13.638 20.6373C14.5487 21.9144 16.8745 24.6673 20.0007 24.6673C23.1268 24.6673 25.4526 21.9144 26.3633 20.6373C26.566 20.3531 26.6673 20.211 26.6673 20.0007ZM20.0007 18.0007C21.1052 18.0007 22.0007 18.8961 22.0007 20.0007C22.0007 21.1052 21.1052 22.0007 20.0007 22.0007C18.8961 22.0007 18.0007 21.1052 18.0007 20.0007C18.0007 18.8961 18.8961 18.0007 20.0007 18.0007Z"
                          fill="#B69265"
                        />
                        <path
                          fill-rule="evenodd"
                          clip-rule="evenodd"
                          d="M20.0007 22.5007C18.6199 22.5007 17.5007 21.3814 17.5007 20.0007C17.5007 18.6199 18.6199 17.5007 20.0007 17.5007C21.3814 17.5007 22.5007 18.6199 22.5007 20.0007C22.5007 21.3814 21.3814 22.5007 20.0007 22.5007ZM18.5007 20.0007C18.5007 20.8291 19.1722 21.5007 20.0007 21.5007C20.8291 21.5007 21.5007 20.8291 21.5007 20.0007C21.5007 19.1722 20.8291 18.5007 20.0007 18.5007C19.1722 18.5007 18.5007 19.1722 18.5007 20.0007Z"
                          fill="#B69265"
                        />
                        <path
                          fill-rule="evenodd"
                          clip-rule="evenodd"
                          d="M20.0007 14.834C18.2869 14.834 16.818 15.5883 15.6809 16.476C14.5427 17.3645 13.7015 18.4138 13.2309 19.0737L13.1954 19.1231C13.0267 19.3579 12.834 19.6258 12.834 20.0007C12.834 20.3755 13.0267 20.6434 13.1954 20.8782L13.2309 20.9276C13.7015 21.5875 14.5427 22.6368 15.6809 23.5253C16.818 24.413 18.2869 25.1673 20.0007 25.1673C21.7144 25.1673 23.1834 24.413 24.3204 23.5253C25.4586 22.6368 26.2998 21.5875 26.7704 20.9276L26.8059 20.8782C26.9746 20.6434 27.1673 20.3755 27.1673 20.0007C27.1673 19.6258 26.9746 19.3579 26.8059 19.1231L26.7704 19.0737C26.2998 18.4138 25.4586 17.3645 24.3204 16.476C23.1833 15.5883 21.7144 14.834 20.0007 14.834ZM14.0451 19.6543C14.4852 19.0371 15.2622 18.0714 16.2962 17.2642C17.3312 16.4562 18.5882 15.834 20.0007 15.834C21.4131 15.834 22.6701 16.4562 23.7051 17.2642C24.7391 18.0714 25.5161 19.0371 25.9562 19.6543C26.0651 19.807 26.1169 19.8815 26.1478 19.9409C26.1674 19.9786 26.1674 19.9889 26.1673 19.9991L26.1673 20.0007L26.1673 20.0022C26.1674 20.0124 26.1674 20.0227 26.1478 20.0604C26.1169 20.1198 26.0651 20.1943 25.9562 20.347C25.5161 20.9642 24.7391 21.9299 23.7051 22.7371C22.6701 23.5451 21.4131 24.1673 20.0007 24.1673C18.5882 24.1673 17.3312 23.5451 16.2962 22.7371C15.2622 21.9299 14.4852 20.9642 14.0451 20.347C13.9362 20.1943 13.8844 20.1198 13.8535 20.0604C13.8339 20.0227 13.8339 20.0124 13.834 20.0022L13.834 20.0007L13.834 19.9991C13.8339 19.9889 13.8339 19.9786 13.8535 19.9409C13.8844 19.8815 13.9362 19.807 14.0451 19.6543Z"
                          fill="#B69265"
                        />
                      </svg>
                    </div>
                  </template>
                </v-tooltip>
              </div>
            </template>
          </dataTable>
        </template>
      </v-card>
    </VCard>

    <!-- Confirmation Modal -->
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

// Add new styles for the SVG icon
.action-svg {
  width: 24px;
  height: 24px;
  margin-right: 8px;
}
</style>
