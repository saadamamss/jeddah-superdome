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

      <v-container class="bg-white mt-6">
        <!-- Header Section -->
        <div class="d-flex justify-space-between align-center mb-6">
          <h1 class="text-h5 font-weight-medium">إدارة المواقع</h1>
          <div class="d-flex align-center mb-6">
            <v-text-field
              v-model="search"
              placeholder="البحث للخيم او السكن..."
              variant="outlined"
              density="comfortable"
              hide-details
              class="search-field rounded-lg min-width-300px ps-0"
              bg-color="white"
            >
              <template #prepend-inner>
                <svg
                  width="18"
                  height="18"
                  viewBox="0 0 18 18"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    d="M15.6666 8.16669C15.6666 4.02455 12.3088 0.666687 8.16663 0.666687C4.02449 0.666687 0.666626 4.02455 0.666626 8.16669C0.666626 12.3088 4.02449 15.6667 8.16663 15.6667C12.3088 15.6667 15.6666 12.3088 15.6666 8.16669Z"
                    fill="#B69265"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M8.16663 0.041687C3.67931 0.041687 0.041626 3.67937 0.041626 8.16669C0.041626 12.654 3.67931 16.2917 8.16663 16.2917C10.1853 16.2917 12.032 15.5555 13.453 14.337L16.8914 17.7753C17.1354 18.0194 17.5312 18.0194 17.7752 17.7753C18.0193 17.5312 18.0193 17.1355 17.7752 16.8914L14.3369 13.4531C15.5554 12.0321 16.2916 10.1854 16.2916 8.16669C16.2916 3.67937 12.6539 0.041687 8.16663 0.041687ZM1.29163 8.16669C1.29163 4.36973 4.36967 1.29169 8.16663 1.29169C11.9636 1.29169 15.0416 4.36973 15.0416 8.16669C15.0416 11.9636 11.9636 15.0417 8.16663 15.0417C4.36967 15.0417 1.29163 11.9636 1.29163 8.16669Z"
                    fill="#B69265"
                  />
                </svg>
              </template>
            </v-text-field>
            <v-btn icon variant="text" class="ms-4 me-4 btn-notification">
              <svg
                width="20"
                height="20"
                viewBox="0 0 20 20"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  d="M6.85746 10.5061C4.36901 8.64561 2.59564 6.59915 1.62734 5.44867C1.3276 5.09253 1.22938 4.8319 1.17033 4.3728C0.968115 2.8008 0.867009 2.0148 1.32795 1.5074C1.7889 1 2.60404 1 4.23433 1H15.7657C17.396 1 18.2111 1 18.672 1.5074C19.133 2.0148 19.0319 2.8008 18.8297 4.37281C18.7706 4.83191 18.6724 5.09254 18.3726 5.44867C17.403 6.60062 15.6261 8.65071 13.1326 10.5135C12.907 10.6821 12.7583 10.9567 12.7307 11.2614C12.4837 13.992 12.2559 15.4876 12.1141 16.2442C11.8853 17.4657 10.1532 18.2006 9.22601 18.8563C8.67406 19.2466 8.0043 18.782 7.93278 18.1778C7.79643 17.0261 7.53961 14.6864 7.25927 11.2614C7.23409 10.9539 7.08486 10.6761 6.85746 10.5061Z"
                  fill="#B69265"
                />
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M4.18176 0.247316C4.1991 0.247317 4.21649 0.247318 4.23394 0.247318L15.8175 0.247316C16.588 0.247281 17.2422 0.247251 17.7583 0.317618C18.3046 0.392099 18.8269 0.560228 19.2268 1.00041C19.6302 1.44445 19.7406 1.97957 19.7491 2.52659C19.757 3.03745 19.6753 3.67204 19.5801 4.41193L19.5732 4.46581C19.5395 4.72764 19.489 4.98088 19.3836 5.23442C19.2767 5.49184 19.1294 5.71109 18.946 5.92897C17.9664 7.09285 16.1451 9.19621 13.5811 11.1117C13.5396 11.1427 13.4872 11.2161 13.4773 11.3263C13.2282 14.0792 13.009 15.5358 12.8509 16.3796C12.6799 17.2921 11.9839 17.9237 11.3838 18.3585C11.0697 18.5861 10.7364 18.791 10.4394 18.9717C10.4156 18.9862 10.3921 19.0005 10.3689 19.0146C10.0915 19.1833 9.85574 19.3267 9.65864 19.466C9.11813 19.8482 8.49455 19.8214 8.0179 19.5437C7.56811 19.2817 7.25159 18.8038 7.1876 18.2633C7.04713 17.0769 6.79249 14.7542 6.51139 11.3199C6.50238 11.2097 6.48517 11.1778 6.48373 11.1751C6.48275 11.1733 6.48022 11.1686 6.47182 11.1594C6.46243 11.149 6.44366 11.1308 6.40798 11.1041C3.84915 9.19099 2.03146 7.09132 1.05314 5.92893C0.870436 5.71185 0.718843 5.49783 0.610757 5.23754C0.504945 4.98273 0.459826 4.7282 0.426074 4.46581C0.423755 4.44778 0.421443 4.42982 0.419139 4.41192C0.323922 3.67203 0.242256 3.03744 0.250164 2.52659C0.258631 1.97957 0.369053 1.44445 0.772438 1.00041C1.17232 0.560228 1.69466 0.392099 2.24094 0.317618C2.75704 0.247251 3.4112 0.247281 4.18176 0.247316ZM2.44357 1.80387C2.05947 1.85624 1.94377 1.94181 1.88271 2.00903C1.82515 2.07239 1.75565 2.18397 1.74998 2.5498C1.74395 2.93944 1.80975 3.46543 1.91382 4.27444C1.94273 4.49924 1.96807 4.59485 1.99606 4.66228C2.02179 4.72422 2.06736 4.80453 2.20077 4.96304C3.15905 6.10163 4.8881 8.09487 7.30617 9.90273C7.50044 10.048 7.67868 10.2278 7.80763 10.4699C7.93468 10.7085 7.98668 10.9567 8.00639 11.1975C8.28596 14.6131 8.53877 16.9178 8.67719 18.0869C8.68141 18.1226 8.6944 18.1578 8.71438 18.1884C8.73481 18.2197 8.75752 18.2386 8.77296 18.2476C8.77493 18.2488 8.77666 18.2497 8.77815 18.2504C8.78171 18.2485 8.78651 18.2456 8.79262 18.2413C9.03472 18.0701 9.31574 17.8993 9.58097 17.7382C9.60737 17.7221 9.63362 17.7062 9.65965 17.6903C9.95846 17.5085 10.2442 17.3318 10.5036 17.1439C11.0505 16.7477 11.3187 16.4124 11.3766 16.1034C11.523 15.3216 11.7369 13.9154 11.9834 11.1911C12.0286 10.692 12.2736 10.2161 12.6833 9.91002C15.1063 8.09985 16.8389 6.10302 17.7985 4.96301C17.9148 4.82476 17.9666 4.73562 17.9984 4.65892C18.0319 4.57834 18.06 4.47171 18.0854 4.27444C18.1895 3.46543 18.2553 2.93944 18.2493 2.5498C18.2436 2.18397 18.1741 2.07239 18.1165 2.00903C18.0555 1.94181 17.9398 1.85624 17.5557 1.80387C17.153 1.74897 16.6023 1.74732 15.7653 1.74732H4.23394C3.3969 1.74732 2.84624 1.74897 2.44357 1.80387ZM8.77029 18.2539C8.77033 18.2539 8.77082 18.2537 8.77174 18.2535L8.77029 18.2539Z"
                  fill="#B69265"
                />
              </svg>

              <!-- <v-icon color="#B69265">mdi-filter-variant</v-icon> -->
            </v-btn>

            <v-btn
              prepend-icon="mdi-plus"
              color="#B69265"
              class="text-white me-4 text-none font-weight-medium"
              @click="addNewLocation"
            >
              إضافة موقع جديد
            </v-btn>
          </div>
        </div>

        <!-- Search and Add Section -->

        <!-- Tabs Section -->
        <div class="tabs-container mb-6">
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
      </v-container>
    </div>

    <!-- Add Site Modal -->
    <v-dialog v-model="dialog" width="500" class="add-site-dialog">
      <v-card class="rounded-lg">
        <v-card-text class="pa-6">
          <div class="d-flex justify-space-between align-center mb-4">
            <h2 class="text-h6">إضافة موقع جديد</h2>
            <v-btn icon variant="text" @click="dialog = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </div>
          <p class="text-subtitle-1 text-medium-emphasis mb-6">
            حدد تفاصيل الموقع الجديد
          </p>
          <v-form @submit.prevent="handleSubmit">
            <!-- Site Type -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">نوع الموقع</label>
              <v-select
                v-model="siteType"
                :items="[{ title: 'السكن', value: 'السكن' }]"
                variant="outlined"
                density="comfortable"
                placeholder="اختر نوع الموقع"
                hide-details
                class="rounded-lg bg-grey-lighten-4"
              />
            </div>

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
                  class="rounded-lg bg-grey-lighten-4"
                />
                <v-select
                  value="+966"
                  :items="[{ title: '+966', value: '+966' }]"
                  variant="outlined"
                  density="comfortable"
                  hide-details
                  class="country-code-select ms-2 rounded-lg bg-grey-lighten-4"
                  style="width: 120px"
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
              />
            </div>

            <!-- Map -->
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">الاحداثيات</label>
              <div
                class="map-container rounded-lg overflow-hidden"
                style="height: 200px"
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
                @click="dialog = false"
              >
                إلغاء
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
</style>
