<script setup>
// pagination added
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
// import StatisticsBar from "@/@core/components/widgets/StatisticsBar.vue";
// import supervisorCard from "@/@core/components/widgets/supervisorCard.vue";
// import UserWidget from "@/@core/components/widgets/UserWidget.vue";
import inspectorsCard from "@/@core/components/widgets/inspectorsCard.vue";
import { ref, onMounted, computed, watch } from "vue";
import { useRoute } from "vue-router";
import axios from "@/plugins/axios";
import BreadcrumbNav from "@/components/BreadcrumbNav.vue";
import { useBreadcrumbs } from "@/composables/useBreadcrumbs";
import { hasPermission } from "@core/utils/helpers";

const appStore = useAppStore();
const isLoading = ref(true);
const route = useRoute();
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const dialog = ref(false);
const dialogStep = ref(1);
const loading = ref(false);
const siteType = ref("السكن");
const housingPermitNumber = ref("");
const authorizedContactNumber = ref("");
const selectedCompany = ref(null);
const activeTab = ref("all");
const teamData = ref(null);
const teamMembers = ref([]);
const shiftsData = ref([]);
const supervisors = ref([]);
const fieldTeamMembers = ref([]);
const zones = ref([]);
const teamZones = ref([]);
const mapDialog = ref(false);
const name = ref("");
const inspectorsPagination = ref({
  totalItems: 0,
  itemsPerPage: 15,
  currentPage: 1,
  totalPages: 1,
});

// Form data and options
const formModel = ref({
  zone_id: null,
  supervisor_id: null,
  inspector_ids: [],
  shift_id: null,
});

// API functions
const fetchTeamData = async () => {
  try {
    isLoading.value = true;
    const response = await axios.get(`/teams/${route.params.id}`, {
      params: {
        page: inspectorsPagination.value.currentPage,
        per_page: inspectorsPagination.value.itemsPerPage,
        search: search.value || undefined,
      },
    });
    teamData.value = response.data;

    // Extract team members and shifts from the response if available
    if (teamData.value?.data) {
      teamMembers.value = teamData.value.data.inspectors || [];
      teamZones.value = teamData.value.data.zones || [];
      shiftsData.value = teamData.value.data.shifts || [];

      // Update pagination data
      if (teamData.value.data.inspectors_pagination) {
        const paginationData = teamData.value.data.inspectors_pagination;
        inspectorsPagination.value = {
          totalItems: paginationData.i_total_objects,
          itemsPerPage: paginationData.i_per_page,
          currentPage: paginationData.i_current_page,
          totalPages: paginationData.i_total_pages,
        };
      }
    }

    console.log("Team data fetched:", teamData.value);
  } catch (error) {
    console.error("Error fetching team data:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب بيانات الفريق",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

// Fetch supervisors and inspectors for the team creation form
const fetchSupervisorsAndInspectors = async () => {
  try {
    const [supervisorsResponse, inspectorsResponse] = await Promise.all([
      axios.get("/users?role_id=2"),
      axios.get("/users?role_id=3"),
    ]);

    if (supervisorsResponse.data?.data) {
      supervisors.value = supervisorsResponse.data.data.map((supervisor) => ({
        title: `${supervisor.firstname} ${supervisor.lastname}`,
        value: supervisor.id,
      }));
    }

    if (inspectorsResponse.data?.data) {
      fieldTeamMembers.value = inspectorsResponse.data.data.map(
        (inspector) => ({
          title: `${inspector.firstname} ${inspector.lastname}`,
          value: inspector.id,
          avatar: inspector.avatar_url || "https://i.pravatar.cc/150",
        })
      );
    }
  } catch (error) {
    console.error("Error fetching users:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب بيانات المستخدمين",
      color: "error",
    });
  }
};

// Fetch zones for the team creation form
const fetchZones = async () => {
  try {
    const response = await axios.get("/zones?page=1");
    if (response.data?.data) {
      zones.value = response.data.data.map((zone) => ({
        title: zone.name || zone.title,
        value: zone.id,
      }));
    }
  } catch (error) {
    console.error("Error fetching zones:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب بيانات المناطق",
      color: "error",
    });
  }
};

// Fetch shifts for the team creation form
const fetchShifts = async () => {
  try {
    const response = await axios.get("/shifts?page=1");
    if (response.data?.data) {
      // For the dropdown, we need title and value properties
      shiftsData.value = response.data.data.map((shift) => ({
        title: shift.name || shift.title,
        value: shift.id,
        start_time: shift.start_time,
        end_time: shift.end_time,
      }));
    }
  } catch (error) {
    console.error("Error fetching shifts:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب بيانات المناوبات",
      color: "error",
    });
  }
};

const siteTypes = [
  { title: "موقع سكني", value: "residential" },
  { title: "موقع تجاري", value: "commercial" },
  { title: "موقع صناعي", value: "industrial" },
];

