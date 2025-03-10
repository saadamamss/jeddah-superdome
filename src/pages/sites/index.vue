<script setup>
import { useAppStore } from "@/stores/app";
import AppDateTimePicker from "@/@core/components/AppDateTimePicker";
import StatsWidget from "@/@core/components/widgets/StatsWidget";
import Pagination from "@/@core/components/Pagination";
import SiteCard from "@/@core/components/widgets/SiteCard.vue";
import { ref, watch, onMounted } from "vue";
import axios from "@/plugins/axios";
import { useAuthStore } from "@/stores/Auth";
import LocationPicker from "@/components/maps/LocationPicker.vue";

const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const loading = ref(false);
const filterMenu = ref(false);
const siteType = ref("السكن");
const housingPermitNumber = ref("");
const authorizedContactNumber = ref("");
const selectedCompany = ref(null);
const activeTab = ref("all");
const sites = ref([]);
const totalItems = ref(0);
const selectedLocation = ref({ lat: 24.7136, lng: 46.6753 }); // Default to Riyadh coordinates

// Add new filter refs
const filters = ref({
  type: null,
  status: null,
  dateRange: null,
});

const formData = ref({
  name: "",
  location_type_id: null,
  location_sub_type_id: null,
  contract_id: null,
  delegate_name: "",
  delegate_phone: "",
  license_no: "",
  camp_number: "",
  square_number: "",
  latitude: "24.7136",
  longitude: "46.6753",
  companies: [],
});

// Add form validation rules
const rules = {
  name: [
    (v) => !!v || "اسم الموقع مطلوب",
    (v) => (v && v.length >= 3) || "يجب أن يكون اسم الموقع 3 أحرف على الأقل",
  ],
  location_type_id: [(v) => !!v || "نوع الموقع مطلوب"],
  license_no: [
    (v) => !!v || "رقم الترخيص مطلوب",
    (v) => /^\d+$/.test(v) || "يجب أن يكون رقم الترخيص رقمًا",
  ],
  camp_number: [
    (v) => !!v || "رقم المخيم مطلوب",
    (v) => /^\d+$/.test(v) || "يجب أن يكون رقم المخيم رقمًا",
  ],
  square_number: [
    (v) => !!v || "رقم المربع مطلوب",
    (v) => /^\d+$/.test(v) || "يجب أن يكون رقم المربع رقمًا",
  ],
  delegate_name: [
    (v) => !!v || "اسم المفوض مطلوب",
    (v) => (v && v.length >= 3) || "يجب أن يكون اسم المفوض 3 أحرف على الأقل",
  ],
  delegate_phone: [
    (v) => !!v || "رقم هاتف المفوض مطلوب",
    (v) => /^[0-9]{9}$/.test(v) || "يجب أن يكون رقم الهاتف 9 أرقام",
  ],
  contract_id: [(v) => !!v || "رقم العقد مطلوب"],
  companies: [(v) => (v && v.length > 0) || "يجب اختيار شركة واحدة على الأقل"],
};

// Add form ref
const form = ref(null);

// Update companies to be reactive
const companies = ref([]);
const companiesLoading = ref(false);
const companiesTotal = ref(0);
const companiesPage = ref(1);
const companiesPerPage = ref(10);
const companiesSearch = ref("");

const housing_count = ref(0);
const camp_count = ref(0);
const siteTypes = ref([]);

// Add contracts state
const contracts = ref([]);
const contractsLoading = ref(false);

// Add function to fetch contracts
const fetchContracts = async () => {
  try {
    contractsLoading.value = true;
    const response = await axios.get("contracts");
    if (response.data && response.data.data) {
      contracts.value = response.data.data.map((contract) => ({
        title: contract.name || `Contract #${contract.id}`,
        value: contract.id,
        raw: contract,
      }));
    }
  } catch (error) {
    console.error("Error fetching contracts:", error);
    useAppStore().showSnackbar({
      message: "حدث خطأ أثناء جلب بيانات العقود",
      color: "error",
    });
  } finally {
    contractsLoading.value = false;
  }
};

// Add function to fetch companies
const fetchCompanies = async (page = 1, search = "") => {
  try {
    companiesLoading.value = true;

    // Build query parameters
    const queryParams = new URLSearchParams({
      page: page.toString(),
      per_page: companiesPerPage.value.toString(),
      search: search,
    });

    const response = await axios.get(`companies?${queryParams.toString()}`);
    if (response.data && response.data.data) {
      // Transform the data to match select requirements
      const newCompanies = response.data.data.map((company) => ({
        title: company.name,
        value: company.id,
        raw: company, // Store the raw company data if needed
      }));

      // If it's the first page, replace the companies array
      // If it's a subsequent page, append to the existing array
      if (page === 1) {
        companies.value = newCompanies;
      } else {
        companies.value = [...companies.value, ...newCompanies];
      }

      companiesTotal.value = response.data.meta?.total || 0;
      companiesPage.value = page;
    }
  } catch (error) {
    console.error("Error fetching companies:", error);
    useAppStore().showSnackbar({
      message: "حدث خطأ أثناء جلب بيانات الشركات",
      color: "error",
    });
  } finally {
    companiesLoading.value = false;
  }
};

