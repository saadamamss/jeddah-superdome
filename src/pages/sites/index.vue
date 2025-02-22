<script setup>
import { useAppStore } from "@/stores/app";
import AppDateTimePicker from "@/@core/components/AppDateTimePicker";
import StatsWidget from "@/@core/components/widgets/StatsWidget";
import Pagination from "@/@core/components/Pagination";
import SiteCard from "@/@core/components/widgets/SiteCard.vue";
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
    status: "active",
  },
  {
    siteId: "6127782188",
    authorizationNumber: "231 3456 568",
    status: "inActive",
  },
  {
    siteId: "6127782189",
    authorizationNumber: "231 3456 569",
    status: "active",
  },
  {
    siteId: "6127782190",
    authorizationNumber: "231 3456 570",
    status: "inActive",
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
        <v-row>
          <v-col cols="12" md="6">
            <StatsWidget title="المساكن" number="234">
              <template #icon>
                <svg
                  width="2.2rem"
                  height="2.2rem"
                  viewBox="0 0 33 33"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    d="M18.9168 11.5C18.0678 11.5 17.3181 11.543 16.6811 11.6829C16.0328 11.8253 15.4391 12.0815 14.9687 12.5519C14.4983 13.0223 14.2421 13.616 14.0997 14.2643C13.9598 14.9013 13.9168 15.651 13.9168 16.5L13.9168 28.1667H20.2496V25.8333C20.2496 25.097 20.8465 24.5 21.5829 24.5C22.3193 24.5 22.9162 25.097 22.9162 25.8333V28.1667H29.2501L29.2501 16.5C29.2501 15.651 29.2072 14.9013 29.0672 14.2643C28.9248 13.616 28.6687 13.0223 28.1982 12.5519C27.7278 12.0815 27.1342 11.8253 26.4858 11.6829C25.8488 11.543 25.0991 11.5 24.2501 11.5H18.9168Z"
                    fill="#B69265"
                  />
                  <path
                    d="M1.91693 29.5001C1.91693 28.7637 2.51388 28.1667 3.25026 28.1667L3.25026 20.1667H6.91634C7.46863 20.1667 7.91634 19.719 7.91634 19.1667C7.91634 18.6145 7.46863 18.1667 6.91634 18.1667H3.25026L3.25026 14.8334L6.91634 14.8334C7.46863 14.8334 7.91634 14.3857 7.91634 13.8334C7.91634 13.2811 7.46863 12.8334 6.91634 12.8334L3.25026 12.8334L3.25026 9.50008H6.91634C7.46863 9.50008 7.91634 9.05237 7.91634 8.50008C7.91634 7.9478 7.46863 7.50008 6.91634 7.50008L3.25026 7.50008V7.16675C3.25026 6.31777 3.29324 5.56806 3.43318 4.93106C3.5756 4.28273 3.83175 3.68905 4.30216 3.21864C4.77256 2.74824 5.36625 2.49209 6.01458 2.34966C6.65158 2.20973 7.40129 2.16675 8.25026 2.16675L16.2503 2.16675C17.0992 2.16675 17.849 2.20973 18.486 2.34966C19.1343 2.49209 19.728 2.74824 20.1984 3.21864C20.6688 3.68905 20.9249 4.28273 21.0674 4.93106C21.2073 5.56806 21.2503 6.31777 21.2503 7.16675V8.70008C21.2503 9.0772 21.2503 9.26577 21.1331 9.38292C21.016 9.50008 20.8274 9.50008 20.4503 9.50008H18.9169C18.0242 9.50008 17.1026 9.54274 16.2521 9.72958C15.3681 9.92377 14.3799 10.3125 13.5546 11.1378C12.7293 11.963 12.3406 12.9513 12.1464 13.8353C11.9596 14.6858 11.9169 15.6074 11.9169 16.5001L11.9169 28.1667H20.2503V25.8334C20.2503 25.097 20.8472 24.5001 21.5836 24.5001C22.32 24.5001 22.9169 25.097 22.9169 25.8334V28.1667L29.2503 28.1667C29.9866 28.1667 30.5836 28.7637 30.5836 29.5001C30.5836 30.2365 29.9866 30.8334 29.2503 30.8334L3.25026 30.8334C2.51388 30.8334 1.91693 30.2365 1.91693 29.5001Z"
                    fill="#B69265"
                  />
                  <path
                    d="M19.2503 16.5001C19.2503 15.9478 19.698 15.5001 20.2503 15.5001H22.9169C23.4692 15.5001 23.9169 15.9478 23.9169 16.5001C23.9169 17.0524 23.4692 17.5001 22.9169 17.5001H20.2503C19.698 17.5001 19.2503 17.0524 19.2503 16.5001Z"
                    fill="#B69265"
                  />
                  <path
                    d="M19.2503 20.5001C19.2503 19.9478 19.698 19.5001 20.2503 19.5001H22.9169C23.4692 19.5001 23.9169 19.9478 23.9169 20.5001C23.9169 21.0524 23.4692 21.5001 22.9169 21.5001H20.2503C19.698 21.5001 19.2503 21.0524 19.2503 20.5001Z"
                    fill="#B69265"
                  />
                </svg>
              </template>
            </StatsWidget>
          </v-col>
          <v-col cols="12" md="6">
            <StatsWidget title="المخيمات" number="23">
              <template #icon>
                <svg
                  width="2.2rem"
                  height="2.2rem"
                  viewBox="0 0 30 27"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <g opacity="0.4">
                    <path
                      d="M9.66798 23.1686C10.2203 23.1686 10.668 23.6163 10.668 24.1686V25.9686C10.668 26.22 10.668 26.3457 10.7461 26.4238C10.8242 26.5019 10.9499 26.5019 11.2013 26.5019H13.468C13.7194 26.5019 13.8451 26.5019 13.9232 26.4238C14.0013 26.3457 14.0013 26.22 14.0013 25.9686V24.1686C14.0013 23.6163 14.449 23.1686 15.0013 23.1686C15.5536 23.1686 16.0013 23.6163 16.0013 24.1686V25.9686C16.0013 26.22 16.0013 26.3457 16.0794 26.4238C16.1575 26.5019 16.2832 26.5019 16.5346 26.5019L22.3601 26.5019C22.7181 26.5019 22.897 26.5019 22.9763 26.3844C23.0556 26.2669 22.9886 26.1009 22.8546 25.769L20.1807 19.1444C20.1245 19.0052 20.0964 18.9356 20.0396 18.8915C19.9829 18.8474 19.9093 18.8374 19.762 18.8176C18.2331 18.6113 16.6399 18.5019 15.0015 18.5019C10.138 18.5019 5.67398 19.4649 2.10692 21.0677L0.976769 21.6634C0.833219 21.7391 0.761444 21.7769 0.722939 21.844C0.684435 21.911 0.687864 21.9907 0.694723 22.15C0.710281 22.5115 0.735046 22.8435 0.774462 23.1461C0.885797 24.001 1.12969 24.7705 1.73126 25.3915C2.33714 26.0169 3.09529 26.2747 3.93826 26.3917C4.7334 26.502 5.73422 26.502 6.93059 26.5019L8.13465 26.5019C8.38606 26.5019 8.51177 26.5019 8.58988 26.4238C8.66798 26.3457 8.66798 26.22 8.66798 25.9686V24.1686C8.66798 23.6163 9.11569 23.1686 9.66798 23.1686Z"
                      fill="#B69265"
                    />
                    <path
                      d="M13.2755 8.06219C13.9204 7.99608 14.6545 7.92083 15.3273 8.33782C15.709 8.57433 15.9499 8.89274 16.1323 9.22373C16.2934 9.51616 16.4401 9.87964 16.5939 10.2608L18.8722 15.9051C19.021 16.2737 19.0954 16.458 19.0065 16.578C18.9177 16.6981 18.7175 16.6806 18.3172 16.6456C17.2332 16.5509 16.1258 16.5019 15.0005 16.5019C14.6284 16.5019 14.2582 16.5073 13.8902 16.5179C13.627 16.5254 13.4953 16.5292 13.4145 16.4507C13.3337 16.3721 13.3337 16.2417 13.3337 15.981V14.8352C13.3337 14.283 12.8859 13.8352 12.3337 13.8352C11.7814 13.8352 11.3337 14.283 11.3337 14.8352V16.1937C11.3337 16.4231 11.3337 16.5377 11.2656 16.6137C11.1976 16.6896 11.0824 16.7023 10.8521 16.7277C10.1004 16.8105 9.36104 16.9155 8.63584 17.0416C8.33884 17.0932 8.19034 17.119 8.09533 17.0391C8.00033 16.9592 8.00033 16.8101 8.00033 16.512V15.5019C8.00033 14.9496 7.55261 14.5019 7.00033 14.5019C6.44804 14.5019 6.00033 14.9496 6.00033 15.5019L6.00033 17.194C6.00033 17.392 6.00033 17.491 5.94546 17.5627C5.89059 17.6345 5.79391 17.6607 5.60056 17.7131C4.05753 18.1314 2.60185 18.6502 1.25612 19.257L1.22807 19.2696C1.07931 19.348 1.00493 19.3872 0.94451 19.3896C0.838115 19.3938 0.739442 19.3342 0.693508 19.2382C0.667421 19.1836 0.667417 19.0995 0.667408 18.9314L0.667233 15.5401C0.665766 15.0731 0.664167 14.5641 0.885516 14.0887C1.10653 13.614 1.47867 13.3029 1.8164 13.0206C4.80268 10.5089 8.38432 8.75636 12.1875 8.19276C12.4992 8.14657 12.9602 8.09307 13.2755 8.06219Z"
                      fill="#B69265"
                    />
                  </g>
                  <path
                    d="M22.229 19.3824C22.1231 19.5056 22.2053 19.7095 22.3699 20.1172L24.7975 26.1314C24.8657 26.3004 24.8999 26.3849 24.9725 26.4308C25.0451 26.4768 25.1341 26.4714 25.3119 26.4608C25.5709 26.4453 25.8133 26.4237 26.0391 26.3936C26.8804 26.2814 27.6395 26.0347 28.2502 25.4288C28.862 24.8219 29.112 24.0655 29.2256 23.227C29.2728 22.879 29.2992 22.4913 29.3141 22.0647C29.32 21.896 29.3229 21.8116 29.2802 21.7428C29.2374 21.6739 29.1594 21.6385 29.0032 21.5678L27.9224 21.0783C26.4188 20.4004 24.755 19.8364 22.9686 19.4104C22.5462 19.3097 22.335 19.2593 22.229 19.3824Z"
                    fill="#B69265"
                  />
                  <path
                    d="M21.1338 16.9652C21.1875 17.0086 21.2601 17.0213 21.4052 17.0467C24.0775 17.5145 26.5563 18.2685 28.7444 19.255L28.7459 19.2557C28.9326 19.3403 29.026 19.3826 29.1016 19.3725C29.1805 19.362 29.2506 19.3168 29.2927 19.2493C29.3331 19.1845 29.3331 19.082 29.3331 18.8771L29.3332 17.1279C29.3344 16.0825 29.3353 15.2414 28.926 14.4487C28.5179 13.6583 27.9044 13.2209 27.1499 12.6831L27.0428 12.6067C25.4655 11.4801 23.7626 10.5643 21.9621 9.89586C21.6551 9.78191 21.3119 9.82543 21.0431 10.0124C20.7743 10.1993 20.614 10.5059 20.614 10.8333V11.9687C20.614 12.2181 20.614 12.3429 20.537 12.4208L20.5348 12.423C20.4569 12.5 20.3322 12.5 20.0827 12.5C19.7276 12.5 19.55 12.5 19.4706 12.6159L19.4684 12.6192C19.3906 12.7362 19.4571 12.9009 19.59 13.2302L20.9983 16.719C21.0528 16.8541 21.0801 16.9217 21.1338 16.9652Z"
                    fill="#B69265"
                  />
                  <path
                    d="M11.7895 5.07345L12.1008 5.86285C12.2137 6.1492 12.2702 6.29238 12.209 6.4025C12.1478 6.51263 11.9935 6.54087 11.6849 6.59736C8.12021 7.24976 4.4639 8.90552 1.56292 11.0946C1.16276 11.3965 0.962678 11.5475 0.814919 11.4739C0.667161 11.4003 0.667157 11.1544 0.667149 10.6626L0.667099 7.49452C0.666045 6.41577 0.665217 5.56787 1.06105 4.76583C1.45879 3.9599 2.05972 3.50991 2.80657 2.95066C3.14719 2.69513 3.66762 2.32572 4.01992 2.09128C4.82978 1.55221 5.55425 1.06997 6.23148 0.790635C7.00133 0.473092 7.76448 0.390496 8.62185 0.669922C9.51689 0.961622 10.0997 1.55529 10.5605 2.32348C10.9831 3.02798 11.3544 3.96965 11.7895 5.07345Z"
                    fill="#B69265"
                  />
                </svg>
              </template>
            </StatsWidget>
          </v-col>
        </v-row>

        <v-container fluid class="mt-6 px-0 rounded-lg">
          <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
            <!-- Header Section -->
            <v-card-title class="pt-4 pb-6">
              <div class="d-flex align-center justify-space-between ga-4">
                <h2 class="text-h5 font-weight-bold">إدارة المواقع</h2>
                <div class="d-flex align-center ga-2">
                  <v-text-field
                    v-model="search"
                    label="البحث المخيم او المسكن... "
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
                      <path
                        fill-rule="evenodd"
                        clip-rule="evenodd"
                        d="M6.18176 2.24732C6.1991 2.24732 6.21649 2.24732 6.23394 2.24732L17.8175 2.24732C18.588 2.24728 19.2422 2.24725 19.7583 2.31762C20.3046 2.3921 20.8269 2.56023 21.2268 3.00041C21.6302 3.44445 21.7406 3.97957 21.7491 4.52659C21.757 5.03745 21.6753 5.67204 21.5801 6.41193L21.5732 6.46581C21.5395 6.72764 21.489 6.98088 21.3836 7.23442C21.2767 7.49184 21.1294 7.71109 20.946 7.92897C19.9664 9.09285 18.1451 11.1962 15.5811 13.1117C15.5396 13.1427 15.4872 13.2161 15.4773 13.3263C15.2282 16.0792 15.009 17.5358 14.8509 18.3796C14.6799 19.2921 13.9839 19.9237 13.3838 20.3585C13.0697 20.5861 12.7364 20.791 12.4394 20.9717C12.4156 20.9862 12.3921 21.0005 12.3689 21.0146C12.0915 21.1833 11.8557 21.3267 11.6586 21.466C11.1181 21.8482 10.4945 21.8214 10.0179 21.5437C9.56811 21.2817 9.25159 20.8038 9.1876 20.2633C9.04713 19.0769 8.79249 16.7542 8.51139 13.3199C8.50238 13.2097 8.48517 13.1778 8.48373 13.1751C8.48275 13.1733 8.48022 13.1686 8.47182 13.1594C8.46243 13.149 8.44366 13.1308 8.40798 13.1041C5.84915 11.191 4.03146 9.09132 3.05314 7.92893C2.87044 7.71185 2.71884 7.49783 2.61076 7.23754C2.50494 6.98273 2.45983 6.7282 2.42607 6.46581C2.42375 6.44778 2.42144 6.42982 2.41914 6.41192C2.32392 5.67203 2.24226 5.03744 2.25016 4.52659C2.25863 3.97957 2.36905 3.44445 2.77244 3.00041C3.17232 2.56023 3.69466 2.3921 4.24094 2.31762C4.75704 2.24725 5.4112 2.24728 6.18176 2.24732ZM4.44357 3.80387C4.05947 3.85624 3.94377 3.94181 3.88271 4.00903C3.82515 4.07239 3.75565 4.18397 3.74998 4.5498C3.74395 4.93944 3.80975 5.46543 3.91382 6.27444C3.94273 6.49924 3.96807 6.59485 3.99606 6.66228C4.02179 6.72422 4.06736 6.80453 4.20077 6.96304C5.15905 8.10163 6.8881 10.0949 9.30617 11.9027C9.50044 12.048 9.67868 12.2278 9.80763 12.4699C9.93468 12.7085 9.98668 12.9567 10.0064 13.1975C10.286 16.6131 10.5388 18.9178 10.6772 20.0869C10.6814 20.1226 10.6944 20.1578 10.7144 20.1884C10.7348 20.2197 10.7575 20.2386 10.773 20.2476C10.7749 20.2488 10.7767 20.2497 10.7781 20.2504C10.7817 20.2485 10.7865 20.2456 10.7926 20.2413C11.0347 20.0701 11.3157 19.8993 11.581 19.7382C11.6074 19.7221 11.6336 19.7062 11.6597 19.6903C11.9585 19.5085 12.2442 19.3318 12.5036 19.1439C13.0505 18.7477 13.3187 18.4124 13.3766 18.1034C13.523 17.3216 13.7369 15.9154 13.9834 13.1911C14.0286 12.692 14.2736 12.2161 14.6833 11.91C17.1063 10.0998 18.8389 8.10302 19.7985 6.96301C19.9148 6.82476 19.9666 6.73562 19.9984 6.65892C20.0319 6.57834 20.06 6.47171 20.0854 6.27444C20.1895 5.46543 20.2553 4.93944 20.2493 4.5498C20.2436 4.18397 20.1741 4.07239 20.1165 4.00903C20.0555 3.94181 19.9398 3.85624 19.5557 3.80387C19.153 3.74897 18.6023 3.74732 17.7653 3.74732H6.23394C5.3969 3.74732 4.84624 3.74897 4.44357 3.80387ZM10.7703 20.2539C10.7703 20.2539 10.7708 20.2537 10.7717 20.2535L10.7703 20.2539Z"
                        fill="#B69265"
                      />
                    </svg>

                    <VMenu
                      activator="parent"
                      width="380px"
                      location="bottom end"
                      offset="14px"
                    >
                      <VList class="py-0 text-dark">
                        <VListItem
                          title="تصفية"
                          class="notification-section"
                          height="48px"
                        >
                        </VListItem>

                        <VDivider />

                        <template>
                          <VListItem
                            title="notification.title"
                            subtitle="notification.subtitle"
                            link
                            lines="one"
                            min-height="60px"
                            class="text-dark"
                          >
                            <!-- Slot: Prepend -->
                            <!-- Handles Avatar: Image, Icon, Text -->
                            <template #prepend>
                              <VListItemAction start>
                                <div>
                                  <svg
                                    width="3.5rem"
                                    height="3.2rem"
                                    viewBox="0 0 28 29"
                                    fill="none"
                                    xmlns="http://www.w3.org/2000/svg"
                                  >
                                    <rect
                                      y="0.5"
                                      width="28"
                                      height="28"
                                      rx="4"
                                      fill="#e8e4df"
                                    />
                                    <mask
                                      id="mask0_20_50273"
                                      style="mask-type: luminance"
                                      maskUnits="userSpaceOnUse"
                                      x="4"
                                      y="4"
                                      width="20"
                                      height="21"
                                    >
                                      <path
                                        d="M23.65 24.15V4.85H4.35V24.15H23.65Z"
                                        fill="white"
                                        stroke="white"
                                        stroke-width="0.7"
                                      />
                                    </mask>
                                    <g mask="url(#mask0_20_50273)">
                                      <path
                                        d="M9.25344 10.2832H4.29297"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M23.7066 10.2832H18.7461"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M22.7447 12.2831C22.7447 11.9519 22.4763 11.6835 22.1451 11.6835H19.1916C18.448 11.6835 17.7658 11.2708 17.4206 10.6122C17.0754 9.95352 16.3932 9.54086 15.6496 9.54086H12.3509C11.6073 9.54086 10.9251 9.95352 10.5799 10.6122C10.2347 11.2708 9.55254 11.6835 8.80891 11.6835H5.85543C5.52426 11.6835 5.25586 11.9519 5.25586 12.2831C5.25586 12.9533 5.7791 13.5069 6.44824 13.5447L10.5011 13.7735C10.903 13.7962 11.2866 13.9487 11.5944 14.208C11.9296 14.4905 12.3539 14.6455 12.7924 14.6455H15.2081C15.6466 14.6455 16.0709 14.4905 16.4062 14.208C16.7139 13.9487 17.0976 13.7962 17.4994 13.7735L21.5523 13.5447C22.2214 13.5069 22.7447 12.9533 22.7447 12.2831Z"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M16.9428 13.8916L16.2184 13.4361C16.0002 13.299 15.7478 13.2262 15.4901 13.2262H12.5054C12.2477 13.2262 11.9953 13.299 11.7771 13.4361L11.0527 13.8916"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M7.74011 13.6176L6.73058 19.2731C6.68933 19.5044 6.86714 19.7168 7.1021 19.7168C7.26027 19.7168 7.40167 19.6182 7.45628 19.4697L9.57179 13.721"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M20.3997 14.4014L20.2598 13.6176"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M18.4277 13.721L20.5432 19.4698C20.5979 19.6182 20.7393 19.7168 20.8974 19.7168C21.1323 19.7168 21.3102 19.5044 21.2689 19.2731L20.6411 15.7562"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M7.42473 11.6836V9.93547C7.42473 9.57547 7.13289 9.28367 6.77289 9.28367C6.41293 9.28367 6.12109 9.57547 6.12109 9.93547V11.6836"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M21.8779 11.6836V9.93547C21.8779 9.57547 21.586 9.28367 21.226 9.28367C20.8661 9.28367 20.5742 9.57547 20.5742 9.93547V11.6836"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M11.6035 17.1964V16.8266C11.6035 16.44 11.9169 16.1266 12.3035 16.1266H15.6993C16.0858 16.1266 16.3992 16.44 16.3992 16.8266V18.9387C16.3992 19.3253 16.0858 19.6387 15.6993 19.6387H12.3035C11.9169 19.6387 11.6035 19.3253 11.6035 18.9387V18.5662"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M14.7066 17.883C14.7066 18.2729 14.3906 18.5889 14.0008 18.5889C13.6109 18.5889 13.2949 18.2729 13.2949 17.883C13.2949 17.4932 13.6109 17.1773 14.0008 17.1773C14.3906 17.1773 14.7066 17.4932 14.7066 17.883Z"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M14.9316 16.127V14.6458"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                      <path
                                        d="M13.0684 14.6458V16.127"
                                        stroke="#B69265"
                                        stroke-width="0.7"
                                        stroke-miterlimit="10"
                                        stroke-linecap="round"
                                        stroke-linejoin="round"
                                      />
                                    </g>
                                  </svg>
                                </div>
                              </VListItemAction>
                            </template>
                          </VListItem>
                          <VDivider />
                        </template>

                        <VListItem class="notification-section">
                          <VBtn color="dark" variant="outlined" block>
                            تطبيق التصفية
                          </VBtn>
                        </VListItem>
                      </VList>
                    </VMenu>
                  </VBtn>

                  <VBtn
                    height="48"
                    elevation="0"
                    class="rounded-lg"
                    append-icon="mdi-plus"
                    @click="addNewLocation"
                  >
                    إضافة موقع جديد
                  </VBtn>
                </div>
              </div>
              <!-- Search and Add Section -->

              <!-- Tabs Section -->
              <div class="tabs-container mb-6 border-b-0">
                <div class="d-flex">
                  <div
                    class="tab-item px-4 py-2"
                    :class="{ active: activeTab === 'all' }"
                    @click="activeTab = 'all'"
                  >
                    جميع المواقع
                  </div>
                  <div
                    class="tab-item px-4 py-2"
                    :class="{ active: activeTab === 'correction' }"
                    @click="activeTab = 'correction'"
                  >
                    تصحيح المواقع
                  </div>
                </div>
                <div class="tab-border"></div>
              </div>

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
                  <SiteCard
                    :site-id="site.siteId"
                    :authorization-number="site.authorizationNumber"
                    :status="site.status"
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
                  placeholder="رقم تصريح السكن"
                  hide-details
                  variant="outlined"
                  density="comfortable"
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
