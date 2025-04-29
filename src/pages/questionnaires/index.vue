<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import questionnaireWidget from "@/@core/components/widgets/questionnaireWidget.vue";
import CustomVSelect from "@core/components/global/CustomVSelect.vue";
import CustomVAutocomplete from "@core/components/global/CustomVAutocomplete.vue";
import axios from "@/plugins/axios";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const questionnaires = ref([]);
const totalItems = ref(0);

const isExporting = ref(false);
const isGettingFilters = ref(false);
const phases = ref([]);
const contracts = ref([]);

const filters = reactive({
  search: "",
  imtithal_phase_id: undefined,
  contract_id: undefined,
});

// Watch for changes in search, currentPage, or perPage
watch([search, currentPage, perPage], () => {
  fetchQuestionnaires();
});

watch(
  () => [filters.imtithal_phase_id, filters.contract_id],
  () => {
    fetchQuestionnaires();
  }
);

// Function to fetch questionnaires
const fetchQuestionnaires = async () => {
  try {
    isLoading.value = true;
    // const params = new URLSearchParams({
    //   page: currentPage.value,
    //   per_page: perPage.value,
    //   search: search.value,
    // });

    const response = await axios.get("questionnaires", {
      params: {
        page: currentPage.value,
        per_page: perPage.value,
        ...filters,
      },
    });

    if (response.data?.status?.success) {
      questionnaires.value = response.data.data;

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

const debouncedSearch = ref(null);
watch(
  () => filters.search,
  (newValue) => {
    if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
    debouncedSearch.value = setTimeout(() => {
      filters.page = 1; // Reset to first page when searching
      fetchQuestionnaires();
    }, 500); // 500ms debounce
  }
);

const exportData = async () => {
  return;
  try {
    isExporting.value = true;

    const response = await axios.post(
      "/ratings-export-excel",
      {},
      {
        responseType: "blob", // Important to receive binary data
      }
    );

    const url = window.URL.createObjectURL(new Blob([response.data]));

    const link = document.createElement("a");
    link.href = url;

    // Optional: Get file name from headers
    const disposition = response.headers["content-disposition"];
    let fileName = "التقييمات.xlsx";
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

const getFiltersUtilities = async () => {
  try {
    isGettingFilters.value = true;
    const phasesResponse = await axios.get("/phases?page=-1");
    phases.value = phasesResponse?.data?.data || [];

    const contractsResponse = await axios.get("/contracts?page=-1");
    contracts.value = contractsResponse?.data?.data || [];
  } catch (error) {
    console.error("Error fetching ratings:", error);
  } finally {
    isGettingFilters.value = false;
  }
};

onMounted(() => {
  fetchQuestionnaires();
  getFiltersUtilities();
});
</script>

<template>
  <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2 pt-0">
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
            title: 'إدارة الاستبيانات',
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

    <div>
      <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
        <v-card-title class="pt-4 pb-6">
          <div
            class="d-flex align-center justify-space-between ga-4 border-b pb-4"
          >
            <h2 class="text-h5 font-weight-bold mb-4">إدارة الاستبيانات</h2>
            <div class="d-flex align-center ga-4 flex-wrap">
              <v-text-field
                v-model="filters.search"
                label="البحث"
                rounded="lg"
                clearable
                height="46"
                class="bg-white"
                style="
                  width: 280px;
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

              <CustomVSelect
                v-model="filters.imtithal_phase_id"
                :items="phases"
                min-width="140"
                max-width="200"
                :return-object="false"
                placeholder="المرحلة"
                :loading="isGettingFilters"
                item-title="title"
                item-value="id"
              />

              <CustomVAutocomplete
                v-model="filters.contract_id"
                :items="contracts"
                min-width="140"
                max-width="200"
                :return-object="false"
                placeholder="الباقة"
                :loading="isGettingFilters"
                item-title="name"
                item-value="id"
              />

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
            </div>
          </div>
          <v-row v-if="isLoading">
            <v-col v-for="item in 9" :key="item" cols="6" sm="6" lg="4">
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
          <v-row v-else class="mt-3">
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
                :questionnaire="questionnaire"
                :id="questionnaire.id"
                :title="questionnaire.title"
                :contract="questionnaire.contract"
                :imtithal_phase="questionnaire.imtithal_phase"
                :created_at="questionnaire.created_at"
                :updated_at="questionnaire.updated_at"
                class="h-100"
              />
            </v-col>
          </v-row>

          <Pagination
            v-if="!isLoading && totalItems"
            :total-items="totalItems"
            v-model:per-page="perPage"
            v-model:page="currentPage"
          />
        </v-card-title>
      </VCard>
    </div>
  </v-container>
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
