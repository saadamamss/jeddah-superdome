<script setup>
import UploadFile from "@/@core/components/uploadFile.vue";
import { ref, reactive, computed } from "vue";
import { useRoute } from "vue-router";
import BreadcrumbNav from "@/components/BreadcrumbNav.vue";
import { useBreadcrumbs } from "@/composables/useBreadcrumbs";

const isLoading = ref(true);
const route = useRoute();
const form = ref(null);
const valid = ref(false);

// Form fields
const hasFullClassification = ref("");
const hasOperationContract = ref("");
const housingRating = ref(5);
const evidenceFiles = ref([]);
const expandedPanels = ref([]);
const cleaningStatus = ref(null);

const uploading = reactive({
  idCopy: false,
});

const { generateBreadcrumbItems } = useBreadcrumbs();

const breadcrumbItems = computed(() =>
  generateBreadcrumbItems([
    {
      title: "القطاعات الرئيسية",
      href: "/phases",
      disabled: false,
    },
    {
      title: "إضافة قطاع",
      href: route.path,
      disabled: true,
    },
  ])
);

const handleFileChange = async (type, newFiles) => {
  if (!newFiles || newFiles.length === 0) return;
  uploading[type] = true;
  try {
    // Simulate file upload
    await new Promise((resolve) => setTimeout(resolve, 1000));
    evidenceFiles.value = newFiles;
  } catch (error) {
    console.error("Error uploading file:", error);
  } finally {
    uploading[type] = false;
  }
};

const toggleAllPanels = () => {
  if (expandedPanels.value.length === 4) {
    expandedPanels.value = [];
  } else {
    expandedPanels.value = [0, 1, 2, 3];
  }
};

const validate = async () => {
  if (!form.value) return false;
  const { valid: isValid } = await form.value.validate();
  return isValid;
};

const links = ref([
  { title: "إدارة المواقع", href: "/", disabled: false },
  { title: "تفاصيل للسكن", href: "/sites", disabled: false },
  { title: "شركة رواف منى", href: "/sites/company-details", disabled: false },
  { title: "التقييمات", href: "/sites/company-details", disabled: true },
]);

onMounted(() => {
  setTimeout(() => {
    isLoading.value = false;
  }, 100);
});

defineExpose({
  validate,
  files: evidenceFiles,
});
</script>

