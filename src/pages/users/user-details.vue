<script setup>
import { useAppStore } from "@/stores/app";
import AppDateTimePicker from "@/@core/components/AppDateTimePicker";
import StatsWidget from "@/@core/components/widgets/StatsWidget";
import Pagination from "@/@core/components/Pagination";
import userRatingCard from "@/@core/components/widgets/userRatingCard.vue";
import StatisticsBar from "@/@core/components/widgets/StatisticsBar.vue";
import UserStatisticsCard from "@/@core/components/widgets/UserStatisticsCard.vue";
import UserWidget from "@/@core/components/widgets/UserWidget.vue";
import { ref, onMounted, computed } from "vue";
import { useRoute } from "vue-router";
import BreadcrumbNav from "@/components/BreadcrumbNav.vue";
import { useBreadcrumbs } from "@/composables/useBreadcrumbs";
import axios from "@/plugins/axios";

// Store
const appStore = useAppStore();
const route = useRoute();

// State
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const timeFilter = ref("today");
const statusFilter = ref("all");
const mapDialog = ref(false);
const loading = ref(false);

// Form data
const userForm = ref({
  firstname: "",
  secondname: "",
  thirdname: "",
  lastname: "",
  email: "",
  phone: "",
  city: "",
  blood_type: "",
  gender: "",
  nationality: "",
  sa_id: "",
  role_id: null,
  personal_image: "",
});

// Available roles for selection
const roles = ref([
  { id: 1, title: "ADMIN" },
  { id: 2, title: "SUPERVISOR" },
  { id: 3, title: "INSPECTOR" },
]);

// Blood types for selection
const bloodTypes = ["A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-"];

// Data
const userLocation = ref({ lat: 24.7136, lng: 46.6753 }); // Default to Riyadh coordinates
const userData = ref(null);
const userStatistics = ref(null);

const links = ref([
  { title: "إدارة المواقع", href: "/", disabled: false },
  { title: "تفاصيل للسكن", href: "/sites", disabled: false },
  { title: "شركة رواف منى", href: "/sites/company-details", disabled: false },
  { title: "التقييمات", href: "/sites/company-details", disabled: true },
]);

const sites = ref([
  {
    siteId: "6127782187",
    authorizationNumber: "231 3456 567",
    status: "معتمد",
  },
  {
    siteId: "6127782188",
    authorizationNumber: "231 3456 568",
    status: "مرفوض",
  },
  {
    siteId: "6127782189",
    authorizationNumber: "231 3456 569",
    status: "مرفوض",
  },
  {
    siteId: "6127782190",
    authorizationNumber: "231 3456 570",
    status: "معتمد",
  },
]);

const userInfo = ref({
  name: "محمد علي",
  email: "mohamed@gmail.com",
  phone: "+966555555555",
  image: "https://cdn.vuetifyjs.com/images/lists/5.jpg",
});

const { generateBreadcrumbItems } = useBreadcrumbs();

const breadcrumbItems = computed(() =>
  generateBreadcrumbItems([
    {
      title: "المستخدمين",
      href: "/users",
      disabled: false,
    },
    {
      title: "تفاصيل المستخدم",
      href: route.path,
      disabled: true,
    },
  ])
);

// Methods
const openMapDialog = () => {
  mapDialog.value = true;
};

const getMapUrl = (location) => {
  return `https://www.google.com/maps/embed/v1/place?key=YOUR_GOOGLE_MAPS_API_KEY&q=${location.lat},${location.lng}&zoom=15`;
};

const fetchUserData = async () => {
  try {
    isLoading.value = true;
    const response = await axios.get(`users/${route.params.id}`);
    if (response.data?.data) {
      userData.value = response.data.data.user;
      userStatistics.value = response.data.data.statistics;
      populateForm(response.data.data.user);
    }
  } catch (error) {
    console.error("Error fetching user data:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء جلب بيانات المستخدم",
      color: "error",
    });
    userData.value = null;
    userStatistics.value = null;
  } finally {
    isLoading.value = false;
  }
};

const populateForm = (user) => {
  userForm.value = {
    firstname: user.firstname || "",
    secondname: user.secondname || "",
    thirdname: user.thirdname || "",
    lastname: user.lastname || "",
    email: user.email || "",
    phone: user.phone || "",
    city: user.city || "",
    blood_type: user.blood_type || "",
    gender: user.gender || "",
    nationality: user.nationality || "",
    sa_id: user.sa_id || "",
    role_id: user.role?.id || null,
    personal_image: user.personal_image || "",
  };
};

