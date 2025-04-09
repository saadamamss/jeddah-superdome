<script setup>
// pagination added
import { ref, watch, onMounted } from "vue";
import axios from "@/plugins/axios";
import Pagination from "@/@core/components/Pagination";
import companyCard from "@/@core/components/widgets/companyCard.vue";
import StatisticsBar from "@/@core/components/widgets/StatisticsBar.vue";
import CustomVSelect from "@core/components/global/CustomVSelect.vue";
import { format } from "@core/utils/formatters";
import { printElement } from "@core/utils/helpers";

const router = useRouter();
const route = useRoute();

const isLoading = ref(true);
const isExporting = ref(false);
const isGettingFilters = ref(false);
const ratings = ref([]);
const phases = ref([]);
const contracts = ref([]);

const isDatepickerMenu = ref(false);
const filters = reactive({
  location_type_id: "1",
  search: "",
  status: undefined,
  created_at: undefined,
  imtithal_phase_id: undefined,
  contract_id: undefined,
});

const pagination = reactive({
  per_page: 12,
  page: 1,
  total: 0,
  items_on_page: 0,
});

const onClearDateFilter = () => {
  filters.created_at = undefined;
};

const fetchRatings = async () => {
  try {
    isLoading.value = true;
    const response = await axios.get("/ratings", {
      params: {
        page: pagination.page,
        per_page: pagination.per_page,
        ...filters,
        created_at: filters.created_at
          ? format(filters.created_at, "YYYY-MM-DD")
          : null,
      },
    });
    ratings.value = response.data?.data || [];
    const ratingsPagination = response.data?.pagination || {};

    pagination.page = ratingsPagination.i_current_page;
    pagination.per_page = ratingsPagination.i_per_page;
    pagination.total = ratingsPagination.i_total_objects;
    pagination.items_on_page = ratingsPagination.items_on_page;
  } catch (error) {
    console.error("Error fetching ratings:", error);
  } finally {
    isLoading.value = false;
  }
};

watch(
  () => [
    filters.location_type_id,
    filters.status,
    filters.created_at,
    filters.imtithal_phase_id,
    filters.contract_id,
    pagination.page,
    pagination.per_page,
  ],
  () => {
    fetchRatings();
  }
);

watch(
  () => [filters.location_type_id],
  (locationType) => {
    router.push({ query: { ...route.query, location_type: locationType } });
  }
);

const debouncedSearch = ref(null);
watch(
  () => filters.search,
  (newValue) => {
    if (debouncedSearch.value) clearTimeout(debouncedSearch.value);
    debouncedSearch.value = setTimeout(() => {
      filters.page = 1; // Reset to first page when searching
      fetchRatings();
    }, 500); // 500ms debounce
  }
);

