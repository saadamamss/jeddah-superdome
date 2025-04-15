<script setup>
import { useAppStore } from "@/stores/app";
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "@/plugins/axios";
import BreadcrumbNav from "@/components/BreadcrumbNav.vue";
import { useBreadcrumbs } from "@/composables/useBreadcrumbs";

const appStore = useAppStore();
const route = useRoute();
const router = useRouter();
const isLoading = ref(true);
const questionnaire = ref(null);

const questionnaireId = computed(() => route.query.id);

// Setup breadcrumbs
const { generateBreadcrumbItems } = useBreadcrumbs();
const breadcrumbItems = computed(() =>
  generateBreadcrumbItems([
    {
      title: "الاستبيانات",
      href: "/questionnaires",
      disabled: false,
    },
    {
      title: questionnaire.value?.title || "تفاصيل الاستبيان",
      href: `/questionnaires/details?id=${questionnaireId.value}`,
      disabled: true,
    },
  ])
);

// Function to fetch questionnaire details
const fetchQuestionnaireDetails = async () => {
  try {
    isLoading.value = true;
    const response = await axios.get(`questionnaires/${questionnaireId.value}`);
    console.log("Questionnaire details response:", response.data);

    if (response.data?.status?.success) {
      questionnaire.value = response.data.data;
      console.log("Questionnaire data:", questionnaire.value);
    }
  } catch (error) {
    console.error("Error fetching questionnaire details:", error);
    appStore.showSnackbar({
      message:
        error.response?.data?.message || "حدث خطأ أثناء جلب بيانات الاستبيان",
      color: "error",
    });
    router.push("/questionnaires");
  } finally {
    isLoading.value = false;
  }
};

const goBack = () => {
  router.push("/questionnaires");
};

const formatDate = (date) => {
  if (!date) return "";
  try {
    const dateObj = new Date(date);
    if (isNaN(dateObj.getTime())) return date;

    // Format date in Arabic with Gregorian calendar
    const formatter = new Intl.DateTimeFormat("ar", {
      calendar: "gregory",
      day: "numeric",
      month: "short",
      year: "numeric",
      hour: "numeric",
      minute: "numeric",
      hour12: true,
    });

    return formatter.format(dateObj);
  } catch (error) {
    console.error("Error formatting date:", error);
    return date;
  }
};

