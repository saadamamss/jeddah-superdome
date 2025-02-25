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
          <h2 class="text-h5 font-weight-bold">إضافة ملاحظة</h2>
        </div>
        <div class="d-flex gap-4">
          <v-btn
            color="default"
            variant="outlined"
            @click="router.back()"
            class="px-6"
          >
            إلغاء
          </v-btn>
          <v-btn
            color="primary"
            class="px-6"
            :loading="loading"
            @click="handleSubmit"
          >
            حفظ
          </v-btn>
        </div>
      </v-card-title>

      <v-divider></v-divider>

      <v-card-text class="px-6 py-4">
        <v-row>
          <v-col cols="12">
            <div class="text-subtitle-1 font-weight-bold mb-2">
              وصف الملاحظة
            </div>
            <v-textarea
              v-model="description"
              placeholder="اكتب وصف الملاحظة هنا..."
              variant="outlined"
              rows="4"
              class="rounded-lg"
            ></v-textarea>
          </v-col>

          <v-col cols="12">
            <div class="text-subtitle-1 font-weight-bold mb-2">
              موقع الملاحظة
            </div>
            <v-text-field
              v-model="location"
              placeholder="حدد موقع الملاحظة"
              variant="outlined"
              class="rounded-lg"
              append-inner-icon="mdi-map-marker"
            ></v-text-field>
          </v-col>

          <v-col cols="12">
            <div class="text-subtitle-1 font-weight-bold mb-2">
              الأدلة والشواهد البيئية
            </div>
            <v-card
              class="border-dashed rounded-lg pa-8"
              elevation="0"
              @click="triggerFileInput"
            >
              <input
                type="file"
                ref="fileInput"
                style="display: none"
                @change="handleFileUpload"
                multiple
                accept="image/*"
              />
              <div class="text-center">
                <svg
                  width="48"
                  height="48"
                  viewBox="0 0 48 48"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                  class="mx-auto mb-4"
                >
                  <rect width="48" height="48" rx="24" fill="#F8F9F9" />
                  <path
                    d="M29.3337 25.6667H25.3337V29.6667C25.3337 30.0333 25.0337 30.3333 24.667 30.3333C24.3003 30.3333 24.0003 30.0333 24.0003 29.6667V25.6667H20.0003C19.6337 25.6667 19.3337 25.3667 19.3337 25C19.3337 24.6333 19.6337 24.3333 20.0003 24.3333H24.0003V20.3333C24.0003 19.9667 24.3003 19.6667 24.667 19.6667C25.0337 19.6667 25.3337 19.9667 25.3337 20.3333V24.3333H29.3337C29.7003 24.3333 30.0003 24.6333 30.0003 25C30.0003 25.3667 29.7003 25.6667 29.3337 25.6667Z"
                    fill="#B69265"
                  />
                  <path
                    opacity="0.4"
                    d="M31.3663 18.7667C34.3663 21.7667 34.3663 26.5667 31.3663 29.5667C28.3663 32.5667 23.5663 32.5667 20.5663 29.5667C17.5663 26.5667 17.5663 21.7667 20.5663 18.7667C23.5663 15.7667 28.3663 15.7667 31.3663 18.7667Z"
                    fill="#B69265"
                  />
                </svg>
                <div class="text-body-1 text-medium-emphasis mb-1">
                  اسحب وأفلت الملفات هنا أو تصفح من جهازك
                </div>
                <div class="text-caption text-medium-emphasis">
                  الحد الأقصى لحجم الملف: 10 ميجابايت
                </div>
              </div>
            </v-card>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script setup>
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import BreadcrumbNav from "@/components/BreadcrumbNav.vue";
import { useBreadcrumbs } from "@/composables/useBreadcrumbs";

const router = useRouter();
const loading = ref(false);
const description = ref("");
const location = ref("");
const fileInput = ref(null);
const uploadedFiles = ref([]);

const { generateBreadcrumbItems } = useBreadcrumbs();

const breadcrumbItems = computed(() =>
  generateBreadcrumbItems([
    {
      title: "الملاحظات",
      href: "/notes",
      disabled: false,
    },
    {
      title: "إضافة ملاحظة",
      href: route.path,
      disabled: true,
    },
  ])
);

const triggerFileInput = () => {
  fileInput.value.click();
};

const handleFileUpload = (event) => {
  const files = Array.from(event.target.files);
  uploadedFiles.value = [...uploadedFiles.value, ...files];
};

const handleSubmit = async () => {
  loading.value = true;
  try {
    // Add your submit logic here
    await new Promise((resolve) => setTimeout(resolve, 1000));
    router.push("/notes");
  } finally {
    loading.value = false;
  }
};
</script>

<style lang="scss" scoped>
.border-dashed {
  border: 2px dashed rgba(182, 146, 101, 0.2);
  background-color: #f8f9f9;
  cursor: pointer;
  transition: all 0.3s ease;

  &:hover {
    border-color: rgba(182, 146, 101, 0.4);
  }
}

:deep(.v-btn) {
  text-transform: none;
  letter-spacing: 0;
}

:deep(.v-field) {
  border-radius: 8px !important;
  background-color: #f8f9f9 !important;
}

:deep(.v-field--focused) {
  background-color: #f8f9f9 !important;

  .v-field__outline {
    color: rgba(182, 146, 101, 0.2) !important;
  }
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
