<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import axios from "@/plugins/axios";
import UserWidget from "@/@core/components/widgets/UserWidget.vue";

const route = useRoute();
const isLoading = ref(true);
const rating = ref(null);
const ratingBy = ref(null);

const fetchRatingDetails = async () => {
  try {
    isLoading.value = true;
    const response = await axios.get(`/ratings/${route.params.id}`);
    if (response.data.status.success) {
      rating.value = response.data.data;
      ratingBy.value = rating.value.rating_by;
    }
  } catch (error) {
    console.error("Error fetching rating details:", error);
  } finally {
    isLoading.value = false;
  }
};

onMounted(() => {
  fetchRatingDetails();
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
      <v-row v-if="isLoading">
        <v-col cols="12">
          <v-skeleton-loader type="article" />
        </v-col>
      </v-row>

      <div v-else-if="rating">
        <v-container fluid class="px-0 rounded-lg">
          <v-card elevation="0" class="rounded-lg px-4 bg-light-white">
            <!-- Header -->
            <v-card-title class="pt-4 pb-6">
              <div class="d-flex align-center justify-space-between ga-4">
                <div>
                  <h2 class="text-h5 font-weight-bold mb-2">
                    {{ rating.questionnaire?.title }}
                  </h2>
                  <div class="d-flex align-center gap-4">
                    <span class="text-grey"
                      >تاريخ التقييم: {{ rating.created_at }}</span
                    >
                    <v-chip
                      :color="
                        rating.status === 'PENDING' ? 'warning' : 'success'
                      "
                      size="small"
                    >
                      {{
                        rating.status === "PENDING" ? "قيد المراجعة" : "مكتمل"
                      }}
                    </v-chip>
                  </div>
                </div>
              </div>
            </v-card-title>

            <!-- Rating By User -->
            <v-card-text v-if="rating?.rating_by" class="pb-6">
              <div class="d-flex align-center mb-4">
                <span class="text-grey me-4">تم التقييم بواسطة:</span>

                <UserWidget
                  :user="rating.rating_by"
                  :no-shadow="true"
                  :transparent="true"
                  :no-padding="true"
                />
              </div>
            </v-card-text>

            <!-- Questionnaire Categories -->
            <v-card-text>
              <div
                v-for="category in rating.questionnaire
                  ?.questionnaire_categories"
                :key="category.id"
                class="mb-8"
              >
                <v-card variant="outlined" class="pa-4">
                  <h3 class="text-h6 font-weight-bold mb-6 primary--text">
                    {{ category.title }}
                  </h3>

                  <!-- Questions -->
                  <div v-if="category.questions?.length" class="questions-list">
                    <div
                      v-for="question in category.questions"
                      :key="question.id"
                      class="question-item mb-4 pb-4 border-b"
                    >
                      <p class="text-body-1 mb-3">{{ question.question }}</p>

                      <!-- Answer based on question type -->
                      <div class="answer-section">
                        <!-- Yes/No Answer -->
                        <template v-if="question.question_type === 'yes_no'">
                          <v-chip
                            :color="
                              question.answer?.yes_no_value === '1'
                                ? 'success'
                                : 'error'
                            "
                            size="small"
                          >
                            {{
                              question.answer?.yes_no_value === "1"
                                ? "نعم"
                                : "لا"
                            }}
                          </v-chip>
                        </template>

                        <!-- Rating Answer -->
                        <template
                          v-else-if="question.question_type === 'rating'"
                        >
                          <v-rating
                            :model-value="Number(question.answer?.rating_value)"
                            readonly
                            color="warning"
                            half-increments
                          ></v-rating>
                        </template>

                        <!-- Text Input Answer -->
                        <template
                          v-else-if="question.question_type === 'text_input'"
                        >
                          <p class="text-body-2 text-grey-darken-1">
                            {{
                              question.answer?.text_input_value ||
                              "لا توجد إجابة"
                            }}
                          </p>
                        </template>

                        <!-- Select Option Answer -->
                        <template
                          v-else-if="question.question_type === 'select'"
                        >
                          <p class="text-body-2 text-grey-darken-1">
                            {{
                              question.answer?.selected_option ||
                              "لا توجد إجابة"
                            }}
                          </p>
                        </template>

                        <!-- Answer Status -->
                        <div class="mt-2 d-flex align-center">
                          <v-chip
                            :color="
                              question.answer?.status === 'PENDING'
                                ? 'warning'
                                : 'success'
                            "
                            size="x-small"
                            class="me-2"
                          >
                            {{
                              question.answer?.status === "PENDING"
                                ? "قيد المراجعة"
                                : "تمت الموافقة"
                            }}
                          </v-chip>
                          <span
                            v-if="question.answer?.approved_at"
                            class="text-caption text-grey"
                          >
                            تمت الموافقة في: {{ question.answer.approved_at }}
                          </span>
                        </div>

                        <!-- Uploaded Files -->
                        <div
                          v-if="
                            question.has_upload && question.answer?.upload_urls
                          "
                          class="mt-3"
                        >
                          <v-btn
                            v-for="(url, index) in question.answer.upload_urls"
                            :key="index"
                            :href="url"
                            target="_blank"
                            variant="outlined"
                            color="primary"
                            size="small"
                            class="me-2"
                            prepend-icon="mdi-file-document-outline"
                          >
                            عرض الملف {{ index + 1 }}
                          </v-btn>
                        </div>
                      </div>
                    </div>
                  </div>
                  <p v-else class="text-grey text-center py-4">
                    لا توجد أسئلة في هذا القسم
                  </p>
                </v-card>
              </div>
            </v-card-text>
          </v-card>
        </v-container>
      </div>
      <div v-else class="text-center py-8">
        <p>لم يتم العثور على التقييم</p>
      </div>
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}

.questions-list {
  .question-item {
    &:not(:last-child) {
      border-bottom: 1px solid rgba(0, 0, 0, 0.12);
    }
  }
}

.answer-section {
  background-color: rgba(0, 0, 0, 0.02);
  padding: 12px;
  border-radius: 8px;
}
</style>
