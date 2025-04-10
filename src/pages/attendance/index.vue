<script setup>
// pagination added
import { ref, watch, onMounted } from "vue";
import axios from "@/plugins/axios";
import Pagination from "@/@core/components/Pagination";
import CustomVSelect from "@core/components/global/CustomVSelect.vue";
import { format } from "@core/utils/formatters";
import { printElement } from "@core/utils/helpers";

const router = useRouter();
const route = useRoute();

const isLoading = ref(true);
const isExporting = ref(false);
const isGettingFilters = ref(false);
const ratings = ref([]);
const phases = ref([]);
const contracts = ref([]);

const isDatepickerMenu = ref(false);
const filters = reactive({
  location_type_id: "1",
  search: "",
  status: undefined,
  created_at: undefined,
  imtithal_phase_id: undefined,
  contract_id: undefined,
});

const pagination = reactive({
  per_page: 12,
  page: 1,
  total: 0,
  items_on_page: 0,
});

const onClearDateFilter = () => {
  filters.created_at = undefined;
};

const fetchRatings = async () => {
  try {
    isLoading.value = true;
    const response = await axios.get("/ratings", {
      params: {
        page: pagination.page,
        per_page: pagination.per_page,
        ...filters,
        created_at: filters.created_at
          ? format(filters.created_at, "YYYY-MM-DD")
          : null,
      },
    });
    ratings.value = response.data?.data || [];
    const ratingsPagination = response.data?.pagination || {};

    pagination.page = ratingsPagination.i_current_page;
    pagination.per_page = ratingsPagination.i_per_page;
    pagination.total = ratingsPagination.i_total_objects;
    pagination.items_on_page = ratingsPagination.items_on_page;
  } catch (error) {
    console.error("Error fetching ratings:", error);
  } finally {
    isLoading.value = false;
  }
};

watch(
  () => [
    filters.location_type_id,
    filters.status,
    filters.created_at,
    filters.imtithal_phase_id,
    filters.contract_id,
    pagination.page,
    pagination.per_page,
  ],
  () => {
    fetchRatings();
  }
);

watch(
  () => [filters.location_type_id],
  (locationType) => {
    router.push({ query: { ...route.query, location_type: locationType } });
  }
);

const debouncedSearch = ref(null);
watch(
  () => filters.search,
  (newValue) => {
    if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
    debouncedSearch.value = setTimeout(() => {
      filters.page = 1; // Reset to first page when searching
      fetchRatings();
    }, 500); // 500ms debounce
  }
);

const exportData = async () => {
  try {
    isExporting.value = true;

    const response = await axios.post(
      "/ratings-export-excel",
      {},
      {
        responseType: "blob", // Important to receive binary data
      }
    );

    const url = window.URL.createObjectURL(new Blob([response.data]));

    const link = document.createElement("a");
    link.href = url;

    // Optional: Get file name from headers
    const disposition = response.headers["content-disposition"];
    let fileName = "التقييمات.xlsx";
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

const getFiltersUtilities = async () => {
  try {
    isGettingFilters.value = true;
    const phasesResponse = await axios.get("/reports?page=-1");
    phases.value = phasesResponse?.data?.data || [];

    const contractsResponse = await axios.get("/contracts?page=-1");
    contracts.value = contractsResponse?.data?.data || [];
  } catch (error) {
    console.error("Error fetching ratings:", error);
  } finally {
    isGettingFilters.value = false;
  }
};

onMounted(() => {
  if (route.query.location_type) {
    filters.location_type_id = route.query.location_type;
  }

  fetchRatings();
  getFiltersUtilities();
});
</script>

<template>
  <div>
    <v-container>
      <v-row>
        <v-col cols="12">
          <v-card>
            <v-card-title class="text-h5"> سجل الحضور و الإنصراف </v-card-title>
            <v-card-text>
              <!-- Add attendance table or content here -->
              <v-table>
                <thead>
                  <tr>
                    <th>الموظف</th>
                    <th>وقت الحضور</th>
                    <th>وقت الإنصراف</th>
                    <th>التاريخ</th>
                    <th>الحالة</th>
                  </tr>
                </thead>
                <tbody>
                  <!-- Add table rows here -->
                </tbody>
              </v-table>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
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
</style>