<template>
  <div class="rate-form">
    <v-row v-if="isLoading">
      <v-col v-for="item in 4" :key="item" cols="6" sm="6" md="3">
        <VCard elevation="0">
          <v-card-text class="px-1 py-2">
            <v-skeleton-loader
              class="mx-auto"
              elevation="0"
              max-width="400"
              type="list-item-two-line, table-heading"
            ></v-skeleton-loader>
          </v-card-text>
        </VCard>
      </v-col>
    </v-row>

    <div v-else>
      <BreadcrumbNav :items="breadcrumbItems" />
      <v-container fluid class="mt-6 px-0 rounded-lg">
        <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
          <!-- Header Section -->
          <v-card-title
            class="d-flex align-center justify-space-between ga-4 pt-4"
          >
            <div>
              <h2 class="text-h5 font-weight-bold mb-4">#434343434</h2>
              <p class="text-subtitle-2">12 مارس 2024 - 12:24 مساءً</p>
            </div>
            <v-btn
              color="primary"
              variant="tonal"
              class="custom-btn"
              @click="toggleAllPanels"
            >
              {{ expandedPanels.length === 4 ? "إغلاق الكل" : "فتح الكل" }}
            </v-btn>
          </v-card-title>

          <v-form class="form-colors" ref="form" v-model="valid">
            <v-expansion-panels v-model="expandedPanels" class="mt-4" multiple>
              <v-expansion-panel
                class="custom-expansion mt-4"
                title="بوابه السكن"
                value="3"
              >
                <template v-slot:actions>
                  <svg
                    width="24"
                    height="24"
                    viewBox="0 0 24 24"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path
                      opacity="0.4"
                      d="M18.714 15.7442C19.1587 15.4167 19.2537 14.7907 18.9262 14.346C18.6718 14.0006 18.4174 13.6722 18.1942 13.3863C17.7487 12.8154 17.1354 12.0509 16.469 11.2834C15.807 10.5209 15.0729 9.73264 14.3877 9.12714C14.0462 8.8254 13.6932 8.5479 13.349 8.34042C13.0323 8.14949 12.5971 7.93903 12.121 7.93903C11.6449 7.93903 11.2096 8.14949 10.8929 8.34042C10.5487 8.5479 10.1958 8.8254 9.85429 9.12714C9.169 9.73264 8.43497 10.5209 7.77293 11.2834C7.10657 12.0509 6.49325 12.8154 6.04773 13.3863C5.82456 13.6722 5.57012 14.0006 5.31573 14.346C4.98821 14.7907 5.0832 15.4167 5.5279 15.7442C5.70666 15.8758 5.91472 15.9392 6.12092 15.939L12.121 15.939L18.121 15.939C18.3272 15.9392 18.5353 15.8758 18.714 15.7442Z"
                      fill="#B69265"
                    />
                    <path
                      d="M18.714 15.7442C19.1587 15.4167 19.2537 14.7907 18.9262 14.346C18.6718 14.0006 18.4174 13.6722 18.1942 13.3863C17.7487 12.8154 17.1354 12.0509 16.469 11.2834C15.807 10.5209 15.0729 9.73264 14.3877 9.12714C14.0462 8.8254 13.6932 8.5479 13.349 8.34042C13.0323 8.14949 12.5971 7.93903 12.121 7.93903C11.6449 7.93903 11.2096 8.14949 10.8929 8.34042C10.5487 8.5479 10.1958 8.8254 9.85429 9.12714C9.28713 9.62826 8.68658 10.2546 8.12109 10.8878L13.1211 15.939L18.121 15.939C18.3272 15.9392 18.5353 15.8758 18.714 15.7442Z"
                      fill="#B69265"
                    />
                  </svg>
                </template>
                <template v-slot:text>
                  <div class="bg-white rounded-lg pa-4">
                    <div class="mb-5">
                      <div>
                        <div class="mb-5">
                          هل تم تنظيف السكن بالكامل وفق المعايير المطلوبة؟
                        </div>
                        <div class="radio-group">
                          <label class="radio-label">
                            <input
                              type="radio"
                              name="cleaning"
                              value="yes"
                              v-model="cleaningStatus"
                              class="radio-input"
                            />
                            <span class="radio-text">نعم</span>
                          </label>
                          <label class="radio-label">
                            <input
                              type="radio"
                              name="cleaning"
                              value="no"
                              v-model="cleaningStatus"
                              class="radio-input"
                            />
                            <span class="radio-text">لا</span>
                          </label>
                        </div>
                      </div>
                    </div>

                    <UploadFile
                      v-model="evidenceFiles"
                      :loading="uploading.idCopy"
                      title="إرفاق أدلة *"
                      :required="true"
                      :multiple="true"
                      :maxFiles="3"
                      class="mb-4"
                      @change="(files) => handleFileChange('idCopy', files)"
                    />
                  </div>
                  <div class="bg-white rounded-lg pa-4 mt-4">
                    <div class="mb-4">
                      <label class="text-subtitle-2 mb-2 d-block"
                        >من هو الشخص المسؤول عن عملية التنظيف؟</label
                      >
                      <v-text-field
                        v-model="housingPermitNumber"
                        hide-details
                        variant="outlined"
                        density="comfortable"
                        placeholder="من هو الشخص المسؤول عن عملية التنظيف؟"
                        class="rounded-pill bg-grey-lighten-4"
                        required
                      />
                    </div>
                  </div>
                  <div class="bg-white rounded-lg pa-4 mt-4">
                    <div class="mb-4">
                      <label class="text-subtitle-2 mb-2 d-block"
                        >ما هي درجة تقييم حالة السكن بعد التفتيش؟</label
                      >
                      <v-text-field
                        v-model="housingPermitNumber"
                        hide-details
                        variant="outlined"
                        density="comfortable"
                        placeholder="ما هي درجة تقييم حالة السكن بعد التفتيش؟"
                        class="rounded-pill bg-grey-lighten-4"
                        required
                      />
                    </div>
                  </div>
                </template>
              </v-expansion-panel>

              <v-expansion-panel
                class="custom-expansion mt-4"
                title="داخل السكن"
                value="2"
              >
                <template v-slot:text>
                  <v-row>
                    <v-col cols="12">
                      <v-slider
                        v-model="housingRating"
                        :min="0"
                        :max="10"
                        :step="1"
                        thumb-label
                        class="custom-slider"
                      ></v-slider>
                    </v-col>
                  </v-row>
                </template>
              </v-expansion-panel>
            </v-expansion-panels>
          </v-form>
        </VCard>
      </v-container>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

