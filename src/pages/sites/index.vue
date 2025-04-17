<script setup>
import { useAppStore } from "@/stores/app";
// import AppDateTimePicker from "@/@core/components/AppDateTimePicker";
import StatsWidget from "@/@core/components/widgets/StatsWidget";
import Pagination from "@/@core/components/Pagination";
import SiteCard from "@/@core/components/widgets/SiteCard.vue";
import CorrectSiteCard from "@/@core/components/widgets/CorrectSiteCard.vue";
import axios from "@/plugins/axios";
import LocationPicker from "@/components/maps/LocationPicker.vue";
import { hasPermission } from "@core/utils/helpers";

const isLoading = ref(true);
const isExporting = ref(false);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const loading = ref(false);
const filterMenu = ref(false);
const activeTab = ref("all");
const sites = ref([]);
const totalItems = ref(0);
const selectedLocation = ref({ lat: 24.7136, lng: 46.6753 }); // Default to Riyadh coordinates

// Add new filter refs
const filters = ref({
  location_type_id: null,
  location_sub_type_id: null,
  dateRange: null,
});

const tempDateFilter = ref(null);

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
const locationSubTypes = ref([]);

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
      ...(filters.value.location_type_id && {
        location_type_id: filters.value.location_type_id,
      }),
      ...(filters.value.location_sub_type_id && {
        location_sub_type_id: filters.value.location_sub_type_id,
      }),
      ...(tempDateFilter.value && {
        created_at: tempDateFilter.value,
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

// Add computed property for active filters
const hasActiveFilters = computed(() => {
  return (
    filters.value.location_type_id !== null ||
    filters.value.location_sub_type_id !== null ||
    filters.value.dateRange !== null
  );
});

// Function to apply filters
const applyFilters = () => {
  fetchLocations(1);
  filterMenu.value = false; // Close the filter menu after applying
};

// Function to reset filters
const resetFilters = () => {
  filters.value = {
    location_type_id: null,
    location_sub_type_id: null,
    dateRange: null,
  };
  fetchLocations(1); // Fetch locations after resetting filters
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
  if (!formData.value.location_type_id) {
    useAppStore().showSnackbar({
      message: "يرجى اختيار نوع الموقع",
      color: "error",
    });
    return;
  }
  dialogStep.value = 2;
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
  if (form.value) {
    form.value.resetValidation();
  }
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
    const response = await axios.get("/locations-types");
    // console.log(response.data.data.sub_types);
    if (response.data?.data) {
      locationSubTypes.value = response.data.data.sub_types;
    }
  } catch (error) {
    console.error("Error fetching site types:", error);
    useAppStore().showSnackbar({
      message: "حدث خطأ أثناء جلب أنواع المواقع",
      color: "error",
    });
  }
};

// Add export function
const exportData = async () => {
  try {
    isExporting.value = true;

    const response = await axios.post(
      "/locations-export",
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
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
      <div>
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

                        <!-- Location Type Filter -->
                        <div class="mb-4">
                          <v-select
                            v-model="filters.location_type_id"
                            label="نوع الموقع"
                            :items="siteTypes"
                            item-title="name"
                            item-value="id"
                            variant="outlined"
                            hide-details
                            density="comfortable"
                            class="mb-3 rounded-lg bg-grey-lighten-4"
                          ></v-select>
                        </div>

                        <!-- Location Sub Type Filter -->
                        <div class="mb-4">
                          <v-select
                            v-model="filters.location_sub_type_id"
                            label="نوع الموقع الفرعي"
                            :items="locationSubTypes"
                            item-title="name"
                            item-value="id"
                            variant="outlined"
                            hide-details
                            density="comfortable"
                            class="mb-3 rounded-lg bg-grey-lighten-4"
                          ></v-select>
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

                  <!-- Add New Site Button -->
                  <v-btn
                    v-if="hasPermission('locations.create')"
                    color="primary"
                    class="me-4"
                    prepend-icon="mdi-plus"
                    @click="addNewLocation"
                  >
                    إضافة موقع جديد
                  </v-btn>
                </div>
              </div>
              <!-- Search and Add Section -->

              <!-- Tabs Section -->
              <div class="tabs-container mb-6 border-b-0">
                <div class="d-flex">
                  <div
                    v-if="hasPermission('locations.view')"
                    class="tab-item px-4 py-2"
                    :class="{ active: activeTab === 'all' }"
                    @click="activeTab = 'all'"
                  >
                    جميع المواقع
                  </div>
                  <div
                    v-if="hasPermission('location_corrections.view')"
                    class="tab-item px-4 py-2"
                    :class="{ active: activeTab === 'correction' }"
                    @click="activeTab = 'correction'"
                  >
                    تصحيح المواقع
                  </div>
                </div>
                <div class="tab-border"></div>
              </div>

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
              <v-row v-else class="mt-6">
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
                    v-if="activeTab === 'all'"
                    :site-id="site.id"
                    :name="site.name"
                    :license-no="site.license_no"
                    :delegate-phone="site.delegate_phone"
                    :companies-count="site.companies_count"
                    :location-type="site.location_type"
                    :status="site.status"
                    :latitude="site.latitude"
                    :longitude="site.longitude"
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

                  <CorrectSiteCard
                    v-if="activeTab === 'correction'"
                    :site-id="site.id"
                    :data="site"
                    :can-update="hasPermission('location_corrections.edit')"
                  />
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

            <!-- Location Sub Type -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block"
                >نوع الموقع الفرعي</label
              >
              <v-select
                v-model="formData.location_sub_type_id"
                :items="locationSubTypes"
                item-title="name"
                item-value="id"
                variant="outlined"
                density="comfortable"
                placeholder="اختر نوع الموقع الفرعي"
                hide-details
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
