<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/stores/app.store'
import { logger } from '@/@core/utils/logger'
import StatsWidget from '@/@core/components/widgets/StatsWidget.vue'
import Pagination from '@/@core/components/Pagination.vue'
import dataTable from '@/components/dataTable.vue'
import Editicon from '@/components/icons/editicon.vue'
import { dashboardService } from '@/services/dashboard.service'
import { companiesService } from '@/services/companies.service'
import { flightsService } from '@/services/flights.service'
import { pilgrimsService } from '@/services/pilgrims.service'
import { useTablePage } from '@/composables/useTablePage'

const { t } = useI18n()
const appStore = useAppStore()

const companyHeaders = ref([
  { title: t('dashboard.headers.companies.licenseNumber'), value: 'licenseNumber' },
  { title: t('dashboard.headers.companies.companyName'), value: 'companyName' },
  { title: t('dashboard.headers.companies.connectedHujjaj'), value: 'connectedHujjaj', align: 'start' },
  { title: t('dashboard.headers.companies.contactNumber'), value: 'contactNumber' },
  { title: '', value: 'actions' },
])

const flightHeaders = ref([
  { title: t('dashboard.headers.flights.flightNumber'), value: 'flightNumber' },
  { title: t('dashboard.headers.flights.company'), value: 'company' },
  { title: t('dashboard.headers.flights.date'), value: 'date' },
  { title: t('dashboard.headers.flights.time'), value: 'time' },
  { title: t('dashboard.headers.flights.destination'), value: 'destination' },
  { title: t('dashboard.headers.flights.pilgrimsCount'), value: 'noOfHuj' },
])

const pilgrimHeaders = ref([
  { title: t('dashboard.headers.pilgrims.name'), value: 'name' },
  { title: t('dashboard.headers.pilgrims.identity'), value: 'identity' },
  { title: t('dashboard.headers.pilgrims.nationality'), value: 'nationality' },
  { title: t('dashboard.headers.pilgrims.phone'), value: 'phone' },
])

const homeData = ref<{
  data?: {
    statistics?: {
      campsCount?: number
      locationsCount?: number
      mainCompaniesCount?: number
      companiesCount?: number
    }
  }
}>({})
const isLoadingStats = ref(true)
const activeTab = ref('companies')

const {
  items: companies,
  isLoading: isLoadingCompanies,
  currentPage: companyPage,
  perPage: companyPerPage,
  totalItems: companyTotalItems,
  fetchData: fetchCompanies,
} = useTablePage((params) => companiesService.getAll(params), {
  entityName: t('dashboard.tabs.companies'),
  defaultPerPage: 5,
  fetchOnMount: false,
})

const {
  items: flights,
  isLoading: isLoadingFlights,
  currentPage: flightPage,
  perPage: flightPerPage,
  totalItems: flightTotalItems,
  fetchData: fetchFlights,
} = useTablePage((params) => flightsService.getAll(params), {
  entityName: t('dashboard.tabs.flights'),
  defaultPerPage: 5,
  fetchOnMount: false,
})

const {
  items: pilgrims,
  isLoading: isLoadingPilgrims,
  currentPage: pilgrimPage,
  perPage: pilgrimPerPage,
  totalItems: pilgrimTotalItems,
  fetchData: fetchPilgrims,
} = useTablePage((params) => pilgrimsService.getAll(params), {
  entityName: t('dashboard.tabs.pilgrims'),
  defaultPerPage: 5,
  fetchOnMount: false,
})

const currentPage = computed({
  get: () => {
    switch (activeTab.value) {
      case 'companies':
        return companyPage.value
      case 'flights':
        return flightPage.value
      case 'pilgrims':
        return pilgrimPage.value
      default:
        return 1
    }
  },
  set: (val: number) => {
    switch (activeTab.value) {
      case 'companies':
        companyPage.value = val
        break
      case 'flights':
        flightPage.value = val
        break
      case 'pilgrims':
        pilgrimPage.value = val
        break
    }
  },
})

