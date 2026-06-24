<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<template>
  <div class="d-flex justify-center align-center" style="height: 100vh">
    <div class="pa-4 text-center">
      <div class="mb-4">
        <h1 class="mb-4">{{ t('permissionDenied.title') }}</h1>
        <p>{{ t('permissionDenied.subtitle') }}</p>
      </div>
      <VBtn class="mb-12" rounded="lg" to="/dashboard"> {{ t('permissionDenied.backHome') }} </VBtn>
    </div>
  </div>
</template>
