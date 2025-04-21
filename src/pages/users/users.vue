<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import UserCard from "@/@core/components/widgets/userCard";
import CustomVAutocomplete from "@/@core/components/global/CustomVAutocomplete.vue";
import axios from "@/plugins/axios";
import { hasPermission } from "@core/utils/helpers";
// import UserWidget from "@/@core/components/widgets/UserWidget.vue";
// import UserStatisticsCard from "@/@core/components/widgets/UserStatisticsCard.vue";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const loading = ref(false);
const companies = ref([]);

// Data for users
const users = ref([]);
const totalUsers = ref(0);
const userDialog = ref(false);
const isEditingDialog = ref(false);
const selectedUser = ref(null);
const deleteDialog = ref(false);
const userToDelete = ref(null);
const filterDialog = ref(false);
const userDetailsDialog = ref(false);
const userStatistics = ref(null);

// Form data for new user
const newUserForm = ref({
  firstname: "",
  lastname: "",
  parent_companies: [],
  email: "",
  phone: "",
  password: "",
  password_confirmation: "",
  role_id: null,
  admin_role_id: null,
  city: "",
  blood_type: "",
  gender: "",
  nationality: "",
  sa_id: "",
  // personal_image: "",
});

// Form data for edit user
const editUserForm = ref({
  firstname: "",
  lastname: "",
  parent_companies: [],
  email: "",
  phone: "",
  role_id: null,
  admin_role_id: null,
  city: "",
  blood_type: "",
  gender: "",
  nationality: "",
  sa_id: "",
  // personal_image: "",
});

// Filter form data
const filterForm = ref({
  role_id: null,
  district: null,
});

// Available roles for selection
const roles = ref([
  { id: 1, title: "ADMIN" },
  { id: 2, title: "SUPERVISOR" },
  { id: 3, title: "INSPECTOR" },
]);

// Blood types for selection
const bloodTypes = ["A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"];

// Available districts
const districts = [
  "العزيزية",
  "الشوقية",
  "النوارية",
  "السفلة",
  "بطحاء قريش",
  "العوالي",
  "الرصيفة",
  "الزاهر",
];

// Add validation rules
const rules = {
  required: (v) => !!v || "هذا الحقل مطلوب",
  email: (v) => {
    if (!v) return true;
    return /.+@.+\..+/.test(v) || "البريد الإلكتروني غير صالح";
  },
  phone: (v) => {
    if (!v) return true;
    return /^[0-9]{10}$/.test(v) || "رقم الهاتف يجب أن يكون 10 أرقام";
  },
  password: (v) => {
    if (!v) return true;
    return v.length >= 8 || "كلمة المرور يجب أن تكون 8 أحرف على الأقل";
  },
  passwordMatch: (v) => {
    if (!v) return true;
    return v === newUserForm.value.password || "كلمة المرور غير متطابقة";
  },
  saId: (v) => {
    if (!v) return true;
    return /^[0-9]{10}$/.test(v) || "رقم الهوية يجب أن يكون 10 أرقام";
  },
};

// Add form ref
const addUserForm = ref(null);