const updateUser = async () => {
  try {
    loading.value = true;
    const response = await axios.put(
      `users/${route.params.id}`,
      userForm.value
    );
    if (response.data) {
      appStore.showSnackbar({
        message: "تم تحديث بيانات المستخدم بنجاح",
        color: "success",
      });
      await fetchUserData();
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

// Lifecycle
onMounted(() => {
  fetchUserData();
});
</script>

<template>
  <div>
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
      <v-container fluid class="mt-n4 rounded-lg">
        <BreadcrumbNav :items="breadcrumbItems" />

        <VCard class="mb-4 shadow-0">
          <div class="pa-4 pb-0">
            <user-widget noPadding noShadow :user="userData" />
          </div>

          <!-- User Form -->
          <v-form @submit.prevent="updateUser" class="pa-6">
            <v-row>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الأول</label>
                <v-text-field
                  v-model="userForm.firstname"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الثاني</label>
                <v-text-field v-model="userForm.secondname"></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الثالث</label>
                <v-text-field v-model="userForm.thirdname"></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">اسم العائلة</label>
                <v-text-field
                  v-model="userForm.lastname"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">البريد الإلكتروني</label>
                <v-text-field
                  v-model="userForm.email"
                  type="email"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">رقم الهاتف</label>
                <v-text-field v-model="userForm.phone" required></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الدور</label>
                <v-select
                  v-model="userForm.role_id"
                  :items="roles"
                  item-title="title"
                  item-value="id"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">المدينة</label>
                <v-text-field v-model="userForm.city" required></v-text-field>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">فصيلة الدم</label>
                <v-select
                  v-model="userForm.blood_type"
                  :items="bloodTypes"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الجنس</label>
                <v-select
                  v-model="userForm.gender"
                  :items="['male', 'female']"
                  required
                ></v-select>
              </v-col>
              <v-col cols="12">
                <label class="mb-2 d-block">الجنسية</label>
                <v-text-field
                  v-model="userForm.nationality"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="12">
                <label class="mb-2 d-block">رقم الهوية</label>
                <v-text-field v-model="userForm.sa_id" required></v-text-field>
              </v-col>
            </v-row>

            <div class="d-flex justify-end mt-4">
              <v-btn color="primary" type="submit" :loading="loading">
                حفظ التغييرات
              </v-btn>
            </div>
          </v-form>

          <div class="statistics-bar rounded-lg pa-6">
            <v-row>
              <v-col cols="12" md="3">
                <UserStatisticsCard
                  title="عدد أيام الحضور"
                  :number="userStatistics?.attendance_days_count || 0"
                >
                  <template #icon>
                    <svg
                      width="20"
                      height="20"
                      viewBox="0 0 20 20"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <g opacity="0.4">
                        <path
                          d="M8.12488 1.875C5.93875 1.875 4.16654 3.64721 4.16654 5.83333C4.16654 8.01946 5.93875 9.79167 8.12488 9.79167C10.311 9.79167 12.0832 8.01946 12.0832 5.83333C12.0832 3.64721 10.311 1.875 8.12488 1.875Z"
                          fill="#B69265"
                        />
                      </g>
                    </svg>
                  </template>
                </UserStatisticsCard>
              </v-col>
              <v-col cols="12" md="3">
                <UserStatisticsCard
                  title="عدد التقارير"
                  :number="userStatistics?.reports_count || 0"
                >
                  <template #icon>
                    <svg
                      width="20"
                      height="20"
                      viewBox="0 0 20 20"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <g opacity="0.4">
                        <path
                          d="M8.12488 1.875C5.93875 1.875 4.16654 3.64721 4.16654 5.83333C4.16654 8.01946 5.93875 9.79167 8.12488 9.79167C10.311 9.79167 12.0832 8.01946 12.0832 5.83333C12.0832 3.64721 10.311 1.875 8.12488 1.875Z"
                          fill="#B69265"
                        />
                      </g>
                    </svg>
                  </template>
                </UserStatisticsCard>
              </v-col>
              <v-col cols="12" md="3">
                <UserStatisticsCard
                  title="التقييمات الجديدة"
                  :number="userStatistics?.new_ratings || 0"
                >
                  <template #icon>
                    <svg
                      width="20"
                      height="20"
                      viewBox="0 0 20 20"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <g opacity="0.4">
                        <path
                          d="M8.12488 1.875C5.93875 1.875 4.16654 3.64721 4.16654 5.83333C4.16654 8.01946 5.93875 9.79167 8.12488 9.79167C10.311 9.79167 12.0832 8.01946 12.0832 5.83333C12.0832 3.64721 10.311 1.875 8.12488 1.875Z"
                          fill="#B69265"
                        />
                      </g>
                    </svg>
                  </template>
                </UserStatisticsCard>
              </v-col>
              <v-col cols="12" md="3">
                <UserStatisticsCard
                  title="التقييمات المكتملة"
                  :number="userStatistics?.completed_ratings || 0"
                >
                  <template #icon>
                    <svg
                      width="20"
                      height="20"
                      viewBox="0 0 20 20"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <g opacity="0.4">
                        <path
                          d="M8.12488 1.875C5.93875 1.875 4.16654 3.64721 4.16654 5.83333C4.16654 8.01946 5.93875 9.79167 8.12488 9.79167C10.311 9.79167 12.0832 8.01946 12.0832 5.83333C12.0832 3.64721 10.311 1.875 8.12488 1.875Z"
                          fill="#B69265"
                        />
                      </g>
                    </svg>
                  </template>
                </UserStatisticsCard>
              </v-col>
            </v-row>
          </div>
        </VCard>
      </v-container>
    </div>
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
</style>
