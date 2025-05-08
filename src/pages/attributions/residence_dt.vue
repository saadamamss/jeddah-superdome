<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { hasPermission } from "@core/utils/permissions";
import SearchableAutocomplete from "@/@core/components/global/SearchableAutocomplete.vue";
import Pagination from "@/@core/components/Pagination";
import { useRouter, useRoute } from "vue-router";
import ResidenceCompany from "./components/rasidance_company.vue";
import ResidenceObserversCard from "./components/rasidance_Observers_card.vue";
import ResidenceSupervisorsCard from "./components/rasidance_supervisors_card.vue";
import axios from "@axios";
import { useAppStore } from "@/stores/app";
import AddSiteModal from "./components/siteAssignmentModal.vue";
import { validationRules } from "@/@core/utils/validators";

const router = useRouter();
const route = useRoute();
const appStore = useAppStore();
const inspectorIds = ref([]);

// Data for residence details
const residenceData = ref({
  name: "",
  location: {
    id: 0,
    name: "",
    location_type: { title: "" },
    camp_number: "",
    gate_number: "",
    service_center_no: "",
    delegate_name: "",
    delegate_phone: "",
    service_center_phone: "",
    latitude: "",
    longitude: "",
  },
  company: {
    id: 0,
    name: "",
    license_no: "",
    parent_company_name: "",
    parent_company: null,
    pligrims_nationality: "",
    ratings_count: 0,
    latest_raters: [],
  },
  supervisors: [],
  inspectors: [],
});

const isLoading = ref(true);
const dialog = ref(false);
const search = ref("");
const teamMembers = computed(() => residenceData.value.inspectors || []);
const fieldTeamMembers = ref([]);

// Variables for inspector assignment
const addInspectorDialog = ref(false);
const availableInspectors = ref([]);
const selectedInspectors = ref([]);
const isLoadingInspectors = ref(false);
const isAssigning = ref(false);

// New Assignment Modal
const showNewAssignmentModal = ref(false);
const locations = ref([]);
const companies = ref([]);
const isLoadingLocations = ref(false);
const isLoadingCompanies = ref(false);

// Fetch available inspectors
async function fetchAvailableInspectors() {
  try {
    isLoadingInspectors.value = true;
    const response = await axios.get("/users?role_id=3");
    if (response.data.status?.success) {
      availableInspectors.value = response.data.data || [];
    } else {
      appStore.showSnackbar({
        message: "حدث خطأ أثناء تحميل بيانات المراقبين",
        color: "error",
      });
    }
  } catch (error) {
    console.error("Error fetching inspectors:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء تحميل بيانات المراقبين",
      color: "error",
    });
  } finally {
    isLoadingInspectors.value = false;
  }
}

// Assign inspectors to location
async function assignInspectorsToLocation() {
  if (!inspectorIds.value.length) {
    appStore.showSnackbar({
      message: "الرجاء اختيار مراقب واحد على الأقل",
      color: "error",
    });
    return;
  }

  try {
    isAssigning.value = true;
    const locationId = route.query.location_id;
    const companyId = route.query.company_id;

    const response = await axios.post("/assign-inspectors-to-location", {
      inspector_ids: inspectorIds.value,
      location_id: locationId,
      company_id: companyId,
    });

    if (response.data.status?.success) {
      appStore.showSnackbar({
        message: "تم إسناد المراقبين بنجاح",
        color: "success",
      });
      addInspectorDialog.value = false;
      selectedInspectors.value = [];
      fetchResidenceDetails();
    } else {
      appStore.showSnackbar({
        message: response.data.message || "حدث خطأ أثناء إسناد المراقبين",
        color: "error",
      });
    }
  } catch (error) {
    console.error("Error assigning inspectors:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء إسناد المراقبين",
      color: "error",
    });
  } finally {
    isAssigning.value = false;
  }
}

// Open dialog to add inspectors
function openAddInspectorDialog() {
  fetchAvailableInspectors();
  addInspectorDialog.value = true;
}

