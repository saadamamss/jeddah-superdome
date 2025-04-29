<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2 pt-0">
      <!-- ... existing stats row ... -->

      <v-container fluid class="mt-6 px-0 rounded-lg">
        <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
          <v-card-title class="pt-4 pb-6">
            <div class="d-flex align-center justify-space-between ga-4">
              <h2 class="text-h5 font-weight-bold mb-4">إدارة المواقع</h2>

              <div class="d-flex align-center ga-2">
                <!-- ... existing filter menu ... -->

                <!-- Search Field -->
                <v-text-field
                  v-model="search"
                  :label="
                    activeTab === 'correction'
                      ? 'البحث في التصحيحات...'
                      : 'البحث المخيم او المسكن...'
                  "
                  rounded="lg"
                  clearable
                  height="48"
                  class="bg-white me-4"
                  style="
                    width: 320px;
                    max-width: 100%;
                    color: #929a9f !important;
                    border-radius: 1rem;
                  "
                />

                <!-- Add New Site Button - Hide in correction tab -->
                <v-btn
                  v-if="activeTab !== 'correction'"
                  color="primary"
                  class="me-4"
                  prepend-icon="mdi-plus"
                  @click="addNewLocation"
                >
                  إضافة موقع جديد
                </v-btn>
              </div>
            </div>

            <!-- Tabs Section -->
            <div class="tabs-container mb-6 border-b-0">
              <div class="d-flex">
                <div
                  class="tab-item px-4 py-2"
                  :class="{ active: activeTab === 'all' }"
                  @click="handleAllSitesTabClick"
                >
                  جميع المواقع
                </div>
                <div
                  class="tab-item px-4 py-2"
                  :class="{ active: activeTab === 'correction' }"
                  @click="handleCorrectionTabClick"
                >
                  تصحيح المواقع
                  <v-chip
                    v-if="getPendingCorrectionsCount > 0"
                    size="small"
                    color="warning"
                    class="ms-2"
                  >
                    {{ getPendingCorrectionsCount }}
                  </v-chip>
                </div>
              </div>
              <div class="tab-border"></div>
            </div>

            <!-- Loading State -->
            <div
              v-if="isLoading"
              class="d-flex justify-center align-center py-12"
            >
              <v-progress-circular
                indeterminate
                color="primary"
                size="48"
              ></v-progress-circular>
            </div>

            <!-- Content Area -->
            <template v-else>
              <v-row class="mt-6">
                <!-- Regular Sites -->
                <template v-if="activeTab === 'all'">
                  <v-col
                    v-for="site in filteredSites"
                    :key="site.id"
                    cols="12"
                    sm="6"
                    md="4"
                    lg="4"
                    class="pa-3"
                  >
                    <SiteCard
                      :site-id="site.id"
                      :name="site.name"
                      :license-no="site.license_no"
                      :delegate-phone="site.delegate_phone"
                      :companies-count="site.companies_count"
                      :location-type="site.location_type"
                      :status="site.status"
                      @update="handleUpdate"
                    />
                  </v-col>

                  <!-- Empty State for Sites -->
                  <v-col v-if="filteredSites.length === 0" cols="12">
                    <div class="text-center py-8">
                      <v-icon size="64" color="grey-lighten-1" class="mb-4">
                        mdi-domain
                      </v-icon>
                      <div class="text-h6 text-grey-darken-1">
                        لا توجد مواقع
                      </div>
                      <div class="text-body-2 text-medium-emphasis mt-2">
                        لم يتم العثور على أي مواقع تطابق معايير البحث
                      </div>
                    </div>
                  </v-col>
                </template>

                <!-- Corrections -->
                <template v-else>
                  <v-col
                    v-for="correction in filteredCorrections"
                    :key="correction.id"
                    cols="12"
                    sm="6"
                    md="4"
                    lg="4"
                    class="pa-3"
                  >
                    <CorrectSiteCard
                      :correction="correction"
                      :loading="loadingStates[correction.id]"
                      @approve="handleApprove"
                      @reject="handleReject"
                    />
                  </v-col>

                  <!-- Empty State for Corrections -->
                  <v-col v-if="filteredCorrections.length === 0" cols="12">
                    <div class="text-center py-8">
                      <v-icon size="64" color="grey-lighten-1" class="mb-4">
                        mdi-clipboard-text-search-outline
                      </v-icon>
                      <div class="text-h6 text-grey-darken-1">
                        لا توجد تصحيحات
                      </div>
                      <div class="text-body-2 text-medium-emphasis mt-2">
                        لم يتم العثور على أي تصحيحات تطابق معايير البحث
                      </div>
                    </div>
                  </v-col>
                </template>
              </v-row>

              <!-- Pagination -->
              <div v-if="totalItems > 0" class="d-flex justify-center mt-6">
                <Pagination
                  :total-items="totalItems"
                  v-model:per-page="perPage"
                  v-model:page="currentPage"
                  @update:page="fetchData"
                />
              </div>
            </template>
          </v-card-title>
        </VCard>
      </v-container>
    </v-container>

    <!-- ... existing dialogs ... -->
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from "vue";
import SiteCard from "@/@core/components/widgets/SiteCard.vue";
import CorrectSiteCard from "@/@core/components/widgets/CorrectSiteCard.vue";
import AppDateTimePicker from "@/@core/components/AppDateTimePicker";
import StatsWidget from "@/@core/components/widgets/StatsWidget";
import Pagination from "@/@core/components/Pagination";
import axios from "@/plugins/axios";
import { useAppStore } from "@/stores/app";

