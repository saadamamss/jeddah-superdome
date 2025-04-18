<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import noteCard from "@/@core/components/widgets/noteCard";
import { ref, onMounted, watch, computed } from "vue";
import axios from "@/plugins/axios";
import dataTable from "@/components/dataTable.vue";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const loading = ref(false);
const activeTab = ref("ratings");

// Check if any filters are active
const hasActiveFilters = computed(() => {
  return (
    statusFilter.value !== "" ||
    typeFilter.value !== "" ||
    dateFilter.value !== null
  );
});

// Data for notes, ratings and integrations
const notes = ref([]);
const ratings = ref([]);
const noteTypes = ref([]);
const totalNotes = ref(0);
const totalRatings = ref(0);

// Filter variables
const statusFilter = ref("");
const typeFilter = ref("");
const dateFilter = ref(null);
const dateMenu = ref(false);
const filterMenu = ref(false);

// Temporary filter values (for keeping state until applied)
const tempStatusFilter = ref("");
const tempTypeFilter = ref("");
const tempDateFilter = ref(null);

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
  typeFilter.value = tempTypeFilter.value;
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
  typeFilter.value = "";
  dateFilter.value = null;

  tempStatusFilter.value = "";
  tempTypeFilter.value = "";
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
watch(
  [statusFilter, typeFilter, dateFilter],
  ([newStatus, newType, newDate]) => {
    tempStatusFilter.value = newStatus;
    tempTypeFilter.value = newType;
    tempDateFilter.value = newDate;
  }
);

// Watch filter menu open to initialize temp filters
watch(filterMenu, (isOpen) => {
  if (isOpen) {
    // When opening menu, initialize temp filters with current values
    tempStatusFilter.value = statusFilter.value;
    tempTypeFilter.value = typeFilter.value;
    tempDateFilter.value = dateFilter.value;
  }
});

// Table headers for ratings
const tableHeaders = [
  { title: "السؤال", key: "question", sortable: true, align: "right" },
  { title: "اسم الشركة الرئيسية", key: "company_name", align: "center" },
  { title: "رقم مركز الخدمة", key: "center_number", align: "center" },
  { title: "الإجابة", key: "answer", align: "center" },
  {
    title: "مَن هو الشخص المسؤول عن عملية التنظيف؟",
    key: "responsible_person",
    align: "center",
  },
  {
    title: "الحالة",
    key: "status",
    align: "center",
    customRender: (item) => {
      let className = "";

      if (item.status === "مغلق") {
        className = "status-closed";
      } else if (item.status === "مفتوح") {
        className = "status-open";
      } else if (item.status === "مرفوض") {
        className = "status-rejected";
      }

      return `<span class="status-chip ${className}">${item.status}</span>`;
    },
  },
  { title: "سعود الغامدي", key: "reporter", align: "center", width: "120px" },
];

// Mock data for ratings (in real app this would come from API)
const fetchRatings = async () => {
  try {
    isLoading.value = true;
    // Simulating API call with mock data for demonstration
    setTimeout(() => {
      ratings.value = [
        {
          id: 1,
          question: "من هو الشخص المسؤول عن عملية التنظيف؟",
          company_name: "شركة رواف منى",
          center_number: "345 542 223 33",
          answer: "محمد القحطاني",
          responsible_person: "محمد القحطاني",
          status: "مغلق",
          reporter: "سعود الغامدي",
        },
        {
          id: 2,
          question: "هل تم تنظيف السكن بالكامل وفق المعايير المطلوبة؟",
          company_name: "الرفادة لخدمات الحجاج",
          center_number: "345 542 223 33",
          answer: "نعم",
          responsible_person: "محمد القحطاني",
          status: "مفتوح",
          reporter: "سعود الغامدي",
        },
        {
          id: 3,
          question: "ما هي درجة تقييم حالة السكن بعد التنظيف؟",
          company_name: "مؤسسة بني ضيف الأحمد",
          center_number: "345 542 223 33",
          answer: "6",
          responsible_person: "محمد القحطاني",
          status: "مرفوض",
          reporter: "محمد الغامدي",
        },
        {
          id: 4,
          question: "هل تم تنظيف السكن بالكامل وفق المعايير المطلوبة؟",
          company_name: "دار الطيوب للعمرة والزيارة",
          center_number: "345 542 223 33",
          answer: "لا",
          responsible_person: "محمد القحطاني",
          status: "مرفوض",
          reporter: "تركي السعيدي",
        },
        {
          id: 5,
          question: "ما هي درجة تقييم حالة السكن بعد التنظيف؟",
          company_name: "قافلة الإيمان",
          center_number: "345 542 223 33",
          answer: "7",
          responsible_person: "محمد القحطاني",
          status: "مفتوح",
          reporter: "محمد القحطاني",
        },
        {
          id: 6,
          question: "من هو الشخص المسؤول عن عملية التنظيف؟",
          company_name: "صالح الغامدي للحج والعمرة",
          center_number: "345 542 223 33",
          answer: "محمد القحطاني",
          responsible_person: "محمد القحطاني",
          status: "مغلق",
          reporter: "طلال الشيخي",
        },
        {
          id: 7,
          question: "من هو الشخص المسؤول عن عملية التنظيف؟",
          company_name: "صالح الغامدي للحج والعمرة",
          center_number: "345 542 223 33",
          answer: "محمد القحطاني",
          responsible_person: "محمد القحطاني",
          status: "مغلق",
          reporter: "نايف الرحماني",
        },
        {
          id: 8,
          question: "من هو الشخص المسؤول عن عملية التنظيف؟",
          company_name: "صالح الغامدي للحج والعمرة",
          center_number: "345 542 223 33",
          answer: "محمد القحطاني",
          responsible_person: "محمد القحطاني",
          status: "مغلق",
          reporter: "محمد القحطاني",
        },
      ];
      totalRatings.value = ratings.value.length;
      isLoading.value = false;
    }, 1000);
  } catch (error) {
    console.error("Error fetching ratings:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    ratings.value = [];
    totalRatings.value = 0;
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
    currentPage.value = 1; // Reset to first page when searching
    if (activeTab.value === "notes") {
      fetchNotes();
    } else {
      fetchRatings();
    }
  }, 500); // 500ms debounce
});

