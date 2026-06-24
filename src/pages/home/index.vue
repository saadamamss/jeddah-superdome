<script setup lang="ts">
import { useAuthStore } from '@/stores/auth.store'
import CompanyProfileCard from '@/components/CompanyProfileCard.vue'
import AdminDashboard from '@/components/AdminDashboard.vue'

const auth = useAuthStore()

const isCompanyUser = computed(() => auth.isCompany)
</script>

<template>
  <AdminDashboard v-if="!isCompanyUser" />
  <CompanyProfileCard v-else />
</template>
