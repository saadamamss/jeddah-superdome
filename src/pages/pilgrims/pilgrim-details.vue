<template>
  <v-dialog v-model="detailsDialog" max-width="600" persistent>
    <v-card rounded="lg">
      <v-card-title
        class="text-h5 mb-6 bg-primary_2 py-4 rounded-lg white--text d-flex justify-space-between"
      >
        <h3 class="text-h6">تفاصيل الحاج محمد بن عبدالله</h3>
        <button @click="detailsDialog = false">
          <v-icon size="">mdi-close</v-icon>
        </button>
      </v-card-title>

      <v-tabs
        v-model="tab"
        grow
        color="primary"
        height="60px"
        class="px-6"
        hide-slider
      >
        <v-tab
          value="pilgrim"
          class="rounded-lg shadow-sm mx-2 tab-shadow"
          style="height: 40px"
          :class="tab == 'pilgrim' ? 'bg-primary text-white' : 'bg-white'"
        >
          البيانات الأساسية
        </v-tab>
        <v-tab
          value="trip"
          class="rounded-lg shadow-sm mx-2 tab-shadow"
          style="height: 40px"
          :class="tab == 'trip' ? 'bg-primary text-white' : 'bg-white'"
          >بيانات الرحلة والتفويج</v-tab
        >
      </v-tabs>

      <v-tabs-window v-model="tab">
        <v-tabs-window-item value="pilgrim">
          <v-card-text>
            <v-row>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Person />
                </span>
                <div>
                  <div class="info-label">الأسم الكامل</div>
                  <div class="info-value">عبدالله محمد على القحطانى</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Personinfo />
                </span>
                <div>
                  <div class="info-label">رقم الهوية</div>
                  <div class="info-value">2990029183121</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Company />
                </span>
                <div>
                  <div class="info-label">اسم الشركة</div>
                  <div class="info-value">شركة راية لخدمات الحج والعمرة</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Identity />
                </span>
                <div>
                  <div class="info-label">الرقم المرجعى للشركة</div>
                  <div class="info-value">2998789709</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Translation />
                </span>
                <div>
                  <div class="info-label">الجنسية</div>
                  <div class="info-value">سعودي</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Userquestion />
                </span>
                <div>
                  <div class="info-label">الجنس</div>
                  <div class="info-value">ذكر</div>
                </div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-tabs-window-item>
        <v-tabs-window-item value="trip">
          <v-card-text>
            <v-row>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Stamp />
                </span>
                <div>
                  <div class="info-label">المرحلة</div>
                  <div class="info-value">بإنتظار التفويج</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Userdirection />
                </span>
                <div>
                  <div class="info-label">الوجهة التالية</div>
                  <div class="info-value">بوابة 4 خلال 15 دقيقة</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Calender />
                </span>
                <div>
                  <div class="info-label">موعد التفويج</div>
                  <div class="info-value">اليوم، الساعة 3:45 مساءً</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Door />
                </span>
                <div>
                  <div class="info-label">رقم البوابة</div>
                  <div class="info-value">4</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Way />
                </span>
                <div>
                  <div class="info-label">رقم المسار</div>
                  <div class="info-value">C2</div>
                </div>
              </v-col>
              <v-col
                cols="12"
                class="d-flex justify-start align-start gap-3 border-b border-secondary"
              >
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Bus />
                </span>
                <div>
                  <div class="info-label">رقم الحافلة</div>
                  <div class="info-value">27</div>
                </div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-card>
  </v-dialog>
</template>

<script setup>
import Bus from "@/components/icons/bus.vue";
import Calender from "@/components/icons/calender.vue";
import Company from "@/components/icons/company.vue";
import Door from "@/components/icons/door.vue";
import Identity from "@/components/icons/identity.vue";
import Person from "@/components/icons/person.vue";
import Personinfo from "@/components/icons/personinfo.vue";
import Stamp from "@/components/icons/stamp.vue";
import Translation from "@/components/icons/translation.vue";
import Userdirection from "@/components/icons/userdirection.vue";
import Userquestion from "@/components/icons/userquestion.vue";
import Way from "@/components/icons/way.vue";
import { ref } from "vue";
const tab = ref("pilgrim");
const detailsDialog = defineModel();
</script>
<style scoped lang="scss">
.info-label {
  margin-bottom: 4px;
  color: #667178;
}

.info-value {
  font-weight: bold;
  color: #25343e;
}

.tab-shadow {
  box-shadow: 0 0 10px 0.5px rgba(0, 0, 0, 0.08);
  border: none;
}

/* RTL support */
.v-card-text,
.v-tab {
  text-align: right;
  direction: rtl;
}

.v-tab {
  font-size: 1rem;
  font-weight: 500;
}
</style>
