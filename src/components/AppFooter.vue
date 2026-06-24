<template>
  <v-footer class="pa-0 bg-white">
    <v-container :fluid="true" style="background-color: #f8f9f9">
      <!-- Logo -->
      <v-row class="px-lg-16 align-center py-10">
        <v-col cols="12" md="2" sm="6">
          <div class="mb-6 mb-md-0 d-flex justify-center justify-sm-start">
            <router-link class="text-decoration-none" to="/">
              <Logo color="#9C8055" height="112" text-color="#222" width="186" />
            </router-link>
          </div>
        </v-col>
        <v-col class="order-2 order-sm-3 order-md-2" cols="12" md="8">
          <!-- Navigation Links -->
          <div class="d-flex align-center justify-center mt-8">
            <v-btn
              v-for="item in footerLinks"
              :key="item.title"
              class="mx-md-4 mb-2 mb-md-0 font-weight-bold text-h text-md-sm text-grey-700"
              :ripple="false"
              variant="text"
              @click="scrollToSection(item.section)"
            >
              {{ item.title }}
            </v-btn>
          </div>
        </v-col>
        <v-col class="order-3 order-sm-2 order-md-3" cols="12" md="2" sm="6">
          <div class="d-flex justify-center justify-sm-end">
            <router-link to="#">
              <span class="d-inline-block shadow font-weight-medium bg-white px-3 py-2 text-white rounded-lg mt-10">
                <QrApp />
              </span>
            </router-link>
          </div>
        </v-col>
      </v-row>

      <!-- Copyright -->
      <div class="text-center py-0 caption d-flex justify-center align-center ga-2">
        <p class="text-grey-600">© جميع حقوق الطبع و النشر محفوظة 2025 - Jeddah Superdome</p>
      </div>
    </v-container>
  </v-footer>
</template>

<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import QrApp from './icons/qr-app.vue'
import Logo from './icons/logo.vue'

const router = useRouter()
const route = useRoute()

const footerLinks = [
  { title: ' الخدمات', section: 'services' },
  { title: 'الميزات', section: 'features' },
  { title: 'الأسئلة الشائعة', section: 'faq' },
]

const scrollToSection = async (section: string) => {
  // If not on landing page, navigate to it first
  if (route.path !== '/') {
    await router.push('/')
    // Wait for navigation and DOM update
    await nextTick()
  }

  const element = document.getElementById(section)
  if (element) {
    element.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }
}
</script>

<style scoped lang="scss">
.v-footer {
  border-top: 1px solid rgba(0, 0, 0, 0.1);
}

.border-t {
  border-top: 1px solid rgba(0, 0, 0, 0.1);
}

:deep(.v-list-item) {
  min-height: 40px;
  cursor: pointer;

  &:hover {
    color: rgb(var(--v-theme-primary));
  }
}
</style>
