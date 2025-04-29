<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import axios from "@/plugins/axios";
import LocationPicker from "@/components/maps/LocationPicker.vue";
import { format } from "@core/utils/formatters";
import { exportPDFFromURL } from "@core/utils/helpers";
import { canAccessObserverInfo } from "@core/utils/permissions";
// import UserWidget from "@/@core/components/widgets/UserWidget.vue";

const route = useRoute();
const isLoading = ref(true);
const rating = ref(null);
const ratingBy = ref(null);
const ratingId = computed(() => route.params.id);
const breadcrumbItems = computed(() => [
  {
    title: "الرئيسية",
    disabled: false,
    to: "/",
  },
  {
    title: "⁠⁠إدارة التقييمات",
    disabled: false,
    to: `/rating?location_type=${rating.value?.location?.location_type_id}`,
  },
  {
    title: `التقييم ${ratingId.value}#`,
    disabled: true,
  },
]);

const fetchRatingDetails = async () => {
  try {
    isLoading.value = true;
    const response = await axios.get(`/ratings/${ratingId.value}`);
    if (response.data.status.success) {
      rating.value = response.data.data;
      ratingBy.value = rating.value.rating_by;
    }
  } catch (error) {
    console.error("Error fetching rating details:", error);
  } finally {
    isLoading.value = false;
  }
};

const isPrintingAsPDF = ref(false);
const printRate = async (rateId) => {
  try {
    isPrintingAsPDF.value = rateId;

    await exportPDFFromURL(
      `/export-rating-pdf/${rateId}`,
      {},
      `rate_${rateId}`
    );
  } catch (error) {
    console.error("printing failed:", error);
  } finally {
    isPrintingAsPDF.value = null;
  }
};

