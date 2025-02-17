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
      <v-row>
        <v-col cols="12" md="6">
          <StatsWidget title="الفرق الميدانية" number="23">
            <template #icon>
              <svg
                width="2.2rem"
                height="2.2rem"
                viewBox="0 0 32 33"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  d="M19.4065 3.66663L12.5935 3.66663C10.7701 3.6666 9.30027 3.66658 8.14433 3.82199C6.94418 3.98335 5.93368 4.32854 5.13113 5.13109C4.32857 5.93365 3.98338 6.94415 3.82202 8.14429C3.66661 9.30024 3.66663 10.77 3.66666 12.5934L3.66666 22.0863C3.66663 22.8462 3.66661 23.4915 3.72175 24.0247C3.7378 24.18 3.74582 24.2576 3.78571 24.3144C3.83836 24.3894 3.93768 24.4352 4.02889 24.4266C4.09799 24.4202 4.16988 24.3707 4.31367 24.2717C4.83058 23.9158 5.41457 23.5556 6.02088 23.2458C7.11095 22.6889 8.4922 22.1791 9.72441 22.4016C10.5943 22.5586 11.2279 23.0767 11.78 23.7346C12.304 24.3588 12.8495 25.2274 13.5032 26.2683L13.5609 26.3602L15.8009 30.5754C15.9108 30.7822 15.9658 30.8856 16.0609 30.9428C16.156 31 16.2731 31 16.5073 31H19.4065C21.2299 31 22.6997 31 23.8557 30.8446C25.0558 30.6832 26.0663 30.3381 26.8689 29.5355C27.6714 28.7329 28.0166 27.7224 28.178 26.5223C28.3334 25.3663 28.3334 23.8966 28.3333 22.0731V12.5935C28.3334 10.77 28.3334 9.30025 28.178 8.14429C28.0166 6.94415 27.6714 5.93365 26.8689 5.13109C26.0663 4.32854 25.0558 3.98335 23.8557 3.82199C22.6997 3.66658 21.2299 3.6666 19.4065 3.66663Z"
                  fill="#B69265"
                />
                <path
                  d="M9.33331 2C10.0697 2 10.6666 2.59695 10.6666 3.33333V6C10.6666 6.73638 10.0697 7.33333 9.33331 7.33333C8.59693 7.33333 7.99998 6.73638 7.99998 6L7.99998 3.33333C7.99998 2.59695 8.59693 2 9.33331 2Z"
                  fill="#B69265"
                />
                <path
                  d="M16 2C16.7364 2 17.3333 2.59695 17.3333 3.33333V6C17.3333 6.73638 16.7364 7.33333 16 7.33333C15.2636 7.33333 14.6666 6.73638 14.6666 6L14.6666 3.33333C14.6666 2.59695 15.2636 2 16 2Z"
                  fill="#B69265"
                />
                <path
                  d="M22.6666 2C23.403 2 24 2.59695 24 3.33333V6C24 6.73638 23.403 7.33333 22.6666 7.33333C21.9303 7.33333 21.3333 6.73638 21.3333 6V3.33333C21.3333 2.59695 21.9303 2 22.6666 2Z"
                  fill="#B69265"
                />
                <path
                  d="M13.1196 30.6691C13.3475 30.7704 13.4614 30.8211 13.5263 30.7515C13.5911 30.682 13.53 30.5671 13.4079 30.3373L11.8278 27.3639C11.1361 26.2628 10.6677 25.5227 10.2472 25.0216C9.84068 24.5373 9.58675 24.4105 9.36809 24.371C8.82828 24.2735 7.96282 24.5003 6.92988 25.0281C6.38249 25.3077 5.84896 25.6401 5.38129 25.9657C5.13412 26.1377 5.01054 26.2238 5.01304 26.366C5.01554 26.5083 5.13657 26.5864 5.37862 26.7425L5.41354 26.7649C5.86583 27.0527 6.44295 27.3412 7.1227 27.6811L12.5 30.3697C12.7237 30.4817 12.9259 30.5829 13.1196 30.6691Z"
                  fill="#B69265"
                />
                <path
                  d="M10.6666 11.6667C10.1144 11.6667 9.66665 12.1144 9.66665 12.6667C9.66665 13.219 10.1144 13.6667 10.6666 13.6667L16 13.6667C16.5523 13.6667 17 13.219 17 12.6667C17 12.1144 16.5523 11.6667 16 11.6667L10.6666 11.6667Z"
                  fill="#B69265"
                />
                <path
                  d="M10.6666 18.3333C10.1144 18.3333 9.66665 18.781 9.66665 19.3333C9.66665 19.8856 10.1144 20.3333 10.6666 20.3333L21.3333 20.3333C21.8856 20.3333 22.3333 19.8856 22.3333 19.3333C22.3333 18.781 21.8856 18.3333 21.3333 18.3333L10.6666 18.3333Z"
                  fill="#B69265"
                />
              </svg>
            </template>
          </StatsWidget>
        </v-col>
        <v-col cols="12" md="6">
          <StatsWidget title="الفرق الميدانية" number="23">
            <template #icon>
              <svg
                width="2.2rem"
                height="2.2rem"
                viewBox="0 0 32 33"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  d="M19.4065 3.66663L12.5935 3.66663C10.7701 3.6666 9.30027 3.66658 8.14433 3.82199C6.94418 3.98335 5.93368 4.32854 5.13113 5.13109C4.32857 5.93365 3.98338 6.94415 3.82202 8.14429C3.66661 9.30024 3.66663 10.77 3.66666 12.5934L3.66666 22.0863C3.66663 22.8462 3.66661 23.4915 3.72175 24.0247C3.7378 24.18 3.74582 24.2576 3.78571 24.3144C3.83836 24.3894 3.93768 24.4352 4.02889 24.4266C4.09799 24.4202 4.16988 24.3707 4.31367 24.2717C4.83058 23.9158 5.41457 23.5556 6.02088 23.2458C7.11095 22.6889 8.4922 22.1791 9.72441 22.4016C10.5943 22.5586 11.2279 23.0767 11.78 23.7346C12.304 24.3588 12.8495 25.2274 13.5032 26.2683L13.5609 26.3602L15.8009 30.5754C15.9108 30.7822 15.9658 30.8856 16.0609 30.9428C16.156 31 16.2731 31 16.5073 31H19.4065C21.2299 31 22.6997 31 23.8557 30.8446C25.0558 30.6832 26.0663 30.3381 26.8689 29.5355C27.6714 28.7329 28.0166 27.7224 28.178 26.5223C28.3334 25.3663 28.3334 23.8966 28.3333 22.0731V12.5935C28.3334 10.77 28.3334 9.30025 28.178 8.14429C28.0166 6.94415 27.6714 5.93365 26.8689 5.13109C26.0663 4.32854 25.0558 3.98335 23.8557 3.82199C22.6997 3.66658 21.2299 3.6666 19.4065 3.66663Z"
                  fill="#B69265"
                />
                <path
                  d="M9.33331 2C10.0697 2 10.6666 2.59695 10.6666 3.33333V6C10.6666 6.73638 10.0697 7.33333 9.33331 7.33333C8.59693 7.33333 7.99998 6.73638 7.99998 6L7.99998 3.33333C7.99998 2.59695 8.59693 2 9.33331 2Z"
                  fill="#B69265"
                />
                <path
                  d="M16 2C16.7364 2 17.3333 2.59695 17.3333 3.33333V6C17.3333 6.73638 16.7364 7.33333 16 7.33333C15.2636 7.33333 14.6666 6.73638 14.6666 6L14.6666 3.33333C14.6666 2.59695 15.2636 2 16 2Z"
                  fill="#B69265"
                />
                <path
                  d="M22.6666 2C23.403 2 24 2.59695 24 3.33333V6C24 6.73638 23.403 7.33333 22.6666 7.33333C21.9303 7.33333 21.3333 6.73638 21.3333 6V3.33333C21.3333 2.59695 21.9303 2 22.6666 2Z"
                  fill="#B69265"
                />
                <path
                  d="M13.1196 30.6691C13.3475 30.7704 13.4614 30.8211 13.5263 30.7515C13.5911 30.682 13.53 30.5671 13.4079 30.3373L11.8278 27.3639C11.1361 26.2628 10.6677 25.5227 10.2472 25.0216C9.84068 24.5373 9.58675 24.4105 9.36809 24.371C8.82828 24.2735 7.96282 24.5003 6.92988 25.0281C6.38249 25.3077 5.84896 25.6401 5.38129 25.9657C5.13412 26.1377 5.01054 26.2238 5.01304 26.366C5.01554 26.5083 5.13657 26.5864 5.37862 26.7425L5.41354 26.7649C5.86583 27.0527 6.44295 27.3412 7.1227 27.6811L12.5 30.3697C12.7237 30.4817 12.9259 30.5829 13.1196 30.6691Z"
                  fill="#B69265"
                />
                <path
                  d="M10.6666 11.6667C10.1144 11.6667 9.66665 12.1144 9.66665 12.6667C9.66665 13.219 10.1144 13.6667 10.6666 13.6667L16 13.6667C16.5523 13.6667 17 13.219 17 12.6667C17 12.1144 16.5523 11.6667 16 11.6667L10.6666 11.6667Z"
                  fill="#B69265"
                />
                <path
                  d="M10.6666 18.3333C10.1144 18.3333 9.66665 18.781 9.66665 19.3333C9.66665 19.8856 10.1144 20.3333 10.6666 20.3333L21.3333 20.3333C21.8856 20.3333 22.3333 19.8856 22.3333 19.3333C22.3333 18.781 21.8856 18.3333 21.3333 18.3333L10.6666 18.3333Z"
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
          <v-card-title
            class="d-flex align-center justify-space-between ga-4 pt-4"
          >
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
          </v-card-title>
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
              />
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
        <v-card-text class="pa-6">
          <h2 class="text-h6">
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