const exportData = async () => {
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
    const phasesResponse = await axios.get("/reports?page=-1");
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
  if (route.query.location_type) {
    filters.location_type_id = route.query.location_type;
  }

  fetchRatings();
  getFiltersUtilities();
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
              title: '⁠⁠إدارة التقييمات',
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
      <v-card elevation="0" class="rounded-lg px-4 bg-light-white border-0">
        <!-- Header Section -->
        <v-card-title class="pt-4 pb-6">
          <div class="d-flex align-center justify-space-between ga-4 flex-wrap">
            <h2 class="text-h5 font-weight-bold mb-4">التقييمات</h2>
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

              <v-menu
                v-model="isDatepickerMenu"
                :close-on-content-click="false"
                transition="scale-transition"
                offset-y
                min-width="auto"
              >
                <template v-slot:activator="{ props }">
                  <v-text-field
                    :value="format(filters.created_at, 'YYYY-MM-DD')"
                    placeholder="التاريخ"
                    v-bind="props"
                    min-width="180"
                    height="50"
                    hide-details
                    hide-no-data
                    variant="filled"
                    bg-color="#F8F9F9"
                    class="v-filter-input"
                    clearable
                    @click:clear="onClearDateFilter"
                  ></v-text-field>
                  <!-- prepend-inner-icon="mdi-calendar" -->
                </template>

                <v-date-picker
                  v-model="filters.created_at"
                  locale="ar"
                  @update:modelValue="isDatepickerMenu = false"
                  hide-header
                  clearable
                ></v-date-picker>
              </v-menu>

              <CustomVSelect
                v-model="filters.imtithal_phase_id"
                :items="phases"
                min-width="140"
                max-width="200"
                :return-object="false"
                placeholder="المرحلة"
                :loading="isGettingFilters"
                item-title="id"
                item-value="id"
              />

              <CustomVSelect
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

              <CustomVSelect
                v-model="filters.status"
                :items="[
                  {
                    label: 'قيد المراجعة',
                    value: 'PENDING',
                  },
                  {
                    label: 'معتمد',
                    value: 'APPROVED',
                  },
                  {
                    label: 'مرفوض',
                    value: 'REJECTED',
                  },
                ]"
                min-width="140"
                max-width="200"
                :return-object="false"
                placeholder="الحالة"
                :loading="isGettingFilters"
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
          <v-tabs
            v-model="filters.location_type_id"
            bg-color="transparent"
            class="mt-6"
          >
            <v-tab
              value="1"
              class="rounded-lg border-0 font-weight"
              :class="{
                'text-primary': filters.location_type_id == '1',
              }"
              :style="`background-color: ${
                filters.location_type_id == '1' ? '#F8F9F9' : ''
              }`"
            >
              المخيمات
            </v-tab>
            <v-tab
              value="2"
              class="rounded-lg border-0 font-weight"
              :class="{
                'text-primary': filters.location_type_id == '2',
              }"
              :style="`background-color: ${
                filters.location_type_id == '2' ? '#F8F9F9' : ''
              }`"
            >
              المساكن
            </v-tab>
          </v-tabs>
          <v-divider class="mt-4" />
        </v-card-title>

        <v-card-text class="px-2 px-sm-4 mt-2">
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
          <v-row v-else>
            <v-col
              v-for="rating in ratings"
              :key="rating.id"
              cols="12"
              md="6"
              class="pa-3"
            >
              <v-card
                :id="`rating_${rating.id}`"
                class="h-100 pa-4 pa-sm-6 rounded-lg"
              >
                <v-card-title class="pa-0">
                  <div class="d-flex justify-space-between align-start ga-2">
                    <div>
                      <h2 class="text-body-1 mb-1">
                        <router-link
                          :to="`/rating/details/${rating.id}`"
                          class="font-weight-bold d-flex align-center ga-2 text-dark"
                        >
                          <span>التقييم</span>
                          <span dir="ltr" class="d-flex"
                            ><span> #</span><span>{{ rating?.id }}</span></span
                          >
                        </router-link>
                      </h2>
                      <span
                        class="d-flex align-center ga-2"
                        style="color: #75797c"
                      >
                        <small class="d-flex align-center ga-1">
                          <small> {{ format(rating.created_at, "D") }}</small>
                          <small> {{ format(rating.created_at, "MMM") }}</small>
                          <small>{{ format(rating.created_at, "YYYY") }}</small>
                        </small>
                        <small>-</small>
                        <small class="d-flex align-center ga-1">
                          <div class="d-flex align-center">
                            <small>
                              {{ format(rating.created_at, "mm") }}</small
                            >
                            <small>:</small>
                            <small> {{ format(rating.created_at, "h") }}</small>
                          </div>
                          <small>{{ format(rating.created_at, "A") }}</small>
                        </small>
                      </span>
                    </div>
                    <div class="d-flex align-center gap-3">
                      <VBtn
                        variant="outlined"
                        icon
                        elevation="0"
                        class="bg-white no-print"
                        size="small"
                      >
                        <img
                          width="22"
                          src="/imgs/icons/pin-map.svg"
                          alt="print icon"
                        />
                      </VBtn>
                      <VBtn
                        variant="outlined"
                        icon
                        elevation="0"
                        class="bg-white no-print"
                        size="small"
                        @click="
                          printElement(
                            `rating_${rating.id}`,
                            `تقييم - ${rating.id}`
                          )
                        "
                      >
                        <img
                          width="22"
                          src="/imgs/icons/print.svg"
                          alt="print icon"
                        />
                      </VBtn>
                      <v-chip
                        v-if="rating.status === 'PENDING'"
                        color="warning"
                        size="small"
                      >
                        قيد المراجعة
                      </v-chip>
                      <v-chip
                        v-else-if="rating.status === 'APPROVED'"
                        color="success"
                        size="small"
                      >
                        معتمد
                      </v-chip>
                      <v-chip
                        v-else-if="rating.status === 'REJECTED'"
                        color="danger"
                        size="small"
                      >
                        مرفوض
                      </v-chip>
                    </div>
                  </div>
                  <v-row class="mt-4">
                    <v-col cols="12" md="6" v-if="rating?.rating_by">
                      <div>
                        <div class="d-flex align-center ga-2">
                          <div
                            class="overflow-hidden rounded-pill"
                            style="
                              width: 30px;
                              height: 30px;
                              background-color: #f3f1ed;
                            "
                          >
                            <img
                              v-if="rating.rating_by?.avatar"
                              width="100%"
                              height="100%"
                              :src="rating.rating_by.avatar"
                              alt="icon"
                            />
                            <img
                              v-else
                              width="100%"
                              height="100%"
                              src="/imgs/placholderimg.svg"
                              alt="icon"
                            />
                          </div>
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            المراقب
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.rating_by?.firstname
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6" v-if="rating?.imtithal_phase">
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/welcome.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            اسم المرحلة
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.imtithal_phase?.title
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6" v-if="rating?.location?.contract">
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/building-icon.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            اسم الباقة
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.location.contract?.name
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      v-if="rating?.location?.gate_number"
                    >
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/building-icon.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            رقم البوابة
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.location.gate_number
                          }}</span>
                        </div>
                      </div>
                    </v-col>

                    <v-col
                      cols="12"
                      md="6"
                      v-if="rating?.location?.square_number"
                    >
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/building-icon.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            رقم المربع
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.location.square_number
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      v-if="rating?.location?.camp_number"
                    >
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/building-icon.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            رقم المخيم
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.location.camp_number
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      v-if="rating?.location?.service_center_no"
                    >
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/welcome.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            رقم مركز الخدمة
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.location.service_center_no
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      v-if="rating?.company?.pligrims_nationality"
                    >
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/building-icon.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            جنسية الحجاج
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.company.pligrims_nationality
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      v-if="rating?.company?.parent_company_name"
                      class="mb-4"
                    >
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/building-icon.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            اسم الشركة الرئيسية
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.company.parent_company_name
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      v-if="rating?.company?.name"
                      class="mb-4"
                    >
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/building-icon.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            اسم مقدم الخدمة
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.company.name
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      v-if="rating?.location?.delegate_phone"
                    >
                      <div>
                        <div class="d-flex align-center ga-2">
                          <img
                            width="32"
                            height="32"
                            src="/imgs/icons/welcome.svg"
                            alt="icon"
                          />
                          <h3
                            class="text-body-2"
                            style="font-weight: 300; color: #565a5d"
                          >
                            رقم التواصل مع المفوض
                          </h3>
                        </div>
                        <div class="w-100 ps-10">
                          <span class="text-subtitle-2">{{
                            rating.location.delegate_phone
                          }}</span>
                        </div>
                      </div>
                    </v-col>
                  </v-row>
                </v-card-title>
              </v-card>
            </v-col>
          </v-row>

          <!-- pagination -->
          <Pagination
            v-if="pagination && pagination.total"
            :total-items="pagination.total"
            v-model:per-page="pagination.per_page"
            v-model:page="pagination.page"
          />
        </v-card-text>
      </v-card>
    </v-container>
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