const currentPerPage = computed({
  get: () => {
    switch (activeTab.value) {
      case 'companies':
        return companyPerPage.value
      case 'flights':
        return flightPerPage.value
      case 'pilgrims':
        return pilgrimPerPage.value
      default:
        return 5
    }
  },
  set: (val: number) => {
    switch (activeTab.value) {
      case 'companies':
        companyPerPage.value = val
        break
      case 'flights':
        flightPerPage.value = val
        break
      case 'pilgrims':
        pilgrimPerPage.value = val
        break
    }
  },
})

const currentTotalItems = computed(() => {
  switch (activeTab.value) {
    case 'companies':
      return companyTotalItems.value
    case 'flights':
      return flightTotalItems.value
    case 'pilgrims':
      return pilgrimTotalItems.value
    default:
      return 0
  }
})

const fetchHomeData = async () => {
  try {
    isLoadingStats.value = true
    const response = await dashboardService.getStatistics()
    if (response?.data) {
      homeData.value = response
    }
  } catch (error) {
    logger.error('Error fetching home statistics:', error)
    appStore.showSnackbar({
      message: t('dashboard.error'),
      color: 'error',
    })
  } finally {
    isLoadingStats.value = false
  }
}

const fetchedTabs = new Set<string>()

const fetchTabData = () => {
  if (fetchedTabs.has(activeTab.value)) return
  fetchedTabs.add(activeTab.value)
  switch (activeTab.value) {
    case 'companies':
      fetchCompanies()
      break
    case 'flights':
      fetchFlights()
      break
    case 'pilgrims':
      fetchPilgrims()
      break
  }
}

watch(activeTab, () => {
  fetchTabData()
})

onMounted(async () => {
  await fetchHomeData()
  fetchTabData()
})
</script>