// Add debounced search for companies
const debouncedCompanySearch = ref(null);
watch(companiesSearch, (newValue) => {
  if (debouncedCompanySearch.value) clearTimeout(debouncedCompanySearch.value);
  debouncedCompanySearch.value = setTimeout(() => {
    companiesPage.value = 1; // Reset to first page when searching
    fetchCompanies(1, newValue);
  }, 500); // 500ms debounce
});

// Function to fetch locations data with search and filters
const fetchLocations = async (page = 1) => {
  try {
    isLoading.value = true;
    console.log("Fetching page:", page);

    // Build query parameters
    const queryParams = new URLSearchParams({
      page: page.toString(),
      search: search.value,
      ...(filters.value.type && { type: filters.value.type }),
      ...(filters.value.status && { status: filters.value.status }),
      ...(filters.value.dateRange && {
        start_date: filters.value.dateRange[0],
        end_date: filters.value.dateRange[1],
      }),
    });

    // Choose endpoint based on active tab
    const endpoint =
      activeTab.value === "correction" ? "location-corrections" : "locations";
    const response = await axios.get(`${endpoint}?${queryParams.toString()}`);
    const responseLocations = await axios.get(`locations-types`);

    siteTypes.value = responseLocations.data.data.sub_types;

    if (activeTab.value === "correction") {
      // Handle correction response structure
      sites.value = response.data.data || [];
      totalItems.value = response.data.pagination?.i_total_objects || 0;
      perPage.value = response.data.pagination?.i_per_page || perPage.value;
      currentPage.value = response.data.pagination?.i_current_page || 1;
    } else {
      // Handle regular locations response structure
      if (response.data?.data?.locations) {
        sites.value = response.data.data.locations;
        if (response.data.data.statistics) {
          housing_count.value =
            response.data.data.statistics.housing_count || 0;
          camp_count.value = response.data.data.statistics.camp_count || 0;
          // For pagination, we'll use the total count of locations
          totalItems.value =
            (housing_count.value || 0) + (camp_count.value || 0);
        }
      } else {
        sites.value = [];
        housing_count.value = 0;
        camp_count.value = 0;
        totalItems.value = 0;
      }
    }
  } catch (error) {
    console.error("Error fetching locations:", error);
    sites.value = [];
    totalItems.value = 0;
    housing_count.value = 0;
    camp_count.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Watch for search changes with debounce
const debouncedSearch = ref(null);
watch(search, (newValue) => {
  if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
  debouncedSearch.value = setTimeout(() => {
    currentPage.value = 1; // Reset to first page when searching
    fetchLocations(1);
  }, 500); // 500ms debounce
});

// Watch for filter changes
watch(
  filters,
  () => {
    currentPage.value = 1; // Reset to first page when filters change
    fetchLocations(1);
  },
  { deep: true }
);

// Watch for page changes
watch(
  [currentPage],
  async (newPage) => {
    await fetchLocations(newPage[0]);
  },
  { immediate: true }
);

// Watch for tab changes
watch(activeTab, () => {
  currentPage.value = 1; // Reset to first page when changing tabs
  fetchLocations(1);
});

// Function to apply filters
const applyFilters = () => {
  fetchLocations(1);
};

// Function to reset filters
const resetFilters = () => {
  filters.value = {
    type: null,
    status: null,
    dateRange: null,
  };
};

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

// Add function to fetch correction details
const fetchCorrectionDetails = async (id) => {
  try {
    loading.value = true;
    const response = await axios.get(`location-corrections/${id}`);
    console.log("Correction details response:", response);

    if (response.data?.data) {
      const correctionData = response.data.data;

      // Map API response fields to form fields
      formData.value = {
        id: correctionData.id,
        location_id: correctionData.location_id,
        camp_number: correctionData.camp_number || "",
        delegate_phone: correctionData.contact_number
          ? correctionData.contact_number.replace("+1.", "")
          : "",
        square_number: correctionData.square_number || "",
        latitude: correctionData.latitude?.toString() || "",
        longitude: correctionData.longitude?.toString() || "",
      };

      // Set map location if coordinates exist
      if (correctionData.latitude && correctionData.longitude) {
        selectedLocation.value = {
          lat: parseFloat(correctionData.latitude),
          lng: parseFloat(correctionData.longitude),
        };
      }

      dialog.value = true;
    } else {
      throw new Error("No data received");
    }
  } catch (error) {
    console.error("Error fetching correction details:", error);
    useAppStore().showSnackbar({
      message: "خطأ في جلب بيانات التصحيح",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Add function to handle reCorrection
const handleReCorrection = async (id) => {
  await fetchCorrectionDetails(id);
};

// Add function to handle form submission and site creation
const handleSubmit = async () => {
  const { valid } = await form.value.validate();

  if (!valid) {
    useAppStore().showSnackbar({
      message: "يرجى التحقق من صحة جميع الحقول",
      color: "error",
    });
    return;
  }

  try {
    loading.value = true;

    // Format phone number
    const formattedPhone = formData.value.delegate_phone.startsWith("966")
      ? formData.value.delegate_phone
      : `966${formData.value.delegate_phone}`;

    const payload = {
      ...formData.value,
      delegate_phone: formattedPhone,
    };

    // Choose endpoint based on whether we're in correction mode
    const endpoint =
      activeTab.value === "correction"
        ? `location-corrections/${formData.value.id}`
        : "locations";

    // Make API call
    const response = await axios.post(endpoint, payload);

    if (response.data) {
      useAppStore().showSnackbar({
        message:
          activeTab.value === "correction"
            ? "تم تحديث الموقع بنجاح"
            : "تم إنشاء الموقع بنجاح",
        color: "success",
      });
      dialog.value = false;
      resetForm();
      await fetchLocations(currentPage.value);
    }
  } catch (error) {
    console.error("Error submitting location:", error);
    useAppStore().showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حفظ البيانات",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Add function to open dialog and reset form
const addNewLocation = () => {
  resetForm();
  dialog.value = true;
};

// Add function to handle next step
const nextStep = () => {
  if (formData.value.location_type_id) {
    dialogStep.value = 2;
  }
};

// Add function to handle previous step
const prevStep = () => {
  dialogStep.value = 1;
};

// Add function to close dialog
const closeDialog = () => {
  dialog.value = false;
  resetForm();
};

// Add function to reset form
const resetForm = () => {
  formData.value = {
    name: "",
    location_type_id: null,
    location_sub_type_id: null,
    contract_id: null,
    delegate_name: "",
    delegate_phone: "",
    license_no: "",
    camp_number: "",
    square_number: "",
    latitude: "24.7136",
    longitude: "46.6753",
    companies: [],
  };
  selectedLocation.value = { lat: 24.7136, lng: 46.6753 };
  dialogStep.value = 1;
};

// Add location change handler
const handleLocationChange = (location) => {
  formData.value.latitude = location.lat.toString();
  formData.value.longitude = location.lng.toString();
};

// Update onMounted to fetch all required data
onMounted(async () => {
  await Promise.all([
    fetchCompanies(),
    fetchContracts(),
    fetchSiteTypes(),
    fetchLocations(1),
  ]);
});

// Add function to handle site approval
const handleApprove = async (siteId) => {
  try {
    loading.value = true;
    await axios.put(`location-corrections/${siteId}`, {
      status: "approved",
    });
    useAppStore().showSnackbar({
      message: "تم اعتماد التصحيح بنجاح",
      color: "success",
    });
    await fetchLocations(currentPage.value);
  } catch (error) {
    console.error("Error approving correction:", error);
    useAppStore().showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء اعتماد التصحيح",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Add function to handle site edit
const handleEdit = (site) => {
  formData.value = {
    name: site.name,
    location_type_id: site.location_type_id,
    location_sub_type_id: site.location_sub_type_id,
    contract_id: site.contract_id,
    delegate_name: site.delegate_name,
    delegate_phone: site.delegate_phone.replace("966", ""),
    license_no: site.license_no,
    camp_number: site.camp_number,
    square_number: site.square_number,
    latitude: site.latitude,
    longitude: site.longitude,
    companies: site.companies.map((company) => ({
      title: company.name,
      value: company.id,
      raw: company,
    })),
  };
  selectedLocation.value = {
    lat: parseFloat(site.latitude),
    lng: parseFloat(site.longitude),
  };
  dialog.value = true;
  dialogStep.value = 2;
};

const handleReject = async (siteId) => {
  try {
    loading.value = true;
    await axios.put(`location-corrections/${siteId}`, {
      status: "rejected",
    });
    useAppStore().showSnackbar({
      message: "تم رفض التصحيح بنجاح",
      color: "success",
    });
    await fetchLocations(currentPage.value);
  } catch (error) {
    console.error("Error rejecting correction:", error);
    useAppStore().showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء رفض التصحيح",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Add dialog control
const dialog = ref(false);
const dialogStep = ref(1);

// Add function to fetch site types
const fetchSiteTypes = async () => {
  try {
    const response = await axios.get("locations-types");
    if (response.data?.data) {
      siteTypes.value = response.data.data.sub_types;
    }
  } catch (error) {
    console.error("Error fetching site types:", error);
    useAppStore().showSnackbar({
      message: "حدث خطأ أثناء جلب أنواع المواقع",
      color: "error",
    });
  }
};
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
        <v-row>
          <v-col cols="12" md="6">
            <StatsWidget title="المساكن" :number="housing_count">
              <template #icon>
                <svg
                  width="2.2rem"
                  height="2.2rem"
                  viewBox="0 0 33 33"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    d="M18.9168 11.5C18.0678 11.5 17.3181 11.543 16.6811 11.6829C16.0328 11.8253 15.4391 12.0815 14.9687 12.5519C14.4983 13.0223 14.2421 13.616 14.0997 14.2643C13.9598 14.9013 13.9168 15.651 13.9168 16.5L13.9168 28.1667H20.2496V25.8333C20.2496 25.097 20.8465 24.5 21.5829 24.5C22.3193 24.5 22.9162 25.097 22.9162 25.8333V28.1667H29.2501L29.2501 16.5C29.2501 15.651 29.2072 14.9013 29.0672 14.2643C28.9248 13.616 28.6687 13.0223 28.1982 12.5519C27.7278 12.0815 27.1342 11.8253 26.4858 11.6829C25.8488 11.543 25.0991 11.5 24.2501 11.5H18.9168Z"
                    fill="#B69265"
                  />
                  <path
                    d="M1.91693 29.5001C1.91693 28.7637 2.51388 28.1667 3.25026 28.1667L3.25026 20.1667H6.91634C7.46863 20.1667 7.91634 19.719 7.91634 19.1667C7.91634 18.6145 7.46863 18.1667 6.91634 18.1667H3.25026L3.25026 14.8334L6.91634 14.8334C7.46863 14.8334 7.91634 14.3857 7.91634 13.8334C7.91634 13.2811 7.46863 12.8334 6.91634 12.8334L3.25026 12.8334L3.25026 9.50008H6.91634C7.46863 9.50008 7.91634 9.05237 7.91634 8.50008C7.91634 7.9478 7.46863 7.50008 6.91634 7.50008L3.25026 7.50008V7.16675C3.25026 6.31777 3.29324 5.56806 3.43318 4.93106C3.5756 4.28273 3.83175 3.68905 4.30216 3.21864C4.77256 2.74824 5.36625 2.49209 6.01458 2.34966C6.65158 2.20973 7.40129 2.16675 8.25026 2.16675L16.2503 2.16675C17.0992 2.16675 17.849 2.20973 18.486 2.34966C19.1343 2.49209 19.728 2.74824 20.1984 3.21864C20.6688 3.68905 20.9249 4.28273 21.0674 4.93106C21.2073 5.56806 21.2503 6.31777 21.2503 7.16675V8.70008C21.2503 9.0772 21.2503 9.26577 21.1331 9.38292C21.016 9.50008 20.8274 9.50008 20.4503 9.50008H18.9169C18.0242 9.50008 17.1026 9.54274 16.2521 9.72958C15.3681 9.92377 14.3799 10.3125 13.5546 11.1378C12.7293 11.963 12.3406 12.9513 12.1464 13.8353C11.9596 14.6858 11.9169 15.6074 11.9169 16.5001L11.9169 28.1667H20.2503V25.8334C20.2503 25.097 20.8472 24.5001 21.5836 24.5001C22.32 24.5001 22.9169 25.097 22.9169 25.8334V28.1667L29.2503 28.1667C29.9866 28.1667 30.5836 28.7637 30.5836 29.5001C30.5836 30.2365 29.9866 30.8334 29.2503 30.8334L3.25026 30.8334C2.51388 30.8334 1.91693 30.2365 1.91693 29.5001Z"
                    fill="#B69265"
                  />
                  <path
                    d="M19.2503 16.5001C19.2503 15.9478 19.698 15.5001 20.2503 15.5001H22.9169C23.4692 15.5001 23.9169 15.9478 23.9169 16.5001C23.9169 17.0524 23.4692 17.5001 22.9169 17.5001H20.2503C19.698 17.5001 19.2503 17.0524 19.2503 16.5001Z"
                    fill="#B69265"
                  />
                  <path
                    d="M19.2503 20.5001C19.2503 19.9478 19.698 19.5001 20.2503 19.5001H22.9169C23.4692 19.5001 23.9169 19.9478 23.9169 20.5001C23.9169 21.0524 23.4692 21.5001 22.9169 21.5001H20.2503C19.698 21.5001 19.2503 21.0524 19.2503 20.5001Z"
                    fill="#B69265"
                  />
                </svg>
              </template>
            </StatsWidget>
          </v-col>
          <v-col cols="12" md="6">
            <StatsWidget title="المخيمات" :number="camp_count">
              <template #icon>
                <svg
                  width="2.2rem"
                  height="2.2rem"
                  viewBox="0 0 30 27"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <g opacity="0.4">
                    <path
                      d="M9.66798 23.1686C10.2203 23.1686 10.668 23.6163 10.668 24.1686V25.9686C10.668 26.22 10.668 26.3457 10.7461 26.4238C10.8242 26.5019 10.9499 26.5019 11.2013 26.5019H13.468C13.7194 26.5019 13.8451 26.5019 13.9232 26.4238C14.0013 26.3457 14.0013 26.22 14.0013 25.9686V24.1686C14.0013 23.6163 14.449 23.1686 15.0013 23.1686C15.5536 23.1686 16.0013 23.6163 16.0013 24.1686V25.9686C16.0013 26.22 16.0013 26.3457 16.0794 26.4238C16.1575 26.5019 16.2832 26.5019 16.5346 26.5019L22.3601 26.5019C22.7181 26.5019 22.897 26.5019 22.9763 26.3844C23.0556 26.2669 22.9886 26.1009 22.8546 25.769L20.1807 19.1444C20.1245 19.0052 20.0964 18.9356 20.0396 18.8915C19.9829 18.8474 19.9093 18.8374 19.762 18.8176C18.2331 18.6113 16.6399 18.5019 15.0015 18.5019C10.138 18.5019 5.67398 19.4649 2.10692 21.0677L0.976769 21.6634C0.833219 21.7391 0.761444 21.7769 0.722939 21.844C0.684435 21.911 0.687864 21.9907 0.694723 22.15C0.710281 22.5115 0.735046 22.8435 0.774462 23.1461C0.885797 24.001 1.12969 24.7705 1.73126 25.3915C2.33714 26.0169 3.09529 26.2747 3.93826 26.3917C4.7334 26.502 5.73422 26.502 6.93059 26.5019L8.13465 26.5019C8.38606 26.5019 8.51177 26.5019 8.58988 26.4238C8.66798 26.3457 8.66798 26.22 8.66798 25.9686V24.1686C8.66798 23.6163 9.11569 23.1686 9.66798 23.1686Z"
                      fill="#B69265"
                    />
                    <path
                      d="M13.2755 8.06219C13.9204 7.99608 14.6545 7.92083 15.3273 8.33782C15.709 8.57433 15.9499 8.89274 16.1323 9.22373C16.2934 9.51616 16.4401 9.87964 16.5939 10.2608L18.8722 15.9051C19.021 16.2737 19.0954 16.458 19.0065 16.578C18.9177 16.6981 18.7175 16.6806 18.3172 16.6456C17.2332 16.5509 16.1258 16.5019 15.0005 16.5019C14.6284 16.5019 14.2582 16.5073 13.8902 16.5179C13.627 16.5254 13.4953 16.5292 13.4145 16.4507C13.3337 16.3721 13.3337 16.2417 13.3337 15.981V14.8352C13.3337 14.283 12.8859 13.8352 12.3337 13.8352C11.7814 13.8352 11.3337 14.283 11.3337 14.8352V16.1937C11.3337 16.4231 11.3337 16.5377 11.2656 16.6137C11.1976 16.6896 11.0824 16.7023 10.8521 16.7277C10.1004 16.8105 9.36104 16.9155 8.63584 17.0416C8.33884 17.0932 8.19034 17.119 8.09533 17.0391C8.00033 16.9592 8.00033 16.8101 8.00033 16.512V15.5019C8.00033 14.9496 7.55261 14.5019 7.00033 14.5019C6.44804 14.5019 6.00033 14.9496 6.00033 15.5019L6.00033 17.194C6.00033 17.392 6.00033 17.491 5.94546 17.5627C5.89059 17.6345 5.79391 17.6607 5.60056 17.7131C4.05753 18.1314 2.60185 18.6502 1.25612 19.257L1.22807 19.2696C1.07931 19.348 1.00493 19.3872 0.94451 19.3896C0.838115 19.3938 0.739442 19.3342 0.693508 19.2382C0.667421 19.1836 0.667417 19.0995 0.667408 18.9314L0.667233 15.5401C0.665766 15.0731 0.664167 14.5641 0.885516 14.0887C1.10653 13.614 1.47867 13.3029 1.8164 13.0206C4.80268 10.5089 8.38432 8.75636 12.1875 8.19276C12.4992 8.14657 12.9602 8.09307 13.2755 8.06219Z"
                      fill="#B69265"
                    />
                  </g>
                  <path
                    d="M22.229 19.3824C22.1231 19.5056 22.2053 19.7095 22.3699 20.1172L24.7975 26.1314C24.8657 26.3004 24.8999 26.3849 24.9725 26.4308C25.0451 26.4768 25.1341 26.4714 25.3119 26.4608C25.5709 26.4453 25.8133 26.4237 26.0391 26.3936C26.8804 26.2814 27.6395 26.0347 28.2502 25.4288C28.862 24.8219 29.112 24.0655 29.2256 23.227C29.2728 22.879 29.2992 22.4913 29.3141 22.0647C29.32 21.896 29.3229 21.8116 29.2802 21.7428C29.2374 21.6739 29.1594 21.6385 29.0032 21.5678L27.9224 21.0783C26.4188 20.4004 24.755 19.8364 22.9686 19.4104C22.5462 19.3097 22.335 19.2593 22.229 19.3824Z"
                    fill="#B69265"
                  />
                  <path
                    d="M21.1338 16.9652C21.1875 17.0086 21.2601 17.0213 21.4052 17.0467C24.0775 17.5145 26.5563 18.2685 28.7444 19.255L28.7459 19.2557C28.9326 19.3403 29.026 19.3826 29.1016 19.3725C29.1805 19.362 29.2506 19.3168 29.2927 19.2493C29.3331 19.1845 29.3331 19.082 29.3331 18.8771L29.3332 17.1279C29.3344 16.0825 29.3353 15.2414 28.926 14.4487C28.5179 13.6583 27.9044 13.2209 27.1499 12.6831L27.0428 12.6067C25.4655 11.4801 23.7626 10.5643 21.9621 9.89586C21.6551 9.78191 21.3119 9.82543 21.0431 10.0124C20.7743 10.1993 20.614 10.5059 20.614 10.8333V11.9687C20.614 12.2181 20.614 12.3429 20.537 12.4208L20.5348 12.423C20.4569 12.5 20.3322 12.5 20.0827 12.5C19.7276 12.5 19.55 12.5 19.4706 12.6159L19.4684 12.6192C19.3906 12.7362 19.4571 12.9009 19.59 13.2302L20.9983 16.719C21.0528 16.8541 21.0801 16.9217 21.1338 16.9652Z"
                    fill="#B69265"
                  />
                  <path
                    d="M11.7895 5.07345L12.1008 5.86285C12.2137 6.1492 12.2702 6.29238 12.209 6.4025C12.1478 6.51263 11.9935 6.54087 11.6849 6.59736C8.12021 7.24976 4.4639 8.90552 1.56292 11.0946C1.16276 11.3965 0.962678 11.5475 0.814919 11.4739C0.667161 11.4003 0.667157 11.1544 0.667149 10.6626L0.667099 7.49452C0.666045 6.41577 0.665217 5.56787 1.06105 4.76583C1.45879 3.9599 2.05972 3.50991 2.80657 2.95066C3.14719 2.69513 3.66762 2.32572 4.01992 2.09128C4.82978 1.55221 5.55425 1.06997 6.23148 0.790635C7.00133 0.473092 7.76448 0.390496 8.62185 0.669922C9.51689 0.961622 10.0997 1.55529 10.5605 2.32348C10.9831 3.02798 11.3544 3.96965 11.7895 5.07345Z"
                    fill="#B69265"
                  />
                </svg>
              </template>
            </StatsWidget>
          </v-col>
        </v-row>

        <v-container fluid class="mt-6 px-0 rounded-lg">
          <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
            <!-- Header Section -->
            <v-card-title class="pt-4 pb-6">
              <div class="d-flex align-center justify-space-between ga-4">
                <h2 class="text-h5 font-weight-bold mb-4">إدارة المواقع</h2>
                <div class="d-flex align-center ga-2">
                  <!-- Add New Site Button -->
                  <v-btn
                    color="primary"
                    class="me-4"
                    prepend-icon="mdi-plus"
                    @click="addNewLocation"
                  >
                    إضافة موقع جديد
                  </v-btn>

                  <!-- Search Field -->
                  <v-text-field
                    v-model="search"
                    label="البحث المخيم او المسكن... "
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

                  <!-- Filter Button and Menu -->
                  <v-menu
                    v-model="filterMenu"
                      location="bottom end"
                      :close-on-content-click="false"
                    offset="14"
                  >
                    <template v-slot:activator="{ props }">
                      <v-btn
                        v-bind="props"
                        icon
                        variant="text"
                        size="small"
                        color="primary"
                      >
                        <v-icon>mdi-filter-variant</v-icon>
                      </v-btn>
                    </template>

                    <VList class="py-0 text-dark" width="380">
                        <VListItem
                          title="تصفية"
                          class="notification-section"
                          height="48px"
                        >
                          <template #append>
                            <v-btn
                              icon="mdi-close"
                              variant="text"
                              size="small"
                            @click="filterMenu = false"
                            ></v-btn>
                          </template>
                        </VListItem>

                        <VDivider />

                        <!-- Filter Options -->
                        <VListItem>
                          <v-container>
                            <!-- Site Type Filter -->
                            <v-row class="mb-4">
                              <v-col cols="12">
                                <label class="text-subtitle-2 mb-2 d-block"
                                  >نوع الموقع</label
                                >
                                <v-select
                                  v-model="filters.type"
                                  :items="siteTypes"
                                  item-title="name"
                                  item-value="id"
                                  variant="outlined"
                                  density="comfortable"
                                  placeholder="اختر نوع الموقع"
                                  hide-details
                                  class="rounded-lg"
                                  :menu-props="{
                                    contentClass: 'filter-select-menu',
                                  }"
                                />
                              </v-col>
                            </v-row>

                            <!-- Status Filter -->
                            <v-row class="mb-4">
                              <v-col cols="12">
                                <label class="text-subtitle-2 mb-2 d-block"
                                  >الحالة</label
                                >
                                <v-select
                                  v-model="filters.status"
                                  :items="[
                                    { title: 'نشط', value: 'active' },
                                    { title: 'غير نشط', value: 'inactive' },
                                    { title: 'معلق', value: 'pending' },
                                  ]"
                                  item-title="title"
                                  item-value="value"
                                  variant="outlined"
                                  density="comfortable"
                                  placeholder="اختر الحالة"
                                  hide-details
                                  class="rounded-lg"
                                  :menu-props="{
                                    contentClass: 'filter-select-menu',
                                  }"
                                />
                              </v-col>
                            </v-row>

                            <!-- Date Range Filter -->
                            <v-row class="mb-4">
                              <v-col cols="12">
                                <label class="text-subtitle-2 mb-2 d-block"
                                  >التاريخ</label
                                >
                                <AppDateTimePicker
                                  v-model="filters.dateRange"
                                  range
                                  no-title
                                  class="rounded-lg"
                                />
                              </v-col>
                            </v-row>
                          </v-container>
                        </VListItem>

                        <VDivider />

                        <VListItem class="notification-section pa-4">
                          <div class="d-flex gap-3">
                            <VBtn
                              color="error"
                              variant="outlined"
                              block
                              @click="resetFilters"
                            >
                              إعادة تعيين
                            </VBtn>
                            <VBtn
                              color="primary"
                              block
                              @click="
                                applyFilters();
                                $event.target
                                  .closest('.v-menu')
                                  .querySelector('.v-overlay__content')
                                  .__vue__?.close();
                              "
                            >
                              تطبيق التصفية
                            </VBtn>
                          </div>
                        </VListItem>
                      </VList>
                  </v-menu>
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
                    جميع المواقع
                  </div>
                  <div
                    class="tab-item px-4 py-2"
                    :class="{ active: activeTab === 'correction' }"
                    @click="activeTab = 'correction'"
                  >
                    تصحيح المواقع
                  </div>
                </div>
                <div class="tab-border"></div>
              </div>

              <v-row class="mt-6">
                <v-col
                  v-for="site in sites"
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
                    :is-correction="activeTab === 'correction'"
                    @approve="handleApprove"
                    @reject="handleReject"
                    @update="handleReCorrection"
                  >
                    <template v-if="activeTab === 'correction'" #actions>
                      <v-btn
                        v-if="
                          site.status !== 'approved' &&
                          site.status !== 'rejected'
                        "
                        color="success"
                        variant="outlined"
                        class="me-2"
                        :loading="loading"
                        @click="handleApprove(site.id)"
                      >
                        اعتماد
                      </v-btn>
                      <v-btn
                        v-if="
                          site.status !== 'approved' &&
                          site.status !== 'rejected'
                        "
                        color="warning"
                        variant="outlined"
                        :loading="loading"
                        @click="handleReCorrection(site.id)"
                      >
                        تصحيح
                      </v-btn>
                    </template>
                  </SiteCard>
                </v-col>
                <v-col
                  v-if="!isLoading && sites.length === 0"
                  cols="12"
                  class="text-center"
                >
                  <p class="text-medium-emphasis">لا توجد مواقع متاحة</p>
                </v-col>
              </v-row>

              <!-- pagination -->
              <Pagination
                v-if="totalItems > 0"
                :total-items="totalItems"
                v-model:per-page="perPage"
                v-model:page="currentPage"
                @update:page="fetchLocations"
              />
            </v-card-title>
          </VCard>
        </v-container>
      </div>
    </v-container>

    <!-- Add New Site Dialog -->
      <v-dialog v-model="dialog" width="500" class="add-site-dialog">
        <v-card class="rounded-lg">
          <v-card-text class="pa-4">
            <div class="d-flex justify-space-between align-center mb-6">
            <h2 class="text-h6">
              {{ dialogStep === 1 ? "اختر نوع الموقع" : "إضافة موقع جديد" }}
            </h2>
            <v-btn icon variant="text" @click="closeDialog">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </div>

          <!-- Step 1: Select Site Type -->
          <v-form v-if="dialogStep === 1" @submit.prevent="nextStep">
              <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">نوع الموقع</label>
              <v-select
                v-model="formData.location_type_id"
                :items="siteTypes"
                item-title="name"
                item-value="id"
                variant="outlined"
                density="comfortable"
                placeholder="اختر نوع الموقع"
                hide-details
                class="rounded-lg bg-grey-lighten-4"
                required
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
              <v-btn color="#B69265" class="text-white px-6" type="submit">
                التالي
              </v-btn>
            </div>
          </v-form>

          <!-- Step 2: Fill Details -->
          <v-form v-else ref="form" @submit.prevent="handleSubmit">
            <!-- Name -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">اسم الموقع</label>
                <v-text-field
                v-model="formData.name"
                placeholder="أدخل اسم الموقع"
                  variant="outlined"
                  density="comfortable"
                  class="rounded-lg bg-grey-lighten-4"
                  required
                />
              </div>

            <!-- License Number -->
              <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">رقم الترخيص</label>
                  <v-text-field
                v-model="formData.license_no"
                placeholder="أدخل رقم الترخيص"
                    variant="outlined"
                    density="comfortable"
                class="rounded-lg bg-grey-lighten-4"
                    type="text"
                    required
                  />
              </div>

            <!-- Delegate Name -->
              <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">اسم المفوض</label>
                <v-text-field
                v-model="formData.delegate_name"
                placeholder="أدخل اسم المفوض"
                  variant="outlined"
                  density="comfortable"
                  class="rounded-lg bg-grey-lighten-4"
                required
                />
              </div>

            <!-- Delegate Phone -->
              <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block"
                >رقم هاتف المفوض</label
              >
                <v-text-field
                v-model="formData.delegate_phone"
                placeholder="5xxxxxxxx"
                  variant="outlined"
                  density="comfortable"
                  class="rounded-lg bg-grey-lighten-4"
                required
              />
            </div>

            <!-- Contract -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">العقد</label>
              <v-select
                v-model="formData.contract_id"
                :items="contracts"
                item-title="title"
                item-value="value"
                variant="outlined"
                density="comfortable"
                placeholder="اختر العقد"
                hide-details
                class="rounded-lg bg-grey-lighten-4"
                required
              />
            </div>

            <!-- Companies -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">الشركات</label>
              <v-select
                v-model="formData.companies"
                :items="companies"
                item-title="title"
                item-value="value"
                variant="outlined"
                density="comfortable"
                placeholder="اختر الشركات"
                multiple
                hide-details
                class="rounded-lg bg-grey-lighten-4"
                  required
                />
              </div>

              <!-- Map Location -->
              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block"
                  >الموقع على الخريطة</label
                >
                <LocationPicker
                  v-model="selectedLocation"
                  class="rounded-lg overflow-hidden"
                style="height: 300px"
                  @update:model-value="handleLocationChange"
                />
                <!-- Coordinates display -->
                <div class="d-flex gap-4 mt-2">
                  <v-text-field
                    v-model="formData.latitude"
                    label="خط العرض"
                    variant="outlined"
                    density="comfortable"
                    class="rounded-lg"
                    hide-details
                  readonly
                  />
                  <v-text-field
                    v-model="formData.longitude"
                    label="خط الطول"
                    variant="outlined"
                    density="comfortable"
                    class="rounded-lg"
                    hide-details
                  readonly
                  />
                </div>
              </div>

              <!-- Action Buttons -->
              <div class="d-flex justify-end mt-6">
                <v-btn
                  variant="text"
                  color="grey-darken-1"
                  class="me-4"
                @click="prevStep"
                >
                رجوع
                </v-btn>
                <v-btn
                  color="#B69265"
                  class="text-white px-6"
                  type="submit"
                  :loading="loading"
                >
                إنشاء
                </v-btn>
              </div>
            </v-form>
          </v-card-text>
        </v-card>
      </v-dialog>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

// Add filter styles
.notification-section {
  :deep(.v-select),
  :deep(.v-text-field) {
    .v-field {
      border-radius: 8px;
      background-color: rgb(250, 250, 250);

      &.v-field--focused {
        background-color: white;
      }
    }
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
    border-radius: 8px;
    height: 40px;
  }
}

// Date picker styles
:deep(.v-date-picker) {
  border-radius: 8px;
  overflow: hidden;
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

// Add styles for the dialog
.add-site-dialog {
  :deep(.v-card) {
    .v-field {
    border-radius: 8px;
    background-color: rgb(250, 250, 250);

    &.v-field--focused {
      background-color: white;
    }
    }
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
    border-radius: 8px;
  }

  :deep(.v-select) {
    .v-field__input {
      min-height: 44px;
      padding-top: 0;
      padding-bottom: 0;
    }
  }
}
</style>