// Fetch residence details
async function fetchResidenceDetails() {
  try {
    isLoading.value = true;
    // Get id from route params and company_id from query params
    const id = route.params.id;
    const companyId = route.query.company_id;
    const locationId = route.query.location_id;

    // Construct URL with both parameters if available
    let url = `/assign-location/details?company_id=${companyId}&location_id=${locationId}`;

    // Add pagination parameters
    url += `&page=${inspectorsPagination.value.currentPage}&per_page=${inspectorsPagination.value.itemsPerPage}`;

    // Add search parameter if it exists
    if (search.value) {
      url += `&search=${encodeURIComponent(search.value)}`;
    }

    const response = await axios.get(url);

    if (response.data.status?.success) {
      const data = response.data.data;

      // Map the response data to our structure
      residenceData.value = {
        name: data.location?.name || "",
        location: data.location || {
          id: 0,
          name: "",
          location_type: { title: "" },
          camp_number: "",
          gate_number: "",
          service_center_no: "",
          delegate_name: "",
          delegate_phone: "",
          service_center_phone: "",
          latitude: "",
          longitude: "",
        },
        company: data.company || {
          id: 0,
          name: "",
          license_no: "",
          parent_company_name: "",
          parent_company: null,
          pligrims_nationality: "",
          ratings_count: 0,
          latest_raters: [],
        },
        supervisors: data.supervisors || [],
        inspectors:
          data.inspectors_data?.map((item) => {
            const inspector = item.inspector;
            inspector.assign_id = item.assign_id;
            return inspector;
          }) || [],
      };

      inspectorsPagination.value.totalItems =
        data.pagination?.i_total_objects || 0;
      inspectorsPagination.value.currentPage =
        data.pagination?.i_current_page || 1;
      inspectorsPagination.value.itemsPerPage =
        data.pagination?.i_per_page || 6;
    } else {
      appStore.showSnackbar({
        message: "حدث خطأ أثناء تحميل بيانات المسكن",
        color: "error",
      });
    }
  } catch (error) {
    console.error("Error fetching residence details:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء تحميل بيانات المسكن",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
}
// Fetch 100 of inspectors residence details
async function fetchInspectorsList() {
  try {
    isLoading.value = true;
    // Get id from route params and company_id from query params
    const id = route.params.id;
    const companyId = route.query.company_id;
    const locationId = route.query.location_id;

    // Construct URL with both parameters if available
    let url = `/assign-location/details?company_id=${companyId}&location_id=${locationId}`;

    // Add pagination parameters
    url += `&page=${inspectorsPagination.value.currentPage}&per_page=1000`;

    // Add search parameter if it exists
    if (search.value) {
      url += `&search=${encodeURIComponent(search.value)}`;
    }

    const response = await axios.get(url);

    if (response.data.status?.success) {
      const data = response.data.data;

      // Map the response data to our structure
      residenceData.value = {
        name: data.location?.name || "",
        location: data.location || {
          id: 0,
          name: "",
          location_type: { title: "" },
          camp_number: "",
          gate_number: "",
          service_center_no: "",
          delegate_name: "",
          delegate_phone: "",
          service_center_phone: "",
          latitude: "",
          longitude: "",
        },
        company: data.company || {
          id: 0,
          name: "",
          license_no: "",
          parent_company_name: "",
          parent_company: null,
          pligrims_nationality: "",
          ratings_count: 0,
          latest_raters: [],
        },
        supervisors: data.supervisors || [],
        inspectors: data.inspectors_data?.map((item) => item.inspector) || [],
      };

      inspectorsPagination.value.totalItems =
        data.pagination?.i_total_objects || 0;
      inspectorsPagination.value.currentPage =
        data.pagination?.i_current_page || 1;
      inspectorsPagination.value.itemsPerPage =
        data.pagination?.i_per_page || 6;
    } else {
      appStore.showSnackbar({
        message: "حدث خطأ أثناء تحميل بيانات المسكن",
        color: "error",
      });
    }
  } catch (error) {
    console.error("Error fetching residence details:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء تحميل بيانات المسكن",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
}

// Call API on component mount
onMounted(() => {
  fetchResidenceDetails();
});

// Instead of filtering locally, we now get filtered results from the API
const filteredTeamMembers = computed(() => teamMembers.value);

const breadcrumbItems = computed(() => [
  {
    title: "الرئيسية",
    to: "/",
  },
  {
    title: "الإسنادات",
    to: "/attributions",
  },
  {
    title: residenceData.value.location?.name || "اسم المسكن",
    disabled: true,
  },
]);

const inspectorsPagination = ref({
  currentPage: 1,
  itemsPerPage: 6,
  totalItems: 0,
});

// Watch for pagination changes to refetch data
watch(
  () => inspectorsPagination.value.currentPage,
  () => fetchResidenceDetails()
);

watch(
  () => inspectorsPagination.value.itemsPerPage,
  () => {
    inspectorsPagination.value.currentPage = 1;
    fetchResidenceDetails();
  }
);

// Watch for search input changes with debounce
let searchTimeout = null;
watch(
  () => search.value,
  () => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
      inspectorsPagination.value.currentPage = 1;
      fetchResidenceDetails();
    }, 500); // 500ms debounce
  }
);

function handlePageSizeChange(newSize) {
  inspectorsPagination.value.itemsPerPage = newSize;
  inspectorsPagination.value.currentPage = 1;
}

const handleNewAssignment = async (formData) => {
  try {
    const assignmentData = {
      inspector_ids: formData.inspector_ids,
      location_id: formData.location_id,
      company_id: formData.company_id,
    };

    const response = await axios.post(
      "/assign-inspectors-to-location",
      assignmentData
    );

    if (response.data.status?.success) {
      appStore.showSnackbar({
        message: "تم إسناد الموقع بنجاح",
        color: "success",
      });
      showNewAssignmentModal.value = false;
      fetchResidenceDetails();
    } else {
      throw new Error(response.data?.message || "حدث خطأ أثناء إسناد الموقع");
    }
  } catch (error) {
    console.error("Error assigning location:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message ||
        error.message ||
        "حدث خطأ أثناء إسناد الموقع",
      color: "error",
    });
  }
};
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-md-0 py-2 pt-0">
      <div>
        <v-breadcrumbs :items="breadcrumbItems" divider="<"></v-breadcrumbs>

        <v-container fluid class="mt-0 px-0">
          <v-progress-linear
            v-if="isLoading"
            indeterminate
            color="primary"
          ></v-progress-linear>

          <template v-if="!isLoading">
            <!-- Location & Company Section -->
            <ResidenceCompany
              :company="residenceData.company"
              :location="residenceData.location"
            />

            <!-- Supervisors Section -->
            <div class="mt-8 bg-white rounded-lg pa-4">
              <h2 class="text-h6">المشرفون</h2>
              <v-row class=" ">
                <v-col
                  v-for="supervisor in residenceData.supervisors"
                  :key="supervisor.id"
                  cols="12"
                  sm="6"
                  md="6"
                >
                  <ResidenceSupervisorsCard :user="supervisor" />
                </v-col>
              </v-row>
            </div>

            <!-- Team Members Section -->
            <div class="mt-8">
              <div
                class="d-flex align-center justify-space-between bg-white rounded-lg pa-4"
              >
                <h6 class="text-h6 font-weight-medium">
                  المراقبون ({{ teamMembers.length }} مراقبين)
                </h6>
                <div class="d-flex align-center gap-3">
                  <v-text-field
                    v-model="search"
                    placeholder="البحث باسم المراقب أو رقم الهوية..."
                    rounded="lg"
                    clearable
                    hide-details
                    class="bg-light-gray-1"
                    style="width: 320px"
                    density="comfortable"
                  >
                    <template v-slot:prepend-inner>
                      <v-icon color="medium-emphasis">mdi-magnify</v-icon>
                    </template>
                  </v-text-field>

                  <v-btn
                    v-if="hasPermission('teams.create')"
                    color="primary"
                    class="rounded-lg text-none"
                    append-icon="mdi-plus"
                    @click="openAddInspectorDialog"
                  >
                    اسناد مراقب
                  </v-btn>
                </div>
              </div>

              <div class="mt-4">
                <v-row>
                  <v-col
                    v-for="inspector in filteredTeamMembers"
                    :key="inspector.id"
                    cols="12"
                    sm="6"
                    md="4"
                  >
                    <ResidenceObserversCard
                      :user="inspector"
                      :visit-count="inspector.daily_visits_count"
                      @update="fetchResidenceDetails()"
                    />
                  </v-col>
                </v-row>
              </div>
            </div>

            <!-- Pagination -->
            <div class="mt-4" v-if="teamMembers.length > 0">
              <Pagination
                :total-items="inspectorsPagination.totalItems"
                v-model:per-page="inspectorsPagination.itemsPerPage"
                v-model:page="inspectorsPagination.currentPage"
                @update:per-page="handlePageSizeChange"
              />
            </div>
          </template>
        </v-container>
      </div>
    </v-container>

    <!-- Add Inspector Dialog -->
    <v-dialog v-model="addInspectorDialog" max-width="600">
      <v-card>
        <v-card-title class="text-h6 pa-4">
          <h2>إسناد مراقب</h2>
          <span class="text-sm text-[#667178]"
            >قم بإختيار مراقب أو أكثر لإسناد هذا الموقع اليه.</span
          >
        </v-card-title>
        <v-card-text class="pa-4">
          <v-progress-linear
            v-if="isLoadingInspectors"
            indeterminate
            color="primary"
          ></v-progress-linear>

          <div v-if="!isLoadingInspectors">
            <SearchableAutocomplete
              search-url="/users?role_id=3"
              v-model="inspectorIds"
              item-title="label"
              :item-title-concat="['firstname', 'lastname']"
              item-value="id"
              clearable
              multiple
              :return-object="false"
              :rules="[validationRules.required]"
            />
          </div>
        </v-card-text>
        <v-card-actions class="pa-4">
          <v-spacer></v-spacer>
          <v-btn
            color="grey-darken-1"
            variant="text"
            @click="addInspectorDialog = false"
          >
            إلغاء
          </v-btn>
          <v-btn
            color="primary"
            variant="elevated"
            @click="assignInspectorsToLocation"
            :loading="isAssigning"
            :disabled="isAssigning"
          >
            إسناد
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- New Assignment Modal -->
    <AddSiteModal
      :model-value="showNewAssignmentModal"
      @update:model-value="showNewAssignmentModal = $event"
      :inspector-id="route.params.id"
      :locations="locations"
      :companies="companies"
      :loading-locations="isLoadingLocations"
      :loading-companies="isLoadingCompanies"
      :show-inspector-select="false"
      @save="handleNewAssignment"
    />
  </div>
</template>

<style lang="scss" scoped>
.bg-light-gray-1 {
  background-color: rgb(250, 250, 250);
}

:deep(.v-field.v-field--rounded) {
  border-radius: 12px;
}

:deep(.v-text-field .v-field__input) {
  min-height: 44px;
  padding-top: 0;
  padding-bottom: 0;
}
</style>
