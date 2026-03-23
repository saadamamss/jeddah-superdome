<template>
  <v-app-bar elevation="1" height="80" :class="appBarClasses">
    <v-container
      class="d-flex align-center justify-space-between h-100 ga-4"
    >
      <!-- Logo -->
      <!-- Mobile Menu Button -->
      <v-btn icon class="d-md-none" @click="toggleDrawer">
        <v-icon style="height: 30px; width: 30px">mdi-menu</v-icon>
      </v-btn>
      <router-link to="/" class="text-decoration-none">
        <!-- <v-img src="@/assets/logo.svg" alt="امتثال" width="120" /> -->

        <Logo
          v-if="appBarClasses['app-bar-white']"
          color="#9C8055"
          text-color="#222"
        />
        <Logo v-else />
      </router-link>

      <!-- Navigation Menu -->
      <div class="d-none d-md-flex align-center">
        <v-btn
          v-for="item in menuItems"
          :key="item.title"
          variant="text"
          class="font-weight-medium mx-2"
          color="secondary"
          :ripple="false"
          @click="scrollToSection(item.section)"
        >
          {{ item.title }}
        </v-btn>
      </div>

      <!-- Auth Buttons -->
      <div class="d-flex align-center auth-btns ga-2">
        <!-- <v-btn
          variant="outlined"
          class="auth-btns__login font-weight-bold rounded-pill"
          :ripple="false"
          :class="
            !buttonClasses['button-white']
              ? 'bg-light-gray text-gray text-white border '
              : 'bg-dark-white text-white border border-dark-white'
          "
          color="white"
          to="/auth/login"
        >
          تسجيل الدخول
        </v-btn> -->
        <v-btn
          variant="outlined"
          class="auth-btns__login font-weight-bold rounded-lg"
          :ripple="false"
          color="white"
          href="/auth/login"
        >
          تسجيل الدخول
        </v-btn>
      </div>
    </v-container>
  </v-app-bar>

  <!-- Mobile Navigation Drawer -->
  <v-navigation-drawer v-model="drawer" location="right" temporary>
    <v-list>
      <v-list-item
        v-for="item in menuItems"
        :key="item.title"
        :title="item.title"
        color="primary"
        @click="scrollToSection(item.section)"
      />
    </v-list>
  </v-navigation-drawer>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, nextTick } from "vue";
import { useRouter, useRoute } from "vue-router";
import Logo from "./icons/logo.vue";

const router = useRouter();
const route = useRoute();
const drawer = ref(false);
const scrollPosition = ref(0);

const menuItems = [
  { title: " الخدمات", section: "services" },
  { title: "الميزات", section: "features" },
  { title: "الأسئلة الشائعة", section: "faq" },
];

const isLandingPage = computed(() => route.path === "/");

const appBarClasses = computed(() => ({
  "app-bar-transparent":
    isLandingPage.value && scrollPosition.value < window.innerHeight,
  "app-bar-white":
    !isLandingPage.value || scrollPosition.value >= window.innerHeight,
}));

const buttonClasses = computed(() => ({
  "button-white":
    !isLandingPage.value || scrollPosition.value >= window.innerHeight,
  "button-secondary":
    !isLandingPage.value || scrollPosition.value >= window.innerHeight,
}));

const handleScroll = () => {
  scrollPosition.value = window.scrollY;
};

onMounted(() => {
  window.addEventListener("scroll", handleScroll);
  handleScroll(); // Initial check
});

onUnmounted(() => {
  window.removeEventListener("scroll", handleScroll);
});

const scrollToSection = async (section) => {
  // Close mobile drawer if open
  drawer.value = false;

  // If not on landing page, navigate to it first
  if (route.path !== "/") {
    await router.push("/");
    // Wait for navigation and DOM update
    await nextTick();
  }

  const element = document.getElementById(section);
  if (element) {
    element.scrollIntoView({ behavior: "smooth", block: "start" });
  }
};

const toggleDrawer = () => {
  drawer.value = !drawer.value;
};
</script>

<style lang="scss">
.v-main {
  padding-top: 0px !important;
}

.v-app-bar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  transition: all 0.3s ease;
  border-bottom: none !important;
  box-shadow: none !important;

  &.app-bar-transparent {
    background: #23282b66 !important;
    border: none;

    .v-toolbar__content {
      background: transparent !important;
      box-shadow: none !important;
    }

    button {
      color: white !important;
    }

    .v-btn.bg-\[#FFFFFF\] {
      background-color: rgba(255, 255, 255, 0.1) !important;
      color: white !important;
      border: none !important;
    }
  }
  .auth-btns__login {
    background-color: #9c8055;
    height: 40px;
    min-height: 35px !important;
    border: none;
    transition: all 0.3s ease-in-out;
    &:hover {
      background-color: #c09e6a;
    }
  }
  &.app-bar-white {
    background-color: white !important;
    border: none;

    .v-toolbar__content {
      background-color: white !important;
      box-shadow: none !important;
    }

    button {
      color: #75797c !important;
    }

    .v-btn.bg-\[#FFFFFF\] {
      background-color: white !important;
      color: #75797c !important;
      border: 1px solid rgba(0, 0, 0, 0.1) !important;
    }

    .v-btn.bg-secondary {
      color: white !important;
    }
  }

  .v-toolbar__content {
    border: none !important;
  }
}

.v-btn.bg-secondary {
  background-color: #b49164 !important;
}

:deep(.v-btn) {
  text-transform: none;
  letter-spacing: 0;
  font-size: 0.95rem;

  &::before {
    display: none;
  }

  &:hover {
    background: transparent;
    opacity: 0.8;
  }
}
</style>