const links = ref([
  { title: "إدارة المواقع", href: "/", disabled: false },
  { title: "تفاصيل للسكن", href: "/sites", disabled: false },
  { title: "شركة رواف منى", href: "/sites/company-details", disabled: false },
  { title: "التقييمات", href: "/sites/company-details", disabled: true },
]);

const companies = [
  { title: "شركة رواف منى", value: "rawaf" },
  { title: "شركة الخليج", value: "gulf" },
];

const sites = ref([
  {
    siteId: "6127782187",
    authorizationNumber: "231 3456 567",
  },
  {
    siteId: "6127782188",
    authorizationNumber: "231 3456 568",
  },
]);

const violationOptions = ref([
  {
    name: "series-1",
    data: [30, 40, 35, 50, 49, 60, 70],
  },
]);

const complaintOptions = ref([
  {
    name: "Gold medals",
    data: [148, 113, 104, 71, 77],
  },
  {
    name: "Silver medals",
    data: [113, 122, 98, 88, 72],
  },
  {
    name: "Bronze medals",
    data: [124, 95, 65, 91, 76],
  },
]);

const filteredTeamMembers = computed(() => {
  if (!search.value) return teamMembers.value;

  const searchTerm = search.value.toLowerCase();
});

const handleSubmit = async () => {
  if (
    !formModel.value.zone_id ||
    !formModel.value.supervisor_id ||
    !formModel.value.shift_id ||
    formModel.value.inspector_ids.length === 0
  ) {
    appStore.showSnackbar({
      message: "يرجى ملء جميع الحقول المطلوبة",
      color: "error",
    });
    return;
  }

  loading.value = true;
  try {
    await axios.post("/teams", {
      zone_id: formModel.value.zone_id,
      supervisor_id: formModel.value.supervisor_id,
      shift_id: formModel.value.shift_id,
      inspector_ids: formModel.value.inspector_ids,
    });

    appStore.showSnackbar({
      message: "تم إضافة الفريق بنجاح",
      color: "success",
    });
    dialog.value = false;
    fetchTeamData(); // Refresh team data
    resetForm();
  } catch (error) {
    console.error("Error creating team:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء إضافة الفريق",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

const resetForm = () => {
  formModel.value = {
    zone_id: null,
    supervisor_id: null,
    inspector_ids: [],
    shift_id: null,
  };
};

const closeDialog = () => {
  dialog.value = false;
  resetForm();
};

const addNewTeam = () => {
  fetchSupervisorsAndInspectors();
  fetchZones();
  fetchShifts();
  dialog.value = true;
};

const { generateBreadcrumbItems } = useBreadcrumbs();

const breadcrumbItems = computed(() =>
  generateBreadcrumbItems([
    {
      title: "الفرق",
      href: "/teams",
      disabled: false,
    },
    {
      title: "إدارة الفرق",
      href: route.path,
      disabled: true,
    },
  ])
);

const handleEditInspector = (id) => {
  // Handle edit inspector logic
  console.log("Edit inspector", id);
};

const handleDeleteInspector = async (id) => {
  try {
    isLoading.value = true;
    await axios.delete(`/teams/${route.params.id}/update-inspector`, {
      data: { inspector_id: id },
    });

    appStore.showSnackbar({
      message: "تم حذف المراقب من الفريق بنجاح",
      color: "success",
    });

    // Refresh the team data after deletion
    await fetchTeamData();
  } catch (error) {
    console.error("Error deleting inspector:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حذف المراقب",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

const openMapDialog = () => {
  if (teamData.value?.data?.supervisor) {
    name.value = `${teamData.value.data.supervisor.firstname} ${teamData.value.data.supervisor.lastname}`;
  } else {
    name.value = "المشرف";
  }
  mapDialog.value = true;
};

const handleMapError = (error) => {
  console.error("Map error:", error);
  appStore.showSnackbar({
    message: "حدث خطأ أثناء تحميل الخريطة",
    color: "error",
  });
};

const handlePageSizeChange = async (newSize) => {
  // Reset to page 1 when changing page size
  inspectorsPagination.value.currentPage = 1;
  inspectorsPagination.value.itemsPerPage = newSize;
  await fetchTeamData();
};

// Watch for search changes
watch(search, (newValue) => {
  // Reset to first page when searching
  inspectorsPagination.value.currentPage = 1;
  // Debounce the API call
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    fetchTeamData();
  }, 300);
});

let searchTimeout;

onMounted(() => {
  fetchTeamData();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-md-0 py-2">
      <v-row v-if="isLoading">
        <v-col v-for="item in 4" :key="item" cols="6" sm="6" md="3">
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

      <div v-else>
        <BreadcrumbNav :items="breadcrumbItems" />
        <v-container fluid class="mt-6 px-0 rounded-lg">
          <v-card elevation="0" class="rounded-lg bg-main-content p-4">
            <!-- Header Section -->

            <v-card-text class="pa-0 px-4">
              <!-- Search and Add Section -->

              <div
                style="background-color: rgba(255, 255, 255, 1)"
                class="mt-2 rounded-lg justify-space-between"
              >
                <div class="pa-3 shadow-md rounded-lg">
                  <div class="text-h6 font-weight-bold mb-4">المشرف</div>

                  <!-- Supervisor Card -->
                  <div
                    v-if="teamData?.data.team.supervisor"
                    class="bg-white mt-4 rounded-lg pa-4"
                  >
                    <div class="d-flex align-items-center gap-3">
                      <v-avatar size="56">
                        <v-img
                          :src="
                            teamData?.data.team.supervisor.avatar_url ||
                            'https://placehold.co/400'
                          "
                          alt="Supervisor"
                        ></v-img>
                      </v-avatar>
                      <div>
                        <h3 class="text-h6 mb-0">
                          {{ teamData?.data.team.supervisor.firstname }}
                          {{ teamData?.data.team.supervisor.lastname }}
                        </h3>
                        <p class="text-body-2 text-medium-emphasis mb-0">
                          {{ teamData?.data.team.supervisor.role.title }}
                        </p>
                      </div>
                    </div>

                    <div class="mt-4 bg-light-gray-1 rounded-lg pa-4">
                      <div class="d-flex flex-wrap gap-4 justify-space-between">
                        <!-- sa id  -->
                        <div class="d-flex align-start gap-2">
                          <svg
                            width="17"
                            height="16"
                            viewBox="0 0 17 16"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                          >
                            <path
                              opacity="0.4"
                              d="M9.49935 2.66415C9.50038 2.38801 9.72507 2.16499 10.0012 2.16602C10.8774 2.16928 11.5934 2.18549 12.1718 2.28166C12.7625 2.37988 13.2574 2.56849 13.6598 2.9497C14.1145 3.38051 14.3149 3.92797 14.409 4.59048C14.4994 5.22758 14.4994 6.03842 14.4994 7.04757V10.2829C14.4994 11.292 14.4994 12.1029 14.409 12.74C14.3149 13.4025 14.1145 13.95 13.6598 14.3808C13.2089 14.808 12.6422 14.9929 11.9562 15.0803C11.2892 15.1653 10.438 15.1652 9.36814 15.1652H9.36813H7.29723H7.29722C6.22737 15.1652 5.37621 15.1653 4.70921 15.0803C4.02312 14.9929 3.45649 14.808 3.00556 14.3808C2.55082 13.95 2.35042 13.4025 2.2564 12.74C2.16599 12.1029 2.166 11.2921 2.16602 10.2829V10.2829V7.04755V7.04754C2.166 6.0384 2.16599 5.22758 2.2564 4.59048C2.35042 3.92797 2.55082 3.38051 3.00556 2.9497C3.40796 2.56849 3.9029 2.37988 4.49358 2.28166C5.07196 2.18549 5.78798 2.16928 6.66415 2.16602C6.94029 2.16499 7.16498 2.38801 7.16601 2.66415L6.66788 3.86479H9.99749L9.49935 2.66415Z"
                              fill="#B69265"
                            />
                            <path
                              fill-rule="evenodd"
                              clip-rule="evenodd"
                              d="M8.28162 0.833029C8.01803 0.832856 7.80659 0.832716 7.61453 0.876921C7.17241 0.978676 6.78274 1.25442 6.56529 1.65415C6.4703 1.82879 6.42361 2.03171 6.37148 2.25836L6.29911 2.57137C6.24699 2.79664 6.19922 3.00306 6.17829 3.17625C6.15582 3.36221 6.15429 3.57708 6.25168 3.79177C6.3512 4.01116 6.51674 4.19041 6.71883 4.31314C6.90893 4.4286 7.11495 4.4667 7.30745 4.48357C7.49199 4.49974 7.71918 4.49973 7.97944 4.49972H8.68531C8.94557 4.49973 9.17276 4.49974 9.3573 4.48357C9.5498 4.4667 9.75582 4.4286 9.94592 4.31314C10.148 4.19041 10.3135 4.01116 10.4131 3.79177C10.5105 3.57708 10.5089 3.36221 10.4865 3.17625C10.4655 3.00306 10.4178 2.79665 10.3656 2.57138L10.2933 2.25836C10.2411 2.03172 10.1944 1.82879 10.0995 1.65415C9.88201 1.25442 9.49234 0.978676 9.05022 0.876921C8.85815 0.832716 8.64672 0.832856 8.38312 0.833029H8.28162ZM7.83882 1.85144C7.9051 1.83619 7.99259 1.83305 8.33237 1.83305C8.67216 1.83305 8.75964 1.83619 8.82593 1.85144C9.01402 1.89473 9.15227 2.00563 9.22101 2.132C9.24196 2.17051 9.25962 2.22719 9.33046 2.53348L9.386 2.77357C9.44525 3.02974 9.47999 3.18293 9.49367 3.29621C9.5 3.34855 9.49935 3.37565 9.49847 3.38658C9.48603 3.40979 9.46328 3.43628 9.42662 3.45855L9.42591 3.45892C9.42524 3.45925 9.424 3.45984 9.42204 3.46065C9.41812 3.46225 9.41088 3.46488 9.39904 3.46798C9.37411 3.47451 9.33427 3.48175 9.27 3.48739C9.13444 3.49927 8.95168 3.49972 8.66444 3.49972H8.0003C7.71307 3.49972 7.53031 3.49927 7.39475 3.48739C7.33048 3.48175 7.29064 3.47451 7.26571 3.46798C7.25387 3.46488 7.24662 3.46225 7.24271 3.46065C7.24158 3.46019 7.24069 3.45979 7.24001 3.45948C7.23951 3.45925 7.23912 3.45906 7.23883 3.45892L7.23794 3.45844C7.20128 3.43617 7.17872 3.40979 7.16628 3.38658C7.16539 3.37565 7.16475 3.34855 7.17107 3.29621C7.18476 3.18293 7.2195 3.02974 7.27875 2.77357L7.33428 2.53349C7.40513 2.22719 7.42279 2.17051 7.44374 2.132C7.51248 2.00563 7.65072 1.89473 7.83882 1.85144ZM7.16696 3.39228C7.16695 3.39229 7.16699 3.39245 7.16696 3.39228V3.39228Z"
                              fill="#B69265"
                            />
                            <path
                              d="M8.32797 6.16634C7.32236 6.16634 6.52982 6.99984 6.52982 7.99967C6.52982 8.99951 7.32236 9.83301 8.32797 9.83301C9.33358 9.83301 10.1261 8.99951 10.1261 7.99967C10.1261 6.99984 9.33358 6.16634 8.32797 6.16634Z"
                              fill="#B69265"
                            />
                            <path
                              d="M10.1835 13.1663C10.7259 13.1663 11.1657 12.7266 11.1657 12.1841C11.1657 11.9484 11.0807 11.6984 10.889 11.5098C9.48694 10.1302 7.16177 10.1982 5.78696 11.5034C5.58808 11.6922 5.49902 11.9469 5.49902 12.1876C5.49902 12.7282 5.93721 13.1663 6.47775 13.1663H10.1835Z"
                              fill="#B69265"
                            />
                          </svg>

                          <div>
                            <div class="text-body-2 text-medium-emphasis">
                              رقم الهوية
                            </div>
                            <div class="text-body-2 font-weight-medium">
                              {{ teamData?.data.team.supervisor.sa_id }}
                            </div>
                          </div>
                        </div>
                        <!-- builds num  -->
                        <div class="d-flex align-start gap-2">
                          <svg
                            width="17"
                            height="16"
                            viewBox="0 0 17 16"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                          >
                            <path
                              opacity="0.4"
                              d="M9.49935 2.66415C9.50038 2.38801 9.72507 2.16499 10.0012 2.16602C10.8774 2.16928 11.5934 2.18549 12.1718 2.28166C12.7625 2.37988 13.2574 2.56849 13.6598 2.9497C14.1145 3.38051 14.3149 3.92797 14.409 4.59048C14.4994 5.22758 14.4994 6.03842 14.4994 7.04757V10.2829C14.4994 11.292 14.4994 12.1029 14.409 12.74C14.3149 13.4025 14.1145 13.95 13.6598 14.3808C13.2089 14.808 12.6422 14.9929 11.9562 15.0803C11.2892 15.1653 10.438 15.1652 9.36814 15.1652H9.36813H7.29723H7.29722C6.22737 15.1652 5.37621 15.1653 4.70921 15.0803C4.02312 14.9929 3.45649 14.808 3.00556 14.3808C2.55082 13.95 2.35042 13.4025 2.2564 12.74C2.16599 12.1029 2.166 11.2921 2.16602 10.2829V10.2829V7.04755V7.04754C2.166 6.0384 2.16599 5.22758 2.2564 4.59048C2.35042 3.92797 2.55082 3.38051 3.00556 2.9497C3.40796 2.56849 3.9029 2.37988 4.49358 2.28166C5.07196 2.18549 5.78798 2.16928 6.66415 2.16602C6.94029 2.16499 7.16498 2.38801 7.16601 2.66415L6.66788 3.86479H9.99749L9.49935 2.66415Z"
                              fill="#B69265"
                            />
                            <path
                              fill-rule="evenodd"
                              clip-rule="evenodd"
                              d="M8.28162 0.833029C8.01803 0.832856 7.80659 0.832716 7.61453 0.876921C7.17241 0.978676 6.78274 1.25442 6.56529 1.65415C6.4703 1.82879 6.42361 2.03171 6.37148 2.25836L6.29911 2.57137C6.24699 2.79664 6.19922 3.00306 6.17829 3.17625C6.15582 3.36221 6.15429 3.57708 6.25168 3.79177C6.3512 4.01116 6.51674 4.19041 6.71883 4.31314C6.90893 4.4286 7.11495 4.4667 7.30745 4.48357C7.49199 4.49974 7.71918 4.49973 7.97944 4.49972H8.68531C8.94557 4.49973 9.17276 4.49974 9.3573 4.48357C9.5498 4.4667 9.75582 4.4286 9.94592 4.31314C10.148 4.19041 10.3135 4.01116 10.4131 3.79177C10.5105 3.57708 10.5089 3.36221 10.4865 3.17625C10.4655 3.00306 10.4178 2.79665 10.3656 2.57138L10.2933 2.25836C10.2411 2.03172 10.1944 1.82879 10.0995 1.65415C9.88201 1.25442 9.49234 0.978676 9.05022 0.876921C8.85815 0.832716 8.64672 0.832856 8.38312 0.833029H8.28162ZM7.83882 1.85144C7.9051 1.83619 7.99259 1.83305 8.33237 1.83305C8.67216 1.83305 8.75964 1.83619 8.82593 1.85144C9.01402 1.89473 9.15227 2.00563 9.22101 2.132C9.24196 2.17051 9.25962 2.22719 9.33046 2.53348L9.386 2.77357C9.44525 3.02974 9.47999 3.18293 9.49367 3.29621C9.5 3.34855 9.49935 3.37565 9.49847 3.38658C9.48603 3.40979 9.46328 3.43628 9.42662 3.45855L9.42591 3.45892C9.42524 3.45925 9.424 3.45984 9.42204 3.46065C9.41812 3.46225 9.41088 3.46488 9.39904 3.46798C9.37411 3.47451 9.33427 3.48175 9.27 3.48739C9.13444 3.49927 8.95168 3.49972 8.66444 3.49972H8.0003C7.71307 3.49972 7.53031 3.49927 7.39475 3.48739C7.33048 3.48175 7.29064 3.47451 7.26571 3.46798C7.25387 3.46488 7.24662 3.46225 7.24271 3.46065C7.24158 3.46019 7.24069 3.45979 7.24001 3.45948C7.23951 3.45925 7.23912 3.45906 7.23883 3.45892L7.23794 3.45844C7.20128 3.43617 7.17872 3.40979 7.16628 3.38658C7.16539 3.37565 7.16475 3.34855 7.17107 3.29621C7.18476 3.18293 7.2195 3.02974 7.27875 2.77357L7.33428 2.53349C7.40513 2.22719 7.42279 2.17051 7.44374 2.132C7.51248 2.00563 7.65072 1.89473 7.83882 1.85144ZM7.16696 3.39228C7.16695 3.39229 7.16699 3.39245 7.16696 3.39228V3.39228Z"
                              fill="#B69265"
                            />
                            <path
                              d="M8.32797 6.16634C7.32236 6.16634 6.52982 6.99984 6.52982 7.99967C6.52982 8.99951 7.32236 9.83301 8.32797 9.83301C9.33358 9.83301 10.1261 8.99951 10.1261 7.99967C10.1261 6.99984 9.33358 6.16634 8.32797 6.16634Z"
                              fill="#B69265"
                            />
                            <path
                              d="M10.1835 13.1663C10.7259 13.1663 11.1657 12.7266 11.1657 12.1841C11.1657 11.9484 11.0807 11.6984 10.889 11.5098C9.48694 10.1302 7.16177 10.1982 5.78696 11.5034C5.58808 11.6922 5.49902 11.9469 5.49902 12.1876C5.49902 12.7282 5.93721 13.1663 6.47775 13.1663H10.1835Z"
                              fill="#B69265"
                            />
                          </svg>

                          <div>
                            <div class="text-body-2 text-medium-emphasis">
                              عدد المساكن المسنده
                            </div>
                            <div class="text-body-2 font-weight-medium">
                              {{ teamData?.data.team.locations_count || "-" }}
                            </div>
                          </div>
                        </div>

                        <!-- map -->
                        <!-- <div class="d-flex justify-space-between align-center">
                          <div
                            class="d-flex align-start gap-2 cursor-pointer"
                            @click="openMapDialog"
                          >
                            <svg
                              width="12"
                              height="16"
                              viewBox="0 0 12 16"
                              fill="none"
                              xmlns="http://www.w3.org/2000/svg"
                            >
                              <path
                                opacity="0.4"
                                fill-rule="evenodd"
                                clip-rule="evenodd"
                                d="M5.99951 12.0007C6.31218 12.0007 6.61284 11.8793 6.8375 11.6633C6.99484 11.512 7.15552 11.3593 7.31885 11.2047C9.29685 9.32799 11.5522 7.18932 10.3108 4.20332C9.58284 2.45332 7.83618 1.33398 5.99951 1.33398C4.16285 1.33398 2.41617 2.45332 1.68884 4.20332C0.452171 7.17732 2.68217 9.29999 4.65017 11.1733C4.8235 11.3387 4.99486 11.5013 5.16153 11.6633C5.38619 11.8793 5.68685 12.0007 5.99951 12.0007ZM5.99951 7.66732C6.92018 7.66732 7.66618 6.92132 7.66618 6.00065C7.66618 5.07998 6.92018 4.33398 5.99951 4.33398C5.07885 4.33398 4.33285 5.07998 4.33285 6.00065C4.33285 6.92132 5.07885 7.66732 5.99951 7.66732Z"
                                fill="#B69265"
                              />
                              <path
                                fill-rule="evenodd"
                                clip-rule="evenodd"
                                d="M5.99935 8.16732C4.80468 8.16732 3.83268 7.19532 3.83268 6.00065C3.83268 4.80598 4.80468 3.83398 5.99935 3.83398C7.19402 3.83398 8.16602 4.80598 8.16602 6.00065C8.16602 7.19532 7.19402 8.16732 5.99935 8.16732ZM5.99935 4.83398C5.35602 4.83398 4.83268 5.35732 4.83268 6.00065C4.83268 6.64398 5.35602 7.16732 5.99935 7.16732C6.64268 7.16732 7.16602 6.64398 7.16602 6.00065C7.16602 5.35732 6.64268 4.83398 5.99935 4.83398Z"
                                fill="#B69265"
                              />
                              <path
                                fill-rule="evenodd"
                                clip-rule="evenodd"
                                d="M4.81465 12.024C5.13465 12.3313 5.55532 12.5007 5.99932 12.5007C6.44332 12.5007 6.86399 12.3313 7.18399 12.0233C7.33999 11.8733 7.49998 11.7213 7.66198 11.568C9.65928 9.67335 12.1459 7.31452 10.772 4.01131C9.98198 2.11065 8.06332 0.833984 5.99932 0.833984C3.93532 0.833984 2.01732 2.11065 1.22732 4.01131C-0.142016 7.30265 2.32398 9.64999 4.30531 11.536L4.32448 11.5543C4.48995 11.712 4.65354 11.868 4.81465 12.024ZM2.15066 4.39531C2.78732 2.86331 4.33399 1.83398 5.99932 1.83398C7.66466 1.83398 9.21199 2.86331 9.84932 4.39531C10.9587 7.06331 8.93333 8.98464 6.974 10.8426C6.93684 10.878 6.89975 10.9132 6.86276 10.9484C6.73716 11.0677 6.61272 11.1859 6.49066 11.3033C6.358 11.4307 6.18332 11.5007 5.99932 11.5007C5.81532 11.5007 5.64065 11.43 5.50932 11.304C5.34132 11.1413 5.16933 10.9773 4.99467 10.8113C3.0534 8.96338 1.04536 7.05188 2.15066 4.39531Z"
                                fill="#B69265"
                              />
                              <path
                                d="M7.81131 15.1673H4.18728C2.03328 15.1673 0.951964 15.1673 0.419964 14.378C0.368631 14.3013 0.323949 14.222 0.287282 14.142C-0.119178 13.2718 0.535303 12.3389 1.44059 11.0486L1.44197 11.0466C1.60064 10.8206 1.91262 10.766 2.13862 10.9247C2.36462 11.0833 2.41929 11.3953 2.26062 11.6213C1.54129 12.6467 1.04329 13.398 1.19529 13.7227C1.21129 13.7573 1.22928 13.7887 1.24994 13.82C1.48394 14.1673 2.55927 14.1673 4.18725 14.1673H7.81131C9.43928 14.1673 10.5146 14.1673 10.7493 13.8193C10.77 13.7887 10.788 13.7573 10.802 13.726C10.9553 13.398 10.4573 12.6467 9.73797 11.6213C9.5793 11.3953 9.63397 11.0833 9.85997 10.9247C10.086 10.766 10.398 10.8206 10.5566 11.0466C11.4626 12.338 12.1173 13.2713 11.71 14.1453C11.6746 14.222 11.6306 14.3013 11.5793 14.3773C11.0466 15.1673 9.96531 15.1673 7.81131 15.1673Z"
                                fill="#B69265"
                              />
                            </svg>

                            <div>
                              <div class="text-body-2 text-medium-emphasis">
                                عرض الموقع
                              </div>
                              <div
                                class="text-body-2 font-weight-bold underline"
                              >
                                عرض موقع المراقب
                              </div>
                            </div>
                          </div>
                        </div> -->

                        <!-- Shift Information -->
                        <div class="d-flex align-center gap-2">
                          <svg
                            width="16"
                            height="16"
                            viewBox="0 0 16 16"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                          >
                            <path
                              opacity="0.4"
                              fill-rule="evenodd"
                              clip-rule="evenodd"
                              d="M8.00016 14.5938C11.682 14.5938 14.6668 11.609 14.6668 7.92716C14.6668 4.24526 11.682 1.2605 8.00016 1.2605C4.31826 1.2605 1.3335 4.24526 1.3335 7.92716C1.3335 11.609 4.31826 14.5938 8.00016 14.5938ZM8.00016 8.92716C8.55243 8.92716 9.00016 8.47943 9.00016 7.92716C9.00016 7.3749 8.55243 6.92716 8.00016 6.92716C7.4479 6.92716 7.00016 7.3749 7.00016 7.92716C7.00016 8.47943 7.4479 8.92716 8.00016 8.92716Z"
                              fill="#B69265"
                            />
                            <path
                              fill-rule="evenodd"
                              clip-rule="evenodd"
                              d="M8.49756 4.58667C8.49756 4.31053 8.2737 4.08667 7.99756 4.08667C7.72142 4.08667 7.49756 4.31053 7.49756 4.58667V6.51148C6.91497 6.7174 6.49756 7.27303 6.49756 7.92612C6.49756 8.75453 7.16915 9.42612 7.99756 9.42612C8.22829 9.42612 8.44686 9.37402 8.64214 9.28095L9.64861 10.2874C9.84387 10.4827 10.1605 10.4827 10.3557 10.2874C10.551 10.0921 10.551 9.77557 10.3557 9.5803L9.35035 8.57494C9.4447 8.37859 9.49756 8.15853 9.49756 7.92612C9.49756 7.27303 9.08015 6.7174 8.49756 6.51148V4.58667ZM7.49756 7.92612C7.49756 7.65 7.72143 7.42612 7.99756 7.42612C8.27368 7.42612 8.49756 7.65 8.49756 7.92612C8.49756 8.20225 8.27368 8.42612 7.99756 8.42612C7.72143 8.42612 7.49756 8.20225 7.49756 7.92612Z"
                              fill="#B69265"
                            />
                            <path
                              fill-rule="evenodd"
                              clip-rule="evenodd"
                              d="M8.00016 0.760498C4.04212 0.760498 0.833496 3.96912 0.833496 7.92716C0.833496 11.8852 4.04212 15.0938 8.00016 15.0938C11.9582 15.0938 15.1668 11.8852 15.1668 7.92716C15.1668 3.96912 11.9582 0.760498 8.00016 0.760498ZM1.8335 7.92716C1.8335 4.52141 4.59441 1.7605 8.00016 1.7605C11.4059 1.7605 14.1668 4.52141 14.1668 7.92716C14.1668 11.3329 11.4059 14.0938 8.00016 14.0938C4.59441 14.0938 1.8335 11.3329 1.8335 7.92716Z"
                              fill="#B69265"
                            />
                          </svg>

                          <div>
                            <div class="text-body-2 text-medium-emphasis">
                              نوبة العمل
                            </div>
                            <div class="text-body-2 font-weight-medium">
                              {{ teamData?.data.team.shift.name || "-" }}
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Map Dialog -->
              <v-dialog v-model="mapDialog" max-width="800">
                <v-card>
                  <v-card-title
                    class="d-flex justify-space-between align-center pa-4"
                  >
                    <span>موقع {{ name }}</span>
                    <v-btn icon @click="mapDialog = false">
                      <v-icon>mdi-close</v-icon>
                    </v-btn>
                  </v-card-title>
                  <v-card-text class="pa-0">
                    <MapDisplay
                      :latitude="teamData?.data.team.supervisor?.latitude"
                      :longitude="teamData?.data.team.supervisor?.longitude"
                      :readonly="true"
                      height="500px"
                      @error="handleMapError"
                    />
                  </v-card-text>
                </v-card>
              </v-dialog>

              <!-- team inspectors -->
              <div class="mt-6">
                <div
                  class="d-flex align-center justify-space-between bg-white rounded-lg pa-4"
                >
                  <h6 class="text-h6 font-weight-bold">
                    الفريق ({{ teamMembers.length || 0 }} مراقبين)
                  </h6>
                  <div class="d-flex align-center ga-2">
                    <v-text-field
                      v-model="search"
                      label="البحث عن الفريق..."
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

                    <VBtn
                      v-if="hasPermission('teams.create')"
                      color="primary"
                      height="40"
                      class="rounded-lg"
                      prepend-icon="mdi-plus"
                      @click="addNewTeam"
                    >
                      إضافة
                    </VBtn>
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
                      <inspectorsCard
                        :user="inspector"
                        @edit="handleEditInspector"
                        @delete="handleDeleteInspector"
                      />
                    </v-col>

                    <v-col v-if="teamMembers.length === 0" cols="12">
                      <div class="text-center py-8 bg-light-gray-1 rounded-lg">
                        <p class="text-medium-emphasis">
                          لا يوجد مفتشين في هذا الفريق
                        </p>
                      </div>
                    </v-col>
                    <v-col
                      v-else-if="filteredTeamMembers.length === 0"
                      cols="12"
                    >
                      <div class="text-center py-8 bg-light-gray-1 rounded-lg">
                        <p class="text-medium-emphasis">
                          لا توجد نتائج تطابق بحثك
                        </p>
                      </div>
                    </v-col>
                  </v-row>
                </div>
              </div>
              <!-- pagination -->
              <Pagination
                v-if="teamMembers.length > 0"
                :total-items="inspectorsPagination.totalItems"
                v-model:per-page="inspectorsPagination.itemsPerPage"
                v-model:page="inspectorsPagination.currentPage"
                @update:page="fetchTeamData"
                @update:per-page="handlePageSizeChange"
              />

              <!-- Add Team Modal -->
              <v-dialog v-model="dialog" width="500" class="add-site-dialog">
                <v-card class="rounded-lg">
                  <v-card-text class="pa-4">
                    <div class="border-b mb-4">
                      <div class="d-flex justify-space-between align-center">
                        <h2 class="text-h6 font-weight-bold">
                          إدارة الفرق الميدانية
                        </h2>
                        <v-btn icon @click="closeDialog">
                          <v-icon>mdi-close</v-icon>
                        </v-btn>
                      </div>
                      <p class="text-subtitle-1 text-medium-emphasis mb-2">
                        حدد تفاصيل الفريق الميداني
                      </p>
                    </div>

                    <v-form @submit.prevent="handleSubmit">
                      <!-- Zone/Location -->
                      <div class="mb-4">
                        <label class="text-subtitle-2 mb-2 d-block"
                          >المواقع</label
                        >
                        <v-select
                          v-model="formModel.zone_id"
                          :items="zones"
                          item-title="title"
                          item-value="value"
                          variant="outlined"
                          density="comfortable"
                          placeholder="اختر الموقع"
                          hide-details
                          class="rounded-lg bg-grey-lighten-4 px-2"
                        />
                      </div>

                      <!-- Supervisor -->
                      <div class="mb-4">
                        <label class="text-subtitle-2 mb-2 d-block"
                          >المشرف الميداني</label
                        >
                        <v-select
                          v-model="formModel.supervisor_id"
                          :items="supervisors"
                          item-title="title"
                          item-value="value"
                          variant="outlined"
                          density="comfortable"
                          placeholder="اختر المشرف"
                          hide-details
                          class="rounded-lg bg-grey-lighten-4 px-2"
                        />
                      </div>

                      <!-- Team Members -->
                      <div class="mb-4">
                        <label class="text-subtitle-2 mb-2 d-block"
                          >فريق المراقبين الميدانيون</label
                        >
                        <v-select
                          v-model="formModel.inspector_ids"
                          :items="fieldTeamMembers"
                          item-title="title"
                          item-value="value"
                          variant="outlined"
                          density="comfortable"
                          placeholder="اختر الفريق"
                          multiple
                          hide-details
                          class="rounded-lg bg-grey-lighten-4"
                        >
                          <template v-slot:selection="{ item }">
                            <div class="d-flex align-center ga-2 ms-n2">
                              <v-avatar size="32" class="">
                                <v-img
                                  :src="item.raw.avatar"
                                  :alt="item.title"
                                ></v-img>
                              </v-avatar>
                              <span class="text-subtitle-2">{{
                                item.title
                              }}</span>
                            </div>
                          </template>

                          <template v-slot:item="{ item, props }">
                            <v-list-item
                              v-bind="props"
                              :title="item.title"
                              class="field-team-item"
                            >
                              <template v-slot:prepend>
                                <v-avatar size="32" class="me-3">
                                  <v-img
                                    :src="item.raw.avatar"
                                    :alt="item.title"
                                  ></v-img>
                                </v-avatar>
                              </template>
                            </v-list-item>
                          </template>
                        </v-select>
                      </div>

                      <!-- Shift -->
                      <div class="mb-4">
                        <label class="text-subtitle-2 mb-2 d-block"
                          >التحويل</label
                        >
                        <v-select
                          v-model="formModel.shift_id"
                          :items="shiftsData"
                          item-title="title"
                          item-value="value"
                          variant="outlined"
                          density="comfortable"
                          placeholder="اختر التحويل"
                          hide-details
                          class="rounded-lg bg-grey-lighten-4 px-2"
                        />
                      </div>

                      <!-- Action Buttons -->
                      <div class="d-flex justify-end mt-6">
                        <v-btn
                          variant="text"
                          color="grey-darken-1"
                          class="me-4"
                          @click="closeDialog"
                        >
                          إلغاء
                        </v-btn>
                        <v-btn
                          color="#B69265"
                          class="text-white px-6"
                          type="submit"
                          :loading="loading"
                        >
                          إضافة
                        </v-btn>
                      </div>
                    </v-form>
                  </v-card-text>
                </v-card>
              </v-dialog>
              <!-- add new location -->
            </v-card-text>
          </v-card>
        </v-container>
      </div>
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

.field-team-item {
  padding: 8px;
  border-radius: 8px;
  margin: 4px 0;

  &:hover {
    background-color: rgba(182, 146, 101, 0.05);
  }

  &.v-list-item--active {
    background-color: rgba(182, 146, 101, 0.1);
  }
}

:deep(.v-select) {
  .v-field__input {
    padding-top: 0;
    padding-bottom: 0;
    min-height: 48px;
    display: flex;
    align-items: center;
  }

  .v-field__placeholder {
    transform: translateY(0) !important;
    padding: 0 6px;
    margin-top: 6px;
  }

  .v-field__field {
    padding-top: 0 !important;
  }
}

.shadow-md {
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.04) !important;
}
.bg-main-content {
  background-color: rgba(255, 255, 255, 1) !important;
  padding: 12px;
}
</style>