onMounted(() => {
  fetchRatingDetails();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2 pt-0">
      <v-row v-if="isLoading">
        <v-col cols="12">
          <v-skeleton-loader type="article" />
        </v-col>
      </v-row>

      <div v-else-if="rating">
        <div class="d-flex justify-space-between align-center ga-2 flex-wrap">
          <v-breadcrumbs divider=">" :items="breadcrumbItems">
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

          <VBtn
            variant="link"
            height="42"
            elevation="0"
            class="px-4 bg-white rounded-xl"
            @click="printRate(rating.id)"
            :loading="isPrintingAsPDF"
            :disabled="isPrintingAsPDF"
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
                    d="M7.35396 8H16.646C18.4184 8 19.3045 8 19.992 8.35353C20.4434 8.58564 20.8285 8.92129 21.1132 9.33081C21.5468 9.95455 21.635 10.8039 21.8113 12.5025C22.0226 14.5373 22.1282 15.5547 21.7352 16.3242C21.4784 16.8271 21.0805 17.2506 20.5865 17.5468C19.8307 18 18.7692 18 16.646 18H16.6081C16.481 17.4554 16.3992 17.1526 16.2538 16.9099C16.0297 16.536 15.6969 16.254 15.3082 16.1089C15.0166 16 14.674 16 13.9887 16H10.0113C9.32602 16 8.98337 16 8.69183 16.1089C8.30311 16.254 7.97026 16.536 7.7462 16.9099C7.60077 17.1526 7.51896 17.4554 7.39185 18H7.35396C5.23084 18 4.16928 18 3.41349 17.5468C2.91953 17.2506 2.52158 16.8271 2.26475 16.3242C1.87179 15.5547 1.97742 14.5373 2.18868 12.5025C2.36503 10.8039 2.45321 9.95455 2.88684 9.33081C3.17153 8.92129 3.55659 8.58564 4.00797 8.35353C4.69548 8 5.58164 8 7.35396 8Z"
                    fill="#B69265"
                  />
                  <path
                    d="M18 11C17.4477 11 17 11.4477 17 12C17 12.5523 17.4477 13 18 13H18.009C18.5613 13 19.009 12.5523 19.009 12C19.009 11.4477 18.5613 11 18.009 11H18Z"
                    fill="#B69265"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M13.052 1.25H10.948C10.0495 1.24997 9.3003 1.24995 8.70552 1.32991C8.07773 1.41432 7.51093 1.59999 7.05546 2.05546C6.59999 2.51093 6.41432 3.07773 6.32991 3.70552C6.24995 4.3003 6.24997 5.04951 6.25 5.94799L6.25 7.2544C5.86117 7.25968 5.51709 7.27129 5.21173 7.2968C4.6344 7.34502 4.13151 7.44666 3.66499 7.68655C3.10651 7.97374 2.62704 8.3906 2.27103 8.90269C1.97157 9.33345 1.81377 9.81457 1.70522 10.3696C1.60044 10.9055 1.5314 11.5705 1.44628 12.3904L1.43866 12.4639C1.33644 13.4483 1.25407 14.2417 1.25015 14.8833C1.24612 15.5438 1.32242 16.128 1.59682 16.6653C1.9189 17.296 2.41585 17.8231 3.0278 18.19C3.54485 18.5001 4.12804 18.6291 4.8033 18.6903C5.26638 18.7323 5.81096 18.7447 6.45125 18.7484C6.39076 19.0221 6.3412 19.2644 6.30703 19.4789C6.24759 19.8519 6.22268 20.2066 6.2915 20.5588C6.44953 21.3673 6.94712 22.0768 7.67159 22.4603C7.99394 22.6309 8.34088 22.6935 8.69884 22.7223C9.04456 22.75 9.47142 22.75 9.98109 22.75H14.0189C14.5286 22.75 14.9554 22.75 15.3011 22.7223C15.6591 22.6935 16.006 22.6309 16.3284 22.4603C17.0528 22.0768 17.5504 21.3673 17.7085 20.5588C17.7773 20.2066 17.7524 19.8519 17.6929 19.4789C17.6588 19.2644 17.6092 19.0221 17.5487 18.7484C18.189 18.7447 18.7336 18.7323 19.1967 18.6903C19.872 18.6291 20.4552 18.5001 20.9722 18.19C21.5841 17.8231 22.0811 17.296 22.4032 16.6653C22.6776 16.128 22.7539 15.5438 22.7499 14.8833C22.7459 14.2416 22.6636 13.4483 22.5613 12.4638L22.5537 12.3904C22.4686 11.5705 22.3996 10.9055 22.2948 10.3696C22.1862 9.81457 22.0284 9.33345 21.729 8.90269C21.373 8.3906 20.8935 7.97374 20.335 7.68655C19.8685 7.44666 19.3656 7.34502 18.7883 7.2968C18.4829 7.27129 18.1388 7.25968 17.75 7.2544L17.75 5.94801C17.75 5.04957 17.7501 4.30029 17.6701 3.70552C17.5857 3.07773 17.4 2.51093 16.9445 2.05546C16.4891 1.59999 15.9223 1.41432 15.2945 1.32991C14.6997 1.24995 13.9505 1.24997 13.052 1.25ZM7.10282 16.5244C6.97571 16.7366 6.88576 16.9753 6.80699 17.2496C6.0136 17.2481 5.41787 17.2398 4.93865 17.1964C4.3634 17.1443 4.03792 17.0467 3.79919 16.9036C3.42321 16.6781 3.12427 16.3582 2.93269 15.9831C2.81413 15.7509 2.74677 15.4417 2.75012 14.8924C2.75354 14.3327 2.82749 13.6123 2.93467 12.5799C3.02425 11.7172 3.08701 11.1194 3.17734 10.6575C3.26538 10.2073 3.36848 9.95191 3.50265 9.75892C3.71603 9.45198 4.00668 9.19755 4.35095 9.02052C4.57195 8.90688 4.85589 8.83175 5.33659 8.79159C5.82717 8.75061 6.4552 8.75 7.35396 8.75H16.646C17.5448 8.75 18.1728 8.75061 18.6634 8.79159C19.1441 8.83175 19.4281 8.90688 19.6491 9.02052C19.9933 9.19755 20.284 9.45198 20.4974 9.75892C20.6315 9.95191 20.7346 10.2073 20.8227 10.6575C20.913 11.1194 20.9758 11.7172 21.0653 12.5799C21.1725 13.6123 21.2465 14.3327 21.2499 14.8924C21.2532 15.4417 21.1859 15.7509 21.0673 15.9831C20.8757 16.3582 20.5768 16.6781 20.2008 16.9036C19.9621 17.0467 19.6366 17.1443 19.0614 17.1964C18.5821 17.2398 17.9864 17.2481 17.193 17.2496C17.1142 16.9753 17.0242 16.7366 16.8971 16.5244C16.5905 16.0126 16.127 15.614 15.5705 15.4062C15.1486 15.2487 14.6745 15.2492 14.1016 15.2499H9.89832C9.32548 15.2492 8.85132 15.2487 8.42945 15.4062C7.87298 15.614 7.4095 16.0126 7.10282 16.5244ZM16.25 7.25V6C16.25 5.03599 16.2484 4.38843 16.1835 3.9054C16.1214 3.44393 16.0142 3.24644 15.8839 3.11612C15.7536 2.9858 15.5561 2.87858 15.0946 2.81654C14.6116 2.7516 13.964 2.75 13 2.75H11C10.036 2.75 9.38843 2.7516 8.90539 2.81654C8.44393 2.87858 8.24644 2.9858 8.11612 3.11612C7.9858 3.24644 7.87858 3.44393 7.81654 3.9054C7.7516 4.38843 7.75 5.03599 7.75 6V7.25H16.25ZM10.0113 16.75C9.26465 16.75 9.08796 16.7615 8.95416 16.8115C8.73319 16.894 8.53098 17.0593 8.38954 17.2954C8.29741 17.4491 8.23831 17.6655 8.05967 18.4408C7.92745 19.0146 7.83724 19.4079 7.78835 19.7148C7.74001 20.0183 7.74416 20.1713 7.76365 20.271C7.84113 20.6674 8.07758 20.978 8.3734 21.1346C8.43367 21.1665 8.54573 21.2052 8.81882 21.2271C9.09733 21.2494 9.46354 21.25 10.0113 21.25H13.9887C14.5364 21.25 14.9026 21.2494 15.1811 21.2271C15.4542 21.2052 15.5663 21.1665 15.6266 21.1346C15.9224 20.978 16.1588 20.6674 16.2363 20.271C16.2558 20.1713 16.26 20.0183 16.2116 19.7148C16.1627 19.4079 16.0725 19.0146 15.9403 18.4408C15.7617 17.6655 15.7025 17.4491 15.6104 17.2954C15.469 17.0593 15.2668 16.894 15.0458 16.8115C14.912 16.7615 14.7353 16.75 13.9887 16.75H10.0113Z"
                    fill="#B69265"
                  />
                </svg>
              </span>
              <span>طباعة التقييم</span>
            </div>
          </VBtn>
        </div>
        <v-container fluid class="px-0 rounded-lg">
          <v-card
            :id="`rating_${rating.id}`"
            elevation="0"
            class="rounded-lg px-sm-4 bg-light-white"
          >
            <!-- Header -->
            <v-card-title class="pt-4 pb-6">
              <div class="d-flex justify-space-between align-start ga-2">
                <div>
                  <h2 class="text-h5 d-flex align-center ga-2 mb-1">
                    <span>التقييم</span>
                    <span dir="ltr" class="d-flex"
                      ><span> #</span><span>{{ rating?.id }}</span></span
                    >
                  </h2>
                  <span class="d-flex align-center ga-2" style="color: #75797c">
                    <small class="d-flex align-center ga-1">
                      <span> {{ format(rating.created_at, "D") }}</span>
                      <span> {{ format(rating.created_at, "MMM") }}</span>
                      <span>{{ format(rating.created_at, "YYYY") }}</span>
                    </small>
                    <small>-</small>
                    <small class="d-flex align-center ga-1">
                      <div class="d-flex align-center">
                        <span> {{ format(rating.created_at, "mm") }}</span>
                        <span>:</span>
                        <span> {{ format(rating.created_at, "h") }}</span>
                      </div>
                      <span>{{ format(rating.created_at, "A") }}</span>
                    </small>
                  </span>
                </div>
                <div class="d-flex align-center gap-4">
                  <v-chip
                    v-if="rating.status === 'PENDING'"
                    color="warning"
                    size="default"
                  >
                    قيد المراجعة
                  </v-chip>
                  <v-chip
                    v-else-if="rating.status === 'APPROVED'"
                    color="success"
                    size="default"
                  >
                    معتمد
                  </v-chip>
                  <v-chip
                    v-else-if="rating.status === 'REJECTED'"
                    color="danger"
                    size="default"
                  >
                    مرفوض
                  </v-chip>
                </div>
              </div>
            </v-card-title>

            <v-card-text class="mb-3 px-4 px-sm-6">
              <div
                class="d-flex align-center justify-space-between ga-4 flex-wrap mb-6 pa-6 rounded-lg"
                style="background-color: #f9f9f9"
              >
                <div class="d-flex align-start ga-2">
                  <img width="23" src="/imgs/icons/question.svg" alt="icon" />

                  <div>
                    <h2 class="text-body-1 text-medium-emphasis mb-1">
                      المجموع الكلي للاستبانة
                    </h2>
                    <span class="text-body-2 font-weight-medium">
                      {{
                        rating?.questionnaire?.questionnaire_weight != null
                          ? rating?.questionnaire?.questionnaire_weight
                          : "-"
                      }}
                    </span>
                  </div>
                </div>
                <div class="d-flex align-start ga-2">
                  <img width="23" src="/imgs/icons/reply.svg" alt="icon" />

                  <div>
                    <h2 class="text-body-1 text-medium-emphasis mb-1">
                      نتيجة التقييم
                    </h2>
                    <span class="text-body-2 font-weight-medium">
                      {{ rating?.commitment_rate }}
                    </span>
                  </div>
                </div>
                <div class="d-flex align-start ga-2">
                  <img width="23" src="/imgs/icons/star.svg" alt="icon" />

                  <div>
                    <h2 class="text-body-1 text-medium-emphasis mb-1">
                      نسبة الجاهزية
                    </h2>
                    <span class="d-flex text-body-2 font-weight-medium">
                      <span>{{ rating?.readiness_rate || "-" }}</span>
                      <span v-if="rating?.readiness_rate">%</span>
                    </span>
                  </div>
                </div>
                <div></div>
              </div>

              <v-row class="mt-4">
                <v-col
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="canAccessObserverInfo()"
                  class="mb-4"
                >
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
                    <div class="d-flex ga-1 w-100 ps-10">
                      <span class="text-subtitle-2">{{
                        rating.rating_by?.firstname
                      }}</span>
                      <span class="text-subtitle-2">{{
                        rating.rating_by?.lastname
                      }}</span>
                    </div>
                  </div>
                </v-col>
                <v-col
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.imtithal_phase"
                  class="mb-4"
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
                <v-col
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.location?.contract"
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
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.location?.gate_number"
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
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.location?.square_number"
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
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.location?.camp_number"
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
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.location?.service_center_no"
                  class="mb-4"
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
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.company?.pligrims_nationality"
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
                  cols="6"
                  md="4"
                  xl="3"
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
                  cols="6"
                  md="4"
                  xl="3"
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
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.location?.license_no"
                  class="mb-4"
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
                        رقم تصريح السكن
                      </h3>
                    </div>
                    <div class="w-100 ps-10">
                      <span class="text-subtitle-2">{{
                        rating.location.license_no
                      }}</span>
                    </div>
                  </div>
                </v-col>
                <v-col
                  cols="6"
                  md="4"
                  xl="3"
                  v-if="rating?.location?.delegate_phone"
                  class="mb-4"
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
                <!-- map location -->
                <v-col
                  v-if="
                    rating?.location &&
                    rating?.location?.latitude &&
                    rating?.location.longitude
                  "
                  cols="12"
                  class="no-print"
                >
                  <div class="mt-4">
                    <div class="d-flex align-start ga-2">
                      <img
                        width="23"
                        src="/imgs/icons/map-location.svg"
                        alt="icon"
                      />
                      <h3
                        class="text-body-2"
                        style="font-weight: 300; color: #565a5d"
                      >
                        الموقع على الخريطة
                      </h3>
                    </div>
                    <div class="w-100 ps-6">
                      <div
                        class="w-100 mt-3 rounded-xl"
                        style="height: 250px; background-color: #565a5d"
                      >
                        <LocationPicker
                          :modelValue="{
                            lat: rating?.location.latitude,
                            lng: rating?.location.longitude,
                          }"
                          :isDraggable="false"
                          :isClickable="false"
                          class="rounded-lg overflow-hidden"
                          style="height: 250px"
                        />
                      </div>
                    </div>
                  </div>
                </v-col>
              </v-row>

              <!-- rejection reason -->
              <div v-if="rating?.rejection_reason" class="mt-4">
                <div class="d-flex align-center ga-2 mb-2">
                  <img
                    width="23"
                    src="/imgs/icons/information-circle.svg"
                    alt="icon"
                  />
                  <h2 class="text-h6 font-weight-semibold">سبب رفض التقييم</h2>
                </div>
                <div
                  class="d-flex align-center justify-space-between ga-4 flex-wrap pa-4 rounded-lg"
                  style="background-color: #f9f9f9"
                >
                  <p class="mb-0">
                    {{ rating.rejection_reason }}
                  </p>
                </div>
              </div>
            </v-card-text>

            <hr
              class="my-4 no-print"
              style="border-color: rgba(248, 249, 249, 0.15)"
            />

            <!-- Questionnaire Categories -->
            <v-card-text class="no-print">
              <v-expansion-panels
                elevation="0"
                variant="accordion"
                class="custom-panel mb-4"
                model-value="0"
              >
                <v-expansion-panel
                  v-for="(category, categoryIndex) in rating.questionnaire
                    ?.questionnaire_categories"
                  :key="categoryIndex"
                  class="mb-3 rounded-lg"
                >
                  <v-expansion-panel-title>
                    <div class="d-flex align-center py-4">
                      <span style="color: #75797c; font-weight: 300">{{
                        category.title || "فئة " + (categoryIndex + 1)
                      }}</span>
                    </div>
                  </v-expansion-panel-title>
                  <v-expansion-panel-text>
                    <div
                      v-if="category.questions && category.questions.length > 0"
                      class="px-4"
                    >
                      <div
                        v-for="(question, questionIndex) in category.questions"
                        :key="questionIndex"
                        class="pa-3 mb-3 border rounded-lg"
                      >
                        <h2 class="text-h6 mb-3 font-weight-medium">
                          {{ question.question }}
                        </h2>

                        <div
                          v-if="question.question_type == 'yes_no'"
                          class="py-4"
                        >
                          <v-row>
                            <v-col cols="6">
                              <div
                                class="pa-2 rounded-pill text-center"
                                :style="`${
                                  question.answer?.answer == 1
                                    ? 'background-color: #4e9158;color: white;'
                                    : 'background-color: #F9F9F9;color: black;'
                                }`"
                              >
                                نعم
                              </div>
                            </v-col>
                            <v-col cols="6">
                              <div
                                class="pa-2 rounded-pill text-center"
                                :style="`${
                                  question.answer?.answer == 0
                                    ? 'background-color: #C62800;color: white;'
                                    : 'background-color: #F9F9F9;color: black;'
                                }`"
                              >
                                لا
                              </div>
                            </v-col>
                          </v-row>
                        </div>
                        <div
                          v-else
                          class="d-flex align-center justify-space-between ga-4 flex-wrap pa-4 rounded-lg"
                          style="background-color: #f9f9f9"
                        >
                          <p class="mb-0">
                            {{ question?.answer?.answer || "-" }}
                          </p>
                        </div>
                        <!-- Note -->
                        <div v-if="question?.answer?.note" class="my-4">
                          <h2 class="text-body-1 mb-4">ملاحظات</h2>
                          <div
                            class="d-flex align-center justify-space-between ga-4 flex-wrap pa-4 rounded-lg"
                            style="background-color: #f9f9f9"
                          >
                            <p class="mb-0">
                              {{ question.answer.note }}
                            </p>
                          </div>
                        </div>
                        <div
                          v-if="
                            question?.answer &&
                            question?.answer?.upload_urls?.length
                          "
                          class="my-4"
                        >
                          <h2 class="text-body-1 mb-4">إرفاق أدلة</h2>
                          <v-row>
                            <v-col
                              v-for="file in question.answer.upload_urls"
                              :key="file"
                              cols="12"
                              sm="6"
                            >
                              <div
                                class="d-flex align-center justify-space-between ga-4 pa-2 rounded-lg"
                                style="background-color: #f9f9f9"
                              >
                                <div
                                  class="d-flex align-center justify-space-between ga-4"
                                >
                                  <img
                                    width="50"
                                    src="/imgs/img.png"
                                    alt="image"
                                  />
                                  <h3 class="text-body-2">
                                    الأدلة والشواهد الميدانية
                                  </h3>
                                </div>

                                <VBtn
                                  variant="link"
                                  icon
                                  elevation="0"
                                  class="bg-white"
                                  download
                                  :href="file"
                                >
                                  <span>
                                    <svg
                                      xmlns="http://www.w3.org/2000/svg"
                                      width="16"
                                      height="16"
                                      fill="currentColor"
                                      class="bi bi-download"
                                      viewBox="0 0 16 16"
                                    >
                                      <path
                                        d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5"
                                      />
                                      <path
                                        d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708z"
                                      />
                                    </svg>
                                  </span>
                                </VBtn>
                              </div>
                            </v-col>
                          </v-row>
                        </div>
                      </div>
                    </div>
                    <div v-else class="text-center pa-4 text-medium-emphasis">
                      لا توجد أسئلة في هذه الفئة
                    </div>
                  </v-expansion-panel-text>
                </v-expansion-panel>
              </v-expansion-panels>
            </v-card-text>
          </v-card>
        </v-container>
      </div>
      <div v-else class="text-center py-8">
        <p>لم يتم العثور على التقييم</p>
      </div>
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.questions-list {
  .question-item {
    &:not(:last-child) {
      border-bottom: 1px solid rgba(0, 0, 0, 0.12);
    }
  }
}

.answer-section {
  background-color: rgba(0, 0, 0, 0.02);
  padding: 12px;
  border-radius: 8px;
}
</style>
