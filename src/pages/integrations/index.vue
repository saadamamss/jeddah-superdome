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

// Check if any filters are active
const hasActiveFilters = computed(() => {
  return statusFilter.value !== "" || dateFilter.value !== null;
});

// Data for notes, ratings and integrations
const notes = ref([]);
const ratings = ref([]);
const noteTypes = ref([]);
const totalNotes = ref(0);
const totalRatings = ref(0);

// Filter variables
const statusFilter = ref("");
const dateFilter = ref(null);
const dateMenu = ref(false);
const filterMenu = ref(false);

// Temporary filter values (for keeping state until applied)
const tempStatusFilter = ref("");
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
  { title: "اسم الشركة الرئيسية", key: "company_name", align: "center" },
  { title: "رقم مركز الخدمة", key: "center_number", align: "center" },
  { title: "الإجابة", key: "answer", align: "center" },
  { title: "اسم الراقب", key: "reporter", align: "center" },
  { title: "الحالة", key: "status", align: "center" },
  { title: "سبب الرفض", key: "rejection_reason", align: "center" },
  { title: "الإجراء", key: "actions", align: "center" },
];

// Fetch ratings with pagination and search
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
          reporter: {
            name: "محمد القحطاني",
            avatar: "https://placehold.co/400",
          },
          status: "مغلق",
          rejection_reason: "-",
        },
        {
          id: 2,
          question: "هل تم تنظيف السكن بالكامل وفق المعايير المطلوبة؟",
          company_name: "الرفادة لخدمات الحجاج",
          center_number: "345 542 223 33",
          answer: "نعم",
          reporter: {
            name: "سعود الغامدي",
            avatar: "https://placehold.co/400",
          },
          status: "مفتوح",
          rejection_reason: "-",
        },
        {
          id: 3,
          question: "ما هي درجة تقييم حالة السكن بعد التنظيف؟",
          company_name: "مؤسسة بني ضيف الأحمد",
          center_number: "345 542 223 33",
          answer: "6",
          reporter: {
            name: "محمد الغامدي",
            avatar: "https://placehold.co/400",
          },
          status: "مرفوض",
          rejection_reason: "غير مقبول",
        },
        {
          id: 4,
          question: "هل تم تنظيف السكن بالكامل وفق المعايير المطلوبة؟",
          company_name: "دار الطيوب للعمرة والزيارة",
          center_number: "345 542 223 33",
          answer: "لا",
          reporter: {
            name: "تركي السعيدي",
            avatar: "https://placehold.co/400",
          },
          status: "مرفوض",
          rejection_reason: "غير مقبول",
        },
        {
          id: 5,
          question: "ما هي درجة تقييم حالة السكن بعد التنظيف؟",
          company_name: "قافلة الإيمان",
          center_number: "345 542 223 33",
          answer: "7",
          reporter: {
            name: "محمد القحطاني",
            avatar: "https://placehold.co/400",
          },
          status: "مفتوح",
          rejection_reason: "-",
        },
        {
          id: 6,
          question: "من هو الشخص المسؤول عن عملية التنظيف؟",
          company_name: "صالح الغامدي للحج والعمرة",
          center_number: "345 542 223 33",
          answer: "محمد القحطاني",
          reporter: {
            name: "طلال الشيخي",
            avatar: "https://placehold.co/400",
          },
          status: "مغلق",
          rejection_reason: "-",
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
    currentPage.value = 1;
    fetchRatings();
  }, 500);
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
  currentPage.value = 1;
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
      "/integrations-export",
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
    console.error("Download failed:", error);
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

