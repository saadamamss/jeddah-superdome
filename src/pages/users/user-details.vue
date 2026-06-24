<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useAppStore } from '@/stores/app.store'
import UserStatisticsCard from '@/@core/components/widgets/UserStatisticsCard.vue'
import UserWidget from '@/@core/components/widgets/UserWidget.vue'
import { useRoute } from 'vue-router'
import BreadcrumbNav from '@/components/BreadcrumbNav.vue'
import { useBreadcrumbs } from '@/composables/useBreadcrumbs'
import axios from '@/plugins/axios'

// Store
const appStore = useAppStore()
const route = useRoute()

// State
const isLoading = ref(true)
const loading = ref(false)

// Form data
const userForm = ref({
  firstname: '',
  secondname: '',
  thirdname: '',
  lastname: '',
  email: '',
  phone: '',
  city: '',
  bloodType: '',
  gender: '',
  nationality: '',
  saId: '',
  roleId: null,
  personalImage: '',
})

// Available roles for selection
const roles = ref([
  { id: 1, title: 'ADMIN' },
  { id: 2, title: 'SUPERVISOR' },
  { id: 3, title: 'INSPECTOR' },
])

// Blood types for selection
const bloodTypes = ['A+', 'A-', 'B+', 'B-', 'O+', 'O-', 'AB+', 'AB-']

// Data
const userData = ref(null)
const userStatistics = ref(null)

const { generateBreadcrumbItems } = useBreadcrumbs()

const breadcrumbItems = computed(() =>
  generateBreadcrumbItems([
    {
      title: 'المستخدمين',
      href: '/users',
      disabled: false,
    },
    {
      title: 'تفاصيل المستخدم',
      href: route.path,
      disabled: true,
    },
  ]),
)

// Methods
const fetchUserData = async () => {
  try {
    isLoading.value = true
    const response = await axios.get(`users/${route.params.id}`)
    if (response.data?.data) {
      userData.value = response.data.data.user
      userStatistics.value = response.data.data.statistics
      populateForm(response.data.data.user)
    }
  } catch (error) {
    logger.error('Error fetching user data:', error)
    appStore.showSnackbar({
      message: error.response?.data?.message || 'حدث خطأ أثناء جلب بيانات المستخدم',
      color: 'error',
    })
    userData.value = null
    userStatistics.value = null
  } finally {
    isLoading.value = false
  }
}

const populateForm = (user) => {
  userForm.value = {
    firstname: user.firstname || '',
    secondname: user.secondname || '',
    thirdname: user.thirdname || '',
    lastname: user.lastname || '',
    email: user.email || '',
    phone: user.phone || '',
    city: user.city || '',
    bloodType: user.bloodType || '',
    gender: user.gender || '',
    nationality: user.nationality || '',
    saId: user.saId || '',
    roleId: user.role?.id || null,
    personalImage: user.personalImage || '',
  }
}

const updateUser = async () => {
  try {
    loading.value = true
    const response = await axios.put(`users/${route.params.id}`, userForm.value)
    if (response.data) {
      appStore.showSnackbar({
        message: 'تم تحديث بيانات المستخدم بنجاح',
        color: 'success',
      })
      await fetchUserData()
    }
  } catch (error) {
    logger.error('Error updating user:', error)
    appStore.showSnackbar({
      message: error.response?.data?.message || 'حدث خطأ أثناء تحديث بيانات المستخدم',
      color: 'error',
    })
  } finally {
    loading.value = false
  }
}

// Lifecycle
onMounted(() => {
  fetchUserData()
})
</script>