// Watch for page changes
watch(currentPage, () => {
  if (activeTab.value === "notes") {
    fetchNotes();
  } else {
    fetchRatings();
  }
});

// Watch for tab changes
watch(activeTab, () => {
  currentPage.value = 1; // Reset to first page when changing tabs
  if (activeTab.value === "notes") {
    fetchNotes();
  } else {
    fetchRatings();
  }
});

onMounted(() => {
  // Initial data fetching based on active tab
  if (activeTab.value === "notes") {
    fetchNotes();
  } else {
    fetchRatings();
  }
});

// Add export function
const exportData = async () => {
  try {
    isExporting.value = true;

    const response = await axios.post(
      "/integrations-export",
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
    let fileName = "locations.xlsx";
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
  } finally {
    isExporting.value = false;
  }
};
// Override the default status styling for the notes component
const getStatusStyle = (status) => {
  switch (status) {
    case "PENDING":
      return { color: "#E3F0DB", textColor: "#7AB55C" };
    case "APPROVED":
      return { color: "#FFF8DF", textColor: "#D4B327" };
    case "REJECTED":
      return { color: "#FFEBEB", textColor: "#FF5252" };
    default:
      return { color: "", textColor: "" };
  }
};
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
              title: '⁠⁠إدارة التكاملات',
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
        <!-- Header Section -->
        <v-card-title class="pt-4 pb-6">
          <div class="d-flex align-center justify-space-between ga-4">
            <h2 class="text-h5 font-weight-bold mb-4">إدارة التكاملات</h2>
            <div class="d-flex align-center ga-2">
              <v-text-field
                v-model="search"
                label="البحث... "
                rounded="lg"
                clearable
                height="48"
                class="bg-white"
                style="
                  width: 320px;
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

              <v-menu
                v-model="filterMenu"
                :close-on-content-click="false"
                location="bottom end"
                min-width="350"
              >
                <template v-slot:activator="{ props }">
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
                      <path
                        fill-rule="evenodd"
                        clip-rule="evenodd"
                        d="M6.18176 2.24732C6.1991 2.24732 6.21649 2.24732 6.23394 2.24732L17.8175 2.24732C18.588 2.24728 19.2422 2.24725 19.7583 2.31762C20.3046 2.3921 20.8269 2.56023 21.2268 3.00041C21.6302 3.44445 21.7406 3.97957 21.7491 4.52659C21.757 5.03745 21.6753 5.67204 21.5801 6.41193L21.5732 6.46581C21.5395 6.72764 21.489 6.98088 21.3836 7.23442C21.2767 7.49184 21.1294 7.71109 20.946 7.92897C19.9664 9.09285 18.1451 11.1962 15.5811 13.1117C15.5396 13.1427 15.4872 13.2161 15.4773 13.3263C15.2282 16.0792 15.009 17.5358 14.8509 18.3796C14.6799 19.2921 13.9839 19.9237 13.3838 20.3585C13.0697 20.5861 12.7364 20.791 12.4394 20.9717C12.4156 20.9862 12.3921 21.0005 12.3689 21.0146C12.0915 21.1833 11.8557 21.3267 11.6586 21.466C11.1181 21.8482 10.4945 21.8214 10.0179 21.5437C9.56811 21.2817 9.25159 20.8038 9.1876 20.2633C9.04713 19.0769 8.79249 16.7542 8.51139 13.3199C8.50238 13.2097 8.48517 13.1778 8.48373 13.1751C8.48275 13.1733 8.48022 13.1686 8.47182 13.1594C8.46243 13.149 8.44366 13.1308 8.40798 13.1041C5.84915 11.191 4.03146 9.09132 3.05314 7.92893C2.87044 7.71185 2.71884 7.49783 2.61076 7.23754C2.50494 6.98273 2.45983 6.7282 2.42607 6.46581C2.42375 6.44778 2.42144 6.42982 2.41914 6.41192C2.32392 5.67203 2.24226 5.03744 2.25016 4.52659C2.25863 3.97957 2.36905 3.44445 2.77244 3.00041C3.17232 2.56023 3.69466 2.3921 4.24094 2.31762C4.75704 2.24725 5.4112 2.24728 6.18176 2.24732ZM4.44357 3.80387C4.05947 3.85624 3.94377 3.94181 3.88271 4.00903C3.82515 4.07239 3.75565 4.18397 3.74998 4.5498C3.74395 4.93944 3.80975 5.46543 3.91382 6.27444C3.94273 6.49924 3.96807 6.59485 3.99606 6.66228C4.02179 6.72422 4.06736 6.80453 4.20077 6.96304C5.15905 8.10163 6.8881 10.0949 9.30617 11.9027C9.50044 12.048 9.67868 12.2278 9.80763 12.4699C9.93468 12.7085 9.98668 12.9567 10.0064 13.1975C10.286 16.6131 10.5388 18.9178 10.6772 20.0869C10.6814 20.1226 10.6944 20.1578 10.7144 20.1884C10.7348 20.2197 10.7575 20.2386 10.773 20.2476C10.7749 20.2488 10.7767 20.2497 10.7781 20.2504C10.7817 20.2485 10.7865 20.2456 10.7926 20.2413C11.0347 20.0701 11.3157 19.8993 11.581 19.7382C11.6074 19.7221 11.6336 19.7062 11.6597 19.6903C11.9585 19.5085 12.2442 19.3318 12.5036 19.1439C13.0505 18.7477 13.3187 18.4124 13.3766 18.1034C13.523 17.3216 13.7369 15.9154 13.9834 13.1911C14.0286 12.692 14.2736 12.2161 14.6833 11.91C17.1063 10.0998 18.8389 8.10302 19.7985 6.96301C19.9148 6.82476 19.9666 6.73562 19.9984 6.65892C20.0319 6.57834 20.06 6.47171 20.0854 6.27444C20.1895 5.46543 20.2553 4.93944 20.2493 4.5498C20.2436 4.18397 20.1741 4.07239 20.1165 4.00903C20.0555 3.94181 19.9398 3.85624 19.5557 3.80387C19.153 3.74897 18.6023 3.74732 17.7653 3.74732H6.23394C5.3969 3.74732 4.84624 3.74897 4.44357 3.80387ZM10.7703 20.2539C10.7703 20.2539 10.7708 20.2537 10.7717 20.2535L10.7703 20.2539Z"
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
                          { title: 'قيد المراجعة', value: 'PENDING' },
                          { title: 'مقبولة', value: 'APPROVED' },
                          { title: 'مرفوضة', value: 'REJECTED' },
                        ]"
                        variant="outlined"
                        hide-details
                        density="comfortable"
                        class="mb-3"
                      ></v-select>
                    </div>

                    <!-- Type Filter -->
                    <div class="mb-4" v-if="activeTab === 'notes'">
                      <v-select
                        v-model="tempTypeFilter"
                        label="نوع الملاحظة"
                        :items="[
                          { title: 'الكل', value: '' },
                          ...noteTypes.map((type) => ({
                            title: type.title,
                            value: type.id.toString(),
                          })),
                        ]"
                        variant="outlined"
                        hide-details
                        density="comfortable"
                        class="mb-3"
                      ></v-select>
                    </div>

                    <!-- Date Filter -->
                    <div class="mb-4">
                      <v-label class="text-body-2 mb-1 d-block"
                        >التاريخ</v-label
                      >
                      <v-date-picker
                        v-model="tempDateFilter"
                        density="compact"
                        elevation="0"
                        hide-details
                        class="rounded-lg border"
                        width="100%"
                      ></v-date-picker>
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
                        d="M3.0014 10.9896C3.34179 10.1536 4.90998 8.49792 8.49805 8.49792C8.79946 8.49792 9.59012 8.19468 9.5492 6.55168C9.53252 5.06667 10.0738 2.12886 12.3445 2.00216C12.7945 2 13.2836 2 13.8182 2C15.6173 2 16.5168 2 17.2352 2.2522C18.3902 2.65765 19.3012 3.5151 19.732 4.60214C20 5.27832 20 6.12494 20 7.81818V17C20 17.9293 20 18.394 19.9231 18.7804C19.6075 20.3671 18.3671 21.6075 16.7804 21.9231C16.394 22 15.9293 22 15 22H10.7273C7.46607 22 5.83546 22 4.70307 21.2022C4.37862 20.9736 4.09058 20.7025 3.8477 20.3971C3 19.3313 3 17.7966 3 14.7273V12.1818C3 11.7548 3 11.3586 3.0014 10.9896Z"
                        fill="#B69265"
                      />
                      <path
                        fill-rule="evenodd"
                        clip-rule="evenodd"
                        d="M11.001 22.7342H10.728C7.31309 22.7342 5.59799 22.7342 4.27101 21.7962C3.89101 21.5262 3.553 21.2102 3.265 20.8552C2.25 19.5742 2.25 17.9502 2.25 14.7132V12.1632C2.25 12.1041 2.25 12.0456 2.25 11.9876L2.25 11.9842C2.25 11.9822 2.25 11.9801 2.25 11.9781C2.25022 9.04974 2.26065 7.55419 2.77101 6.25822C3.60601 4.15322 5.362 2.49825 7.589 1.71525C8.968 1.23325 10.589 1.23322 13.818 1.23322C15.6989 1.23322 16.644 1.23322 17.482 1.52521C18.844 2.00821 19.917 3.02119 20.429 4.30519C20.75 5.12018 20.75 6.01721 20.75 7.80317V12.9832C20.75 13.3972 20.414 13.7332 20 13.7332C19.586 13.7332 19.25 13.3972 19.25 12.9832V7.80324C19.25 6.20738 19.25 5.40619 19.034 4.85822C18.683 3.97822 17.935 3.27725 16.984 2.94025C16.39 2.73325 15.53 2.73322 13.818 2.73322H13.8168C13.5435 2.73322 13.2823 2.73322 13.0325 2.7335C13.0217 2.73396 13.0109 2.73419 13 2.73419C11.576 2.73419 10.417 3.89117 10.417 5.31417C10.417 5.47555 10.425 5.64944 10.4335 5.83324L10.4353 5.87247C10.4602 6.41594 10.4884 7.03045 10.335 7.60724C10.112 8.44624 9.45501 9.10323 8.62001 9.32023C8.04401 9.47123 7.423 9.44423 6.875 9.42023L6.84328 9.4189C6.66294 9.41137 6.49173 9.40421 6.33301 9.40421C4.91097 9.40421 3.75319 10.558 3.75 11.9783C3.75 12.0393 3.75 12.1009 3.75 12.1632V14.7132C3.75 17.6022 3.75 19.0522 4.436 19.9172C4.632 20.1592 4.87 20.3812 5.137 20.5712C6.074 21.2332 7.629 21.2332 10.727 21.2332H11C11.414 21.2332 11.75 21.5692 11.75 21.9832C11.75 22.3972 11.414 22.7332 11 22.7332L11.001 22.7342ZM3.80363 8.78353C4.49967 8.23313 5.37868 7.90421 6.33301 7.90421C6.53401 7.90421 6.74 7.91318 6.94 7.92218L6.94478 7.92239C7.4063 7.94232 7.88323 7.96293 8.24 7.86925C8.554 7.78725 8.8 7.54018 8.885 7.22218C8.98 6.86718 8.958 6.39324 8.936 5.93624C8.926 5.73224 8.917 5.52017 8.917 5.31417C8.917 4.3728 9.23769 3.50495 9.77558 2.81389C9.0919 2.87055 8.555 2.96699 8.086 3.13116C6.27 3.76916 4.841 5.11017 4.166 6.81017C3.96016 7.3327 3.85617 7.94829 3.80363 8.78353Z"
                        fill="#B69265"
                      />
                      <path
                        d="M15.501 21.7335C15.313 21.7335 15.125 21.6636 14.979 21.5226C14.872 21.4186 14.633 21.2315 14.38 21.0335C13.133 20.0555 12.251 19.2996 12.25 18.4856V18.4835C12.25 17.6695 13.133 16.9136 14.38 15.9356C14.633 15.7376 14.872 15.5495 14.979 15.4465C15.277 15.1585 15.751 15.1655 16.04 15.4635C16.328 15.7605 16.321 16.2356 16.023 16.5236C15.86 16.6816 15.616 16.8736 15.307 17.1156L15.3015 17.1199C15.1143 17.2671 14.8232 17.4959 14.541 17.7345H21.001C21.415 17.7345 21.751 18.0705 21.751 18.4845C21.751 18.8985 21.415 19.2345 21.001 19.2345H14.541C14.826 19.4755 15.12 19.7075 15.307 19.8535C15.616 20.0955 15.86 20.2875 16.023 20.4455C16.321 20.7335 16.328 21.2085 16.04 21.5055C15.893 21.6575 15.697 21.7335 15.501 21.7335Z"
                        fill="#B69265"
                      />
                    </svg>
                  </span>
                  <span>تصدير</span>
                </div>
              </VBtn>
            </div>
          </div>

          <!-- Tabs Section -->
          <div class="tabs-container mb-6 border-b-0">
            <div class="d-flex">
              <div
                class="tab-item px-4 py-2"
                :class="{ active: activeTab === 'ratings' }"
                @click="activeTab = 'ratings'"
              >
                ⁠⁠إدارة التقييمات
              </div>
              <div
                class="tab-item px-4 py-2"
                :class="{ active: activeTab === 'notes' }"
                @click="activeTab = 'notes'"
              >
                الملاحظات
              </div>
            </div>
            <div class="tab-border"></div>
          </div>

          <!-- Content Section -->
          <div class="mt-6">
            <!-- Ratings Tab -->
            <div v-if="activeTab === 'ratings'">
              <dataTable
                :headers="tableHeaders"
                :items="ratings"
                :loading="isLoading"
                class="elevation-0"
                :items-per-page="perPage"
                v-model:page="currentPage"
                :hide-default-footer="true"
              />
            </div>

            <!-- Notes Tab -->
            <div v-else>
              <v-row v-if="isLoading">
                <v-col
                  v-for="item in 6"
                  :key="item"
                  cols="12"
                  sm="6"
                  md="4"
                  lg="4"
                  class="pa-3"
                >
                  <v-card elevation="1" class="rounded-lg h-100">
                    <v-card-text class="pa-4 h-100">
                      <v-skeleton-loader
                        class="mx-auto"
                        elevation="0"
                        type="card"
                      ></v-skeleton-loader>
                    </v-card-text>
                  </v-card>
                </v-col>
              </v-row>
              <v-row v-else>
                <v-col
                  v-for="note in notes"
                  :key="note.id"
                  cols="12"
                  sm="6"
                  md="4"
                  lg="4"
                  class="pa-3"
                >
                  <noteCard
                    :noteId="note.id"
                    :note-type="note.report_type?.title"
                    :status="note.status"
                    :description="note.description"
                    :report-date="note.report_date"
                    :report-time="note.report_time"
                    :address="note.address"
                    :reportBy="note.reportBy"
                    :created_at="note.created_at"
                    :latitude="note.latitude"
                    :longitude="note.longitude"
                    :id="note.id"
                  />
                </v-col>
                <v-col v-if="notes.length === 0" cols="12" class="text-center">
                  <p class="text-medium-emphasis">لا توجد ملاحظات متاحة</p>
                </v-col>
              </v-row>
            </div>

            <!-- Pagination -->
            <Pagination
              :total-items="activeTab === 'notes' ? totalNotes : totalRatings"
              v-model:per-page="perPage"
              v-model:page="currentPage"
            />
          </div>
        </v-card-title>
      </VCard>
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
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

.filter-select {
  min-width: 150px;
  max-width: 200px;

  :deep(.v-field) {
    border-radius: 8px;
    background-color: white;

    .v-field__input {
      min-height: 40px;
      padding-top: 0;
      padding-bottom: 0;
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

.tabs-container .tab-border {
  bottom: -16px;
}
.tabs-container {
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

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.fade-enter-to,
.fade-leave-from {
  opacity: 1;
}

.status-chip {
  padding: 4px 12px;
  border-radius: 16px;
  font-size: 0.875rem;
  display: inline-block;
  text-align: center;
  min-width: 80px;
}

.status-closed {
  background-color: #e3f0db;
  color: #7ab55c;
}

.status-open {
  background-color: #fff8df;
  color: #d4b327;
}

.status-rejected {
  background-color: #ffebeb;
  color: #ff5252;
}

.status-pending {
  background-color: #e3f0db;
  color: #7ab55c;
}

.status-approved {
  background-color: #fff8df;
  color: #d4b327;
}
</style>