const availableTypes = ref([
  { title: "منصة الرصد و التحكم", value: "all" },
  { title: "منصة سحاب", value: "attendance" },
  // { title: "متأخر", value: "late" },
]);

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
          :number="homeData?.data?.statistics?.camps_count || 0"
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
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M12.3328 10.8332C13.2286 10.8332 13.8286 11.5043 14.1523 12.1598L14.1529 12.161L15.0803 14.0311L15.088 14.0385C15.0943 14.0443 15.1018 14.0505 15.1101 14.0567C15.1184 14.0628 15.1266 14.0683 15.1342 14.0728L15.1453 14.0791L16.8187 14.3594C17.539 14.4804 18.3226 14.8665 18.5877 15.6978C18.8523 16.5278 18.4386 17.2964 17.9218 17.8145L17.9206 17.8158L16.6195 19.1276C16.615 19.137 16.609 19.1515 16.6038 19.1701C16.5979 19.1907 16.5952 19.2084 16.5942 19.22L16.9666 20.8437C17.1347 21.5789 17.1507 22.6002 16.358 23.1829C15.5612 23.7686 14.5904 23.4396 13.9434 23.0543L12.3745 22.1179L12.3645 22.1165C12.3559 22.1155 12.346 22.1149 12.3355 22.1149C12.3249 22.1149 12.3149 22.1155 12.3058 22.1165C12.2999 22.1172 12.2949 22.118 12.291 22.1188L10.7241 23.054C10.0749 23.4424 9.10638 23.766 8.3106 23.1806C7.51975 22.5988 7.53124 21.5807 7.70057 20.8428L8.07279 19.22C8.07182 19.2084 8.06906 19.1907 8.06321 19.1701C8.05794 19.1515 8.05199 19.137 8.04745 19.1276L6.74402 17.8134C6.23035 17.2954 5.81841 16.5279 6.08074 15.6996C6.34429 14.8675 7.12746 14.4805 7.84875 14.3593L9.51649 14.0799L9.52598 14.0745C9.53361 14.0699 9.54188 14.0643 9.5503 14.058C9.55872 14.0518 9.56633 14.0454 9.57285 14.0395L9.58123 14.0314L10.5106 12.1573C10.8369 11.5028 11.4384 10.8332 12.3328 10.8332ZM11.3643 14.9375L12.3006 13.0495C12.3108 13.029 12.3212 13.0092 12.3311 12.9914C12.3403 13.0083 12.3497 13.0264 12.3592 13.0458L12.3602 13.0477L13.2973 14.9375C13.4502 15.2498 13.6907 15.4934 13.9164 15.6613C14.1434 15.8302 14.4461 15.9896 14.7867 16.0469L16.4878 16.3318C16.5135 16.3362 16.5372 16.3409 16.5589 16.3458C16.5431 16.3636 16.5253 16.3826 16.5053 16.4026L16.5029 16.405L15.1804 17.7384C14.9194 18.0016 14.7628 18.3315 14.6798 18.6238C14.5966 18.9168 14.5577 19.2764 14.6369 19.6328L15.0171 21.2901C15.0239 21.3198 15.0297 21.3481 15.0348 21.3749C15.0134 21.3632 14.9907 21.3502 14.9676 21.3365L13.3739 20.3853C13.0375 20.1844 12.6579 20.1149 12.3355 20.1149C12.012 20.1149 11.6348 20.1847 11.2995 20.3814L9.69759 21.3375C9.67488 21.3511 9.65304 21.3637 9.63209 21.3752C9.63714 21.3484 9.64307 21.3199 9.64991 21.2901L10.0286 19.6394L10.03 19.6328C10.1093 19.2764 10.0704 18.9168 9.98717 18.6238C9.90418 18.3315 9.74757 18.0016 9.48656 17.7384L8.16404 16.405C8.14329 16.3841 8.12488 16.3643 8.10859 16.3457C8.13014 16.3408 8.15415 16.336 8.17969 16.3317L9.88033 16.0469L9.88387 16.0463C10.2228 15.9882 10.5233 15.8277 10.7478 15.6599C10.9715 15.4927 11.2116 15.2495 11.3643 14.9375Z"
                fill="#B69265"
              />
            </svg>
          </template>
        </StatsWidget>
      </v-col>
      <v-col cols="6" md="3">
        <StatsWidget
          title="عدد الملاحظات المرسلة"
          :number="homeData?.data?.statistics?.locations_count || 0"
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
              <path
                d="M9.33398 11.167C9.33398 11.7193 9.7817 12.167 10.334 12.167H15.0007C15.5529 12.167 16.0007 11.7193 16.0007 11.167C16.0007 10.6147 15.5529 10.167 15.0007 10.167H10.334C9.7817 10.167 9.33398 10.6147 9.33398 11.167Z"
                fill="#B69265"
              />
              <path
                d="M9.33398 17.8337C9.33398 18.3859 9.7817 18.8337 10.334 18.8337H19.6673C20.2196 18.8337 20.6673 18.3859 20.6673 17.8337C20.6673 17.2814 20.2196 16.8337 19.6673 16.8337H10.334C9.7817 16.8337 9.33398 17.2814 9.33398 17.8337Z"
                fill="#B69265"
              />
            </svg>
          </template>
        </StatsWidget>
      </v-col>
      <v-col cols="6" md="3">
        <StatsWidget title="إغلاق منصة سحاب " :number="0">
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
              <path
                d="M26.6973 2.44942C26.3013 2.06449 25.6682 2.07348 25.2832 2.46952C24.8983 2.86555 24.9073 3.49865 25.3033 3.88359C25.5188 4.093 25.8616 4.36264 26.1879 4.61895L26.2639 4.67859C26.5915 4.93578 26.9419 5.21087 27.2734 5.49176L27.2829 5.49984L18.667 5.49984C18.1147 5.49984 17.667 5.94755 17.667 6.49984C17.667 7.05212 18.1147 7.49984 18.667 7.49984L27.2829 7.49984L27.2734 7.50792C26.9419 7.78881 26.5915 8.06385 26.2639 8.32105L26.1879 8.38073C25.8616 8.63703 25.5188 8.90667 25.3033 9.11609C24.9073 9.50103 24.8983 10.1341 25.2832 10.5302C25.6682 10.9262 26.3013 10.9352 26.6973 10.5503C26.819 10.432 27.0614 10.2378 27.4233 9.95356L27.5032 9.89082C27.8261 9.6374 28.2048 9.34008 28.5663 9.03379C28.9537 8.70552 29.3561 8.33809 29.6689 7.97079C29.8257 7.78673 29.9803 7.57926 30.0998 7.35607C30.215 7.14082 30.3337 6.84422 30.3337 6.49984C30.3337 6.15546 30.215 5.85885 30.0998 5.6436C29.9803 5.42041 29.8257 5.21295 29.6689 5.02889C29.3561 4.66159 28.9537 4.29416 28.5663 3.96589C28.2049 3.6596 27.8261 3.36234 27.5033 3.10893L27.4233 3.04611C27.0614 2.76186 26.819 2.56769 26.6973 2.44942Z"
                fill="#B69265"
              />
              <path
                d="M15.6665 3.80326C15.6831 4.35529 15.249 4.81621 14.6969 4.83273C14.1843 4.84808 13.6751 4.87249 13.1726 4.90592C8.10734 5.24289 4.05253 9.3421 3.71857 14.5137C3.6498 15.5786 3.6498 16.6828 3.71857 17.7477C3.8358 19.5631 4.6451 21.3005 5.67006 22.8408C5.68536 22.8638 5.6997 22.8874 5.71305 22.9116C6.60379 24.5256 5.95075 26.3662 5.28517 27.6285C5.1758 27.8359 5.086 28.0063 5.01166 28.1549C5.11244 28.1584 5.22591 28.1612 5.35725 28.1644C6.78203 28.1991 7.68536 27.8048 8.40476 27.2739L8.4283 27.2565C8.65986 27.0856 8.86964 26.9308 9.04327 26.8183C9.19402 26.7207 9.46996 26.5484 9.80542 26.5071C10.1568 26.4639 10.4975 26.5834 10.6675 26.6432C10.885 26.7198 11.1615 26.8337 11.4809 26.9654L11.5067 26.976C12.0126 27.1845 12.6181 27.3186 13.1726 27.3555C15.0273 27.4788 16.9696 27.4791 18.8281 27.3555C23.8933 27.0185 27.9481 22.9193 28.2821 17.7477C28.3509 16.6828 28.3509 15.5786 28.2821 14.5137C28.2395 13.855 28.1367 13.214 27.9792 12.5963C27.8428 12.0612 28.166 11.5167 28.7012 11.3803C29.2364 11.2439 29.7808 11.5671 29.9172 12.1023C30.105 12.839 30.2274 13.6022 30.2779 14.3848C30.3522 15.5355 30.3522 16.7258 30.2779 17.8766C29.8808 24.0268 25.0503 28.9459 18.9608 29.351C17.0138 29.4806 14.9829 29.4803 13.0398 29.351C12.2865 29.3009 11.4665 29.1227 10.7446 28.8251C10.4274 28.6943 10.2119 28.6059 10.0543 28.548C9.94589 28.6226 9.80194 28.7285 9.59236 28.8831C8.53578 29.6629 7.20174 30.2099 5.30854 30.1638L5.24757 30.1623C4.8824 30.1536 4.49306 30.1443 4.17555 30.0828C3.79296 30.0087 3.31996 29.8236 3.02403 29.3187C2.70209 28.7693 2.83113 28.2139 2.95609 27.8638C3.07403 27.5334 3.2785 27.1459 3.48744 26.7498L3.51605 26.6956C4.13783 25.5164 4.31139 24.5521 3.97856 23.9089C2.8688 22.2324 1.8706 20.1665 1.72272 17.8766C1.64841 16.7258 1.64841 15.5355 1.72272 14.3848C2.11988 8.23461 6.95035 3.31543 13.0398 2.91033C13.5672 2.87524 14.1008 2.84968 14.6371 2.83363C15.1891 2.8171 15.65 3.25122 15.6665 3.80326Z"
                fill="#B69265"
              />
              <path
                d="M10.3337 20.4998C10.3337 21.0521 10.7814 21.4998 11.3337 21.4998H20.667C21.2193 21.4998 21.667 21.0521 21.667 20.4998C21.667 19.9476 21.2193 19.4998 20.667 19.4998H11.3337C10.7814 19.4998 10.3337 19.9476 10.3337 20.4998Z"
                fill="#B69265"
              />
              <path
                d="M10.3337 13.8332C10.3337 14.3855 10.7814 14.8332 11.3337 14.8332H16.0003C16.5526 14.8332 17.0003 14.3855 17.0003 13.8332C17.0003 13.2809 16.5526 12.8332 16.0003 12.8332H11.3337C10.7814 12.8332 10.3337 13.2809 10.3337 13.8332Z"
                fill="#B69265"
              />
            </svg>
          </template>
        </StatsWidget>
      </v-col>
      <v-col cols="6" md="3">
        <StatsWidget title="إغلاق منصة الرصد و التحكم" :number="0">
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
              <path
                d="M26.6973 2.44942C26.3013 2.06449 25.6682 2.07348 25.2832 2.46952C24.8983 2.86555 24.9073 3.49865 25.3033 3.88359C25.5188 4.093 25.8616 4.36264 26.1879 4.61895L26.2639 4.67859C26.5915 4.93578 26.9419 5.21087 27.2734 5.49176L27.2829 5.49984L18.667 5.49984C18.1147 5.49984 17.667 5.94755 17.667 6.49984C17.667 7.05212 18.1147 7.49984 18.667 7.49984L27.2829 7.49984L27.2734 7.50792C26.9419 7.78881 26.5915 8.06385 26.2639 8.32105L26.1879 8.38073C25.8616 8.63703 25.5188 8.90667 25.3033 9.11609C24.9073 9.50103 24.8983 10.1341 25.2832 10.5302C25.6682 10.9262 26.3013 10.9352 26.6973 10.5503C26.819 10.432 27.0614 10.2378 27.4233 9.95356L27.5032 9.89082C27.8261 9.6374 28.2048 9.34008 28.5663 9.03379C28.9537 8.70552 29.3561 8.33809 29.6689 7.97079C29.8257 7.78673 29.9803 7.57926 30.0998 7.35607C30.215 7.14082 30.3337 6.84422 30.3337 6.49984C30.3337 6.15546 30.215 5.85885 30.0998 5.6436C29.9803 5.42041 29.8257 5.21295 29.6689 5.02889C29.3561 4.66159 28.9537 4.29416 28.5663 3.96589C28.2049 3.6596 27.8261 3.36234 27.5033 3.10893L27.4233 3.04611C27.0614 2.76186 26.819 2.56769 26.6973 2.44942Z"
                fill="#B69265"
              />
              <path
                d="M15.6665 3.80326C15.6831 4.35529 15.249 4.81621 14.6969 4.83273C14.1843 4.84808 13.6751 4.87249 13.1726 4.90592C8.10734 5.24289 4.05253 9.3421 3.71857 14.5137C3.6498 15.5786 3.6498 16.6828 3.71857 17.7477C3.8358 19.5631 4.6451 21.3005 5.67006 22.8408C5.68536 22.8638 5.6997 22.8874 5.71305 22.9116C6.60379 24.5256 5.95075 26.3662 5.28517 27.6285C5.1758 27.8359 5.086 28.0063 5.01166 28.1549C5.11244 28.1584 5.22591 28.1612 5.35725 28.1644C6.78203 28.1991 7.68536 27.8048 8.40476 27.2739L8.4283 27.2565C8.65986 27.0856 8.86964 26.9308 9.04327 26.8183C9.19402 26.7207 9.46996 26.5484 9.80542 26.5071C10.1568 26.4639 10.4975 26.5834 10.6675 26.6432C10.885 26.7198 11.1615 26.8337 11.4809 26.9654L11.5067 26.976C12.0126 27.1845 12.6181 27.3186 13.1726 27.3555C15.0273 27.4788 16.9696 27.4791 18.8281 27.3555C23.8933 27.0185 27.9481 22.9193 28.2821 17.7477C28.3509 16.6828 28.3509 15.5786 28.2821 14.5137C28.2395 13.855 28.1367 13.214 27.9792 12.5963C27.8428 12.0612 28.166 11.5167 28.7012 11.3803C29.2364 11.2439 29.7808 11.5671 29.9172 12.1023C30.105 12.839 30.2274 13.6022 30.2779 14.3848C30.3522 15.5355 30.3522 16.7258 30.2779 17.8766C29.8808 24.0268 25.0503 28.9459 18.9608 29.351C17.0138 29.4806 14.9829 29.4803 13.0398 29.351C12.2865 29.3009 11.4665 29.1227 10.7446 28.8251C10.4274 28.6943 10.2119 28.6059 10.0543 28.548C9.94589 28.6226 9.80194 28.7285 9.59236 28.8831C8.53578 29.6629 7.20174 30.2099 5.30854 30.1638L5.24757 30.1623C4.8824 30.1536 4.49306 30.1443 4.17555 30.0828C3.79296 30.0087 3.31996 29.8236 3.02403 29.3187C2.70209 28.7693 2.83113 28.2139 2.95609 27.8638C3.07403 27.5334 3.2785 27.1459 3.48744 26.7498L3.51605 26.6956C4.13783 25.5164 4.31139 24.5521 3.97856 23.9089C2.8688 22.2324 1.8706 20.1665 1.72272 17.8766C1.64841 16.7258 1.64841 15.5355 1.72272 14.3848C2.11988 8.23461 6.95035 3.31543 13.0398 2.91033C13.5672 2.87524 14.1008 2.84968 14.6371 2.83363C15.1891 2.8171 15.65 3.25122 15.6665 3.80326Z"
                fill="#B69265"
              />
              <path
                d="M10.3337 20.4998C10.3337 21.0521 10.7814 21.4998 11.3337 21.4998H20.667C21.2193 21.4998 21.667 21.0521 21.667 20.4998C21.667 19.9476 21.2193 19.4998 20.667 19.4998H11.3337C10.7814 19.4998 10.3337 19.9476 10.3337 20.4998Z"
                fill="#B69265"
              />
              <path
                d="M10.3337 13.8332C10.3337 14.3855 10.7814 14.8332 11.3337 14.8332H16.0003C16.5526 14.8332 17.0003 14.3855 17.0003 13.8332C17.0003 13.2809 16.5526 12.8332 16.0003 12.8332H11.3337C10.7814 12.8332 10.3337 13.2809 10.3337 13.8332Z"
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
                          { title: 'مغلق', value: 'مغلق' },
                          { title: 'مفتوح', value: 'مفتوح' },
                          { title: 'مرفوض', value: 'مرفوض' },
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
                    <v-btn icon variant="text" size="small" class="eye-btn">
                      <v-icon>mdi-eye-outline</v-icon>
                    </v-btn>
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