// Add custom radio styles
:deep(.v-radio-group) {
  .v-radio {
    margin-right: 12px;

    .v-selection-control--dirty {
      background-color: rgba(78, 145, 88, 1);
      border-radius: 8px;
      padding: 8px 16px;
      margin-left: 8px;

      .v-label {
        color: white;
      }
    }
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

.custom-expansion {
  border: none !important;
  background-color: #f8f9f9 !important;
  border-radius: 8px !important;
  margin-bottom: 1rem;

  :deep(.v-expansion-panel-title) {
    padding: 16px 20px;
    min-height: unset;
    font-weight: 500;
  }

  :deep(.v-expansion-panel-text) {
    padding: 0 20px 16px;
  }
}

.custom-slider {
  :deep(.v-slider-thumb__label) {
    background-color: #b69265;
  }

  :deep(.v-slider-track__fill) {
    background-color: #b69265;
  }

  :deep(.v-slider-thumb) {
    color: #b69265;
  }
}

.custom-btn {
  background-color: #b69265 !important;
  color: white !important;
  font-weight: 500;
  padding: 0 20px;
  height: 40px;
}

.text-13 {
  font-size: 13px;
}

.radio-group {
  display: flex;
  gap: 16px;
  margin-top: 8px;
}

.radio-label {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  padding: 8px 16px;
  border-radius: 40px;
  // border: 1px solid #e0e0e0;
  transition: all 0.3s ease;
  width: 50%;
  background-color: rgba(249, 249, 249, 1);

  &:hover {
    background-color: rgba(249, 249, 249, 1);
  }
}

.radio-input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
  background-color: rgba(249, 249, 249, 1);

  &:checked + .radio-text {
    color: white;
  }

  &:checked ~ .radio-label {
    background-color: rgba(78, 145, 88, 1);
    border-color: rgba(78, 145, 88, 1);
  }
}

.radio-text {
  font-size: 14px;
  color: #333;
  transition: color 0.3s ease;
}

input[type="radio"]:checked + .radio-text {
  color: white;
}

.radio-label:has(.radio-input:checked) {
  background-color: rgba(78, 145, 88, 1);
  border-color: rgba(78, 145, 88, 1);
}

.rate-form {
  :deep(.v-field) {
    border-radius: 40px !important;
    background-color: rgba(249, 249, 249, 1) !important;

    &.v-field--focused {
      background-color: rgba(249, 249, 249, 1) !important;
    }

    .v-field__outline {
      display: none;
    }

    .v-field__input {
      padding: 8px 16px;
      min-height: 44px;
    }
  }

  :deep(.v-text-field) {
    .v-input__control {
      background-color: transparent;
    }
  }
}
</style>
