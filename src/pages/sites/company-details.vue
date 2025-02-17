<script setup>
import { useAppStore } from "@/stores/app";
import AppDateTimePicker from "@/@core/components/AppDateTimePicker";
import StatsWidget from "@/@core/components/widgets/StatsWidget";
import Pagination from "@/@core/components/Pagination";
import ratingCard from "@/@core/components/widgets/ratingCard.vue";
import StatisticsBar from "@/@core/components/widgets/StatisticsBar.vue";
import { ref } from "vue";
const appStore = useAppStore();
const filterToggle = ref(0);
const dateRange = ref();
const isLoading = ref(true);
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
const timeFilter = ref("today");
const statusFilter = ref("all");

const siteTypes = [
  { title: "موقع سكني", value: "residential" },
  { title: "موقع تجاري", value: "commercial" },
  { title: "موقع صناعي", value: "industrial" },
];

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
  {
    siteId: "6127782189",
    authorizationNumber: "231 3456 569",
  },
  {
    siteId: "6127782190",
    authorizationNumber: "231 3456 570",
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
  loading.value = true;
  // Add your submit logic here
  await new Promise((resolve) => setTimeout(resolve, 1000));
  loading.value = false;
  dialog.value = false;
  dialogStep.value = 1;
};

const nextStep = () => {
  if (siteType.value) {
    dialogStep.value = 2;
  }
};

const prevStep = () => {
  dialogStep.value = 1;
};

const closeDialog = () => {
  dialog.value = false;
  dialogStep.value = 1;
};

const addNewLocation = () => {
  dialog.value = true;
};

onMounted(() => {
  setTimeout(() => {
    isLoading.value = false;
  }, 100);
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
      <StatisticsBar title="شركة رواف منى" />
      <v-container fluid class="mt-6 px-0 rounded-lg">
        <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
          <!-- Header Section -->
          <v-card-title
            class="d-flex align-center justify-space-between ga-4 pt-4"
          >
            <h2 class="text-h5 font-weight-bold">التقييمات</h2>
            <div class="d-flex align-center gap-3">
              <v-select
                v-model="timeFilter"
                :items="[
                  { title: 'اليوم', value: 'today' },
                  { title: 'الأسبوع', value: 'week' },
                  { title: 'الشهر', value: 'month' },
                  { title: 'السنة', value: 'year' },
                ]"
                variant="outlined"
                density="comfortable"
                hide-details
                class="min-width-150px rounded-lg bg-grey-lighten-4"
              />
              <v-select
                v-model="statusFilter"
                :items="[
                  { title: 'الكل', value: 'all' },
                  { title: 'مقبول', value: 'accepted' },
                  { title: 'مرفوض', value: 'rejected' },
                  { title: 'معلق', value: 'pending' },
                ]"
                variant="outlined"
                density="comfortable"
                hide-details
                class="min-width-150px rounded-lg bg-grey-lighten-4"
              />
            </div>
          </v-card-title>
          <!-- Search and Add Section -->

          <!-- Tabs Section -->

          <v-row class="mt-6">
            <v-col
              v-for="site in sites"
              :key="site.siteId"
              cols="12"
              sm="6"
              md="4"
              lg="4"
              class="pa-3"
            >
              <ratingCard />
            </v-col>
          </v-row>

          <Pagination
            title="الفرق الميدانية"
            :total-items="2300"
            v-model:per-page="perPage"
            v-model:page="currentPage"
          />
        </VCard>
      </v-container>
    </div>

    <!-- Add Site Modal -->
    <v-dialog v-model="dialog" width="500" class="add-site-dialog">
      <v-card class="rounded-lg">
        <v-card-text class="pa-4">
          <h2 class="text-h6 font-weight-bold">
            {{ dialogStep === 1 ? "اختر نوع الموقع" : "إضافة موقع جديد" }}
          </h2>
          <p class="card-gray-text border-b pb-3">حدد تفاصيل الموقع الجديد</p>

          <!-- Step 1: Select Site Type -->
          <v-form v-if="dialogStep === 1" @submit.prevent="nextStep">
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">نوع الموقع</label>
              <v-select
                v-model="siteType"
                :items="siteTypes"
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
          <v-form v-else @submit.prevent="handleSubmit">
            <!-- Housing Permit Number -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block"
                >رقم تصريح السكن</label
              >
              <v-text-field
                v-model="housingPermitNumber"
                variant="outlined"
                density="comfortable"
                placeholder="رقم تصريح السكن"
                hide-details
                class="rounded-lg bg-grey-lighten-4"
                required
              />
            </div>

            <!-- Authorized Contact Number -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block"
                >رقم تواصل المفوض</label
              >
              <div class="d-flex">
                <v-text-field
                  v-model="authorizedContactNumber"
                  variant="outlined"
                  density="comfortable"
                  placeholder="00"
                  hide-details
                  class="rounded-lg bg-grey-lighten-4 flex-grow-1"
                  required
                />
                <v-select
                  value="+966"
                  :items="[
                    { title: '+966', value: '+966' },
                    { title: '+964', value: '+964' },
                    { title: '+965', value: '+965' },
                  ]"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  class="country-code-select ms-2 rounded-lg bg-grey-lighten-4"
                  style="max-width: 110px"
                />
              </div>
            </div>

            <!-- Companies -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">الشركات</label>
              <v-select
                v-model="selectedCompany"
                :items="companies"
                variant="outlined"
                density="comfortable"
                placeholder="اختر الشركات"
                hide-details
                class="rounded-lg bg-grey-lighten-4"
                required
              />
            </div>

            <!-- Map -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">الاحداثيات</label>
              <div
                class="map-container rounded-lg overflow-hidden"
                style="height: 100px"
              >
                map
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
                إضافة
              </v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>
    <!-- add new location -->
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
</style>
