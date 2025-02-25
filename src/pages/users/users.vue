<script setup>
import { useAppStore } from "@/stores/app";
import AppDateTimePicker from "@/@core/components/AppDateTimePicker";
import StatsWidget from "@/@core/components/widgets/StatsWidget";
import Pagination from "@/@core/components/Pagination";
import UserCard from "@/@core/components/widgets/userCard.vue";
import UserWidget from "@/@core/components/widgets/UserWidget.vue";
import { ref, onMounted } from "vue";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const dialog = ref(false);

const loading = ref(false);
const locationDialog = ref(false);
const selectedUser = ref(null);

const users = ref([
  {
    id: "6127782187",
    identityNumber: "231 3456 567",
    location: "الرياض",
    name: "محمد احمد",
    job: "مدير تنفيذي",
  },
  {
    id: "6127782188",
    identityNumber: "231 3456 568",
    location: "جدة",
    name: "احمد محمد",
    job: "مشرف",
  },
  {
    id: "6127782189",
    identityNumber: "231 3456 569",
    location: "الدمام",
    name: "خالد عبدالله",
    job: "موظف",
  },
]);

const handleSubmit = async () => {
  loading.value = true;
  await new Promise((resolve) => setTimeout(resolve, 1000));
  loading.value = false;
  dialog.value = false;
};

const openLocationMap = (user) => {
  selectedUser.value = user;
  locationDialog.value = true;
};

const getMapUrl = (location) => {
  if (!location) return "";
  const query = encodeURIComponent(location);
  return `https://www.google.com/maps/embed/v1/place?key=YOUR_GOOGLE_MAPS_API_KEY&q=${query}&language=ar`;
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
      <div v-if="!isLoading">
        <v-container fluid class="px-0 rounded-lg">
          <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
            <v-card-title class="pt-4 pb-6">
              <div class="d-flex align-center justify-space-between gap-4">
                <h2 class="text-h5 font-weight-bold mb-4">إدارة المستخدمين</h2>
                <div class="d-flex align-center gap-2">
                  <v-text-field
                    v-model="search"
                    label="البحث عن مستخدم... "
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
                    height="48"
                    elevation="0"
                    class="rounded-lg"
                    @click="dialog = true"
                  >
                    إضافة مستخدم جديد
                  </VBtn>
                </div>
              </div>

              <v-row class="mt-6">
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
                    :onClick="() => openLocationMap(user)"
                  />
                </v-col>
              </v-row>

              <!-- pagination -->
              <Pagination
                :total-items="2300"
                v-model:per-page="perPage"
                v-model:page="currentPage"
              />
            </v-card-title>
          </VCard>
        </v-container>
      </div>

      <!-- Add User Modal -->
      <v-dialog v-model="dialog" width="500" class="add-user-dialog">
        <v-card class="rounded-lg">
          <v-card-text class="pa-6">
            <div class="d-flex justify-space-between align-center mb-6">
              <h2 class="text-h6">إضافة مستخدم جديد</h2>
              <v-btn icon variant="text" @click="dialog = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </div>
            <p class="text-subtitle-1 mb-6">
              يرجى إدخال بيانات المستخدم الجديد لإضافته إلى النظام
            </p>

            <v-form @submit.prevent="handleSubmit">
              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block">اسم المستخدم</label>
                <v-text-field
                  placeholder="اسم المستخدم"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  class="rounded-lg"
                />
              </div>

              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block">رقم الهوية</label>
                <v-text-field
                  placeholder="رقم الهوية"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  class="rounded-lg"
                />
              </div>

              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block">رقم الجوال</label>
                <div class="d-flex">
                  <v-text-field
                    placeholder="00"
                    variant="outlined"
                    density="comfortable"
                    hide-details
                    class="rounded-lg flex-grow-1"
                  />
                  <v-select
                    value="+966"
                    :items="['+966', '+964', '+965']"
                    variant="outlined"
                    density="comfortable"
                    hide-details
                    class="country-code-select ms-2 rounded-lg"
                    style="max-width: 110px"
                  />
                </div>
              </div>

              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block"
                  >البريد الإلكتروني</label
                >
                <v-text-field
                  placeholder="البريد الإلكتروني"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  class="rounded-lg"
                />
              </div>

              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block">دور المستخدم</label>
                <v-select
                  placeholder="اختر دور المستخدم"
                  :items="['مدير', 'مشرف', 'موظف']"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  class="rounded-lg"
                />
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

      <!-- Location Map Modal -->
      <v-dialog v-model="locationDialog" width="800">
        <v-card class="rounded-lg">
          <v-card-text class="pa-6">
            <div class="d-flex justify-space-between align-center mb-6">
              <h2 class="text-h6">موقع الموظف</h2>
              <v-btn icon variant="text" @click="locationDialog = false">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </div>
            <div class="map-container" style="height: 400px">
              <iframe
                width="100%"
                height="100%"
                style="border: 0"
                loading="lazy"
                allowfullscreen
                :src="getMapUrl(selectedUser?.location)"
              ></iframe>
            </div>
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

.user-info {
  background-color: #f9f9f9;
  border-radius: 8px;
  padding: 1rem;
}

.cursor-pointer {
  cursor: pointer;
}

.add-user-dialog {
  :deep(.v-card) {
    background-color: white;
  }

  :deep(.v-field) {
    border-radius: 8px !important;
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
}

.map-container {
  border-radius: 8px;
  overflow: hidden;
}
.v-select .v-field {
  border: 1px solid rgba(182, 146, 101, 0.648) !important;
  border-radius: 8px !important;
}
</style>
