<script setup>
import { useSettingsStore } from "@/store/Settings";

const user = ref();
const settingsListStore = useSettingsStore();
const isLoading = ref(false);
const isMenu = ref(false);

const logout = () => {
  isLoading.value = true;
  localStorage.removeItem("SAR_TOKEN");
  location.reload();
  settingsListStore.alertColor = "error";
  settingsListStore.alertMessage = "تم تسجيل الخروج";
  settingsListStore.isAlertShow = true;
  setTimeout(() => {
    settingsListStore.isAlertShow = false;
    settingsListStore.alertMessage = "";
  }, 3000);
};
</script>

<template>
  <div>
    <v-btn
      color="white"
      v-bind="props"
      elevation="0"
      class="btn-user-profile px-3"
    >
      <div class="d-flex align-center ga-3">
        <img
          v-if="user?.avatar"
          width="30"
          height="30"
          :src="user.avatar"
          class="avatar-img"
          style="width: 30px; height: 30px"
        />
        <img
          v-else
          src="/imgs/person.svg"
          width="30"
          height="30"
          class="avatar-img"
          style="width: 30px; height: 30px"
        />
        <span class="text-black text-sm">عبدالله قحطاني</span>

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
          <v-list-item to="/ai">
            <v-list-item-title class="text-sm">حسابي</v-list-item-title>
          </v-list-item>
          <v-list-item to="/ai">
            <v-list-item-title class="text-sm">إدارة المواقع</v-list-item-title>
          </v-list-item>
          <v-list-item to="/ai">
            <v-list-item-title class="text-sm">حسابي</v-list-item-title>
          </v-list-item>
          <v-list-item @click="logout">
            <v-list-item-title>
              <VIcon class="me-2" icon="tabler-logout" size="21" />
              <span v-if="!isLoading" class="text-sm">تسجيل الخروج</span>
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
