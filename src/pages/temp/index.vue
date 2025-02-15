<script setup>
import { useAppStore } from "@/stores/app";
import AiAnalysisTable from "./components/table.vue";
import PerformancePercentageWidget from "@/@core/components/widgets/PerformancePercentageWidget.vue";
import InfrastructureStatusWidget from "@/@core/components/widgets/InfrastructureStatusWidget.vue";

const appStore = useAppStore();

// 👉 Fetch Banners
onMounted(() => {
  appStore.updateAppPageTitle("تحليل النماذج AI");
});
</script>

<template>
  <div>
    <v-row class="statistics-cards mb-3">
      <v-col cols="12" sm="6" md="3">
        <v-card
          class="border d-flex justify-space-between align-center pa-4 h-100"
        >
          <template v-slot:title>
            <h2 class="text-h6 font-weight-semibold">حالة البنية التحتية</h2>
          </template>
          <template v-slot:subtitle>
            <div class="percentage">56%</div>
            <div>
              <span class="text-gray-dark font-weight-semibold">
                اخر تحديث
              </span>
              <span class="percentage-update"> +89 </span>
            </div>
          </template>
          <img src="@/assets/images/bus.svg" alt="drown" />
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card
          class="border d-flex justify-space-between align-center pa-4 h-100"
        >
          <template v-slot:title>
            <h2 class="text-h6 font-weight-semibold">متوسط زمن الاستجابة</h2>
          </template>
          <template v-slot:subtitle>
            <div class="percentage">56%</div>
            <div>
              <span class="text-gray-dark font-weight-semibold">
                اخر تحديث
              </span>
              <span class="percentage-update"> +89 </span>
            </div>
          </template>
          <img src="@/assets/images/alarm.svg" alt="drown" />
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card
          class="border d-flex justify-space-between align-center pa-4 h-100"
        >
          <template v-slot:title>
            <h2 class="text-h6 font-weight-semibold">نسبة الأداء</h2>
          </template>
          <template v-slot:subtitle>
            <div class="percentage">56%</div>
            <div>
              <span class="text-gray-dark font-weight-semibold">
                اخر تحديث
              </span>
              <span class="percentage-update"> +89 </span>
            </div>
          </template>
          <img src="@/assets/images/statistics.svg" alt="drown" />
        </v-card>
      </v-col>
      <v-col cols="12" sm="6" md="3">
        <v-card
          class="border d-flex justify-space-between align-center pa-4 h-100"
        >
          <template v-slot:title>
            <h2 class="text-h6 font-weight-semibold">الاجراءات</h2>
          </template>
          <template v-slot:subtitle>
            <div class="percentage">56</div>
          </template>
          <img src="@/assets/images/process.svg" alt="drown" />
        </v-card>
      </v-col>
    </v-row>
    <AiAnalysisTable class="mb-6" />
    <v-row style="height: 400px">
      <v-col lg="4" class="d-flex">
        <PerformancePercentageWidget class="w-100 h-100" />
      </v-col>
      <v-col lg="8" class="d-flex">
        <InfrastructureStatusWidget class="w-100 h-100" />
      </v-col>
    </v-row>
  </div>
</template>

<style lang="scss">
.statistics-cards {
  .v-card-item {
    padding: 0 !important;
  }

  .card-title {
    font-size: 1.5rem;
    font-weight: 900;
    margin-bottom: 2rem;
  }
  .card-subtitle {
    font-size: 1.25rem;
    font-weight: 900;
    color: #344054;
  }
  .percentage {
    font-size: 1rem;
    font-weight: 900;
    color: #00758d;
    font-weight: 600;
    margin-bottom: 0.6rem;
    margin-top: 0.6rem;
    span {
      font-size: 12px;
      font-weight: 500;
    }
  }
  .percentage-update {
    color: #34c759 !important  ;
    font-size: 12px;
    font-weight: 900;
    margin-right: 0.5rem;
  }
  .drown-img-container {
    width: 100px;
    height: 100px;
    background-color: #e5fbff;
    border-radius: 50%;
    display: flex;
    justify-content: center;
    align-items: center;
    position: relative;
    img {
      width: 64px;
      height: 64px;
    }
    .check-container {
      width: 20px;
      height: 20px;
      background-color: #008000;
      border-radius: 50%;
      display: flex;
      justify-content: center;
      align-items: center;
      position: absolute;
      bottom: 0;
      right: 0;
    }
  }
}
</style>
