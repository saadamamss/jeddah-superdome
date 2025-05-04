<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import noteCard from "@/@core/components/widgets/noteCard";
import { ref, onMounted, watch, computed } from "vue";
import axios from "@/plugins/axios";
import StatsWidget from "@/@core/components/widgets/StatsWidget";

import dataTable from "@/components/dataTable.vue";
import NoteCard from "@/components/NoteCard.vue";
import NoteItem from "@/components/NoteItem.vue";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const loading = ref(false);
const activeTab = ref("ratings");

const isExporting = ref(false);

// Statistics data
const statistics = ref({
  ratings_sahab_count: 0,
  reports_sahab_count: 0,
  sahab_close_count: 0,
  command_center_close_count: 0,
});

// Data for notes, ratings and integrations
const notes = ref([]);
const ratings = ref([]);
const noteTypes = ref([]);
const totalNotes = ref(0);
const totalRatings = ref(0);

// Platform type selection
const type_render = ref("command_center");
const availableTypes = ref([
  { title: "منصة الرصد و التحكم", value: "command_center" },
  { title: "منصة سحاب", value: "sahab" },
]);

// Filter variables
const statusFilter = ref("");
const dateFilter = ref(null);
const dateMenu = ref(false);
const filterMenu = ref(false);

// Temporary filter values (for keeping state until applied)
const tempStatusFilter = ref("");
const tempDateFilter = ref(null);

// Check if any filters are active
const hasActiveFilters = computed(() => {
  return statusFilter.value !== "" || dateFilter.value !== null;
});

// Date selection handler
const dateSelected = () => {
  dateMenu.value = false;
  currentPage.value = 1;
  if (activeTab.value === "notes") {
    fetchNotes();
  } else {
    fetchRatings();
  }
};

// Apply filters
const applyFilters = () => {
  // Copy temp values to actual filters
  statusFilter.value = tempStatusFilter.value;
  dateFilter.value = tempDateFilter.value;

  // Close the filter menu
  filterMenu.value = false;

  // Reset to first page and fetch data
  currentPage.value = 1;
  if (activeTab.value === "notes") {
    fetchNotes();
  } else {
    fetchRatings();
  }
};

// Reset all filters
const resetFilters = () => {
  // Reset both temp and actual filters
  statusFilter.value = "";
  dateFilter.value = null;

  tempStatusFilter.value = "";
  tempDateFilter.value = null;

  // Close the filter menu
  filterMenu.value = false;

  // Reset to first page and fetch data
  currentPage.value = 1;
  if (activeTab.value === "notes") {
    fetchNotes();
  } else {
    fetchRatings();
  }
};

// Watch main filters for changes to update temp filters
watch([statusFilter, dateFilter], ([newStatus, newDate]) => {
  tempStatusFilter.value = newStatus;
  tempDateFilter.value = newDate;
});

// Watch filter menu open to initialize temp filters
watch(filterMenu, (isOpen) => {
  if (isOpen) {
    // When opening menu, initialize temp filters with current values
    tempStatusFilter.value = statusFilter.value;
    tempDateFilter.value = dateFilter.value;
  }
});

// Table headers for ratings
const tableHeaders = [
  { title: "البند", key: "question", sortable: true, align: "right" },
  {
    title: "اسم الشركة الرئيسية",
    key: "company.parent_company.name",
    align: "center",
  },
  {
    title: "رقم مركز الخدمة",
    key: "company.center_no",
    align: "center",
  },
  { title: "الإجابة", key: "answer", align: "center" },
  // { title: "اسم الراقب", key: "reporter", align: "center" },
  { title: "الحالة", key: "status", align: "center" },
  { title: "سبب الرفض", key: "cancel_reason", align: "center" },
  { title: "الإجراء", key: "actions", align: "center" },
];

