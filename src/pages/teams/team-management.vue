<script setup>
// pagination added
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import StatisticsBar from "@/@core/components/widgets/StatisticsBar.vue";
import supervisorCard from "@/@core/components/widgets/supervisorCard.vue";
import UserWidget from "@/@core/components/widgets/UserWidget.vue";
import { ref, onMounted, computed } from "vue";
import { useRoute } from "vue-router";
import axios from "@/plugins/axios";
import BreadcrumbNav from "@/components/BreadcrumbNav.vue";
import { useBreadcrumbs } from "@/composables/useBreadcrumbs";

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
    const response = await axios.get("/teams/2");
    teamData.value = response.data;

    // Extract team members and shifts from the response if available
    if (teamData.value?.data) {
      teamMembers.value = teamData.value.data.inspectors || [];
      shiftsData.value = teamData.value.data.shifts || [];
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
      shiftsData.value = response.data.data.map((shift) => ({
        title: shift.name || shift.title,
        value: shift.id,
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

const addNewLocation = () => {
  dialog.value = true;
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

onMounted(() => {
  fetchTeamData();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
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
        <StatisticsBar title="الشركات" />
        <v-container fluid class="mt-6 px-0 rounded-lg">
          <v-card elevation="0" class="rounded-lg bg-light-white">
            <!-- Header Section -->

            <v-card-text class="pa-0 px-4">
              <!-- Search and Add Section -->

              <!-- Tabs Section -->
              <v-row
                class="mt-2 bg-light-gray-2 rounded-lg justify-space-between"
              >
                <v-col
                  v-for="(site, index) in sites"
                  :key="site.siteId"
                  cols="12"
                  sm="6"
                >
                  <div class="pa-3 bg-light-gray-1 rounded-lg">
                    <div class="text-h6 font-weight-bold mb-4">
                      الشيفت {{ index }}
                    </div>

                    <supervisorCard
                      :site-id="site.siteId"
                      :authorization-number="site.authorizationNumber"
                    />

                    <!-- team -->

                    <div class="bg-light-gray-2">
                      <div
                        class="d-flex align-center justify-space-between bg-white rounded-lg pa-4 mt-6"
                      >
                        <div
                          class="d-flex align-center justify-space-between ga-4 border-b pb-4 mt-2 w-100"
                        >
                          <h6 class="text-h6 font-weight-bold pe-4">الفريق</h6>
                          <div
                            class="d-flex align-center ga-2 flex-grow-1 justify-content-end"
                          >
                            <v-text-field
                              v-model="search"
                              label="البحث عن الفريق..."
                              rounded="lg"
                              clearable
                              height="48"
                              class="bg-white"
                              style="
                                width: 110px;
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
                              height="48"
                              elevation="0"
                              class="rounded-lg"
                              append-icon="mdi-plus"
                              @click="addNewTeam"
                            >
                              إضافة
                            </VBtn>
                          </div>
                        </div>
                      </div>

                      <div class="mt-8 bg-light-gray-1 rounded-lg">
                        <v-row>
                          <v-col cols="12" sm="6">
                            <div class="bg-white mt-4 rounded-lg pa-4">
                              <UserWidget
                                class="flex-grow-1"
                                noShadow
                                noPadding
                                :user="{
                                  name: 'محمد أحمد',
                                  job: 'مشرف الموقع',
                                }"
                              />

                              <div class="bg-light-gray-1 rounded-lg mt-4">
                                <div
                                  class="d-flex align-center gap-4 rounded-lg pa-4"
                                >
                                  <div
                                    class="d-flex justify-space-between align-center"
                                  >
                                    <div class="d-flex align-start gap-2 ms-0">
                                      <svg
                                        width="16"
                                        height="18"
                                        viewBox="0 0 16 18"
                                        fill="none"
                                        xmlns="http://www.w3.org/2000/svg"
                                      >
                                        <path
                                          opacity="0.4"
                                          d="M8.03275 2.83337C9.26153 2.83337 10.2284 2.83336 10.9838 2.92223C11.7543 3.01287 12.3807 3.20288 12.8809 3.6405C13.3906 4.08654 13.6211 4.65971 13.7293 5.3638C13.8327 6.03715 13.8327 6.89384 13.8327 7.95834V12.0417C13.8327 13.1062 13.8327 13.9629 13.7293 14.6363C13.6211 15.3404 13.3906 15.9135 12.8809 16.3596C12.3807 16.7972 11.7543 16.9872 10.9838 17.0779C10.2284 17.1667 9.26152 17.1667 8.03274 17.1667H7.96596C6.73718 17.1667 5.77027 17.1667 5.01487 17.0779C4.24436 16.9872 3.61796 16.7972 3.11781 16.3596C2.60806 15.9135 2.37763 15.3404 2.26945 14.6363C2.16599 13.9629 2.166 13.1062 2.16602 12.0417V12.0417V7.95835V7.95834C2.166 6.89384 2.16599 6.03716 2.26945 5.36381C2.37763 4.65971 2.60806 4.08654 3.11781 3.6405C3.61796 3.20288 4.24436 3.01287 5.01487 2.92223C5.77026 2.83336 6.73717 2.83337 7.96595 2.83337H7.96595H8.03274H8.03275Z"
                                          fill="#B69265"
                                        />
                                        <path
                                          fill-rule="evenodd"
                                          clip-rule="evenodd"
                                          d="M5.16602 8.71358C5.16602 7.13983 6.45339 5.83337 7.99935 5.83337C9.54531 5.83337 10.8327 7.13983 10.8327 8.71358C10.8327 10.33 9.51364 11.3908 8.50602 12.025L8.49846 12.0297L8.49074 12.0342C8.34164 12.1208 8.17226 12.1667 7.99935 12.1667C7.82644 12.1667 7.65706 12.1208 7.50796 12.0342L7.49911 12.0291L7.49047 12.0236C6.4891 11.386 5.16602 10.3354 5.16602 8.71358ZM6.99935 8.66671C6.99935 8.11442 7.44506 7.66671 7.99488 7.66671H8.00382C8.55364 7.66671 8.99935 8.11442 8.99935 8.66671C8.99935 9.21899 8.55364 9.66671 8.00382 9.66671H7.99488C7.44506 9.66671 6.99935 9.21899 6.99935 8.66671Z"
                                          fill="#B69265"
                                        />
                                        <path
                                          d="M7.33268 14.1667C7.05654 14.1667 6.83268 14.3906 6.83268 14.6667C6.83268 14.9429 7.05654 15.1667 7.33268 15.1667H8.66602C8.94216 15.1667 9.16602 14.9429 9.16602 14.6667C9.16602 14.3906 8.94216 14.1667 8.66602 14.1667H7.33268Z"
                                          fill="#B69265"
                                        />
                                      </svg>

                                      <div>
                                        <div
                                          class="text-body-2 text-medium-emphasis"
                                        >
                                          رقم التصريح
                                        </div>
                                        <div
                                          class="text-body-2 font-weight-medium"
                                        >
                                          3443443
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                                <div
                                  class="d-flex align-center gap-4 rounded-lg pa-4 mt-n4"
                                >
                                  <div
                                    class="d-flex justify-space-between align-center"
                                  >
                                    <div class="d-flex align-start gap-2 ms-0">
                                      <svg
                                        width="16"
                                        height="18"
                                        viewBox="0 0 16 18"
                                        fill="none"
                                        xmlns="http://www.w3.org/2000/svg"
                                      >
                                        <path
                                          opacity="0.4"
                                          d="M8.03275 2.83337C9.26153 2.83337 10.2284 2.83336 10.9838 2.92223C11.7543 3.01287 12.3807 3.20288 12.8809 3.6405C13.3906 4.08654 13.6211 4.65971 13.7293 5.3638C13.8327 6.03715 13.8327 6.89384 13.8327 7.95834V12.0417C13.8327 13.1062 13.8327 13.9629 13.7293 14.6363C13.6211 15.3404 13.3906 15.9135 12.8809 16.3596C12.3807 16.7972 11.7543 16.9872 10.9838 17.0779C10.2284 17.1667 9.26152 17.1667 8.03274 17.1667H7.96596C6.73718 17.1667 5.77027 17.1667 5.01487 17.0779C4.24436 16.9872 3.61796 16.7972 3.11781 16.3596C2.60806 15.9135 2.37763 15.3404 2.26945 14.6363C2.16599 13.9629 2.166 13.1062 2.16602 12.0417V12.0417V7.95835V7.95834C2.166 6.89384 2.16599 6.03716 2.26945 5.36381C2.37763 4.65971 2.60806 4.08654 3.11781 3.6405C3.61796 3.20288 4.24436 3.01287 5.01487 2.92223C5.77026 2.83336 6.73717 2.83337 7.96595 2.83337H7.96595H8.03274H8.03275Z"
                                          fill="#B69265"
                                        />
                                        <path
                                          fill-rule="evenodd"
                                          clip-rule="evenodd"
                                          d="M5.16602 8.71358C5.16602 7.13983 6.45339 5.83337 7.99935 5.83337C9.54531 5.83337 10.8327 7.13983 10.8327 8.71358C10.8327 10.33 9.51364 11.3908 8.50602 12.025L8.49846 12.0297L8.49074 12.0342C8.34164 12.1208 8.17226 12.1667 7.99935 12.1667C7.82644 12.1667 7.65706 12.1208 7.50796 12.0342L7.49911 12.0291L7.49047 12.0236C6.4891 11.386 5.16602 10.3354 5.16602 8.71358ZM6.99935 8.66671C6.99935 8.11442 7.44506 7.66671 7.99488 7.66671H8.00382C8.55364 7.66671 8.99935 8.11442 8.99935 8.66671C8.99935 9.21899 8.55364 9.66671 8.00382 9.66671H7.99488C7.44506 9.66671 6.99935 9.21899 6.99935 8.66671Z"
                                          fill="#B69265"
                                        />
                                        <path
                                          d="M7.33268 14.1667C7.05654 14.1667 6.83268 14.3906 6.83268 14.6667C6.83268 14.9429 7.05654 15.1667 7.33268 15.1667H8.66602C8.94216 15.1667 9.16602 14.9429 9.16602 14.6667C9.16602 14.3906 8.94216 14.1667 8.66602 14.1667H7.33268Z"
                                          fill="#B69265"
                                        />
                                      </svg>

                                      <div>
                                        <div
                                          class="text-body-2 text-medium-emphasis"
                                        >
                                          رقم التصريح
                                        </div>
                                        <div
                                          class="text-body-2 font-weight-medium"
                                        >
                                          3443443
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </v-col>
                          <v-col cols="12" sm="6">
                            <div class="bg-white mt-4 rounded-lg pa-4">
                              <UserWidget
                                class="flex-grow-1"
                                noShadow
                                noPadding
                                :user="{
                                  name: 'محمد أحمد',
                                  job: 'مشرف الموقع',
                                }"
                              />

                              <div class="bg-light-gray-1 rounded-lg mt-4">
                                <div
                                  class="d-flex align-center gap-4 rounded-lg pa-4"
                                >
                                  <div
                                    class="d-flex justify-space-between align-center"
                                  >
                                    <div class="d-flex align-start gap-2 ms-0">
                                      <svg
                                        width="16"
                                        height="18"
                                        viewBox="0 0 16 18"
                                        fill="none"
                                        xmlns="http://www.w3.org/2000/svg"
                                      >
                                        <path
                                          opacity="0.4"
                                          d="M8.03275 2.83337C9.26153 2.83337 10.2284 2.83336 10.9838 2.92223C11.7543 3.01287 12.3807 3.20288 12.8809 3.6405C13.3906 4.08654 13.6211 4.65971 13.7293 5.3638C13.8327 6.03715 13.8327 6.89384 13.8327 7.95834V12.0417C13.8327 13.1062 13.8327 13.9629 13.7293 14.6363C13.6211 15.3404 13.3906 15.9135 12.8809 16.3596C12.3807 16.7972 11.7543 16.9872 10.9838 17.0779C10.2284 17.1667 9.26152 17.1667 8.03274 17.1667H7.96596C6.73718 17.1667 5.77027 17.1667 5.01487 17.0779C4.24436 16.9872 3.61796 16.7972 3.11781 16.3596C2.60806 15.9135 2.37763 15.3404 2.26945 14.6363C2.16599 13.9629 2.166 13.1062 2.16602 12.0417V12.0417V7.95835V7.95834C2.166 6.89384 2.16599 6.03716 2.26945 5.36381C2.37763 4.65971 2.60806 4.08654 3.11781 3.6405C3.61796 3.20288 4.24436 3.01287 5.01487 2.92223C5.77026 2.83336 6.73717 2.83337 7.96595 2.83337H7.96595H8.03274H8.03275Z"
                                          fill="#B69265"
                                        />
                                        <path
                                          fill-rule="evenodd"
                                          clip-rule="evenodd"
                                          d="M5.16602 8.71358C5.16602 7.13983 6.45339 5.83337 7.99935 5.83337C9.54531 5.83337 10.8327 7.13983 10.8327 8.71358C10.8327 10.33 9.51364 11.3908 8.50602 12.025L8.49846 12.0297L8.49074 12.0342C8.34164 12.1208 8.17226 12.1667 7.99935 12.1667C7.82644 12.1667 7.65706 12.1208 7.50796 12.0342L7.49911 12.0291L7.49047 12.0236C6.4891 11.386 5.16602 10.3354 5.16602 8.71358ZM6.99935 8.66671C6.99935 8.11442 7.44506 7.66671 7.99488 7.66671H8.00382C8.55364 7.66671 8.99935 8.11442 8.99935 8.66671C8.99935 9.21899 8.55364 9.66671 8.00382 9.66671H7.99488C7.44506 9.66671 6.99935 9.21899 6.99935 8.66671Z"
                                          fill="#B69265"
                                        />
                                        <path
                                          d="M7.33268 14.1667C7.05654 14.1667 6.83268 14.3906 6.83268 14.6667C6.83268 14.9429 7.05654 15.1667 7.33268 15.1667H8.66602C8.94216 15.1667 9.16602 14.9429 9.16602 14.6667C9.16602 14.3906 8.94216 14.1667 8.66602 14.1667H7.33268Z"
                                          fill="#B69265"
                                        />
                                      </svg>

                                      <div>
                                        <div
                                          class="text-body-2 text-medium-emphasis"
                                        >
                                          رقم التصريح
                                        </div>
                                        <div
                                          class="text-body-2 font-weight-medium"
                                        >
                                          3443443
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                                <div
                                  class="d-flex align-center gap-4 rounded-lg pa-4 mt-n4"
                                >
                                  <div
                                    class="d-flex justify-space-between align-center"
                                  >
                                    <div class="d-flex align-start gap-2 ms-0">
                                      <svg
                                        width="16"
                                        height="18"
                                        viewBox="0 0 16 18"
                                        fill="none"
                                        xmlns="http://www.w3.org/2000/svg"
                                      >
                                        <path
                                          opacity="0.4"
                                          d="M8.03275 2.83337C9.26153 2.83337 10.2284 2.83336 10.9838 2.92223C11.7543 3.01287 12.3807 3.20288 12.8809 3.6405C13.3906 4.08654 13.6211 4.65971 13.7293 5.3638C13.8327 6.03715 13.8327 6.89384 13.8327 7.95834V12.0417C13.8327 13.1062 13.8327 13.9629 13.7293 14.6363C13.6211 15.3404 13.3906 15.9135 12.8809 16.3596C12.3807 16.7972 11.7543 16.9872 10.9838 17.0779C10.2284 17.1667 9.26152 17.1667 8.03274 17.1667H7.96596C6.73718 17.1667 5.77027 17.1667 5.01487 17.0779C4.24436 16.9872 3.61796 16.7972 3.11781 16.3596C2.60806 15.9135 2.37763 15.3404 2.26945 14.6363C2.16599 13.9629 2.166 13.1062 2.16602 12.0417V12.0417V7.95835V7.95834C2.166 6.89384 2.16599 6.03716 2.26945 5.36381C2.37763 4.65971 2.60806 4.08654 3.11781 3.6405C3.61796 3.20288 4.24436 3.01287 5.01487 2.92223C5.77026 2.83336 6.73717 2.83337 7.96595 2.83337H7.96595H8.03274H8.03275Z"
                                          fill="#B69265"
                                        />
                                        <path
                                          fill-rule="evenodd"
                                          clip-rule="evenodd"
                                          d="M5.16602 8.71358C5.16602 7.13983 6.45339 5.83337 7.99935 5.83337C9.54531 5.83337 10.8327 7.13983 10.8327 8.71358C10.8327 10.33 9.51364 11.3908 8.50602 12.025L8.49846 12.0297L8.49074 12.0342C8.34164 12.1208 8.17226 12.1667 7.99935 12.1667C7.82644 12.1667 7.65706 12.1208 7.50796 12.0342L7.49911 12.0291L7.49047 12.0236C6.4891 11.386 5.16602 10.3354 5.16602 8.71358ZM6.99935 8.66671C6.99935 8.11442 7.44506 7.66671 7.99488 7.66671H8.00382C8.55364 7.66671 8.99935 8.11442 8.99935 8.66671C8.99935 9.21899 8.55364 9.66671 8.00382 9.66671H7.99488C7.44506 9.66671 6.99935 9.21899 6.99935 8.66671Z"
                                          fill="#B69265"
                                        />
                                        <path
                                          d="M7.33268 14.1667C7.05654 14.1667 6.83268 14.3906 6.83268 14.6667C6.83268 14.9429 7.05654 15.1667 7.33268 15.1667H8.66602C8.94216 15.1667 9.16602 14.9429 9.16602 14.6667C9.16602 14.3906 8.94216 14.1667 8.66602 14.1667H7.33268Z"
                                          fill="#B69265"
                                        />
                                      </svg>

                                      <div>
                                        <div
                                          class="text-body-2 text-medium-emphasis"
                                        >
                                          رقم التصريح
                                        </div>
                                        <div
                                          class="text-body-2 font-weight-medium"
                                        >
                                          3443443
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </v-col>
                        </v-row>
                      </div>
                    </div>
                  </div>
                </v-col>
              </v-row>

              <!-- pagination -->
              <Pagination
                :total-items="2300"
                v-model:per-page="perPage"
                v-model:page="currentPage"
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
</style>
