<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import dayjs from "dayjs";
import CustomVSelect from "@core/components/global/CustomVSelect.vue";
import dataTable from "@/components/dataTable.vue";
import axios from "@/plugins/axios";
import CustomVAutocomplete from "@/@core/components/global/CustomVAutocomplete.vue";

const appStore = useAppStore();
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

const statusItems = [
  { title: "الكل", value: "all" },
  { title: "حاضر", value: "attendance" },
  { title: "غائب", value: "absents" },
  // { title: "متأخر", value: "late" },
];

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

// Helper function to format date for API
const formatDate = (date) => {
  if (!date) return "";

  // Create a date object with timezone handling
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

const handleTableUpdate = (options) => {
  currentPage.value = options.page;
  perPage.value = options.itemsPerPage;
  // You can add additional logic here for sorting or filtering
};

const tableHeaders = [
  { title: "الاسم", key: "name", sortable: false },
  { title: "رقم الهوية", key: "id_number" },
  { title: "الوردية", key: "shift" },
  { title: "وقت الحضور", key: "checkIn" },
  { title: "وقت الانصراف", key: "checkOut" },
  { title: "أيام الحضور", key: "attendanceDays" },
  { title: "أيام الغياب", key: "absenceDays" },
  { title: "اسم مسؤول التحضير", key: "supervisor" },
  { title: "تم التحضير بواسطة", key: "checkedBy" },
];

// Watch for changes in currentPage or perPage
watch([currentPage, perPage], () => {
  fetchAttendance();
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
    fetchAttendance();
  },
  { deep: true }
);

// Separate watch for date filter to handle date-specific logic
watch(
  () => dateFilter.value,
  (newValue) => {
    if (newValue) {
      fetchAttendance();
    }
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

    // Remove undefined values
    Object.keys(params).forEach((key) => {
      if (params[key] === undefined) {
        delete params[key];
      }
    });

    const response = await axios.get("attendance", { params });

    if (response.data?.status?.success) {
      // Map API response to expected format for the table
      attendanceDataa.value = response.data.data.map((item) => ({
        id: item.id,
        name: `${item.user.firstname} ${item.user.lastname}`,
        role:
          item.user.role.title === "INSPECTOR"
            ? "مراقب"
            : item.user.role.title === "SUPERVISOR"
            ? "مشرف"
            : item.user.role.title,
        id_number: item.user.sa_id,
        shift: item.shift?.name || "",
        checkIn: item.attendance_datetime
          ? formatTime(item.attendance_datetime)
          : "-",
        checkOut: item.departure_datetime
          ? formatTime(item.departure_datetime)
          : "-",
        attendanceDays: item.attendance_days?.toString(),
        absenceDays: item.absence_days?.toString(),

        supervisor: item.supervisor
          ? `${item.supervisor.firstname} ${item.supervisor.lastname}`
          : "-",
        checkedBy: item.attendance_by
          ? `${item.attendance_by.firstname} ${item.attendance_by.lastname}`
          : "-",
        avatar: item.user.avatar_url || "/imgs/person.svg",
        raw: item, // Store the raw data for reference if needed
      }));

      // Handle pagination data
      if (response.data.pagination) {
        totalItems.value = response.data.pagination.i_total_objects || 0;
        perPage.value = response.data.pagination.i_per_page || perPage.value;
        currentPage.value =
          response.data.pagination.i_current_page || currentPage.value;
      } else {
        totalItems.value =
          response.data.meta?.total || response.data.data.length;
      }
    }
  } catch (error) {
    console.error("Error fetching attendance:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
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
      fetchAttendance();
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

onMounted(() => {
  fetchAttendance();
  getUtilities();
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
            title: 'سجل الحضور و الإنصراف',
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

    <VCard elevation="0" class="rounded-lg px-2 px-sm-4 bg-light-white">
      <v-card-title class="px-2 pt-4 pb-6">
        <div class="border-b pb-6">
          <div class="d-flex align-center gap-4 mb-6">
            <div
              class="d-flex align-start justify-space-between gap-4 flex-wrap w-100"
            >
              <h2 class="text-h6 font-weight-bold">سجل الحضور و الإنصراف</h2>

              <div class="align-center ga-2 flex-wrap d-flex">
                <v-text-field
                  v-model="filters.search"
                  label="البحث"
                  rounded="lg"
                  clearable
                  height="46"
                  class="bg-white"
                  style="
                    width: 260px;
                    max-width: 100%;
                    color: #929a9f !important;
                    border-radius: 1rem;
                  "
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

                <CustomVSelect
                  v-model="filters.status"
                  :items="statusItems"
                  min-width="160"
                  max-width="170"
                  :return-object="false"
                  placeholder="الحالة"
                  :loading="isGettingFilters"
                  item-title="title"
                  item-value="value"
                />

                <CustomVAutocomplete
                  v-model="filters.attendance_by_user_id"
                  :items="userObservers"
                  min-width="160"
                  max-width="170"
                  :return-object="false"
                  placeholder="المحضر"
                  :loading="isGettingFilters"
                  item-title="name"
                  item-value="id"
                />

                <CustomVAutocomplete
                  v-model="filters.user_id"
                  :items="userOperators"
                  min-width="160"
                  max-width="170"
                  :return-object="false"
                  placeholder="المتحضر"
                  :loading="isGettingFilters"
                  item-title="name"
                  item-value="id"
                />

                <v-menu
                  v-model="isDatepickerMenu"
                  :close-on-content-click="false"
                  transition="scale-transition"
                  offset-y
                  min-width="auto"
                >
                  <template v-slot:activator="{ props }">
                    <v-text-field
                      :value="
                        filters.created_at
                          ? displayFormattedDate(filters.created_at)
                          : ''
                      "
                      placeholder="التاريخ"
                      v-bind="props"
                      min-width="130"
                      max-width="170"
                      height="50"
                      hide-details
                      hide-no-data
                      variant="filled"
                      bg-color="#F8F9F9"
                      class="v-filter-input"
                      clearable
                      @click:clear="onClearDateFilter"
                    >
                      <template #append-inner>
                        <template v-if="filters.created_at">
                          <v-icon
                            @click="onClearDateFilter"
                            color="#9EA1A3"
                            size="small"
                            >mdi-close-circle</v-icon
                          >
                        </template>
                        <template v-else>
                          <svg
                            width="20"
                            height="20"
                            viewBox="0 0 20 20"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                          >
                            <path
                              opacity="0.4"
                              fill-rule="evenodd"
                              clip-rule="evenodd"
                              d="M10.8573 1.66602H9.14269C7.51912 1.66599 6.20951 1.66597 5.17944 1.81569C4.10445 1.97193 3.214 2.30586 2.51502 3.06152C1.82458 3.80794 1.52732 4.74328 1.3868 5.87323C1.24997 6.97347 1.24998 8.37774 1.25 10.1442V10.6878C1.24998 12.4543 1.24997 13.8586 1.3868 14.9588C1.52732 16.0888 1.82458 17.0241 2.51502 17.7705C3.214 18.5262 4.10445 18.8601 5.17944 19.0163C6.20952 19.1661 7.51911 19.166 9.14269 19.166H10.8573C12.4809 19.166 13.7905 19.1661 14.8206 19.0163C15.8955 18.8601 16.786 18.5262 17.485 17.7705C18.1754 17.0241 18.4727 16.0888 18.6132 14.9588C18.75 13.8586 18.75 12.4543 18.75 10.6878V10.1443C18.75 8.37775 18.75 6.97347 18.6132 5.87323C18.4727 4.74328 18.1754 3.80794 17.485 3.06152C16.786 2.30586 15.8955 1.97193 14.8206 1.81569C13.7905 1.66597 12.4809 1.66599 10.8573 1.66602ZM3.74819 7.49935C3.366 7.49935 3.17491 7.49935 3.05321 7.61949C2.93151 7.73962 2.92906 7.92829 2.92417 8.30561C2.91689 8.86658 2.91667 9.4945 2.91667 10.2021V10.63C2.91667 12.4674 2.91819 13.7678 3.04073 14.7531C3.16106 15.7207 3.38552 16.2572 3.73852 16.6388C4.08297 17.0112 4.55392 17.2413 5.41917 17.367C6.31548 17.4973 7.50354 17.4994 9.20833 17.4994H10.7917C12.4965 17.4994 13.6845 17.4973 14.5808 17.367C15.4461 17.2413 15.917 17.0112 16.2615 16.6388C16.6145 16.2572 16.8389 15.7207 16.9593 14.7531C17.0818 13.7678 17.0833 12.4674 17.0833 10.63V10.2021C17.0833 9.4945 17.0831 8.86658 17.0758 8.30561C17.0709 7.92829 17.0685 7.73962 16.9468 7.61949C16.8251 7.49935 16.634 7.49935 16.2518 7.49935H3.74819Z"
                              fill="#9EA1A3"
                            />
                            <path
                              d="M5.00033 0.833008C5.46056 0.833008 5.83366 1.2061 5.83366 1.66634V1.74278C5.60426 1.76197 5.38639 1.78598 5.17977 1.81601C4.8216 1.86807 4.48393 1.93985 4.16699 2.04038V1.66634C4.16699 1.2061 4.54009 0.833008 5.00033 0.833008Z"
                              fill="#9EA1A3"
                            />
                            <path
                              d="M14.8209 1.81601C15.179 1.86807 15.5167 1.93985 15.8337 2.04038V1.66634C15.8337 1.2061 15.4606 0.833008 15.0003 0.833008C14.5401 0.833008 14.167 1.2061 14.167 1.66634V1.74278C14.3964 1.76197 14.6143 1.78598 14.8209 1.81601Z"
                              fill="#9EA1A3"
                            />
                            <path
                              d="M6.66699 9.79134C6.0917 9.79134 5.62533 10.2577 5.62533 10.833C5.62533 11.4083 6.0917 11.8747 6.66699 11.8747H6.67447C7.24977 11.8747 7.71614 11.4083 7.71614 10.833C7.71614 10.2577 7.24977 9.79134 6.67447 9.79134H6.66699Z"
                              fill="#9EA1A3"
                            />
                            <path
                              d="M9.99659 9.79134C9.42129 9.79134 8.95492 10.2577 8.95492 10.833C8.95492 11.4083 9.42129 11.8747 9.99659 11.8747H10.0041C10.5794 11.8747 11.0457 11.4083 11.0457 10.833C11.0457 10.2577 10.5794 9.79134 10.0041 9.79134H9.99659Z"
                              fill="#9EA1A3"
                            />
                            <path
                              d="M13.3262 9.79134C12.7509 9.79134 12.2845 10.2577 12.2845 10.833C12.2845 11.4083 12.7509 11.8747 13.3262 11.8747H13.3337C13.909 11.8747 14.3753 11.4083 14.3753 10.833C14.3753 10.2577 13.909 9.79134 13.3337 9.79134H13.3262Z"
                              fill="#9EA1A3"
                            />
                            <path
                              d="M6.66699 13.1247C6.0917 13.1247 5.62533 13.591 5.62533 14.1663C5.62533 14.7416 6.0917 15.208 6.66699 15.208H6.67447C7.24977 15.208 7.71614 14.7416 7.71614 14.1663C7.71614 13.591 7.24977 13.1247 6.67447 13.1247H6.66699Z"
                              fill="#9EA1A3"
                            />
                            <path
                              d="M9.99659 13.1247C9.42129 13.1247 8.95492 13.591 8.95492 14.1663C8.95492 14.7416 9.42129 15.208 9.99659 15.208H10.0041C10.5794 15.208 11.0457 14.7416 11.0457 14.1663C11.0457 13.591 10.5794 13.1247 10.0041 13.1247H9.99659Z"
                              fill="#9EA1A3"
                            />
                          </svg>
                        </template>
                      </template>
                    </v-text-field>
                  </template>

                  <v-date-picker
                    v-model="filters.created_at"
                    locale="ar"
                    @update:modelValue="isDatepickerMenu = false"
                    hide-header
                    clearable
                  ></v-date-picker>
                </v-menu>

                <VBtn
                  :loading="isExporting"
                  :disabled="isExporting"
                  variant="outlined"
                  height="42"
                  elevation="0"
                  class="px-0 bg-white rounded-xl"
                  @click="exportData"
                  style="padding: 0 16px !important"
                >
                  <span>
                    <svg
                      width="24"
                      height="24"
                      viewBox="0 0 24 24"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <path
                        opacity="0.4"
                        d="M20.9986 13.0104C20.6582 13.8464 19.09 15.5021 15.502 15.5021C15.2005 15.5021 14.4099 15.8053 14.4508 17.4483C14.4675 18.9333 13.9262 21.8711 11.6555 21.9978C11.2055 22 10.7164 22 10.1818 22C8.38275 22 7.48321 22 6.76478 21.7478C5.60979 21.3424 4.69875 20.4849 4.26796 19.3979C4 18.7217 4 17.8751 4 16.1818L4 7C4 6.07069 4 5.60603 4.07686 5.21964C4.39249 3.63287 5.63288 2.39248 7.21964 2.07686C7.60604 2 8.07069 2 9 2L13.2727 2C16.5339 2 18.1645 2 19.2969 2.79784C19.6214 3.02643 19.9094 3.29753 20.1523 3.60289C21 4.66867 21 6.20336 21 9.27273V11.8182C21 12.2452 21 12.6414 20.9986 13.0104Z"
                        fill="#B69265"
                      />
                      <path
                        d="M7.75 2C7.75 1.58579 7.41421 1.25 7 1.25C6.58579 1.25 6.25 1.58579 6.25 2V5.25H3C2.58579 5.25 2.25 5.58579 2.25 6C2.25 6.41421 2.58579 6.75 3 6.75H6.25V10C6.25 10.4142 6.58579 10.75 7 10.75C7.41421 10.75 7.75 10.4142 7.75 10V6.75H11C11.4142 6.75 11.75 6.41421 11.75 6C11.75 5.58579 11.4142 5.25 11 5.25H7.75V2Z"
                        fill="#B69265"
                      />
                      <path
                        fill-rule="evenodd"
                        clip-rule="evenodd"
                        d="M10.1809 22.749C8.29902 22.749 7.35404 22.749 6.51505 22.454C5.15405 21.976 4.08106 20.963 3.56906 19.673C3.24806 18.864 3.24805 17.967 3.24805 16.181V11.999C3.24805 11.585 3.58405 11.249 3.99805 11.249C4.41205 11.249 4.74805 11.585 4.74805 11.999V16.181C4.74805 17.777 4.74805 18.578 4.96305 19.121C5.31405 20.007 6.06105 20.706 7.01105 21.04C7.60904 21.25 8.46804 21.25 10.18 21.25C10.4543 21.25 10.7164 21.25 10.967 21.2497C10.9779 21.2492 10.9889 21.249 11 21.249C12.424 21.249 13.583 20.09 13.583 18.666C13.583 18.5059 13.575 18.3334 13.5666 18.151L13.5647 18.1111C13.5398 17.5665 13.5116 16.9499 13.665 16.374C13.888 15.542 14.543 14.887 15.375 14.665C15.952 14.51 16.571 14.539 17.117 14.565L17.1474 14.5664C17.3312 14.5749 17.5059 14.583 17.667 14.583C19.057 14.583 20.1945 13.4787 20.248 12.1015L20.248 9.273C20.248 6.38201 20.248 4.93099 19.563 4.07C19.364 3.82 19.129 3.599 18.862 3.411C17.9251 2.75 16.3691 2.75 13.2701 2.75H12.997C12.583 2.75 12.247 2.414 12.247 2C12.247 1.586 12.583 1.25 12.997 1.25H13.2701C16.685 1.25 18.4001 1.25 19.726 2.185C20.1091 2.455 20.449 2.775 20.736 3.136C21.747 4.407 21.747 6.03299 21.747 9.27298L21.747 11.933C21.749 11.9551 21.75 11.9774 21.75 12C21.75 12.0526 21.749 12.105 21.747 12.1571C21.7463 14.9883 21.7274 16.4556 21.225 17.722C20.39 19.828 18.6341 21.484 16.4071 22.266C15.1592 22.704 13.714 22.7456 11.0577 22.7496C11.0382 22.7499 11.0186 22.75 10.999 22.75C10.7376 22.7503 10.4648 22.75 10.1791 22.75L10.1809 22.749ZM15.912 20.851C15.443 21.0155 14.9065 21.1121 14.2237 21.169C14.7614 20.4775 15.082 19.6091 15.082 18.667C15.082 18.4764 15.0734 18.2815 15.0649 18.0911L15.063 18.048L15.0628 18.0447C15.0419 17.5877 15.0202 17.1152 15.114 16.763C15.199 16.448 15.447 16.199 15.763 16.115C16.1111 16.0211 16.5785 16.0425 17.0317 16.0633L17.047 16.064L17.0872 16.0658C17.2786 16.0743 17.4754 16.083 17.667 16.083C18.6203 16.083 19.4985 15.7546 20.1941 15.2049C20.1415 16.038 20.0375 16.6513 19.8321 17.17C19.1581 18.871 17.729 20.213 15.912 20.851Z"
                        fill="#B69265"
                      />
                    </svg>
                  </span>
                </VBtn>
              </div>
            </div>
          </div>

          <div
            class="d-flex gap-2 border px-2 py-1"
            style="border-radius: 16px; max-width: fit-content"
          >
            <v-btn
              v-for="tab in [
                { id: 'daily', label: 'الحضور اليومي' },
                { id: 'all', label: 'جميع التحضيرات' },
                { id: 'absents', label: 'تنبيهات الغياب' },
              ]"
              :key="tab.id"
              elevation="0"
              class="rounded-lg font-weight-bold"
              :class="{
                'bg-light-gray text-primary': filters.tap === tab.id,
                'bg-white text-gray-1': filters.tap !== tab.id,
              }"
              @click="filters.tap = tab.id"
            >
              <span class="responsive-text">{{ tab.label }}</span>
            </v-btn>
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
            v-if="attendanceDataa.length"
            :items="attendanceDataa"
            :loading="isLoading"
            :page="currentPage"
            :items-per-page="perPage"
            :total-items="totalItems"
            :headers="tableHeaders"
            @update:options="handleTableUpdate"
          >
            <template #[`item.name`]="{ item }">
              <div class="d-flex align-center gap-2">
                <v-avatar size="32" rounded="lg">
                  <v-img :src="item.avatar || '/imgs/person.svg'" />
                </v-avatar>
                <div>
                  <div>{{ item.name }}</div>
                  <div v-if="item.role" class="font-weight-medium text-sm">
                    {{ item.role }}
                  </div>
                </div>
              </div>
            </template>

            <template #pagination>
              <Pagination
                :total-items="totalItems"
                v-model:per-page="perPage"
                v-model:page="currentPage"
              />
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
              <path
                d="M107.68 91.1789L107.679 91.1793C98.598 100.261 86.0233 105.75 72.1502 105.75C58.3779 105.75 45.8028 100.161 36.6205 91.1789C27.3387 81.9969 21.5502 69.2222 21.5502 55.05C21.5502 27.0051 44.2056 4.35 72.1502 4.35C100.095 4.35 122.75 27.105 122.75 55.05C122.75 69.2222 116.962 81.9969 107.68 91.1789Z"
                fill="#FFF9EB"
                stroke="#B69265"
                stroke-width="0.2"
              />
              <path
                d="M118.45 21.7508C120.714 21.7508 122.55 19.9151 122.55 17.6508C122.55 15.3864 120.714 13.5508 118.45 13.5508C116.185 13.5508 114.35 15.3864 114.35 17.6508C114.35 19.9151 116.185 21.7508 118.45 21.7508Z"
                fill="#FFF9EB"
              />
              <path
                d="M124.45 5.75039C125.997 5.75039 127.25 4.49679 127.25 2.95039C127.25 1.40399 125.997 0.150391 124.45 0.150391C122.904 0.150391 121.65 1.40399 121.65 2.95039C121.65 4.49679 122.904 5.75039 124.45 5.75039Z"
                fill="#FFF9EB"
              />
              <path
                d="M23.05 21.6508C24.5964 21.6508 25.85 20.3972 25.85 18.8508C25.85 17.3044 24.5964 16.0508 23.05 16.0508C21.5036 16.0508 20.25 17.3044 20.25 18.8508C20.25 20.3972 21.5036 21.6508 23.05 21.6508Z"
                fill="#FFF9EB"
              />
              <path
                d="M5.95 75.65C8.82188 75.65 11.15 73.3219 11.15 70.45C11.15 67.5781 8.82188 65.25 5.95 65.25C3.07812 65.25 0.75 67.5781 0.75 70.45C0.75 73.3219 3.07812 75.65 5.95 75.65Z"
                fill="#FFF9EB"
              />
              <path
                d="M107.75 28.4502V91.2502C98.6498 100.35 86.0498 105.85 72.1498 105.85C58.3498 105.85 45.7498 100.25 36.5498 91.2502V28.4502H107.75Z"
                fill="white"
              />
              <path
                d="M73.4074 60.5041L59.4775 46.5742L54.74 51.3118L68.6698 65.2417L73.4074 60.5041Z"
                fill="#B69265"
              />
              <path
                d="M84.8337 39.6497L64.0449 60.4385L68.7825 65.176L89.5712 44.3873L84.8337 39.6497Z"
                fill="#B69265"
              />
              <path
                d="M90.3502 73.75H53.9502V78.35H90.3502V73.75Z"
                fill="#F3F1ED"
              />
              <path
                d="M90.3502 83.8506H53.9502V88.4506H90.3502V83.8506Z"
                fill="#F3F1ED"
              />
              <path
                d="M81.4496 94.0508H62.8496V98.6508H81.4496V94.0508Z"
                fill="#F3F1ED"
              />
            </svg>
            <div class="text-h6 font-weight-medium mt-4">
              لم يتم تسجيل أي حضور أو انصراف حتى الآن.
            </div>
            <div class="text-body-1 text-medium-emphasis">
              بمجرد أن يبدأ المراقبون تسجيل حضورهم أو انصرافهم، ستظهر السجلات
              هنا بشكل تلقائي.
            </div>
          </div>
        </template>
      </v-card>
    </VCard>
  </v-container>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
  // background-color: #f00 !important;
}
.search-container {
  // background-color: white;
  // border-radius: 8px;
  // padding: 4px;
  // border: 1px solid rgba(0, 0, 0, 0.12);

  .filter-select {
    max-width: 150px;
    min-width: 120px;

    :deep(.v-field) {
      // background-color: transparent !important;
      border: none !important;

      .v-field__input {
        min-height: 42px;
        padding-top: 0;
        padding-bottom: 0;
      }

      .v-field__append-inner {
        padding-inline-end: 0;
      }
    }
  }

  .search-field {
    min-width: 200px;

    :deep(.v-field) {
      background-color: transparent !important;
      border: none !important;

      .v-field__input {
        min-height: 42px;
        padding-top: 0;
        padding-bottom: 0;
      }

      .v-field__prepend-inner {
        // padding-inline-start: 12px;
        color: rgba(0, 0, 0, 0.38);
      }
    }
  }
}

.tabs-container {
  position: relative;

  .tab-item {
    cursor: pointer;
    color: rgba(0, 0, 0, 0.6);
    font-size: 0.875rem;
    font-weight: 500;
    transition: all 0.3s ease;

    &.active {
      color: #b69265;
      position: relative;

      &:after {
        content: "";
        position: absolute;
        bottom: -2px;
        left: 0;
        right: 0;
        height: 2px;
        background-color: #b69265;
      }
    }
  }

  .tab-border {
    position: absolute;
    bottom: 0;
    left: 0;
    right: 0;
    height: 1px;
    background-color: rgba(0, 0, 0, 0.12);
  }
}

.add-site-dialog {
  :deep(.v-card) {
    background-color: white;
  }

  :deep(.v-field) {
    border-radius: 8px;
    background-color: rgb(250, 250, 250);

    &.v-field--focused {
      background-color: white;
    }
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
    border-radius: 8px;
  }

  .country-code-select {
    :deep(.v-field__input) {
      padding-top: 0;
      padding-bottom: 0;
    }
  }
}

.min-width-300px {
  min-width: 300px;
}
.tabs-container .tab-border {
  // display: none;
  bottom: -16px;
}
.tabs-container {
  // padding: 16px;
  font-weight: bolder;
}
.tabs-container .tab-item.active {
  color: #b69265;
  background-color: rgba(248, 249, 249, 1);
  border: none;
  border-radius: 8px;
  &:after {
    display: none;
  }
}

.custom-breadcrumbs {
  :deep(.v-breadcrumbs-item--disabled) {
    opacity: 1;
  }

  :deep(.v-breadcrumbs__item) {
    color: rgba(0, 0, 0, 0.6);
    font-size: 13px;
    padding: 0 8px;
  }

  :deep(.v-breadcrumbs-item--disabled) {
    color: rgba(0, 0, 0, 0.6);
  }
}

.text-13 {
  font-size: 13px;
}

.bg-light-gray {
  background-color: rgba(248, 249, 249, 1) !important;
}
.text-gray-1 {
  color: rgba(124, 133, 139, 1) !important;
  // color: #f00 !important;
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
</style>
