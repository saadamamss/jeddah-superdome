<script setup>
import { useAppStore } from "@/stores/app";
import noteCard from "@/@core/components/widgets/noteCard";
import Pagination from "@/@core/components/Pagination";
import noteTypeCard from "@/@core/components/widgets/noteTypeCard";
import { ref, onMounted, watch, computed } from "vue";
import axios from "@/plugins/axios";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const loading = ref(false);
const activeTab = ref("all");

// Check if any filters are active
const hasActiveFilters = computed(() => {
  return (
    statusFilter.value !== "" ||
    typeFilter.value !== "" ||
    dateFilter.value !== null
  );
});

// Data for notes and note types
const notes = ref([]);
const noteTypes = ref([]);
const totalNotes = ref(0);
const totalNoteTypes = ref(0);
const newNoteType = ref("");
const addNoteTypeDialog = ref(false);
const editNoteTypeDialog = ref(false);
const selectedNoteType = ref(null);
const editedNoteType = ref("");

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
  fetchNotes();
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
  fetchNotes();
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
  fetchNotes();
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

// Fetch notes with pagination and search
const fetchNotes = async () => {
  try {
    isLoading.value = true;
    const queryParams = new URLSearchParams({
      page: currentPage.value.toString(),
      per_page: perPage.value.toString(),
      search: search.value,
    });

    // Add filters to query params if they are set
    if (statusFilter.value) {
      queryParams.append("status", statusFilter.value);
    }

    if (typeFilter.value) {
      queryParams.append("report_type_id", typeFilter.value);
    }

    if (dateFilter.value) {
      queryParams.append("date", dateFilter.value);
    }

    const response = await axios.get(`/reports?${queryParams.toString()}`);
    if (response.data?.data) {
      notes.value = response.data.data;
      totalNotes.value = response.data.pagination?.i_total_objects || 0;
      perPage.value = response.data.pagination?.i_per_page || perPage.value;
    }
  } catch (error) {
    console.error("Error fetching notes:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    notes.value = [];
    totalNotes.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Fetch note types with pagination and search
const fetchNoteTypes = async () => {
  try {
    isLoading.value = true;
    const queryParams = new URLSearchParams({
      page: currentPage.value.toString(),
      per_page: perPage.value.toString(),
      search: search.value,
    });

    const response = await axios.get(
      `/reports-types?${queryParams.toString()}`
    );
    if (response.data?.data) {
      noteTypes.value = response.data.data;
      totalNoteTypes.value = response.data.meta?.total || 0;
    }
  } catch (error) {
    console.error("Error fetching note types:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب أنواع الملاحظات",
      color: "error",
    });
    noteTypes.value = [];
    totalNoteTypes.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Add new note type
const addNoteType = async () => {
  if (!newNoteType.value.trim()) {
    appStore.showSnackbar({
      message: "يرجى إدخال عنوان نوع الملاحظة",
      color: "error",
    });
    return;
  }

  try {
    loading.value = true;
    const response = await axios.post("/reports-types", {
      title: newNoteType.value.trim(),
    });

    if (response.data) {
      appStore.showSnackbar({
        message: "تم إضافة نوع الملاحظة بنجاح",
        color: "success",
      });
      newNoteType.value = "";
      addNoteTypeDialog.value = false;
      await fetchNoteTypes();
    }
  } catch (error) {
    console.error("Error adding note type:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء إضافة نوع الملاحظة",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Edit note type
const openEditDialog = (noteType) => {
  selectedNoteType.value = noteType;
  editedNoteType.value = noteType.title;
  editNoteTypeDialog.value = true;
};

const editNoteType = async () => {
  if (!editedNoteType.value.trim()) {
    appStore.showSnackbar({
      message: "يرجى إدخال عنوان نوع الملاحظة",
      color: "error",
    });
    return;
  }

  try {
    loading.value = true;
    const response = await axios.put(
      `/reports-types/${selectedNoteType.value.id}`,
      {
        title: editedNoteType.value.trim(),
      }
    );

    if (response.data) {
      appStore.showSnackbar({
        message: "تم تعديل نوع الملاحظة بنجاح",
        color: "success",
      });
      editedNoteType.value = "";
      editNoteTypeDialog.value = false;
      selectedNoteType.value = null;
      await fetchNoteTypes();
    }
  } catch (error) {
    console.error("Error editing note type:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء تعديل نوع الملاحظة",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Delete note type
const deleteNoteType = async (id) => {
  try {
    loading.value = true;
    const response = await axios.delete(`/reports-types/${id}`);

    if (response.data) {
      appStore.showSnackbar({
        message: "تم حذف نوع الملاحظة بنجاح",
        color: "success",
      });
      await fetchNoteTypes();
    }
  } catch (error) {
    console.error("Error deleting note type:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء حذف نوع الملاحظة",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Watch for changes in search with debounce
const debouncedSearch = ref(null);
watch(search, (newValue) => {
  if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
  debouncedSearch.value = setTimeout(() => {
    currentPage.value = 1; // Reset to first page when searching
    if (activeTab.value === "all") {
      fetchNotes();
    } else {
      fetchNoteTypes();
    }
  }, 500); // 500ms debounce
});

// Watch for page changes
watch(currentPage, () => {
  if (activeTab.value === "all") {
    fetchNotes();
  } else {
    fetchNoteTypes();
  }
});

// Watch for tab changes
watch(activeTab, () => {
  currentPage.value = 1; // Reset to first page when changing tabs

  // Reset filters when changing tabs
  if (activeTab.value === "types") {
    // No need for filters in the noteTypes tab
    statusFilter.value = "";
    typeFilter.value = "";
    dateFilter.value = null;
    fetchNoteTypes();
  } else {
    fetchNotes();
  }
});

onMounted(() => {
  fetchNotes();
  // Load note types for the filter dropdown
  fetchNoteTypes();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
      <v-row v-if="isLoading">
        <v-col v-for="item in 4" :key="item" cols="6" sm="6" md="3">
          <VCard elevation="0">
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

      <div v-else>
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
                title: 'إدارة الملاحظات',
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
              <h2 class="text-h5 font-weight-bold mb-4">إدارة الملاحظات</h2>
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
                        >فلتر البيانات</span
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

                      <!-- Report Type Filter -->
                      <div class="mb-4">
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

                <VBtn
                  height="48"
                  elevation="0"
                  class="rounded-lg"
                  append-icon="mdi-plus"
                  @click="addNoteTypeDialog = true"
                >
                  إضافة نوع ملاحظة
                </VBtn>
              </div>
            </div>
            <!-- Search and Add Section -->

            <!-- Tabs Section -->
            <div class="tabs-container mb-6 border-b-0">
              <div class="d-flex">
                <div
                  class="tab-item px-4 py-2"
                  :class="{ active: activeTab === 'all' }"
                  @click="activeTab = 'all'"
                >
                  قائمة الملاحظات
                </div>
                <div
                  class="tab-item px-4 py-2"
                  :class="{ active: activeTab === 'types' }"
                  @click="activeTab = 'types'"
                >
                  اداره انواع الملاحظات
                </div>
              </div>
              <div class="tab-border"></div>
            </div>

            <v-row class="mt-6">
              <template v-if="activeTab === 'all'">
                <v-col
                  v-for="note in notes"
                  :key="note.id"
                  cols="12"
                  sm="6"
                  md="4"
                  lg="4"
                  class="pa-3"
                >
                  <!-- {{ note.report_by }} -->
                  <noteCard
                    :note-id="note.id"
                    :note-type="note.report_type?.title"
                    :status="note.status"
                    :description="note.description"
                    :report-date="note.report_date"
                    :report-time="note.report_time"
                    :address="note.address"
                    :reportBy="note.report_by"
                    :latitude="note.latitude"
                    :longitude="note.longitude"
                    :created-at="note.created_at"
                    :id="note.id"
                  />
                </v-col>
                <v-col
                  v-if="!isLoading && notes.length === 0"
                  cols="12"
                  class="text-center"
                >
                  <p class="text-medium-emphasis">لا توجد ملاحظات متاحة</p>
                </v-col>
              </template>
              <template v-else>
                <v-col
                  v-for="type in noteTypes"
                  :key="type.id"
                  cols="12"
                  sm="6"
                  md="4"
                  lg="4"
                  class="pa-3"
                >
                  <noteTypeCard
                    :id="type.id"
                    :title="type.title"
                    :total_reports="type.total_reports || 0"
                    :created_at="type.created_at"
                    :updated_at="type.updated_at || type.created_at"
                    @edit="openEditDialog(type)"
                    @delete="deleteNoteType(type.id)"
                  />
                </v-col>
                <v-col
                  v-if="!isLoading && noteTypes.length === 0"
                  cols="12"
                  class="text-center"
                >
                  <p class="text-medium-emphasis">
                    لا توجد أنواع ملاحظات متاحة
                  </p>
                </v-col>
              </template>
            </v-row>

            <!-- pagination -->
            <Pagination
              :total-items="activeTab === 'all' ? totalNotes : totalNoteTypes"
              v-model:per-page="perPage"
              v-model:page="currentPage"
            />
          </v-card-title>
        </VCard>
      </div>
    </v-container>

    <!-- Add Note Type Dialog -->
    <v-dialog v-model="addNoteTypeDialog" max-width="500px">
      <v-card class="pa-4">
        <v-card-title class="text-h5 mb-4">إضافة نوع ملاحظة جديد</v-card-title>
        <v-card-text>
          <v-text-field
            v-model="newNoteType"
            label="عنوان نوع الملاحظة"
            required
          ></v-text-field>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="error"
            variant="text"
            @click="addNoteTypeDialog = false"
          >
            إلغاء
          </v-btn>
          <v-btn color="primary" :loading="loading" @click="addNoteType">
            إضافة
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Edit Note Type Dialog -->
    <v-dialog v-model="editNoteTypeDialog" max-width="500px">
      <v-card class="pa-4">
        <v-card-title class="text-h5 mb-4">تعديل نوع الملاحظة</v-card-title>
        <v-card-text>
          <v-text-field
            v-model="editedNoteType"
            label="عنوان نوع الملاحظة"
            required
          ></v-text-field>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="error"
            variant="text"
            @click="editNoteTypeDialog = false"
          >
            إلغاء
          </v-btn>
          <v-btn color="primary" :loading="loading" @click="editNoteType">
            حفظ
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
  // background-color: #f00 !important;
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
</style>