<template>
  <div>
    <v-row v-if="isLoading">
      <v-col v-for="item in 4" :key="item" cols="6" md="3" sm="6">
        <VCard elevation="0">
          <v-card-text class="px-1 py-2">
            <v-skeleton-loader class="mx-auto" elevation="0" max-width="400" type="list-item-two-line, table-heading" />
          </v-card-text>
        </VCard>
      </v-col>
    </v-row>

    <div v-else>
      <v-container class="mt-n4 rounded-lg" fluid>
        <BreadcrumbNav :items="breadcrumbItems" />

        <VCard class="mb-4 shadow-0">
          <div class="pa-4 pb-0">
            <user-widget no-padding no-shadow :user="userData" />
          </div>

          <!-- User Form -->
          <v-form class="pa-6" @submit.prevent="updateUser">
            <v-row>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الأول</label>
                <v-text-field v-model="userForm.firstname" required />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الثاني</label>
                <v-text-field v-model="userForm.secondname" />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الاسم الثالث</label>
                <v-text-field v-model="userForm.thirdname" />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">اسم العائلة</label>
                <v-text-field v-model="userForm.lastname" required />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">البريد الإلكتروني</label>
                <v-text-field v-model="userForm.email" required type="email" />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">رقم الهاتف</label>
                <v-text-field v-model="userForm.phone" required />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الدور</label>
                <v-select v-model="userForm.roleId" item-title="title" item-value="id" :items="roles" required />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">المدينة</label>
                <v-text-field v-model="userForm.city" required />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">فصيلة الدم</label>
                <v-select v-model="userForm.bloodType" :items="bloodTypes" required />
              </v-col>
              <v-col cols="12" sm="6">
                <label class="mb-2 d-block">الجنس</label>
                <v-select v-model="userForm.gender" :items="['male', 'female']" required />
              </v-col>
              <v-col cols="12">
                <label class="mb-2 d-block">الجنسية</label>
                <v-text-field v-model="userForm.nationality" required />
              </v-col>
              <v-col cols="12">
                <label class="mb-2 d-block">رقم الهوية</label>
                <v-text-field v-model="userForm.saId" required />
              </v-col>
            </v-row>

            <div class="d-flex justify-end mt-4">
              <v-btn color="primary" :loading="loading" type="submit"> حفظ التغييرات </v-btn>
            </div>
          </v-form>

          <div class="statistics-bar rounded-lg pa-6">
            <v-row>
              <v-col cols="12" md="3">
                <UserStatisticsCard :number="userStatistics?.attendanceDaysCount || 0" title="عدد أيام الحضور">
                  <template #icon>
                    <svg fill="none" height="20" viewBox="0 0 20 20" width="20" xmlns="http://www.w3.org/2000/svg">
                      <g opacity="0.4">
                        <path
                          d="M8.12488 1.875C5.93875 1.875 4.16654 3.64721 4.16654 5.83333C4.16654 8.01946 5.93875 9.79167 8.12488 9.79167C10.311 9.79167 12.0832 8.01946 12.0832 5.83333C12.0832 3.64721 10.311 1.875 8.12488 1.875Z"
                          fill="#667178"
                        />
                      </g>
                    </svg>
                  </template>
                </UserStatisticsCard>
              </v-col>
              <v-col cols="12" md="3">
                <UserStatisticsCard :number="userStatistics?.reportsCount || 0" title="عدد التقارير">
                  <template #icon>
                    <svg fill="none" height="20" viewBox="0 0 20 20" width="20" xmlns="http://www.w3.org/2000/svg">
                      <g opacity="0.4">
                        <path
                          d="M8.12488 1.875C5.93875 1.875 4.16654 3.64721 4.16654 5.83333C4.16654 8.01946 5.93875 9.79167 8.12488 9.79167C10.311 9.79167 12.0832 8.01946 12.0832 5.83333C12.0832 3.64721 10.311 1.875 8.12488 1.875Z"
                          fill="#667178"
                        />
                      </g>
                    </svg>
                  </template>
                </UserStatisticsCard>
              </v-col>
              <v-col cols="12" md="3">
                <UserStatisticsCard :number="userStatistics?.newRatings || 0" title="التقييمات الجديدة">
                  <template #icon>
                    <svg fill="none" height="20" viewBox="0 0 20 20" width="20" xmlns="http://www.w3.org/2000/svg">
                      <g opacity="0.4">
                        <path
                          d="M8.12488 1.875C5.93875 1.875 4.16654 3.64721 4.16654 5.83333C4.16654 8.01946 5.93875 9.79167 8.12488 9.79167C10.311 9.79167 12.0832 8.01946 12.0832 5.83333C12.0832 3.64721 10.311 1.875 8.12488 1.875Z"
                          fill="#667178"
                        />
                      </g>
                    </svg>
                  </template>
                </UserStatisticsCard>
              </v-col>
              <v-col cols="12" md="3">
                <UserStatisticsCard :number="userStatistics?.completedRatings || 0" title="التقييمات المكتملة">
                  <template #icon>
                    <svg fill="none" height="20" viewBox="0 0 20 20" width="20" xmlns="http://www.w3.org/2000/svg">
                      <g opacity="0.4">
                        <path
                          d="M8.12488 1.875C5.93875 1.875 4.16654 3.64721 4.16654 5.83333C4.16654 8.01946 5.93875 9.79167 8.12488 9.79167C10.311 9.79167 12.0832 8.01946 12.0832 5.83333C12.0832 3.64721 10.311 1.875 8.12488 1.875Z"
                          fill="#667178"
                        />
                      </g>
                    </svg>
                  </template>
                </UserStatisticsCard>
              </v-col>
            </v-row>
          </div>
        </VCard>
      </v-container>
    </div>
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
      color: #667178;
      position: relative;

      &:after {
        content: '';
        position: absolute;
        bottom: -2px;
        left: 0;
        right: 0;
        height: 2px;
        background-color: #667178;
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
  color: #667178;
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
