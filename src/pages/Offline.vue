<script setup lang="ts">
const reload = () => {
  window.location.reload()
}
</script>

<template>
  <div class="offline-page">
    <div class="offline-content">
      <v-icon size="64" color="warning" class="mb-4">mdi-wifi-off</v-icon>
      <h1 class="text-h5 font-weight-bold mb-2">لا يوجد اتصال بالإنترنت</h1>
      <p class="text-body-1 text-medium-emphasis mb-6">يرجى التحقق من اتصالك بالشبكة والمحاولة مرة أخرى</p>
      <v-btn color="primary" @click="reload">
        <v-icon start>mdi-refresh</v-icon>
        إعادة المحاولة
      </v-btn>
    </div>
  </div>
</template>

<style scoped>
.offline-page {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background-color: #f8f9f9;
}

.offline-content {
  text-align: center;
  padding: 2rem;
}
</style>