// Fetch ratings with pagination and search
const fetchRatings = async () => {
  try {
    isLoading.value = true;

    // Build query parameters
    const params = {
      type: type_render.value,
      page: currentPage.value,
      per_page: perPage.value,
      search: search.value || undefined,
    };

    // Add status filter if present
    if (statusFilter.value) {
      params.status =
        statusFilter.value === "مفتوح"
          ? "open"
          : statusFilter.value === "مرفوض"
          ? "rejected"
          : undefined;
    }

    // Add date filter if present
    if (dateFilter.value) {
      params.date = dateFilter.value;
    }

    const response = await axios.get("/rating-integrations", { params });

    // Log the response to see its structure
    console.log("API Response:", response.data);

    // Update statistics
    statistics.value = {
      ratings_sahab_count: response.data?.data?.ratings_sahab_count || 0,
      reports_sahab_count: response.data?.data?.reports_sahab_count || 0,
      sahab_close_count: response.data?.data?.sahab_close_count || 0,
      command_center_close_count:
        response.data?.data?.command_center_close_count || 0,
    };

    // Safely handle the ratings data
    const responseData = response.data?.data?.ratings || [];

    // Ensure we have an array before mapping
    if (Array.isArray(responseData)) {
      ratings.value = responseData.map((item) => ({
        id: item.id,
        question: item.question,
        company: {
          parent_company: {
            name:
              item.company?.parent_company?.name ||
              item.company?.parent_company_name ||
              "-",
          },
          center_no: item.company?.center_no || "-",
        },
        answer: item.answer,
        reporter: {
          name:
            item.rating_by?.firstname + " " + item.rating_by?.lastname || "N/A",
          avatar: item.rating_by?.avatar_url || "https://placehold.co/400",
        },
        status:
          item.status === "open"
            ? "مفتوح"
            : item.status === "rejected"
            ? "مرفوض"
            : item.status,
        cancel_reason: item.cancel_reason || "-",
      }));

      // Handle pagination
      totalRatings.value =
        response.data?.data?.pagination?.i_total_objects || 0;
      perPage.value = response.data?.data?.pagination?.i_per_page || 6;
      currentPage.value = response.data?.data?.pagination?.i_current_page || 1;
    } else {
      console.error("Invalid response data structure:", responseData);
      ratings.value = [];
      totalRatings.value = 0;
    }
  } catch (error) {
    console.error("Error fetching ratings:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    ratings.value = [];
    totalRatings.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Fetch notes with pagination and search
const fetchNotes = async () => {
  try {
    isLoading.value = true;
    // Simulating API call with mock data for demonstration
    setTimeout(() => {
      notes.value = [
        {
          id: 1,
          report_type: { title: "طلب صيانة" },
          status: "PENDING",
          description: "هناك مشكلة في نظام التكييف في الغرفة 205",
          report_date: "2023-11-15",
          report_time: "14:30",
          address: "مكة المكرمة - العزيزية",
          reportBy: {
            firstname: "أحمد",
            lastname: "محمد",
          },
          latitude: 21.3891,
          longitude: 39.8579,
          created_at: "2023-11-15T14:30:00",
        },
        {
          id: 2,
          report_type: { title: "مشكلة في النظافة" },
          status: "APPROVED",
          description: "الممرات تحتاج إلى تنظيف عاجل",
          report_date: "2023-11-16",
          report_time: "09:15",
          address: "المدينة المنورة - باب السلام",
          reportBy: {
            firstname: "خالد",
            lastname: "العتيبي",
          },
          latitude: 24.4672,
          longitude: 39.6051,
          created_at: "2023-11-16T09:15:00",
        },
        {
          id: 3,
          report_type: { title: "انقطاع المياه" },
          status: "REJECTED",
          description: "انقطاع المياه منذ الصباح في المبنى بالكامل",
          report_date: "2023-11-17",
          report_time: "11:20",
          address: "مكة المكرمة - العزيزية",
          reportBy: {
            firstname: "سارة",
            lastname: "الأحمد",
          },
          latitude: 21.3891,
          longitude: 39.8579,
          created_at: "2023-11-17T11:20:00",
        },
      ];
      totalNotes.value = notes.value.length;
      noteTypes.value = [
        { id: 1, title: "طلب صيانة", total_reports: 12 },
        { id: 2, title: "مشكلة في النظافة", total_reports: 8 },
        { id: 3, title: "انقطاع المياه", total_reports: 5 },
        { id: 4, title: "مشكلة في الكهرباء", total_reports: 9 },
      ];
      isLoading.value = false;
    }, 1000);
  } catch (error) {
    console.error("Error fetching notes:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    notes.value = [];
    totalNotes.value = 0;
    isLoading.value = false;
  }
};

// Watch for changes in search with debounce
const debouncedSearch = ref(null);
watch(search, (newValue) => {
  if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
  debouncedSearch.value = setTimeout(() => {
    currentPage.value = 1;
    fetchRatings();
  }, 500);
});

// Watch for type changes
watch(type_render, () => {
  currentPage.value = 1;
  fetchRatings();
});

// Watch for page changes
watch(currentPage, () => {
  if (activeTab.value === "notes") {
    fetchNotes();
  } else {
    fetchRatings();
  }
});

onMounted(() => {
  fetchRatings();
});

// Add export function
const exportData = async () => {
  try {
    isExporting.value = true;
    const response = await axios.post(
      `/rating-integrations-excel?type=${type_render.value}`,
      {},
      {
        responseType: "blob",
      }
    );

    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement("a");
    link.href = url;
    const fileName = "ratings.xlsx";
    link.setAttribute("download", fileName);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error("Export failed:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء تصدير البيانات",
      color: "error",
    });
  } finally {
    isExporting.value = false;
  }
};

// Override the default status styling for the notes component
const getStatusStyle = (status) => {
  switch (status) {
    case "مغلق":
      return { color: "#e3f0db", textColor: "#7ab55c" };
    case "مفتوح":
      return { color: "#fff8df", textColor: "#d4b327" };
    case "مرفوض":
      return { color: "#ffebeb", textColor: "#ff5252" };
    default:
      return { color: "", textColor: "" };
  }
};

// Add this in the script setup section
const staticNotes = ref([
  {
    id: 1,
    description:
      "تم رصد انقطاع التيار الكهربائي في مخيم النور 3 منذ أكثر من ساعتين، مما تسبب في تعطل أجهزة",
    location: "مخيم النور 3",
    sector: "القطاع الجنوبي",
    date: "07/02",
    time: "09:15 ص",
  },
  {
    id: 2,
    description:
      "تم رصد انقطاع التيار الكهربائي في مخيم النور 3 منذ أكثر من ساعتين، مما تسبب في تعطل أجهزة",
    location: "مخيم النور 3",
    sector: "القطاع الجنوبي",
    date: "07/02",
    time: "09:15 ص",
  },
  {
    id: 3,
    description:
      "تم رصد انقطاع التيار الكهربائي في مخيم النور 3 منذ أكثر من ساعتين، مما تسبب في تعطل أجهزة",
    location: "مخيم النور 3",
    sector: "القطاع الجنوبي",
    date: "07/02",
    time: "09:15 ص",
  },
  {
    id: 4,
    description:
      "تم رصد انقطاع التيار الكهربائي في مخيم النور 3 منذ أكثر من ساعتين، مما تسبب في تعطل أجهزة",
    location: "مخيم النور 3",
    sector: "القطاع الجنوبي",
    date: "07/02",
    time: "09:15 ص",
  },
  {
    id: 5,
    description:
      "تم رصد انقطاع التيار الكهربائي في مخيم النور 3 منذ أكثر من ساعتين، مما تسبب في تعطل أجهزة",
    location: "مخيم النور 3",
    sector: "القطاع الجنوبي",
    date: "07/02",
    time: "09:15 ص",
  },
  {
    id: 6,
    description:
      "تم رصد انقطاع التيار الكهربائي في مخيم النور 3 منذ أكثر من ساعتين، مما تسبب في تعطل أجهزة",
    location: "مخيم النور 3",
    sector: "القطاع الجنوبي",
    date: "07/02",
    time: "09:15 ص",
  },
]);
</script>

<template>
  <div>
    <v-row v-if="isLoading">
      <v-col v-for="item in 4" :key="item" cols="6" sm="6" md="3">
        <VCard elevation="0" class="py-2">
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
    <v-row v-else-if="!isLoading">
      <v-col cols="6" md="3">
        <StatsWidget
          title="عدد التقييمات المرسلة"
          :number="
            type_render === 'sahab'
              ? statistics.ratings_sahab_count
              : totalRatings
          "
        >
          <template #icon>
            <svg
              width="32"
              height="33"
              viewBox="0 0 32 33"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                opacity="0.4"
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M23.9998 18.1722C23.9998 13.6127 23.9998 11.3329 22.4377 9.91646C20.8756 8.5 18.3615 8.5 13.3332 8.5C8.30486 8.5 5.7907 8.5 4.2286 9.91646C2.6665 11.3329 2.6665 13.6127 2.6665 18.1722V20.1611C2.6665 24.7207 2.6665 27.0004 4.2286 28.4169C5.7907 29.8333 8.30486 29.8333 13.3332 29.8333C18.3615 29.8333 20.8756 29.8333 22.4377 28.4169C23.9998 27.0004 23.9998 24.7207 23.9998 20.1611V18.1722Z"
                fill="#B69265"
              />
              <path
                d="M15.1931 16.4962L14.2546 14.6036C13.748 13.5766 12.9161 13.5766 12.4041 14.6036L11.4656 16.4962C11.3376 16.7597 10.9963 17.0124 10.7136 17.0608L9.01249 17.3457C7.92461 17.5285 7.67397 18.3243 8.45255 19.1093L9.77507 20.4428C9.99905 20.6686 10.1217 21.1041 10.0524 21.416L9.67375 23.0667C9.37512 24.3679 10.0684 24.8787 11.2096 24.1958L12.8041 23.2441C13.0974 23.072 13.572 23.072 13.86 23.2441L15.4544 24.1958C16.601 24.8787 17.2889 24.3732 16.9903 23.0667L16.6116 21.416C16.5423 21.1041 16.665 20.6686 16.889 20.4428L18.2115 19.1093C18.9954 18.3243 18.7394 17.5285 17.6515 17.3457L15.9504 17.0608C15.6624 17.0124 15.3211 16.7597 15.1931 16.4962Z"
                fill="#B69265"
              />
            </svg>
          </template>
        </StatsWidget>
      </v-col>
      <v-col cols="6" md="3">
        <StatsWidget
          title="عدد الملاحظات المرسلة"
          :number="type_render === 'sahab' ? statistics.reports_sahab_count : 0"
        >
          <template #icon>
            <svg
              width="30"
              height="29"
              viewBox="0 0 30 29"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                opacity="0.4"
                d="M17.9609 0.930464C16.0138 0.801041 13.9829 0.801305 12.0399 0.930464C5.95099 1.33522 1.11995 6.25027 0.722727 12.3966C0.648414 13.5466 0.648414 14.7359 0.722727 15.8858C0.870626 18.1743 1.86897 20.2388 2.97874 21.914C3.31118 22.556 3.13797 23.519 2.51619 24.6972L2.48755 24.7515C2.27865 25.1471 2.07414 25.5344 1.95617 25.8647C1.83117 26.2146 1.70197 26.7702 2.02421 27.3195C2.32027 27.8243 2.79333 28.0092 3.17569 28.0832C3.49314 28.1447 3.88239 28.1539 4.24758 28.1627L4.30855 28.1642C6.20155 28.2102 7.53547 27.6636 8.59206 26.8846C8.80177 26.7299 8.94578 26.624 9.05421 26.5495C9.2119 26.6074 9.42739 26.6958 9.74485 26.8266C10.4667 27.1238 11.2866 27.3019 12.0399 27.352C13.9829 27.4811 16.0138 27.4814 17.9609 27.352C24.0497 26.9472 28.8807 22.0322 29.2779 15.8858C29.3522 14.7359 29.3522 13.5466 29.2779 12.3966C28.8807 6.25027 24.0497 1.33522 17.9609 0.930464Z"
                fill="#B69265"
              />
            </svg>
          </template>
        </StatsWidget>
      </v-col>
      <v-col cols="6" md="3">
        <StatsWidget
          title="إغلاق منصة سحاب"
          :number="statistics.sahab_close_count"
        >
          <template #icon>
            <svg
              width="32"
              height="33"
              viewBox="0 0 32 33"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                opacity="0.4"
                d="M18.8945 29.0207C24.4718 28.65 28.9144 24.1443 29.28 18.4878C29.3515 17.3809 29.3515 16.2346 29.28 15.1276C28.9144 9.47117 24.4718 4.96551 18.8945 4.59477C16.9917 4.46828 15.0051 4.46854 13.1062 4.59477C7.52884 4.96551 3.0862 9.47117 2.72065 15.1276C2.64911 16.2346 2.64911 17.3809 2.72065 18.4878C2.85379 20.548 3.7649 22.4555 4.83753 24.0661C5.46034 25.1937 5.04932 26.6011 4.40061 27.8304C3.93287 28.7168 3.699 29.16 3.88678 29.4801C4.07456 29.8003 4.494 29.8105 5.33289 29.8309C6.99188 29.8713 8.11057 29.401 8.99856 28.7462C9.5022 28.3748 9.75401 28.1891 9.92757 28.1677C10.1011 28.1464 10.4427 28.2871 11.1257 28.5684C11.7395 28.8212 12.4523 28.9772 13.1062 29.0207C15.0051 29.1469 16.9917 29.1472 18.8945 29.0207Z"
                fill="#B69265"
              />
            </svg>
          </template>
        </StatsWidget>
      </v-col>
      <v-col cols="6" md="3">
        <StatsWidget
          title="إغلاق منصة الرصد و التحكم"
          :number="statistics.command_center_close_count"
        >
          <template #icon>
            <svg
              width="32"
              height="33"
              viewBox="0 0 32 33"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                opacity="0.4"
                d="M18.8945 29.0207C24.4718 28.65 28.9144 24.1443 29.28 18.4878C29.3515 17.3809 29.3515 16.2346 29.28 15.1276C28.9144 9.47117 24.4718 4.96551 18.8945 4.59477C16.9917 4.46828 15.0051 4.46854 13.1062 4.59477C7.52884 4.96551 3.0862 9.47117 2.72065 15.1276C2.64911 16.2346 2.64911 17.3809 2.72065 18.4878C2.85379 20.548 3.7649 22.4555 4.83753 24.0661C5.46034 25.1937 5.04932 26.6011 4.40061 27.8304C3.93287 28.7168 3.699 29.16 3.88678 29.4801C4.07456 29.8003 4.494 29.8105 5.33289 29.8309C6.99188 29.8713 8.11057 29.401 8.99856 28.7462C9.5022 28.3748 9.75401 28.1891 9.92757 28.1677C10.1011 28.1464 10.4427 28.2871 11.1257 28.5684C11.7395 28.8212 12.4523 28.9772 13.1062 29.0207C15.0051 29.1469 16.9917 29.1472 18.8945 29.0207Z"
                fill="#B69265"
              />
            </svg>
          </template>
        </StatsWidget>
      </v-col>
    </v-row>
    <v-container fluid class="px-0 px-0 mt-4 py-2 pt-0">
      <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
        <!-- Header Section -->
        <v-card-title class="pt-4 pb-6">
          <div
            class="d-flex align-start justify-space-between gap-4 flex-wrap w-100"
          >
            <h2 class="text-h5 font-weight-bold mb-4">إدارة التكاملات</h2>
            <div class="d-flex align-center ga-2">
              <v-text-field
                v-model="search"
                label="البحث... "
                rounded="lg"
                clearable
                height="48"
                class="bg-white"
                style="width: 320px; max-width: 100%"
              >
                <template #prepend-inner>
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
                  </svg>
                </template>
              </v-text-field>

              <v-menu
                v-model="filterMenu"
                :close-on-content-click="false"
                location="bottom end"
                min-width="350"
              >
                <template #activator="{ props }">
                  <VBtn
                    color="default"
                    class="btn-icon border"
                    elevation="0"
                    v-bind="props"
                    :variant="hasActiveFilters ? 'flat' : 'outlined'"
                  >
                    <svg
                      width="1.3rem"
                      height="1.3rem"
                      viewBox="0 0 24 24"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <path
                        opacity="0.4"
                        d="M8.85746 12.5061C6.36901 10.6456 4.59564 8.59915 3.62734 7.44867C3.3276 7.09253 3.22938 6.8319 3.17033 6.3728C2.96811 4.8008 2.86701 4.0148 3.32795 3.5074C3.7889 3 4.60404 3 6.23433 3H17.7657C19.396 3 20.2111 3 20.672 3.5074C21.133 4.0148 21.0319 4.8008 20.8297 6.37281C20.7706 6.83191 20.6724 7.09254 20.3726 7.44867C19.403 8.60062 17.6261 10.6507 15.1326 12.5135C14.907 12.6821 14.7583 12.9567 14.7307 13.2614C14.4837 15.992 14.2559 17.4876 14.1141 18.2442C13.8853 19.4657 12.1532 20.2006 11.226 20.8563C10.6741 21.2466 10.0043 20.782 9.93278 20.1778C9.79643 19.0261 9.53961 16.6864 9.25927 13.2614C9.23409 12.9539 9.08486 12.6761 8.85746 12.5061Z"
                        fill="#B69265"
                      />
                    </svg>
                  </VBtn>
                </template>

                <v-card class="pa-4 rounded-lg filter-menu-card">
                  <v-card-title
                    class="px-0 pt-0 d-flex justify-space-between align-center"
                  >
                    <span class="text-subtitle-1 font-weight-bold"
                      >فلترة البيانات</span
                    >
                    <v-btn
                      variant="text"
                      color="primary"
                      size="small"
                      :disabled="!hasActiveFilters"
                      @click="resetFilters"
                    >
                      إعادة تعيين
                      <v-icon right>mdi-refresh</v-icon>
                    </v-btn>
                  </v-card-title>

                  <v-divider class="mb-4"></v-divider>

                  <v-card-text class="px-0 pb-0">
                    <!-- Status Filter -->
                    <div class="mb-4">
                      <v-select
                        v-model="tempStatusFilter"
                        label="الحالة"
                        :items="[
                          { title: 'الكل', value: '' },
                          { title: 'مفتوح', value: 'مفتوح' },
                          { title: 'مرفوض', value: 'مرفوض' },
                        ]"
                        variant="outlined"
                        hide-details
                        density="comfortable"
                        class="mb-3"
                      ></v-select>
                    </div>
                  </v-card-text>

                  <v-card-actions class="px-0 pt-4">
                    <v-spacer></v-spacer>
                    <v-btn
                      variant="text"
                      color="error"
                      @click="filterMenu = false"
                    >
                      إلغاء
                    </v-btn>
                    <v-btn color="primary" @click="applyFilters">
                      تطبيق الفلتر
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </v-menu>

              <!-- export -->
              <VBtn
                :loading="isExporting"
                :disabled="isExporting"
                variant="outlined"
                height="42"
                elevation="0"
                class="px-4 bg-white rounded-xl"
                @click="exportData"
              >
                <div class="d-flex align-center ga-2">
                  <span>تصدير</span>
                </div>
              </VBtn>
            </div>
          </div>

          <div class="border-b pb-6">
            <div class="d-flex justify-space-between align-center">
              <div
                class="d-flex gap-2 border px-2 py-1"
                style="border-radius: 16px; max-width: fit-content"
              >
                <v-btn
                  v-for="tab in [
                    { id: 'ratings', label: 'التقييمات' },
                    { id: 'notes', label: 'الملاحظات' },
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
                  placeholder="اختر "
                  hide-details
                  style="min-width: 100px"
                  class="rounded-lg bg-grey-lighten-4 px-2"
                />
              </div>
            </div>
          </div>

          <!-- Content Section -->
          <v-window v-model="activeTab" class="mt-6">
            <!-- Ratings Tab -->
            <v-window-item value="ratings">
              <dataTable
                :headers="tableHeaders"
                :items="ratings"
                :loading="isLoading"
                class="elevation-0"
                :items-per-page="perPage"
                v-model:page="currentPage"
                :hide-default-footer="true"
              >
                <template #[`item.company.parent_company.name`]="{ item }">
                  <div class="text-center">
                    {{ item.company?.parent_company?.name || "-" }}
                  </div>
                </template>

                <template #[`item.company.center_no`]="{ item }">
                  <div class="text-center">
                    {{ item.company?.center_no || "-" }}
                  </div>
                </template>

                <template #[`item.reporter`]="{ item }">
                  <div class="d-flex align-center justify-center gap-2">
                    <v-avatar size="32">
                      <v-img
                        :src="
                          item.reporter.avatar || 'https://placehold.co/600x400'
                        "
                        :alt="item.reporter.name"
                      ></v-img>
                    </v-avatar>
                    <span>{{ item.reporter.name }}</span>
                  </div>
                </template>

                <template #item.status="{ item }">
                  <div class="status-chip" :class="'status-' + item.status">
                    {{ item.status }}
                  </div>
                </template>

                <template #[`item.actions`]="{ item }">
                  <div class="d-flex align-center justify-center gap-2">
                    <!-- <v-btn icon variant="text" size="small" class="eye-btn">
                      <v-icon>mdi-eye-outline</v-icon>
                    </v-btn> -->
                    <button class="escalate-btn py-2 rounded-pill">
                      <v-icon>mdi-arrow-up-right</v-icon>
                      <span class="ms-2">تصعيد</span>
                    </button>
                  </div>
                </template>
              </dataTable>
            </v-window-item>

            <!-- Notes Tab -->
            <v-window-item value="notes">
              <v-row>
                <v-col
                  v-for="note in staticNotes"
                  :key="note.id"
                  cols="12"
                  sm="6"
                  lg="4"
                >
                  <NoteItem :note="note" />
                </v-col>
              </v-row>
            </v-window-item>
          </v-window>

          <!-- Pagination -->
          <Pagination
            :total-items="activeTab === 'ratings' ? totalRatings : totalNotes"
            v-model:per-page="perPage"
            v-model:page="currentPage"
          />
        </v-card-title>
      </VCard>
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: #fff;
  border-radius: 16px;
}

.search-field {
  :deep(.v-field) {
    border-radius: 8px;
    background-color: white;

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

.filter-menu-card {
  box-shadow: 0 4px 25px 0 rgba(0, 0, 0, 0.1);

  .v-card-title {
    font-size: 1.1rem;
  }

  .v-date-picker {
    box-shadow: none;
  }
}

.status-chip {
  padding: 4px 12px;
  border-radius: 16px;
  font-size: 0.875rem;
  display: inline-block;
  text-align: center;
  min-width: 80px;
}

.status-مغلق {
  background-color: #e3f0db;
  color: #7ab55c;
}

.status-مفتوح {
  background-color: #fff8df;
  color: #d4b327;
}

.status-مرفوض {
  background-color: #ffebeb;
  color: #ff5252;
}

.eye-btn {
  color: #b69265;
  border: 1px solid #fbf7f3;
  border-radius: 8px;
  &:hover {
    background-color: darken(#fbf7f3, 5%);
  }
}

.escalate-btn {
  color: #b69265;
  background-color: #fbf7f3;
  border-radius: 8px;
  padding: 0 16px;
  &:hover {
    background-color: darken(#fbf7f3, 5%);
  }
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

.border-b {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}

.responsive-text {
  font-size: 14px;
  @media (max-width: 600px) {
    font-size: 12px;
  }
}

// Remove the old tab styles
:deep(.v-tabs) {
  display: none;
}

.v-row {
  margin: 0 -12px;
}

.v-col {
  padding: 12px;
}
</style>
