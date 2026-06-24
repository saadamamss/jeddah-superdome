<template>
  <v-dialog v-model="detailsDialog" max-width="600" persistent>
    <v-card rounded="lg">
      <v-card-title class="text-h5 mb-6 bg-primary_2 py-4 rounded-lg white--text d-flex justify-space-between">
        <h3 class="text-h6">تفاصيل الحاج {{ pilgrim?.name }}</h3>
        <button @click="detailsDialog = false">
          <v-icon size="">mdi-close</v-icon>
        </button>
      </v-card-title>

      <v-tabs v-model="tab" class="px-6" color="primary" grow height="60px" hide-slider>
        <v-tab
          class="rounded-lg shadow-sm mx-2 tab-shadow"
          :class="tab == 'pilgrim' ? 'bg-primary text-white' : 'bg-white'"
          style="height: 40px"
          value="pilgrim"
        >
          البيانات الأساسية
        </v-tab>
        <v-tab
          class="rounded-lg shadow-sm mx-2 tab-shadow"
          :class="tab == 'trip' ? 'bg-primary text-white' : 'bg-white'"
          style="height: 40px"
          value="trip"
          >بيانات الرحلة والتفويج</v-tab
        >
      </v-tabs>

      <v-tabs-window v-model="tab">
        <v-tabs-window-item value="pilgrim">
          <v-card-text>
            <v-row v-if="pilgrim">
              <v-col class="d-flex justify-start align-start gap-3 border-b border-secondary" cols="12">
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Person />
                </span>
                <div>
                  <div class="info-label">الأسم الكامل</div>
                  <div class="info-value">{{ pilgrim.name }}</div>
                </div>
              </v-col>
              <v-col class="d-flex justify-start align-start gap-3 border-b border-secondary" cols="12">
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Personinfo />
                </span>
                <div>
                  <div class="info-label">رقم الهوية</div>
                  <div class="info-value">{{ pilgrim.identity }}</div>
                </div>
              </v-col>
              <v-col class="d-flex justify-start align-start gap-3 border-b border-secondary" cols="12">
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Company />
                </span>
                <div>
                  <div class="info-label">اسم الشركة</div>
                  <div class="info-value">{{ pilgrim.company || '—' }}</div>
                </div>
              </v-col>
              <v-col class="d-flex justify-start align-start gap-3 border-b border-secondary" cols="12">
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Identity />
                </span>
                <div>
                  <div class="info-label">الرقم المرجعى للشركة</div>
                  <div class="info-value">{{ pilgrim.companyId || '—' }}</div>
                </div>
              </v-col>
              <v-col class="d-flex justify-start align-start gap-3 border-b border-secondary" cols="12">
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Translation />
                </span>
                <div>
                  <div class="info-label">الجنسية</div>
                  <div class="info-value">{{ pilgrim.nationality }}</div>
                </div>
              </v-col>
              <v-col class="d-flex justify-start align-start gap-3 border-b border-secondary" cols="12">
                <span class="px-3 py-1 bg-primary_2 rounded-lg">
                  <Userquestion />
                </span>
                <div>
                  <div class="info-label">الجنس</div>
                  <div class="info-value">{{ pilgrim.gender }}</div>
                </div>
              </v-col>
            </v-row>
            <v-row v-else>
              <v-col cols="12" class="text-center py-10">
                <v-progress-circular indeterminate color="primary" />
              </v-col>
            </v-row>
          </v-card-text>
        </v-tabs-window-item>
        <v-tabs-window-item value="trip">
          <v-card-text>
            <v-row>
              <v-col cols="12" class="text-center py-10">
                <p class="text-medium-emphasis">لا توجد بيانات رحلة متاحة</p>
              </v-col>
            </v-row>
          </v-card-text>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { pilgrimsService } from '@/services/pilgrims.service'
import Company from '@/components/icons/company.vue'
import Identity from '@/components/icons/identity.vue'
import Person from '@/components/icons/person.vue'
import Personinfo from '@/components/icons/personinfo.vue'
import Translation from '@/components/icons/translation.vue'
import Userquestion from '@/components/icons/userquestion.vue'
import type { Pilgrim } from '@/types'

const props = defineProps<{ pilgrimId: number }>()
const tab = ref('pilgrim')
const detailsDialog = defineModel()
const pilgrim = ref<Pilgrim | null>(null)

watch(
  () => props.pilgrimId,
  (id: number) => {
    if (id) fetchPilgrim(id)
  },
  { immediate: true },
)

const fetchPilgrim = async (id: number) => {
  try {
    const res = await pilgrimsService.getById(id)
    pilgrim.value = res.data
  } catch {
    pilgrim.value = null
  }
}
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