<template>
  <div>
    <v-container class="px-0 px-sm-4 px-xl-0 py-2 pt-0" fluid>
      <!-- Loading Skeleton -->
      <v-row v-if="isLoadingStats">
        <v-col v-for="item in 4" :key="item" cols="6" md="3" sm="6">
          <VCard class="py-2" elevation="0">
            <v-card-text class="px-1 py-2">
              <v-skeleton-loader
                class="mx-auto"
                elevation="0"
                max-width="400"
                type="list-item-two-line, table-heading"
              />
            </v-card-text>
          </VCard>
        </v-col>
      </v-row>

      <!-- Stats Cards -->
      <v-row v-else>
        <v-col cols="6" lg="3" md="3">
          <StatsWidget
            :number="homeData?.data?.statistics?.campsCount || 0"
            :title="t('dashboard.stats.companiesCount')"
          >
            <template #icon>
              <svg fill="none" height="48" viewBox="0 0 48 48" width="48" xmlns="http://www.w3.org/2000/svg">
                <g opacity="0.4">
                  <path
                    d="M41.4478 10.751C42 11.6772 42 12.7748 42 14.9698L42 44H28L28 9.93491C28 6.48917 28 4.7663 29.2323 4.17425C30.4647 3.58219 32.0615 4.53787 35.2553 6.44922L37.8439 7.99842C39.8784 9.21599 40.8956 9.82481 41.4478 10.751Z"
                    fill="#9C8055"
                  />
                  <path
                    d="M6.55219 10.751C6 11.6772 6 12.7748 6 14.9698L6 44H20L20 9.93491C20 6.48917 20 4.7663 18.7677 4.17425C17.5353 3.58219 15.9385 4.53787 12.7447 6.44922L10.1561 7.99842C8.12162 9.21601 7.10437 9.8248 6.55219 10.751Z"
                    fill="#9C8055"
                  />
                </g>
                <path
                  clip-rule="evenodd"
                  d="M41.0841 8.27095C41.7496 8.75502 42.317 9.27964 42.7362 9.98291C43.1603 10.6942 43.3401 11.4323 43.423 12.2296C43.5001 12.9722 43.5001 13.8617 43.5 14.8979L43.5 42.5H44C44.8284 42.5 45.5 43.1716 45.5 44C45.5 44.8284 44.8284 45.5 44 45.5H42.0049L42 45.5H28L27.9952 45.5H20.0048L20 45.5L5.99999 45.5L5.99515 45.5H4C3.17157 45.5 2.5 44.8284 2.5 44C2.5 43.1716 3.17157 42.5 4 42.5H4.49999L4.49998 14.8979C4.49994 13.8617 4.49991 12.9722 4.57704 12.2296C4.65985 11.4323 4.8397 10.6942 5.26376 9.98291C5.68303 9.27964 6.25043 8.75502 6.91593 8.27095C7.54839 7.81091 8.36017 7.3251 9.32949 6.74503L12.0724 5.10351C13.5856 4.19781 14.8572 3.43671 15.9062 2.99196C16.9808 2.53633 18.207 2.24075 19.4172 2.8222C20.6853 3.43144 21.1402 4.61206 21.326 5.74073C21.5002 6.79867 21.5001 8.19343 21.5 9.8046L21.5 16.5H26.5L26.5 9.80463C26.4999 8.19346 26.4998 6.79867 26.674 5.74073C26.8598 4.61206 27.3147 3.43144 28.5828 2.8222C29.793 2.24075 31.0192 2.53633 32.0938 2.99196C33.1428 3.43671 34.4143 4.19774 35.9275 5.10344L38.6705 6.74505C39.6396 7.32496 40.4518 7.811 41.0841 8.27095ZM21.5 19.5L26.5 19.5L26.5 28.5L21.5 28.5L21.5 19.5ZM21.5 31.5L26.5 31.5L26.5 42.5H21.5L21.5 31.5Z"
                  fill="#9C8055"
                  fill-rule="evenodd"
                />
              </svg>
            </template>
          </StatsWidget>
        </v-col>
        <v-col cols="6" lg="3" md="3">
          <StatsWidget
            :number="homeData?.data?.statistics?.locationsCount || 0"
            :title="t('dashboard.stats.pilgrimsInside')"
          >
            <template #icon>
              <svg fill="none" height="48" viewBox="0 0 48 48" width="48" xmlns="http://www.w3.org/2000/svg">
                <g opacity="0.4">
                  <path
                    d="M16.9998 17C16.9998 19.7614 14.7612 22 11.9998 22C9.23833 22 6.99976 19.7614 6.99976 17C6.99976 14.2386 9.23833 12 11.9998 12C14.7612 12 16.9998 14.2386 16.9998 17Z"
                    fill="#9C8055"
                  />
                  <path
                    d="M40.9998 17C40.9998 19.7614 38.7612 22 35.9998 22C33.2383 22 30.9998 19.7614 30.9998 17C30.9998 14.2386 33.2383 12 35.9998 12C38.7612 12 40.9998 14.2386 40.9998 17Z"
                    fill="#9C8055"
                  />
                  <path
                    d="M15.471 30.6378C15.7281 30.4868 15.9641 30.3481 16.1674 30.2224C20.9595 27.2592 27.04 27.2592 31.8321 30.2224C32.0354 30.3481 32.2714 30.4868 32.5285 30.6378C34.8562 32.0056 38.9092 34.3873 35.9704 37.2948C34.3762 38.872 32.6007 40 30.3685 40H17.631C15.3988 40 13.6233 38.872 12.0292 37.2948C9.0903 34.3873 13.1433 32.0057 15.471 30.6378Z"
                    fill="#9C8055"
                  />
                </g>
                <path
                  clip-rule="evenodd"
                  d="M15.4998 15C15.4998 10.3056 19.3053 6.5 23.9998 6.5C28.6942 6.5 32.4998 10.3056 32.4998 15C32.4998 19.6944 28.6942 23.5 23.9998 23.5C19.3053 23.5 15.4998 19.6944 15.4998 15ZM23.9998 9.5C20.9622 9.5 18.4998 11.9624 18.4998 15C18.4998 18.0376 20.9622 20.5 23.9998 20.5C27.0373 20.5 29.4998 18.0376 29.4998 15C29.4998 11.9624 27.0373 9.5 23.9998 9.5Z"
                  fill="#9C8055"
                  fill-rule="evenodd"
                />
                <path
                  clip-rule="evenodd"
                  d="M15.3788 28.9466C20.6543 25.6845 27.3458 25.6845 32.6213 28.9466C32.7768 29.0427 32.9737 29.158 33.2 29.2904C34.2246 29.8899 35.8511 30.8417 36.961 31.9698C37.658 32.6783 38.3532 33.6442 38.4799 34.8484C38.6156 36.1367 38.0688 37.3291 37.0256 38.3612C35.3024 40.0661 33.1677 41.5 30.3688 41.5H17.6313C14.8324 41.5 12.6977 40.0661 10.9745 38.3612C9.93135 37.3291 9.38455 36.1367 9.52015 34.8484C9.6469 33.6442 10.3421 32.6783 11.0391 31.9698C12.149 30.8417 13.7753 29.89 14.7999 29.2905C15.0262 29.158 15.2232 29.0428 15.3788 28.9466ZM31.0436 31.4982C26.7349 28.8339 21.2652 28.8339 16.9565 31.4982C16.6979 31.6581 16.4206 31.8216 16.1345 31.9903C15.1107 32.594 13.9742 33.2641 13.1777 34.0737C12.6877 34.5717 12.5276 34.9352 12.5037 35.1624C12.4886 35.3054 12.4958 35.6462 13.0844 36.2285C14.5495 37.678 15.9658 38.5 17.6313 38.5H30.3688C32.0343 38.5 33.4506 37.678 34.9157 36.2285C35.5043 35.6462 35.5115 35.3054 35.4964 35.1624C35.4725 34.9352 35.3124 34.5717 34.8224 34.0737C34.0259 33.2641 32.8895 32.594 31.8657 31.9904C31.5796 31.8217 31.3022 31.6581 31.0436 31.4982Z"
                  fill="#9C8055"
                  fill-rule="evenodd"
                />
              </svg>
            </template>
          </StatsWidget>
        </v-col>
        <v-col cols="6" lg="3" md="3">
          <StatsWidget
            :number="homeData.data?.statistics?.mainCompaniesCount || 0"
            :title="t('dashboard.stats.flightsCount')"
          >
            <template #icon>
              <svg fill="none" height="48" viewBox="0 0 48 48" width="48" xmlns="http://www.w3.org/2000/svg">
                <path
                  d="M5.53055 17.8256L4.95655 15.6716C4.55455 14.1656 4.35255 13.4117 4.63055 12.7857C5.11655 11.6937 6.55655 11.5057 7.57655 11.2317C8.55655 10.9657 9.04856 10.8336 9.51256 10.9296C10.6226 11.1576 11.4405 12.4996 12.1665 13.3056C13.8265 15.1516 14.6225 15.2736 16.8485 14.1996C20.6145 12.3816 24.3685 10.5397 28.1465 8.74769C33.7065 6.11169 40.4945 7.77565 44.1465 12.7577C44.9605 13.8897 44.2925 15.4357 43.0005 15.7857L32.6846 18.5657C32.1546 18.7097 31.7086 19.0717 31.4606 19.5657L29.0046 24.4216C28.4746 25.3196 27.6065 25.9776 26.5905 26.2516L22.0685 27.4697C21.3565 27.6617 20.6985 27.0517 20.8505 26.3397L21.7566 22.0977C21.8166 21.8137 21.5565 21.5637 21.2765 21.6397L15.1885 23.2796C11.5805 24.2536 9.77456 24.7397 8.36056 23.9397C6.94656 23.1417 6.47455 21.3696 5.53055 17.8256Z"
                  fill="#9C8055"
                  opacity="0.4"
                />
                <path
                  clip-rule="evenodd"
                  d="M21.171 28.9392C21.271 28.9672 21.439 29.0051 21.713 29.0051C22.133 29.0051 22.805 28.9132 23.871 28.5572C23.9618 28.5269 24.2432 28.4547 24.5379 28.379C24.613 28.3597 24.689 28.3402 24.763 28.3211C24.8047 28.3104 24.8471 28.2995 24.8903 28.2884C25.6926 28.0821 26.7393 27.813 27.489 27.5511L27.509 27.5432C29.025 26.9932 30.139 25.6912 30.605 24.5992C30.835 24.1972 31.197 23.4712 31.737 22.3772C32.105 21.6312 32.651 20.5271 32.841 20.2011C32.889 20.1531 32.947 20.1032 33.039 20.0692C34.2539 19.7302 36.3276 19.1688 38.2325 18.653C38.598 18.5541 38.9572 18.4568 39.303 18.3631L39.8104 18.2257C41.7783 17.6928 42.9054 17.3876 43.127 17.3211L43.205 17.2972L43.2081 17.2963C43.7806 17.1276 45.286 16.6839 45.829 14.9251L45.851 14.8472C46.277 13.1432 45.691 12.2911 44.699 11.0511C44.073 10.2291 42.177 8.39313 39.341 7.17513C35.695 5.60713 31.761 5.61716 27.967 7.19916L27.929 7.21517C27.4247 7.44152 24.6851 8.76374 21.7709 10.1703L21.707 10.2011C19.107 11.4571 15.873 13.0172 15.567 13.1492C15.123 13.3332 14.809 13.4092 14.579 13.3892C14.425 13.3752 14.117 13.1852 13.905 12.9732L13.799 12.8671L13.741 12.8291C13.623 12.7091 13.357 12.3912 13.159 12.1532C11.997 10.7612 11.149 9.81714 10.181 9.58314C9.32699 9.32714 8.47299 9.46914 8.03099 9.57914L7.96898 9.59515C5.54298 10.1992 5.05299 10.4272 4.75899 10.6072C3.92099 11.1232 3.45299 11.6492 3.15699 12.4152C2.81499 13.2252 3.04098 14.2832 3.74498 16.9652C4.22498 18.7912 4.87099 21.1011 5.08299 21.6871C5.76099 23.6571 6.42098 24.4352 7.26498 25.0492L7.37499 25.1232C8.43099 25.7572 9.26698 25.8871 10.197 25.8871C10.757 25.8871 12.603 25.6231 15.985 24.6371C16.797 24.4391 17.981 24.1251 19.167 23.7911C19.429 23.7311 19.679 23.6692 19.911 23.6072C19.775 24.2952 19.551 25.3831 19.425 25.8251C19.225 26.5271 19.305 27.2372 19.655 27.8232C19.985 28.3792 20.537 28.7832 21.171 28.9392Z"
                  fill="#9C8055"
                  fill-rule="evenodd"
                />
                <path
                  d="M4.50195 41.0059H36.502C37.33 41.0059 38.002 40.3339 38.002 39.5059C38.002 38.6779 37.33 38.0059 36.502 38.0059H4.50195C3.67395 38.0059 3.00195 38.6779 3.00195 39.5059C3.00195 40.3339 3.67395 41.0059 4.50195 41.0059Z"
                  fill="#9C8055"
                />
              </svg>
            </template>
          </StatsWidget>
        </v-col>
        <v-col cols="6" lg="3" md="3">
          <StatsWidget
            :number="homeData.data?.statistics?.companiesCount || 0"
            :title="t('dashboard.stats.avgWaitTime')"
          >
            <template #icon>
              <svg fill="none" height="44" viewBox="0 0 44 44" width="44" xmlns="http://www.w3.org/2000/svg">
                <circle cx="22" cy="22" fill="#9C8055" opacity="0.4" r="20" />
                <path
                  d="M23.5 14C23.5 13.1716 22.8284 12.5 22 12.5C21.1716 12.5 20.5 13.1716 20.5 14V22C20.5 22.3978 20.658 22.7794 20.9393 23.0607L24.9393 27.0607C25.5251 27.6464 26.4749 27.6464 27.0607 27.0607C27.6464 26.4749 27.6464 25.5251 27.0607 24.9393L23.5 21.3787V14Z"
                  fill="#9C8055"
                />
                <path
                  clip-rule="evenodd"
                  d="M22 0.5C10.1259 0.5 0.5 10.1259 0.5 22C0.5 33.8741 10.1259 43.5 22 43.5C33.8741 43.5 43.5 33.8741 43.5 22C43.5 10.1259 33.8741 0.5 22 0.5ZM3.5 22C3.5 11.7827 11.7827 3.5 22 3.5C32.2173 3.5 40.5 11.7827 40.5 22C40.5 32.2173 32.2173 40.5 22 40.5C11.7827 40.5 3.5 32.2173 3.5 22Z"
                  fill="#9C8055"
                  fill-rule="evenodd"
                />
              </svg>
            </template>
          </StatsWidget>
        </v-col>
      </v-row>

      <!-- Tabs & Data Table -->
      <v-row>
        <v-col cols="12">
          <VCard class="rounded-lg px-4 bg-light-white" elevation="0">
            <v-card-title class="pt-4 pb-6">
              <div>
                <div class="d-flex justify-space-between align-center">
                  <div class="d-flex gap-2 border px-2 py-1" style="border-radius: 16px; max-width: fit-content">
                    <v-btn
                      v-for="tab in [
                        { id: 'companies', label: t('dashboard.tabs.companies') },
                        { id: 'flights', label: t('dashboard.tabs.flights') },
                        { id: 'pilgrims', label: t('dashboard.tabs.pilgrims') },
                      ]"
                      :key="tab.id"
                      class="rounded-lg font-weight-bold"
                      :class="{
                        'bg-light-gray text-primary': activeTab === tab.id,
                        'bg-white text-gray-1': activeTab !== tab.id,
                      }"
                      elevation="0"
                      @click="activeTab = tab.id"
                    >
                      <span>{{ tab.label }}</span>
                    </v-btn>
                  </div>
                </div>
              </div>

              <!-- Tab Content -->
              <v-window v-model="activeTab" class="mt-6">
                <!-- Companies Tab -->
                <v-window-item value="companies">
                  <v-skeleton-loader v-if="isLoadingCompanies" type="table-heading, table-thead, table-tbody@3" />
                  <dataTable
                    v-else
                    v-model:page="companyPage"
                    class="elevation-0"
                    :headers="companyHeaders"
                    :hide-default-footer="true"
                    :items="companies"
                    :items-per-page="companyPerPage"
                  >
                    <!-- eslint-disable-next-line vue/no-unused-vars -->
                    <template #[`item.actions`]="{ item: _item }">
                      <v-btn class="text-black" color="#fff" :prepend-icon="Editicon">
                        {{ t('dashboard.editButton') }}
                      </v-btn>
                    </template>
                  </dataTable>
                </v-window-item>

                <!-- Flights Tab -->
                <v-window-item value="flights">
                  <v-skeleton-loader v-if="isLoadingFlights" type="table-heading, table-thead, table-tbody@3" />
                  <dataTable
                    v-else-if="flights.length"
                    v-model:page="flightPage"
                    class="elevation-0"
                    :headers="flightHeaders"
                    :hide-default-footer="true"
                    :items="flights"
                    :items-per-page="flightPerPage"
                  />
                  <v-row v-else>
                    <v-col class="text-center" cols="12">
                      <p class="text-medium-emphasis">{{ t('dashboard.noData') }}</p>
                    </v-col>
                  </v-row>
                </v-window-item>

                <!-- Pilgrims Tab -->
                <v-window-item value="pilgrims">
                  <v-skeleton-loader v-if="isLoadingPilgrims" type="table-heading, table-thead, table-tbody@3" />
                  <dataTable
                    v-else-if="pilgrims.length"
                    v-model:page="pilgrimPage"
                    class="elevation-0"
                    :headers="pilgrimHeaders"
                    :hide-default-footer="true"
                    :items="pilgrims"
                    :items-per-page="pilgrimPerPage"
                  />
                  <v-row v-else>
                    <v-col class="text-center" cols="12">
                      <p class="text-medium-emphasis">{{ t('dashboard.noData') }}</p>
                    </v-col>
                  </v-row>
                </v-window-item>
              </v-window>

              <!-- Pagination -->
              <Pagination
                v-model:page="currentPage"
                v-model:per-page="currentPerPage"
                :total-items="currentTotalItems"
              />
            </v-card-title>
          </VCard>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}
</style>