// State
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const loading = ref(false);
const filterMenu = ref(false);
const activeTab = ref("all");
const sites = ref([]);
const corrections = ref([]);
const totalItems = ref(0);
const loadingStates = ref({});

// Filters
const filters = ref({
  type: null,
  status: null,
  dateRange: null,
});

// Computed Properties
const filteredSites = computed(() => {
  if (activeTab.value === "correction") return [];

  let filtered = sites.value;
  if (search.value) {
    const query = search.value.toLowerCase();
    filtered = filtered.filter(
      (site) =>
        site.name?.toLowerCase().includes(query) ||
        site.license_no?.toLowerCase().includes(query) ||
        site.delegate_phone?.toLowerCase().includes(query)
    );
  }
  return filtered;
});

const filteredCorrections = computed(() => {
  if (activeTab.value !== "correction") return [];

  let filtered = corrections.value;
  if (search.value) {
    const query = search.value.toLowerCase();
    filtered = filtered.filter(
      (correction) =>
        correction.camp_number?.toLowerCase().includes(query) ||
        correction.square_number?.toLowerCase().includes(query) ||
        correction.contact_number?.toLowerCase().includes(query) ||
        correction.created_by?.firstname?.toLowerCase().includes(query) ||
        correction.created_by?.lastname?.toLowerCase().includes(query)
    );
  }
  return filtered;
});

const getPendingCorrectionsCount = computed(() => {
  return corrections.value.filter((item) => item.status === "pending").length;
});

// Methods
const fetchData = async (page = 1) => {
  try {
    isLoading.value = true;
    const endpoint =
      activeTab.value === "correction" ? "location-corrections" : "locations";

    const response = await axios.get(
      `${endpoint}?${new URLSearchParams({
        page: page.toString(),
        search: search.value,
        ...(filters.value.type && { type: filters.value.type }),
        ...(filters.value.status && { status: filters.value.status }),
        ...(filters.value.dateRange && {
          start_date: filters.value.dateRange[0],
          end_date: filters.value.dateRange[1],
        }),
      }).toString()}`
    );

    if (activeTab.value === "correction") {
      corrections.value = response.data.data;
      totalItems.value = response.data.pagination?.i_total_objects || 0;
      perPage.value = response.data.pagination?.i_per_page || perPage.value;
      currentPage.value = response.data.pagination?.i_current_page || 1;

      // Initialize loading states for corrections
      corrections.value.forEach((correction) => {
        loadingStates.value[correction.id] = false;
      });
    } else {
      if (response.data?.data?.locations) {
        sites.value = response.data.data.locations;
        if (response.data.data.statistics) {
          totalItems.value = response.data.data.statistics.total_count || 0;
        }
      } else {
        sites.value = [];
        totalItems.value = 0;
      }
    }
  } catch (error) {
    console.error("Error fetching data:", error);
    useAppStore().showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

const handleCorrectionTabClick = () => {
  activeTab.value = "correction";
  currentPage.value = 1;
  fetchData();
};

const handleAllSitesTabClick = () => {
  activeTab.value = "all";
  currentPage.value = 1;
  fetchData();
};

const handleApprove = async (id) => {
  loadingStates.value[id] = true;
  try {
    await axios.post(`corrections/${id}/approve`);
    const correction = corrections.value.find((c) => c.id === id);
    if (correction) {
      correction.status = "approved";
    }
    useAppStore().showSnackbar({
      message: "تم اعتماد التصحيح بنجاح",
      color: "success",
    });
  } catch (error) {
    console.error("Error approving correction:", error);
    useAppStore().showSnackbar({
      message: "حدث خطأ أثناء اعتماد التصحيح",
      color: "error",
    });
  } finally {
    loadingStates.value[id] = false;
  }
};

const handleReject = async (id) => {
  loadingStates.value[id] = true;
  try {
    await axios.post(`corrections/${id}/reject`);
    const correction = corrections.value.find((c) => c.id === id);
    if (correction) {
      correction.status = "rejected";
    }
    useAppStore().showSnackbar({
      message: "تم رفض التصحيح بنجاح",
      color: "success",
    });
  } catch (error) {
    console.error("Error rejecting correction:", error);
    useAppStore().showSnackbar({
      message: "حدث خطأ أثناء رفض التصحيح",
      color: "error",
    });
  } finally {
    loadingStates.value[id] = false;
  }
};

// Watchers
watch([search, activeTab], () => {
  currentPage.value = 1;
  fetchData();
});

// Lifecycle
onMounted(() => {
  fetchData();
});
</script>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
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
      background-color: rgba(248, 249, 249, 1);
      border-radius: 8px;
    }
  }

  .tab-border {
    position: absolute;
    bottom: -16px;
    left: 0;
    right: 0;
    height: 1px;
    background-color: rgba(0, 0, 0, 0.12);
  }
}
</style>
