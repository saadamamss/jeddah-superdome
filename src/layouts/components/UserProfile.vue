<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useSettingsStore } from '@/stores/settings.store'
import { useAuthStore } from '@/stores/auth.store'
import { useRouter } from 'vue-router'
import axios from '@/plugins/axios'

interface UserProfileData {
  firstname: string
  lastname: string
  avatarImage: string
}
const user = ref<UserProfileData>({ firstname: '', lastname: '', avatarImage: '' })
const settingsListStore = useSettingsStore()
const authStore = useAuthStore()
const router = useRouter()
const isLoggingOut = ref(false)

const logout = async (e: Event) => {
  e.preventDefault()

  isLoggingOut.value = true
  try {
    await axios.post('/logout')

    // Clear auth store state
    authStore.logout()
    // Navigate to login page
    await router.push('/auth/login')
    // Show success message
    settingsListStore.alertColor = 'success'
    settingsListStore.alertMessage = 'تم تسجيل الخروج بنجاح'
    settingsListStore.isAlertShow = true
    setTimeout(() => {
      settingsListStore.isAlertShow = false
      settingsListStore.alertMessage = ''
    }, 3000)
  } catch (error) {
    logger.error('Logout error:', error)
    settingsListStore.alertColor = 'error'
    settingsListStore.alertMessage = 'حدث خطأ أثناء تسجيل الخروج'
    settingsListStore.isAlertShow = true
  } finally {
    isLoggingOut.value = false
  }
}

onMounted(() => {
  user.value = { firstname: 'عبدالله', lastname: 'القحطانى', avatarImage: '/imgs/User-Image.png' } // authStore.user.user;
  logger.log(user.value)
})
</script>

<template>
  <div>
    <v-btn class="btn-user-profile px-3" color="white" elevation="0" height="48">
      <div class="d-flex align-center ga-3">
        <img
          v-if="user?.avatarImage"
          class="avatar-img"
          height="30"
          :src="user.avatarImage"
          style="width: 30px; height: 30px"
          width="30"
        />
        <img
          v-else
          class="avatar-img rounded-circle"
          height="30"
          src="https://placehold.co/400"
          style="width: 30px; height: 30px"
          width="30"
        />
        <span class="text-black text-sm">{{ user?.firstname }} {{ user?.lastname }}</span>

        <span class="ms-2">
          <svg fill="none" height="6" viewBox="0 0 8 5" width="9" xmlns="http://www.w3.org/2000/svg">
            <path
              d="M7.25 0.75L4 4.25L0.75 0.75"
              stroke="#667178"
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="1.5"
            />
          </svg>
        </span>
      </div>

      <v-menu activator="parent">
        <v-list>
          <!-- <v-list-item :to="{ name: 'home' }">
            <v-list-item-title class="text-sm"
              >الصفحة الرئيسية</v-list-item-title
            >
          </v-list-item> -->
          <!-- <v-list-item to="/ai">
            <v-list-item-title class="text-sm">حسابي</v-list-item-title>
          </v-list-item> -->
          <v-list-item :disabled="isLoggingOut" @click="logout">
            <v-list-item-title>
              <VIcon class="me-2" icon="tabler-logout" size="21" />
              <span v-if="!isLoggingOut" class="text-sm">تسجيل الخروج</span>
              <VIcon v-else class="fixed loading" icon="mingcute:loading-line" size="32" />
            </v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>
    </v-btn>
  </div>
</template>

<style>
.avatar-container {
  position: relative;
  border: 1px solid #f8f7fa;
  border-radius: 50%;
  block-size: 35px;
  inline-size: 35px;
}

.avatar-img {
  display: inline-block;
  block-size: 100%;
  inline-size: 100%;
  object-fit: contain;
}

.v-btn.btn-user-profile {
  --v-btn-height: 40px;

  /* width: var(--v-btn-height); */
  /* min-width: 20px; */
  border-radius: 10px;
}
</style>