onMounted(() => {
  fetchQuestionnaireDetails();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
      <!-- Breadcrumbs -->
      <BreadcrumbNav :items="breadcrumbItems" />

      <v-row v-if="isLoading">
        <v-col cols="12">
          <VCard elevation="0">
            <v-card-text class="px-1 py-2">
              <v-skeleton-loader
                class="mx-auto"
                elevation="0"
                max-width="100%"
                type="list-item-two-line, table-heading, table-row-divider, table-row, table-row-divider, table-row"
              ></v-skeleton-loader>
            </v-card-text>
          </VCard>
        </v-col>
      </v-row>

      <div v-else-if="questionnaire">
        <v-container fluid class="mt-٠ px-0 rounded-lg">
          <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
            <v-card-title class="pt-4 pb-6">
              <div
                class="d-flex align-center justify-space-between ga-4 border-b pb-4"
              >
                <div class="d-flex align-center gap-2">
                  <h2 class="text-h5 font-weight-bold">عرض الاستبيان</h2>
                </div>
              </div>

              <div class="mt-6">
                <div class="mb-4">
                  <div class="d-flex flex-column mb-6">
                    <div
                      class="d-flex justify-space-between align-center mb-3 border-b pb-3"
                    >
                      <div>
                        <h3 class="text-h6 font-weight-bold">
                          {{ questionnaire.title }}
                        </h3>
                      </div>
                      <div class="d-flex gap-2">
                        <v-chip color="primary" variant="outlined" size="small">
                          {{ questionnaire.contract?.name || "غير محدد" }}
                        </v-chip>
                        <v-chip color="primary" variant="outlined" size="small">
                          {{
                            questionnaire.imtithal_phase?.title || "غير محدد"
                          }}
                        </v-chip>
                      </div>
                    </div>

                    <div class="d-flex justify-space-between mb-4">
                      <div class="d-flex flex-column ga-1">
                        <!-- <span class="text-caption text-medium-emphasis"
                          >تاريخ الإنشاء</span
                        >
                        <span class="text-body-2" dir="rtl">
                          {{ formatDate(questionnaire.created_at) }}
                        </span> -->
                      </div>
                      <div class="d-flex flex-column ga-1">
                        <span class="text-sm text-medium-emphasis"
                          >المجموع الكلي للاستبانة</span
                        >
                        <span class="text-body-2" dir="rtl">
                          {{ questionnaire.questionnaire_weight }}
                        </span>
                      </div>
                    </div>
                  </div>

                  <div
                    v-if="
                      questionnaire.questionnaire_categories &&
                      questionnaire.questionnaire_categories.length > 0
                    "
                  >
                    <h4 class="text-subtitle-1 font-weight-bold mb-3">
                      الفئات والأسئلة
                    </h4>

                    <v-expansion-panels
                      elevation="0"
                      variant="accordion"
                      class="custom-panel mb-4"
                      model-value="0"
                    >
                      <v-expansion-panel
                        v-for="(
                          category, categoryIndex
                        ) in questionnaire.questionnaire_categories"
                        :key="categoryIndex"
                        class="mb-3 rounded-lg"
                      >
                        <v-expansion-panel-title>
                          <div class="d-flex align-center py-4">
                            <span style="color: #75797c; font-weight: 300">{{
                              category.title || "فئة " + (categoryIndex + 1)
                            }}</span>
                          </div>
                        </v-expansion-panel-title>
                        <v-expansion-panel-text>
                          <div
                            v-if="
                              category.questions &&
                              category.questions.length > 0
                            "
                            class="px-4"
                          >
                            <div
                              v-for="(
                                question, questionIndex
                              ) in category.questions"
                              :key="questionIndex"
                              class="pa-3 mb-3 border rounded-lg"
                            >
                              <!-- <div
                                class="d-flex justify-space-between align-center mb-2"
                              >
                                <label class="text-subtitle-2"
                                  >سؤال {{ questionIndex + 1 }}</label
                                >
                              </div> -->
                              <div
                                class="d-flex align-center justify-space-between"
                              >
                                <h2 class="mb-3 text-h6 font-weight-medium">
                                  {{ question.question }}
                                </h2>
                                <div class="d-flex align-start ga-2">
                                  <div class="pt-1">
                                    <img
                                      width="18"
                                      src="/imgs/icons/question.svg"
                                      alt="icon"
                                    />
                                  </div>

                                  <div class="d-flex flex-column">
                                    <small class="mb-1"> وزن السؤال </small>
                                    <span
                                      class="text-body-2 font-weight-medium"
                                    >
                                      {{
                                        question?.question_weight != null
                                          ? question.question_weight
                                          : "-"
                                      }}
                                    </span>
                                  </div>
                                </div>
                              </div>

                              <div class="d-flex gap-3">
                                <v-chip
                                  size="small"
                                  color="info"
                                  variant="outlined"
                                >
                                  {{
                                    question.type === "yes_no"
                                      ? "نعم/لا"
                                      : question.type === "text"
                                      ? "نص"
                                      : question.type === "number"
                                      ? "رقم"
                                      : question.type
                                  }}
                                </v-chip>
                                <v-chip
                                  size="small"
                                  color="warning"
                                  variant="outlined"
                                >
                                  {{
                                    question.condition === "no"
                                      ? "لا هو الصحيح"
                                      : question.condition === "yes"
                                      ? "نعم هو الصحيح"
                                      : question.condition === "none"
                                      ? "بدون شرط"
                                      : question.condition
                                  }}
                                </v-chip>
                              </div>
                            </div>
                          </div>
                          <div
                            v-else
                            class="text-center pa-4 text-medium-emphasis"
                          >
                            لا توجد أسئلة في هذه الفئة
                          </div>
                        </v-expansion-panel-text>
                      </v-expansion-panel>
                    </v-expansion-panels>
                  </div>
                  <div v-else class="text-center pa-4 text-medium-emphasis">
                    لا توجد فئات أو أسئلة في هذا الاستبيان
                  </div>
                </div>

                <div class="d-flex justify-end mt-6">
                  <v-btn
                    variant="outlined"
                    color="primary"
                    class="me-4"
                    @click="goBack"
                  >
                    العودة للقائمة
                  </v-btn>
                </div>
              </div>
            </v-card-title>
          </VCard>
        </v-container>
      </div>
      <div v-else class="text-center pa-4">
        <p class="text-h6 text-medium-emphasis">لم يتم العثور على الاستبيان</p>
        <v-btn color="primary" class="mt-4" @click="goBack"
          >العودة للقائمة</v-btn
        >
      </div>
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

:deep(.v-field) {
  border-radius: 8px;
  background-color: rgb(250, 250, 250);

  &.v-field--focused {
    background-color: white;
  }
}

:deep(.v-btn) {
  text-transform: none;
  letter-spacing: 0;
  border-radius: 8px;
}

:deep(.v-expansion-panel) {
  background-color: rgba(250, 250, 250, 0.5);

  &:hover {
    background-color: rgba(250, 250, 250, 0.8);
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

.border {
  border: 1px solid rgba(0, 0, 0, 0.12);
}

.border-b {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}

.ga-1 {
  gap: 0.25rem;
}

.ga-2 {
  gap: 0.5rem;
}

.ga-4 {
  gap: 1rem;
}
</style>
