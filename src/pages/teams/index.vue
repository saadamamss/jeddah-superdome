<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import TeamWidget from "@/@core/components/widgets/TeamWidget.vue";
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

const formModel = ref({
  location: null,
  fieldSupervisor: null,
  fieldTeam: [],
});

const locations = [
  { title: "موقع 1", value: "location1" },
  { title: "موقع 2", value: "location2" },
  { title: "موقع 3", value: "location3" },
];

const supervisors = [
  { title: "أحمد محمد", value: "ahmed" },
  { title: "محمد علي", value: "mohammed" },
  { title: "عبدالله خالد", value: "abdullah" },
];

const fieldTeamMembers = [
  {
    title: "علي الحسين",
    value: "ali",
    avatar: "https://i.pravatar.cc/150?img=1",
  },
  {
    title: "سارة نصر",
    value: "sara",
    avatar: "https://i.pravatar.cc/150?img=2",
  },
  {
    title: "أحمد حسين",
    value: "ahmed",
    avatar: "https://i.pravatar.cc/150?img=3",
  },
  {
    title: "عبدالرحمن صالح",
    value: "abdulrahman",
    avatar: "https://i.pravatar.cc/150?img=4",
  },
];

const siteTypes = [
  { title: "موقع سكني", value: "residential" },
  { title: "موقع تجاري", value: "commercial" },
  { title: "موقع صناعي", value: "industrial" },
];

const companies = [
  { title: "شركة رواف منى", value: "rawaf" },
  { title: "شركة الخليج", value: "gulf" },
];

const teams = ref([
  {
    teamId: "team-01",
    location: "موقع 1",
    teamAvatars: [
      { name: "محمد أحمد", avatar: "https://i.pravatar.cc/150?img=1" },
      { name: "فاطمة علي", avatar: "https://i.pravatar.cc/150?img=2" },
      { name: "عبدالله محمد", avatar: "https://i.pravatar.cc/150?img=3" },
      { name: "سارة خالد", avatar: "https://i.pravatar.cc/150?img=4" },
      { name: "عمر يوسف", avatar: "https://i.pravatar.cc/150?img=5" },
    ],
    supervisor: "أحمد محمد",
  },
  {
    teamId: "team-02",
    location: "موقع 2",
    teamAvatars: [
      { name: "ليلى حسن", avatar: "https://i.pravatar.cc/150?img=6" },
      { name: "أحمد علي", avatar: "https://i.pravatar.cc/150?img=7" },
      { name: "نورة سعد", avatar: "https://i.pravatar.cc/150?img=8" },
    ],
    supervisor: "محمد علي",
  },
  {
    teamId: "team-03",
    location: "موقع 3",
    teamAvatars: [
      { name: "خالد عمر", avatar: "https://i.pravatar.cc/150?img=9" },
      { name: "ريم ناصر", avatar: "https://i.pravatar.cc/150?img=10" },
      { name: "طارق سعيد", avatar: "https://i.pravatar.cc/150?img=11" },
      { name: "منى فهد", avatar: "https://i.pravatar.cc/150?img=12" },
    ],
    supervisor: "عبدالله خالد",
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

const addNewTeam = () => {
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
        <v-container fluid class="mt-٠ px-0 rounded-lg">
          <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
            <v-card-title class="pt-4 pb-6">
              <div
                class="d-flex align-center justify-space-between ga-4 border-b pb-4"
              >
                <h2 class="text-h5 font-weight-bold mb-4">
                  إدارة الفرق الميدانية
                </h2>
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

                  <VBtn color="default" class="btn-icon border" elevation="0">
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
                    </svg>
                  </VBtn>

                  <VBtn
                    height="48"
                    elevation="0"
                    class="rounded-lg"
                    append-icon="mdi-plus"
                    @click="addNewTeam"
                  >
                    إضافة فريق جديد
                  </VBtn>
                </div>
              </div>

              <v-row class="mt-3">
                <v-col
                  v-for="team in teams"
                  :key="team.teamId"
                  cols="12"
                  sm="6"
                  md="4"
                  lg="4"
                  class="pa-3"
                >
                  <TeamWidget
                    :title="team.location"
                    :team-members="team.teamAvatars"
                    :supervisor="team.supervisor"
                  />
                </v-col>
              </v-row>

              <Pagination
                :total-items="2300"
                v-model:per-page="perPage"
                v-model:page="currentPage"
              />
            </v-card-title>
          </VCard>
        </v-container>
      </div>

      <!-- Add Team Modal -->
      <v-dialog v-model="dialog" width="500" class="add-team-dialog">
        <v-card class="rounded-lg">
          <v-card-text class="pa-4">
            <div class="border-b mb-4">
              <div class="d-flex justify-space-between align-center">
                <h2 class="text-h6 font-weight-bold">إدارة الفرق الميدانية</h2>
              </div>
              <p class="text-subtitle-1 text-medium-emphasis mb-2">
                حدد تفاصيل الفرق الميدانية
              </p>
            </div>

            <v-form @submit.prevent="handleSubmit">
              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block">المواقع</label>
                <v-select
                  v-model="formModel.location"
                  :items="locations"
                  variant="outlined"
                  density="comfortable"
                  placeholder="اختر الموقع"
                  hide-details
                  class="rounded-lg bg-grey-lighten-4 px-2"
                />
              </div>

              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block"
                  >المشرف الميداني</label
                >
                <v-select
                  v-model="formModel.fieldSupervisor"
                  :items="supervisors"
                  variant="outlined"
                  density="comfortable"
                  placeholder="اختر المشرف"
                  hide-details
                  class="rounded-lg bg-grey-lighten-4 px-2"
                />
              </div>

              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block"
                  >فريق المراقبين الميدانيون</label
                >
                <v-select
                  v-model="formModel.fieldTeam"
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
                        <v-img :src="item.raw.avatar" :alt="item.title"></v-img>
                      </v-avatar>
                      <span class="text-subtitle-2">{{ item.title }}</span>
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

              <div class="d-flex justify-end mt-6">
                <v-btn
                  variant="text"
                  color="grey-darken-1"
                  class="me-4"
                  @click="dialog = false"
                >
                  إلغاء
                </v-btn>
                <v-btn
                  color="#B69265"
                  class="text-white"
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
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
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

.add-team-dialog {
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
