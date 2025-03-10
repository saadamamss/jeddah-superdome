<template>
  <div class="corrections-page pa-6">
    <!-- Header Section -->
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h2 class="text-h5 font-weight-medium">التصحيحات</h2>
        <span class="text-subtitle-2 text-medium-emphasis">
          إدارة تصحيحات المواقع
        </span>
      </div>

      <!-- Filters Section -->
      <div class="d-flex align-center gap-4">
        <!-- Search -->
        <v-text-field
          v-model="searchQuery"
          prepend-inner-icon="mdi-magnify"
          placeholder="بحث..."
          density="comfortable"
          variant="outlined"
          hide-details
          class="max-width-200"
        />

        <!-- Status Filter -->
        <div class="status-filter d-flex align-center gap-2">
          <v-chip
            v-for="filter in statusFilters"
            :key="filter.value"
            :color="filter.color"
            :variant="selectedStatus === filter.value ? 'elevated' : 'outlined'"
            @click="selectedStatus = filter.value"
            class="px-4"
          >
            {{ filter.label }}
            <span class="ms-2">({{ getStatusCount(filter.value) }})</span>
          </v-chip>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="d-flex justify-center align-center py-12">
      <v-progress-circular
        indeterminate
        color="primary"
        size="48"
      ></v-progress-circular>
    </div>

    <!-- Content -->
    <template v-else>
      <!-- Cards Grid -->
      <v-row>
        <v-col
          v-for="correction in paginatedCorrections"
          :key="correction.id"
          cols="12"
          sm="6"
          md="4"
          lg="4"
        >
          <CorrectSiteCard
            :correction="correction"
            :loading="loadingStates[correction.id]"
            @approve="handleApprove"
            @reject="handleReject"
          />
        </v-col>
      </v-row>

      <!-- Empty State -->
      <div
        v-if="!loading && filteredCorrections.length === 0"
        class="text-center py-12"
      >
        <v-icon size="64" color="grey-lighten-1" class="mb-4"
          >mdi-clipboard-text-search-outline</v-icon
        >
        <div class="text-h6 text-grey-darken-1">لا توجد تصحيحات</div>
        <div class="text-body-2 text-medium-emphasis mt-2">
          لم يتم العثور على أي تصحيحات تطابق معايير البحث
        </div>
      </div>

      <!-- Pagination -->
      <div
        v-if="filteredCorrections.length > itemsPerPage"
        class="d-flex justify-center mt-6"
      >
        <v-pagination
          v-model="currentPage"
          :length="totalPages"
          :total-visible="7"
          rounded="lg"
        ></v-pagination>
      </div>
    </template>

    <!-- Success Snackbar -->
    <v-snackbar
      v-model="snackbar.show"
      :color="snackbar.color"
      :timeout="3000"
      location="top"
    >
      {{ snackbar.text }}
      <template v-slot:actions>
        <v-btn variant="text" @click="snackbar.show = false">إغلاق</v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import CorrectSiteCard from "@core/components/widgets/CorrectSiteCard.vue";
import axiosIns from "@/plugins/axios";

// State
const corrections = ref([]);
const loading = ref(true);
const loadingStates = ref({});
const searchQuery = ref("");
const selectedStatus = ref("all");
const currentPage = ref(1);
const itemsPerPage = 9;

const snackbar = ref({
  show: false,
  text: "",
  color: "success",
});

// Status Filters
const statusFilters = [
  { label: "الكل", value: "all", color: "grey" },
  { label: "قيد الانتظار", value: "pending", color: "warning" },
  { label: "معتمد", value: "approved", color: "success" },
  { label: "مرفوض", value: "rejected", color: "error" },
];

// Computed Properties
const filteredCorrections = computed(() => {
  let filtered = corrections.value;

  // Filter by status
  if (selectedStatus.value !== "all") {
    filtered = filtered.filter(
      (correction) => correction.status === selectedStatus.value
    );
  }

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    filtered = filtered.filter(
      (correction) =>
        correction.camp_number.toLowerCase().includes(query) ||
        correction.square_number.toLowerCase().includes(query) ||
        correction.contact_number.toLowerCase().includes(query) ||
        correction.created_by.firstname.toLowerCase().includes(query) ||
        correction.created_by.lastname.toLowerCase().includes(query)
    );
  }

  return filtered;
});

const totalPages = computed(() =>
  Math.ceil(filteredCorrections.value.length / itemsPerPage)
);

const paginatedCorrections = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  return filteredCorrections.value.slice(start, end);
});

// Methods
const getStatusCount = (status) => {
  if (status === "all") return corrections.value.length;
  return corrections.value.filter((correction) => correction.status === status)
    .length;
};

const showSnackbar = (text, color = "success") => {
  snackbar.value = {
    show: true,
    text,
    color,
  };
};

const fetchCorrections = async () => {
  loading.value = true;
  try {
    const response = await axiosIns.get("corrections");
    corrections.value = response.data.data;
    // Initialize loading states
    corrections.value.forEach((correction) => {
      loadingStates.value[correction.id] = false;
    });
  } catch (error) {
    console.error("Error fetching corrections:", error);
    showSnackbar("حدث خطأ أثناء جلب التصحيحات", "error");
  } finally {
    loading.value = false;
  }
};

const handleApprove = async (id) => {
  loadingStates.value[id] = true;
  try {
    await axiosIns.post(`corrections/${id}/approve`);
    const correction = corrections.value.find((c) => c.id === id);
    if (correction) {
      correction.status = "approved";
    }
    showSnackbar("تم اعتماد التصحيح بنجاح");
  } catch (error) {
    console.error("Error approving correction:", error);
    showSnackbar("حدث خطأ أثناء اعتماد التصحيح", "error");
  } finally {
    loadingStates.value[id] = false;
  }
};

const handleReject = async (id) => {
  loadingStates.value[id] = true;
  try {
    await axiosIns.post(`corrections/${id}/reject`);
    const correction = corrections.value.find((c) => c.id === id);
    if (correction) {
      correction.status = "rejected";
    }
    showSnackbar("تم رفض التصحيح بنجاح");
  } catch (error) {
    console.error("Error rejecting correction:", error);
    showSnackbar("حدث خطأ أثناء رفض التصحيح", "error");
  } finally {
    loadingStates.value[id] = false;
  }
};

// Watchers
watch([searchQuery, selectedStatus], () => {
  currentPage.value = 1;
});

// Lifecycle
onMounted(() => {
  fetchCorrections();
});
</script>

<style lang="scss" scoped>
.corrections-page {
  .max-width-200 {
    max-width: 200px;
  }

  .status-filter {
    .v-chip {
      cursor: pointer;
      font-weight: 500;
    }
  }
}
</style>
