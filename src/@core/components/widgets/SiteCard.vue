<template>
  <v-card elevation="0" class="site-card rounded-lg">
    <v-card-text class="pa-4">
      <div class="d-flex justify-space-between align-center mb-4">
        <div class="d-flex align-center justify-space-between w-100">
          <h2 class="text-h6 font-weight-medium">رقم المخيم</h2>

          <div>
            <v-btn @click="dialog = true" icon variant="text" size="small">
              <v-icon icon="mdi-map-marker" size="20" color="#B69265" />
            </v-btn>
            <v-btn
              size="small"
              class="text-none font-weight-medium px-2 me-2 bg-primary-100"
            >
              <v-icon icon="mdi-arrow-left" size="20" class="me-1" />
              عرض
            </v-btn>
          </div>
        </div>
      </div>

      <div class="d-flex align-center mb-2">
        <v-icon
          icon="mdi-office-building"
          size="16"
          color="#B69265"
          class="me-1"
        />
        <span class="text-body-2 text-medium-emphasis">شركة رواف منى</span>
      </div>

      <div class="site-info mt-6">
        <div class="d-flex justify-space-between align-center mb-4">
          <div class="d-flex align-center">
            <v-icon
              icon="mdi-identifier"
              size="20"
              color="#B69265"
              class="me-2"
            />
            <span class="text-body-2 text-medium-emphasis">رقم البريد</span>
          </div>
          <span class="text-body-2 font-weight-medium">{{ siteId }}</span>
        </div>

        <div class="d-flex justify-space-between align-center">
          <div class="d-flex align-center">
            <v-icon
              icon="mdi-card-account-details-outline"
              size="20"
              color="#B69265"
              class="me-2"
            />
            <span class="text-body-2 text-medium-emphasis">رقم التفويض</span>
          </div>
          <span class="text-body-2 font-weight-medium">{{
            authorizationNumber
          }}</span>
        </div>
      </div>
    </v-card-text>

    <!-- Location Correction Modal -->
    <v-dialog v-model="dialog" width="500" class="location-dialog">
      <v-card class="rounded-lg">
        <v-card-text class="pa-6">
          <div class="d-flex justify-space-between align-center mb-6">
            <h2 class="text-h6">إضافة موقع جديد</h2>
            <v-btn icon variant="text" @click="dialog = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </div>
          <p class="text-subtitle-1 text-medium-emphasis mb-6">
            حدد تفاصيل الموقع الجديد
          </p>
          <v-form @submit.prevent="handleSubmit">
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">نوع الموقع</label>
              <v-select
                v-model="siteType"
                :items="siteTypes"
                variant="outlined"
                density="comfortable"
                placeholder="اختر نوع الموقع"
                hide-details
                class="rounded-lg"
              />
            </div>
            <div class="d-flex justify-end mt-6">
              <v-btn
                variant="text"
                color="grey-darken-1"
                class="me-4"
                @click="dialog = false"
              >
                إلغاء
              </v-btn>
              <v-btn
                color="#B69265"
                class="text-white"
                type="submit"
                :loading="loading"
              >
                التالي
              </v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script setup>
import { ref } from "vue";

defineProps({
  siteId: {
    type: String,
    required: true,
  },
  authorizationNumber: {
    type: String,
    required: true,
  },
});

const dialog = ref(false);
const loading = ref(false);
const siteType = ref(null);
const siteTypes = [
  { title: "موقع سكني", value: "residential" },
  { title: "موقع تجاري", value: "commercial" },
  { title: "موقع صناعي", value: "industrial" },
];

const handleSubmit = async () => {
  loading.value = true;
  // Add your submit logic here
  await new Promise((resolve) => setTimeout(resolve, 1000));
  loading.value = false;
  dialog.value = false;
};
</script>

<style lang="scss" scoped>
.site-card {
  background-color: rgb(250, 250, 250) !important;
  border: 1px solid rgba(182, 146, 101, 0.12);
  transition: all 0.3s ease;

  &:hover {
    border-color: #b69265;
    box-shadow: 0 4px 8px rgba(182, 146, 101, 0.1) !important;
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
  }

  .text-h6 {
    font-size: 1.1rem !important;
    line-height: 1.5;
  }

  .site-info {
    background-color: white;
    border-radius: 8px;
    padding: 1rem;
  }
}

.location-dialog {
  :deep(.v-card) {
    background-color: white;
  }

  :deep(.v-field) {
    border-radius: 8px;
    background-color: rgb(250, 250, 250);
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
    border-radius: 8px;
  }
}
</style>
