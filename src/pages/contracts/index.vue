<script setup>
import { useAppStore } from "@/stores/app";
import Pagination from "@/@core/components/Pagination";
import contractWidget from "@/@core/components/widgets/contractWidget.vue";
import axios from "@/plugins/axios";
import { hasPermission } from "@core/utils/permissions";

const appStore = useAppStore();
const isLoading = ref(true);
const perPage = ref(6);
const currentPage = ref(1);
const search = ref("");
const dialog = ref(false);
const dialogStep = ref(1);
const loading = ref(false);
const siteType = ref("السكن");
const contracts = ref([]);
const totalItems = ref(0);

const quoteForm = ref({
  name: "",
});

// Add handler for search clear
const handleSearchClear = () => {
  search.value = null;
  fetchContracts();
};

// Watch for changes in search or perPage to reset to page 1
watch(
  [search, perPage],
  () => {
    currentPage.value = 1;
    fetchContracts();
  },
  { flush: "post" }
);

// Watch for changes in currentPage only
watch(
  currentPage,
  () => {
    fetchContracts();
  },
  { flush: "post" }
);

// Function to fetch contracts
const fetchContracts = async () => {
  try {
    isLoading.value = true;
    const params = new URLSearchParams();
    params.append("page", currentPage.value);
    params.append("per_page", perPage.value);
    if (search.value) {
      params.append("search", search.value);
    }

    const response = await axios.get(`contracts?${params.toString()}`);

    if (response.data?.status?.success) {
      contracts.value = response.data.data;
      // Update pagination info from API response
      if (response.data.pagination) {
        totalItems.value = response.data.pagination.i_total_objects;
        // Sync local state with server pagination if needed
        if (currentPage.value !== response.data.pagination.i_current_page) {
          currentPage.value = response.data.pagination.i_current_page;
        }
        if (perPage.value !== response.data.pagination.i_per_page) {
          perPage.value = response.data.pagination.i_per_page;
        }
      }
    }
  } catch (error) {
    console.error("Error fetching contracts:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء جلب البيانات",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

const addNewQuote = () => {
  dialog.value = true;
};

// Function to handle form submission
const handleSubmit = async () => {
  try {
    loading.value = true;
    const response = await axios.post("contracts", quoteForm.value);

    if (response.data?.status?.success) {
      appStore.showSnackbar({
        message: "تمت إضافة الباقة بنجاح",
        color: "success",
      });
      dialog.value = false;
      // Reset form
      quoteForm.value = {
        name: "",
      };
      // Refresh contracts list
      fetchContracts();
    }
  } catch (error) {
    console.error("Error adding contract:", error);
    appStore.showSnackbar({
      message: error.response?.data?.message || "حدث خطأ أثناء إضافة الباقة",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchContracts();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
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
              title: '⁠⁠إدارة الباقات (التعاقدات)',
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
          <div
            class="d-flex align-center justify-space-between ga-4 border-b pb-4"
          >
            <h2 class="text-h5 font-weight-bold mb-4">
              إدارة الباقات (التعاقدات)
            </h2>
            <div class="d-flex align-center ga-2">
              <v-text-field
                v-model="search"
                label="البحث عن باقة..."
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
                @click:clear="handleSearchClear"
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
                v-if="hasPermission('contracts.view')"
                height="48"
                elevation="0"
                class="rounded-lg"
                append-icon="mdi-plus"
                @click="addNewQuote"
              >
                إضافة باقة جديدة
              </VBtn>
            </div>
          </div>

          <v-row class="mt-3">
            <template v-if="isLoading">
              <v-col
                v-for="n in 6"
                :key="n"
                cols="12"
                sm="6"
                md="4"
                lg="4"
                class="pa-3"
              >
                <v-card elevation="0" class="rounded-lg" height="140">
                  <v-card-text class="pa-3 pa-sm-4">
                    <v-skeleton-loader type="article"></v-skeleton-loader>
                  </v-card-text>
                </v-card>
              </v-col>
            </template>
            <template v-else>
              <v-col
                v-for="contract in contracts"
                :key="contract.id"
                cols="12"
                sm="6"
                md="4"
                lg="4"
                class="pa-3"
              >
                <contractWidget
                  v-bind="contract"
                  :id="contract.id"
                  :title="contract.name"
                  :locations_count="contract.locations_count"
                />
              </v-col>
            </template>
          </v-row>

          <Pagination
            :total-items="totalItems"
            v-model:per-page="perPage"
            v-model:page="currentPage"
            :loading="isLoading"
          />
        </v-card-title>
      </VCard>

      <!-- Add Contract Modal -->
      <v-dialog v-model="dialog" width="500" class="add-team-dialog">
        <v-card class="rounded-lg">
          <v-card-text class="pa-4">
            <div class="border-b mb-4">
              <div class="d-flex justify-space-between align-center">
                <h2 class="text-h6 font-weight-bold">إضافة باقة جديدة</h2>
              </div>
              <p class="text-subtitle-1 text-medium-emphasis mb-2">
                حدد تفاصيل الباقة جديدة
              </p>
            </div>

            <v-form @submit.prevent="handleSubmit">
              <div class="mb-4">
                <label class="text-subtitle-2 mb-2 d-block">اسم الباقة</label>
                <v-text-field
                  v-model="quoteForm.name"
                  variant="outlined"
                  density="comfortable"
                  placeholder="اسم الباقة"
                  hide-details
                  class="rounded-lg bg-grey-lighten-4 px-2"
                  required
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
