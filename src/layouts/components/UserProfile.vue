<script setup>
import { useSettingsStore } from "@/stores/Settings";
import { useAuthStore } from "@/stores/Auth";
import { useRouter } from "vue-router";
import axios from "@/plugins/axios";

const user = ref();
const settingsListStore = useSettingsStore();
const authStore = useAuthStore();
const router = useRouter();
const isLoggingOut = ref(false);
const isMenu = ref(false);

const logout = async (e) => {
  e.preventDefault();

  isLoggingOut.value = true;
  try {
    await axios.post("/logout");

    // Clear auth store state
    authStore.logout();
    // Navigate to login page
    await router.push("/auth/login");
    // Show success message
    settingsListStore.alertColor = "success";
    settingsListStore.alertMessage = "تم تسجيل الخروج بنجاح";
    settingsListStore.isAlertShow = true;
    setTimeout(() => {
      settingsListStore.isAlertShow = false;
      settingsListStore.alertMessage = "";
    }, 3000);
  } catch (error) {
    console.error("Logout error:", error);
    settingsListStore.alertColor = "error";
    settingsListStore.alertMessage = "حدث خطأ أثناء تسجيل الخروج";
    settingsListStore.isAlertShow = true;
  } finally {
    isLoggingOut.value = false;
  }
};

onMounted(() => {
  user.value = authStore.user.user;
  console.log(user.value);
});
</script>

<template>
  <div>
    <v-btn
      height="48"
      color="white"
      elevation="0"
      class="btn-user-profile px-3"
    >
      <div class="d-flex align-center ga-3">
        <img
          v-if="user?.avatar_image"
          width="30"
          height="30"
          :src="user.avatar_image"
          class="avatar-img"
          style="width: 30px; height: 30px"
        />
        <img
          v-else
          src="https://placehold.co/400"
          width="30"
          height="30"
          class="avatar-img rounded-circle"
          style="width: 30px; height: 30px"
        />
        <span class="text-black text-sm"
          >{{ user?.firstname }} {{ user?.lastname }}</span
        >

        <span class="ms-2">
          <svg
            width="9"
            height="6"
            viewBox="0 0 8 5"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M7.25 0.75L4 4.25L0.75 0.75"
              stroke="#B69265"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
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
          <v-list-item @click="logout" :disabled="isLoggingOut">
            <v-list-item-title>
              <VIcon class="me-2" icon="tabler-logout" size="21" />
              <span v-if="!isLoggingOut" class="text-sm">تسجيل الخروج</span>
              <VIcon
                v-else
                icon="mingcute:loading-line"
                class="fixed loading"
                size="32"
              />
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
