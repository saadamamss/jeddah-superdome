<template>
  <v-container fluid class="pa-0">
    <BreadcrumbNav :items="breadcrumbItems" />

    <v-card elevation="0" class="rounded-lg">
      <v-card-title
        class="d-flex justify-space-between align-center py-4 px-4 ps-0"
      >
        <div class="d-flex align-center">
          <v-btn
            variant="text"
            color="default"
            @click="router.back()"
            class="pa-0 me-0"
          >
            <svg
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
              style="transform: scaleX(-1)"
            >
              <path
                d="M15.0003 19.92L8.48033 13.4C7.71033 12.63 7.71033 11.37 8.48033 10.6L15.0003 4.07999"
                stroke="#292D32"
                stroke-width="1.5"
                stroke-miterlimit="10"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
          </v-btn>
          <h2 class="text-h5 font-weight-bold">تفاصيل الملاحظة</h2>
        </div>
      </v-card-title>

      <v-divider></v-divider>

      <v-card-text class="px-6 py-4">
        <!-- Loading State -->
        <v-row v-if="loading">
          <v-col cols="12">
            <v-skeleton-loader type="article"></v-skeleton-loader>
          </v-col>
        </v-row>

        <!-- Error State -->
        <v-row v-else-if="error">
          <v-col cols="12" class="text-center">
            <p class="text-error">{{ error }}</p>
          </v-col>
        </v-row>

        <!-- Content -->
        <v-row v-else>
          <v-col cols="12">
            <div class="note-details rounded-lg pa-6">
              <!-- Status and Date -->
              <div class="d-flex justify-space-between align-center mb-6">
                <div class="d-flex align-center gap-2">
                  <v-chip
                    :color="getStatusColor(note.status)"
                    :text-color="
                      note.status === 'PENDING' ? 'default' : 'white'
                    "
                    size="small"
                    class="font-weight-medium"
                  >
                    {{ getStatusText(note.status) }}
                  </v-chip>
                  <span
                    v-if="note.rejection_reason"
                    class="text-caption text-error"
                  >
                    ({{ note.rejection_reason }})
                  </span>
                </div>
                <span class="text-caption text-medium-emphasis">
                  {{ formatDate(note.created_at) }}
                </span>
              </div>

              <!-- Note Type -->
              <div class="mb-6">
                <div class="text-subtitle-1 font-weight-bold mb-2">
                  نوع الملاحظة
                </div>
                <div class="d-flex flex-column">
                  <div class="text-h6">{{ note.report_type?.title }}</div>
                  <div class="text-caption text-medium-emphasis">
                    تم إنشاؤه بواسطة:
                    {{ note.report_type?.created_by?.firstname }}
                    {{ note.report_type?.created_by?.lastname }}
                  </div>
                </div>
              </div>

              <!-- Description -->
              <div class="mb-6">
                <div class="text-subtitle-1 font-weight-bold mb-2">الوصف</div>
                <p class="text-body-1">{{ note.description }}</p>
              </div>

              <!-- Location -->
              <div class="mb-6">
                <div class="text-subtitle-1 font-weight-bold mb-2">الموقع</div>
                <div class="d-flex align-center">
                  <v-icon size="small" color="primary" class="me-2"
                    >mdi-map-marker</v-icon
                  >
                  <div class="d-flex flex-column">
                    <span class="text-body-1">{{ note.address }}</span>
                    <span
                      v-if="note.latitude && note.longitude"
                      class="text-caption text-medium-emphasis"
                    >
                      {{ note.latitude }}, {{ note.longitude }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Reporter -->
              <div class="mb-6">
                <div class="text-subtitle-1 font-weight-bold mb-2">المبلغ</div>
                <div class="d-flex align-center">
                  <v-icon size="small" color="primary" class="me-2"
                    >mdi-account</v-icon
                  >
                  <div class="d-flex flex-column">
                    <div class="text-body-1">
                      {{ note.report_by?.firstname }}
                      {{ note.report_by?.lastname }}
                    </div>
                    <span class="text-caption text-medium-emphasis">
                      {{ note.report_by?.email }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Status Details -->
              <div v-if="note.status !== 'PENDING'" class="mb-6">
                <div class="text-subtitle-1 font-weight-bold mb-2">
                  تفاصيل الحالة
                </div>
                <div class="d-flex flex-column gap-2">
                  <div
                    v-if="note.status === 'APPROVED'"
                    class="d-flex align-center"
                  >
                    <v-icon size="small" color="success" class="me-2"
                      >mdi-check-circle</v-icon
                    >
                    <div class="d-flex flex-column">
                      <span class="text-body-1">
                        تمت الموافقة بواسطة: {{ note.approved_by?.firstname }}
                        {{ note.approved_by?.lastname }}
                      </span>
                      <span class="text-caption text-medium-emphasis">
                        {{ formatDate(note.approved_at) }}
                      </span>
                    </div>
                  </div>
                  <div
                    v-if="note.status === 'REJECTED'"
                    class="d-flex align-center"
                  >
                    <v-icon size="small" color="error" class="me-2"
                      >mdi-close-circle</v-icon
                    >
                    <div class="d-flex flex-column">
                      <span class="text-body-1">
                        تم الرفض بواسطة: {{ note.rejected_by?.firstname }}
                        {{ note.rejected_by?.lastname }}
                      </span>
                      <span class="text-caption text-medium-emphasis">
                        {{ formatDate(note.rejected_at) }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Dates -->
              <div>
                <div class="text-subtitle-1 font-weight-bold mb-2">
                  التواريخ
                </div>
                <div class="d-flex flex-column gap-2">
                  <div class="d-flex align-center">
                    <v-icon size="small" color="primary" class="me-2"
                      >mdi-calendar</v-icon
                    >
                    <span class="text-body-1"
                      >تاريخ الإبلاغ: {{ formatDate(note.report_date) }}</span
                    >
                  </div>
                  <div class="d-flex align-center">
                    <v-icon size="small" color="primary" class="me-2"
                      >mdi-clock-outline</v-icon
                    >
                    <span class="text-body-1"
                      >وقت الإبلاغ: {{ note.report_time }}</span
                    >
                  </div>
                  <div class="d-flex align-center">
                    <v-icon size="small" color="primary" class="me-2"
                      >mdi-update</v-icon
                    >
                    <span class="text-body-1"
                      >آخر تحديث: {{ formatDate(note.updated_at) }}</span
                    >
                  </div>
                </div>
              </div>

              <!-- Attachments -->
              <div v-if="note.attachments" class="mt-6">
                <div class="text-subtitle-1 font-weight-bold mb-2">
                  المرفقات
                </div>
                <div class="d-flex flex-wrap gap-2">
                  <!-- Add attachment display logic here based on your attachments structure -->
                </div>
              </div>
            </div>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "@/plugins/axios";
import { useAppStore } from "@/stores/app";
import BreadcrumbNav from "@/components/BreadcrumbNav.vue";
import { useBreadcrumbs } from "@/composables/useBreadcrumbs";

const route = useRoute();
const router = useRouter();
const appStore = useAppStore();

const note = ref(null);
const loading = ref(true);
const error = ref(null);

const { generateBreadcrumbItems } = useBreadcrumbs();

const breadcrumbItems = computed(() =>
  generateBreadcrumbItems([
    {
      title: "الملاحظات",
      href: "/notes",
      disabled: false,
    },
    {
      title: "تفاصيل الملاحظة",
      href: route.path,
      disabled: true,
    },
  ])
);

// Fetch note details
const fetchNoteDetails = async () => {
  try {
    loading.value = true;
    error.value = null;
    const response = await axios.get(`/reports/${route.params.id}`);
    note.value = response.data.data;
  } catch (err) {
    console.error("Error fetching note details:", err);
    error.value = "حدث خطأ أثناء جلب تفاصيل الملاحظة";
    appStore.showSnackbar({
      message: error.value,
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Format date
const formatDate = (date) => {
  if (!date) return "";
  return new Date(date).toLocaleDateString("ar-SA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

// Get status color
const getStatusColor = (status) => {
  switch (status) {
    case "APPROVED":
      return "success";
    case "REJECTED":
      return "error";
    case "PENDING":
      return "warning";
    default:
      return "default";
  }
};

// Get status text
const getStatusText = (status) => {
  switch (status) {
    case "APPROVED":
      return "مقبولة";
    case "REJECTED":
      return "مرفوضة";
    case "PENDING":
      return "قيد المراجعة";
    default:
      return status;
  }
};

onMounted(() => {
  fetchNoteDetails();
});
</script>

<style lang="scss" scoped>
.note-details {
  background-color: #f8f9f9;
}

:deep(.v-btn) {
  text-transform: none;
  letter-spacing: 0;
}

.custom-breadcrumbs {
  :deep(.v-breadcrumbs-item--disabled) {
    opacity: 1;
  }

  :deep(.v-breadcrumbs__item) {
    color: rgba(0, 0, 0, 0.6);
    font-size: 13px;
    padding: 0 8px;
  }

  :deep(.v-breadcrumbs-item--disabled) {
    color: rgba(0, 0, 0, 0.6);
  }
}

.text-13 {
  font-size: 13px;
}
</style>
