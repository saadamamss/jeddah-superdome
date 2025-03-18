<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import questionnaireWidget from "@/@core/components/widgets/questionnaireWidget.vue";
import { ref, onMounted, watch } from "vue";
import axios from "@/plugins/axios";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const questionnaires = ref([]);
const totalItems = ref(0);

// Watch for changes in search, currentPage, or perPage
watch([search, currentPage, perPage], () => {
  fetchQuestionnaires();
});

// Function to fetch questionnaires
const fetchQuestionnaires = async () => {
  try {
    isLoading.value = true;
    const params = new URLSearchParams({
      page: currentPage.value,
      per_page: perPage.value,
      search: search.value,
    });

    const response = await axios.get(`questionnaires?${params.toString()}`);
    console.log("API Response:", response.data);

    if (response.data?.status?.success) {
      questionnaires.value = response.data.data;
      console.log("Questionnaires data:", questionnaires.value);

      // Log the structure of the first questionnaire if available
      if (questionnaires.value.length > 0) {
        console.log(
          "First questionnaire structure:",
          JSON.stringify(questionnaires.value[0], null, 2)
        );
      }

      // Handle pagination data
      if (response.data.pagination) {
        totalItems.value = response.data.pagination.i_total_objects || 0;
        perPage.value = response.data.pagination.i_per_page || perPage.value;
        currentPage.value =
          response.data.pagination.i_current_page || currentPage.value;
      } else {
        totalItems.value =
          response.data.meta?.total || response.data.data.length;
      }
    }
  } catch (error) {
    console.error("Error fetching questionnaires:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

onMounted(() => {
  fetchQuestionnaires();
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
                <h2 class="text-h5 font-weight-bold mb-4">إدارة الاستبيانات</h2>
                <div class="d-flex align-center ga-2">
                  <v-text-field
                    v-model="search"
                    label="البحث عن استبيان..."
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
                </div>
              </div>

              <v-row class="mt-3">
                <v-col
                  v-for="questionnaire in questionnaires"
                  :key="questionnaire.id"
                  cols="12"
                  sm="6"
                  md="4"
                  lg="4"
                  class="pa-3"
                >
                  <questionnaireWidget
                    :id="questionnaire.id"
                    :title="questionnaire.title"
                    :contract="questionnaire.contract"
                    :imtithal_phase="questionnaire.imtithal_phase"
                    :created_at="questionnaire.created_at"
                    :updated_at="questionnaire.updated_at"
                  />
                </v-col>
              </v-row>

              <Pagination
                :total-items="totalItems"
                v-model:per-page="perPage"
                v-model:page="currentPage"
              />
            </v-card-title>
          </VCard>
        </v-container>
      </div>
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

.border {
  border: 1px solid rgba(0, 0, 0, 0.12);
}

.border-b {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}

.ga-2 {
  gap: 0.5rem;
}

.ga-4 {
  gap: 1rem;
}
</style>