// Fetch users with pagination and search
const fetchUsers = async () => {
  try {
    isLoading.value = true;
    const queryParams = new URLSearchParams({
      page: currentPage.value.toString(),
      per_page: perPage.value.toString(),
      search: search.value || "",
      district: filterForm.value.district,
    });

    // Only add role_id to params if it's not null
    if (filterForm.value.role_id !== null) {
      queryParams.append("role_id", filterForm.value.role_id.toString());
    }

    const response = await axios.get(`/users?${queryParams.toString()}`);
    if (response.data?.data) {
      users.value = response.data.data;
      console.log(response.data.data);
      // Update pagination data
      if (response.data.pagination) {
        totalUsers.value = response.data.pagination.i_total_objects;
        perPage.value = response.data.pagination.i_per_page;
        currentPage.value = response.data.pagination.i_current_page;
      }
    }
  } catch (error) {
    console.error("Error fetching users:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
    users.value = [];
    totalUsers.value = 0;
  } finally {
    isLoading.value = false;
  }
};

// Modify addUser function to use newUserForm
const addUser = async () => {
  const { valid } = await addUserForm.value.validate();

  if (!valid) {
    appStore.showSnackbar({
      message: "يرجى التحقق من صحة البيانات المدخلة",
      color: "error",
    });
    return;
  }

  try {
    loading.value = true;
    const response = await axios.post("/users", newUserForm.value);

    if (response.data) {
      appStore.showSnackbar({
        message: "تم إضافة المستخدم بنجاح",
        color: "success",
      });
      resetNewUserForm();
      userDialog.value = false;
      await fetchUsers();
    }
  } catch (error) {
    console.error("Error adding user:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء إضافة المستخدم",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Get user details
const getUserDetails = async (userId) => {
  try {
    loading.value = true;
    const response = await axios.get(`/users/${userId}`);
    if (response.data?.data?.user) {
      selectedUser.value = response.data.data.user;
      userStatistics.value = response.data.data.statistics;
      populateEditForm(response.data.data.user);
      userDetailsDialog.value = true;
    }
  } catch (error) {
    console.error("Error fetching user details:", error);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء جلب بيانات المستخدم",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Modify updateUser function to use editUserForm
const updateUser = async () => {
  try {
    loading.value = true;
    const response = await axios.put(
      `/users/${selectedUser.value.id}`,
      editUserForm.value
    );

    if (response.data) {
      appStore.showSnackbar({
        message: "تم تحديث بيانات المستخدم بنجاح",
        color: "success",
      });
      isEditingDialog.value = false;
      await fetchUsers();
    }
  } catch (error) {
    console.error("Error updating user:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء تحديث بيانات المستخدم",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Delete user confirmation
const confirmDelete = (userId) => {
  userToDelete.value = userId;
  deleteDialog.value = true;
};

// Delete user
const deleteUser = async () => {
  if (!userToDelete.value) return;

  try {
    loading.value = true;
    const response = await axios.delete(`/users/${userToDelete.value}`);

    if (response.data) {
      appStore.showSnackbar({
        message: "تم حذف المستخدم بنجاح",
        color: "success",
      });
      deleteDialog.value = false;
      userToDelete.value = null;
      await fetchUsers();
    }
  } catch (error) {
    console.error("Error deleting user:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء حذف المستخدم",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Helper functions
const resetNewUserForm = () => {
  newUserForm.value = {
    firstname: "",
    // secondname: "",
    // thirdname: "",
    lastname: "",
    email: "",
    phone: "",
    password: "",
    password_confirmation: "",
    role_id: null,
    city: "",
    blood_type: "",
    gender: "",
    nationality: "",
    sa_id: "",
    personal_image: "",
  };
};

const countryCode = ref("+966");

const populateEditForm = (user) => {
  editUserForm.value = {
    firstname: user.firstname || "",
    // secondname: user.secondname || "",
    // thirdname: user.thirdname || "",
    lastname: user.lastname || "",
    parent_companies: user.parent_companies,
    email: user.email || "",
    phone: user.phone || "",
    role_id: user.role?.id || null,
    admin_role_id: user.admin_role?.id || null,
    city: user.city || "",
    blood_type: user.blood_type || "",
    gender: user.gender || "",
    nationality: user.nationality || "",
    sa_id: user.sa_id || "",
    // personal_image: user.personal_image || "",
  };
};

// Watch for changes in search with debounce
const debouncedSearch = ref(null);
watch(search, (newValue) => {
  if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
  debouncedSearch.value = setTimeout(() => {
    currentPage.value = 1;
    fetchUsers();
  }, 500);
});

// Watch for page changes
watch(currentPage, () => {
  fetchUsers();
});

// Add watcher for perPage changes
watch(perPage, () => {
  currentPage.value = 1; // Reset to first page when changing page size
  fetchUsers();
});

// Add applyFilter function before the onMounted
const applyFilter = async () => {
  filterDialog.value = false;
  currentPage.value = 1;
  await fetchUsers();
};

// Add reset filter function
const resetFilter = () => {
  filterForm.value = {
    role_id: null,
    district: "العزيزية",
  };
  filterDialog.value = false;
  fetchUsers();
};

const isGettingUtilities = ref(false);

const getUtilities = async () => {
  try {
    isGettingUtilities.value = true;
    const response = await axios.get("/roles?per_page=1000");
    roles.value = response?.data?.data || [];

    const companiesResponse = await axios.get(
      "/companies?type=parent&per_page=1000"
    );
    companies.value = companiesResponse?.data?.data || [];
  } catch (error) {
    console.error("Error fetching ratings:", error);
  } finally {
    isGettingUtilities.value = false;
  }
};

onMounted(() => {
  fetchUsers();
  getUtilities();
  // hasPermission("users.view")
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-0">
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
              title: 'إدارة المستخدمين',
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
        <v-card-title class="pt-4 pb-6">
          <div class="d-flex align-center justify-space-between gap-4">
            <h2 class="text-h5 font-weight-bold">إدارة المستخدمين</h2>
            <div class="d-flex align-center gap-2">
              <v-text-field
                v-model="search"
                label="البحث عن مستخدم... "
                rounded="lg"
                clearable
                height="48"
                class="bg-white"
                style="width: 320px; max-width: 100%"
              >
                <template v-slot:prepend-inner>
                  <v-icon>mdi-magnify</v-icon>
                </template>
              </v-text-field>
              <!--
                  <VBtn
                    height="48"
                    elevation="0"
                    class="rounded-lg"
                    variant="outlined"
                    @click="filterDialog = true"
                  >
                    فلترة
                  </VBtn> -->
              <VBtn
                color="default"
                class="btn-icon border"
                elevation="0"
                v-bind="props"
                :variant="hasActiveFilters ? 'flat' : 'outlined'"
                @click="filterDialog = true"
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

              <VBtn
                v-if="hasPermission('users.create')"
                height="48"
                elevation="0"
                class="rounded-lg"
                @click="userDialog = true"
              >
                إضافة مستخدم جديد
              </VBtn>
            </div>
          </div>

          <div v-if="isLoading">
            <v-row>
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
          </div>

          <v-row v-else class="mt-6">
            <v-col
              v-for="user in users"
              :key="user.id"
              cols="12"
              sm="6"
              md="4"
              lg="4"
              class="pa-3"
            >
              <UserCard
                :user="user"
                @edit="getUserDetails(user.id)"
                @delete="confirmDelete(user.id)"
                :can-delete="hasPermission('users.delete')"
                :can-edit="hasPermission('users.edit')"
              />
            </v-col>
            <v-col
              v-if="!isLoading && users.length === 0"
              cols="12"
              class="text-center"
            >
              <p class="text-medium-emphasis">لا يوجد مستخدمين</p>
            </v-col>
          </v-row>

          <!-- pagination -->
          <Pagination
            :total-items="totalUsers"
            v-model:per-page="perPage"
            v-model:page="currentPage"
          />
        </v-card-title>
      </VCard>
    </v-container>

    <!-- Add User Dialog -->
    <template v-if="hasPermission('users.create')">
      <v-dialog v-model="userDialog" max-width="600px">
        <v-card class="pa-4">
          <v-card-title class="text-h5 mb-4">إضافة مستخدم جديد</v-card-title>
          <v-card-text>
            <v-form ref="addUserForm" @submit.prevent="addUser">
              <v-row>
                <v-col cols="12" md="6">
                  <label class="mb-2 d-block">الاسم الأول</label>
                  <v-text-field
                    v-model="newUserForm.firstname"
                    :rules="[rules.required]"
                    required
                  ></v-text-field>
                </v-col>
                <!-- <v-col cols="12">
                  <label class="mb-2 d-block">الاسم الثاني</label>
                  <v-text-field v-model="newUserForm.secondname"></v-text-field>
                </v-col> -->
                <!-- <v-col cols="12">
                  <label class="mb-2 d-block">الاسم الثالث</label>
                  <v-text-field v-model="newUserForm.thirdname"></v-text-field>
                </v-col> -->
                <v-col cols="12" md="6">
                  <label class="mb-2 d-block">اسم العائلة</label>
                  <v-text-field
                    v-model="newUserForm.lastname"
                    :rules="[rules.required]"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="12">
                  <label class="mb-2 d-block">رقم الجوال</label>
                  <div class="d-flex gap-2">
                    <v-text-field
                      v-model="newUserForm.phone"
                      type="number"
                      :rules="[rules.required, rules.phone]"
                      placeholder="00"
                      variant="outlined"
                      density="compact"
                      bg-color="white"
                      hide-details
                      style="flex: 1"
                    ></v-text-field>
                    <v-select
                      v-model="countryCode"
                      :items="['+966']"
                      variant="outlined"
                      density="compact"
                      bg-color="white"
                      hide-details
                      style="max-width: 120px"
                    ></v-select>
                  </div>
                </v-col>

                <!-- <v-col cols="12">
                  <label class="mb-2 d-block">المدينة</label>
                  <v-text-field
                    v-model="newUserForm.city"
                    :rules="[rules.required]"
                    required
                  ></v-text-field>
                </v-col> -->
                <!-- <v-col cols="12">
                  <label class="mb-2 d-block">فصيلة الدم</label>
                  <v-select
                    v-model="newUserForm.blood_type"
                    :items="bloodTypes"
                    :rules="[rules.required]"
                    required
                  ></v-select>
                </v-col> -->
                <!-- <v-col cols="12">
                  <label class="mb-2 d-block">الجنس</label>
                  <v-select
                    v-model="newUserForm.gender"
                    :items="[
                      { title: 'ذكر', value: 'male' },
                      { title: 'أنثى', value: 'female' },
                    ]"
                    :rules="[rules.required]"
                    required
                  ></v-select>
                </v-col> -->
                <!-- <v-col cols="12">
                  <label class="mb-2 d-block">الجنسية</label>
                  <v-text-field
                    v-model="newUserForm.nationality"
                    :rules="[rules.required]"
                    required
                  ></v-text-field>
                </v-col> -->
                <v-col cols="12">
                  <label class="mb-2 d-block">البريد الإلكتروني</label>
                  <v-text-field
                    v-model="newUserForm.email"
                    type="email"
                    :rules="[rules.required, rules.email]"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="12">
                  <label class="mb-2 d-block">رقم الهوية</label>
                  <v-text-field
                    v-model="newUserForm.sa_id"
                    type="number"
                    :rules="[rules.required, rules.saId]"
                    placeholder="مثال: 1234567890"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="12">
                  <label class="mb-2 d-block">كلمة المرور</label>
                  <v-text-field
                    v-model="newUserForm.password"
                    type="password"
                    :rules="[rules.required, rules.password]"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="12">
                  <label class="mb-2 d-block">تأكيد كلمة المرور</label>
                  <v-text-field
                    v-model="newUserForm.password_confirmation"
                    type="password"
                    :rules="[rules.required, rules.passwordMatch]"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="12">
                  <label class="mb-2 d-block">الشركة الرئيسية</label>
                  <CustomVAutocomplete
                    v-model="newUserForm.parent_companies"
                    :items="companies"
                    :loading="isGettingFilters"
                    item-title="name"
                    item-value="id"
                    clearable
                    multiple
                    :return-object="false"
                  />
                </v-col>
                <v-col cols="12">
                  <label class="mb-2 d-block"> الدور الأساسي</label>
                  <v-select
                    v-model="newUserForm.role_id"
                    :items="[
                      { id: 1, title: 'مدير النظام' },
                      { id: 4, title: 'المشرف العام' },
                      { id: 2, title: 'المشرف' },
                      { id: 3, title: 'المراقب' },
                    ]"
                    item-title="title"
                    item-value="id"
                    :rules="[rules.required]"
                    required
                  ></v-select>
                </v-col>
                <v-col cols="12">
                  <label class="mb-2 d-block">دور لوحة التحكم</label>
                  <v-select
                    v-model="newUserForm.admin_role_id"
                    :items="roles"
                    item-title="name"
                    item-value="id"
                    required
                  ></v-select>
                </v-col>
              </v-row>
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn
              type="button"
              class="bg-red"
              color="error"
              @click="userDialog = false"
            >
              إلغاء
            </v-btn>
            <v-btn
              color="primary"
              :loading="loading"
              @click="addUser"
              class="px-6 bg-primary text-white"
            >
              إضافة مستخدم
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </template>

    <!-- Edit User Dialog  -->
    <v-dialog
      v-if="hasPermission('users.edit')"
      v-model="userDetailsDialog"
      max-width="600px"
    >
      <v-card class="pa-6">
        <v-card-title class="px-0 d-flex justify-space-between align-center">
          <h2 class="text-h5">تعديل البيانات</h2>
          <v-btn icon variant="text" @click="userDetailsDialog = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-card-title>

        <v-card-subtitle class="px-0 text-medium-emphasis">
          يرجى عند تعديل بيانات المستخدم الضغط علي "حفظ" لإضافته إلى النظام
        </v-card-subtitle>

        <v-card-text class="px-0">
          <v-form @submit.prevent="updateUser">
            <v-row>
              <v-col cols="12" md="6">
                <label class="mb-2 d-block">الاسم الأول</label>
                <v-text-field
                  v-model="editUserForm.firstname"
                  placeholder="اسم المستخدم"
                  variant="outlined"
                  density="compact"
                  bg-color="white"
                  hide-details
                ></v-text-field>
              </v-col>
              <v-col cols="12" md="6">
                <label class="mb-2 d-block">اسم العائلة</label>
                <v-text-field
                  v-model="editUserForm.lastname"
                  placeholder="اسم العائلة"
                  variant="outlined"
                  density="compact"
                  bg-color="white"
                  hide-details
                ></v-text-field>
              </v-col>

              <v-col cols="12">
                <label class="mb-2 d-block">رقم الهوية</label>
                <v-text-field
                  v-model="editUserForm.sa_id"
                  placeholder="6127782187"
                  variant="outlined"
                  density="compact"
                  bg-color="white"
                  hide-details
                ></v-text-field>
              </v-col>

              <v-col cols="12">
                <label class="mb-2 d-block">رقم الجوال</label>
                <div class="d-flex gap-2">
                  <v-text-field
                    v-model="editUserForm.phone"
                    placeholder="00"
                    variant="outlined"
                    density="compact"
                    bg-color="white"
                    hide-details
                    style="flex: 1"
                  ></v-text-field>
                  <v-select
                    v-model="countryCode"
                    :items="['+966']"
                    variant="outlined"
                    density="compact"
                    bg-color="white"
                    hide-details
                    style="max-width: 120px"
                  ></v-select>
                </div>
              </v-col>

              <v-col cols="12">
                <label class="mb-2 d-block">البريد الإلكتروني</label>
                <v-text-field
                  v-model="editUserForm.email"
                  type="email"
                  placeholder="البريد الإلكتروني"
                  variant="outlined"
                  density="compact"
                  bg-color="white"
                  hide-details
                ></v-text-field>
              </v-col>

              <v-col cols="12">
                <label class="mb-2 d-block">كلمة المرور</label>
                <v-text-field
                  v-model="editUserForm.password"
                  type="password"
                  placeholder="***********"
                  variant="outlined"
                  density="compact"
                  bg-color="white"
                  hide-details
                ></v-text-field>
              </v-col>

              <v-col cols="12">
                <label class="mb-2 d-block">الشركة الرئيسية</label>
                <CustomVAutocomplete
                  v-model="editUserForm.parent_companies"
                  :items="companies"
                  :loading="isGettingFilters"
                  item-title="name"
                  item-value="id"
                  clearable
                  multiple
                  :return-object="false"
                />
              </v-col>

              <v-col cols="12">
                <label class="mb-2 d-block"> الدور الأساسي</label>
                <v-select
                  v-model="editUserForm.role_id"
                  :items="[
                    { id: 1, title: 'مدير النظام' },
                    { id: 4, title: 'المشرف العام' },
                    { id: 2, title: 'المشرف' },
                    { id: 3, title: 'المراقب' },
                  ]"
                  item-title="title"
                  item-value="id"
                  :rules="[rules.required]"
                  required
                ></v-select>
              </v-col>

              <v-col cols="12">
                <label class="mb-2 d-block">دور لوحة التحكم</label>
                <v-select
                  v-model="editUserForm.admin_role_id"
                  :items="roles"
                  item-title="name"
                  item-value="id"
                  required
                ></v-select>
              </v-col>
            </v-row>

            <div class="d-flex gap-3 mt-6 justify-space-between">
              <v-btn
                color="error"
                variant="outlined"
                max-width="150px"
                @click="userDetailsDialog = false"
              >
                إلغاء
              </v-btn>
              <v-btn
                color="primary"
                type="submit"
                :loading="loading"
                max-width="150px"
                class="bg-primary"
              >
                حفظ
              </v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <template v-if="hasPermission('users.delete')">
      <v-dialog v-model="deleteDialog" max-width="500px">
        <v-card>
          <v-card-title class="text-h5 pa-4">تأكيد الحذف</v-card-title>
          <v-card-text class="pa-4">
            هل أنت متأكد من حذف هذا المستخدم؟ لا يمكن التراجع عن هذا الإجراء.
          </v-card-text>
          <v-card-actions class="pa-4">
            <v-spacer></v-spacer>
            <v-btn
              color="error"
              variant="text"
              @click="deleteDialog = false"
              :disabled="loading"
            >
              إلغاء
            </v-btn>
            <v-btn
              color="primary"
              variant="flat"
              @click="deleteUser"
              :loading="loading"
            >
              تأكيد الحذف
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </template>

    <!-- Filter Dialog -->
    <v-dialog v-model="filterDialog" max-width="500px">
      <v-card class="pa-4">
        <v-card-title
          class="text-h5 border-b border-color-light-white pb-4 px-0"
        >
          فلترة
        </v-card-title>
        <v-card-text class="px-4">
          <v-form>
            <v-row class="">
              <v-col cols="12" class="bg-gray-1 mt-4">
                <label class="mb-2 d-block">فلترة حسب الدور</label>
                <v-radio-group v-model="filterForm.role_id" class="mt-2">
                  <div class="d-flex flex-row flex-wrap">
                    <v-radio
                      v-for="role in roles"
                      :key="role.id"
                      :label="role.name"
                      :value="role.id"
                      style="width: 50%"
                      class="mx-0 flex-shrink-0"
                    ></v-radio>
                  </div>
                </v-radio-group>
              </v-col>

              <v-col cols="12" class="bg-gray-1 mt-4">
                <label class="mb-2 d-block">فلترة حسب الأحياء</label>
                <v-radio-group v-model="filterForm.district" class="mt-2">
                  <div class="d-flex flex-row flex-wrap">
                    <v-radio
                      v-for="district in districts"
                      :key="district"
                      :label="district"
                      :value="district"
                      style="width: 50%"
                      class="mx-0"
                    ></v-radio>
                  </div>
                </v-radio-group>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        <v-card-actions class="mt-4 pt-4 border-t px-0">
          <v-spacer></v-spacer>
          <v-btn
            variant="text"
            color="error"
            class="bg-gray-1 w-50"
            @click="resetFilter"
          >
            إعادة تعيين
          </v-btn>
          <v-btn class="bg-primary w-50 text-white" @click="applyFilter">
            تطبيق الفلترة
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<style lang="scss">
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.v-text-field {
  :deep(.v-field) {
    border-radius: 8px;
    background-color: rgb(250, 250, 250);

    &.v-field--focused {
      background-color: white;
    }
  }
}

.v-btn {
  text-transform: none;
  letter-spacing: 0;
  border-radius: 8px;
}

.v-radio-group {
  .v-selection-control-group {
    gap: 8px;
  }
}

.v-selection-control {
  &.v-selection-control--dirty {
    .v-selection-control__input {
      border-color: #c5a572 !important;

      &::before {
        opacity: 1 !important;
        background-color: #c5a572 !important;
        transform: scale(0.55) !important;
      }
    }
  }
}

.v-selection-control__input {
  width: 18px !important;
  height: 18px !important;
  border: 2px solid rgba(190, 194, 197, 1) !important;
  border-radius: 50% !important;
  margin: 0 !important;
  padding: 0 !important;
  opacity: 1 !important;
  background: transparent !important;

  &::before {
    opacity: 0 !important;
    background-color: transparent !important;
    transition: all 0.2s ease-in-out !important;
  }
}

.v-label {
  font-size: 14px;
  color: rgba(0, 0, 0, 0.87);
  margin-inline-start: 4px;
}

// Remove ripple effect
:deep(.v-ripple__container) {
  display: none !important;
}

.bg-gray-1 {
  background-color: #f8f8f8;
  border-radius: 8px;
  padding: 16px;
}

.statistics-bar {
  background-color: #f8f8f8;
  border-radius: 8px;
  margin-top: 24px;
}

.v-dialog {
  .v-card {
    border-radius: 12px;
  }

  label {
    font-size: 14px;
    color: rgba(0, 0, 0, 0.87);
  }

  .v-field {
    border-radius: 8px;
    background-color: rgb(250, 250, 250) !important;

    &.v-field--focused {
      background-color: white !important;
    }
  }

  .v-btn {
    height: 48px;
    border-radius: 8px;
    text-transform: none;
    letter-spacing: 0;
    font-weight: 500;
  }
}

.v-dialog > .v-overlay__content {
  overflow-y: scroll !important;
  padding-inline-end: 0px !important;
}
</style>
