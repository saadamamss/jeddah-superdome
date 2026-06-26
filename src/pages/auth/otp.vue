<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const router = useRouter()

onMounted(() => {
  router.replace({ name: 'forgot-password' })
})
</script>

<template>
  <VRow class="auth-wrapper text-black" no-gutters>
    <div class="overlay" />
    <VCol class="d-flex flex-column align-center justify-center py-4 pa-0 mx-auto" cols="12" lg="5" md="8" sm="10">
      <div class="w-100 d-flex flex-column align-center justify-center pb-6 pt-3 px-4">
        <img alt="جدة سوبردوم" src="/jeddah-superdome.svg" width="140" />
        <p class="text-lg text-center mt-4" style="color: #75797c">
          {{ t('auth.otp.redirectMessage') }}
        </p>
      </div>
    </VCol>
  </VRow>
</template>

<style lang="scss" scoped>
.auth-wrapper {
  background-size: cover;
}
</style>
