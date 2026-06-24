<template>
  <div>
    <v-container fluid>
      <VRow>
        <VCol cols="12">
          <VCard class="px-6 py-6" rounded="lg">
            <VCardTitle class="d-flex justify-space-between px-0 py-0 mb-8">
              <h2 class="text-h5">{{ t('companies.profile.title') }}</h2>
              <VBtn class="bg-primary_2" :prepend-icon="Editicon" variant="flat" @click="isAddDialogVisible = true">
                {{ t('companies.profile.editButton') }}
              </VBtn>
            </VCardTitle>

            <VCardText class="bg-primary_2 rounded-lg py-6">
              <v-row>
                <v-col class="d-flex justify-start align-start gap-3" cols="12">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Company />
                  </span>
                  <div>
                    <div class="info-label">{{ t('companies.profile.companyName') }}</div>
                    <div class="info-value">
                      {{ companyDetails.companyName }}
                    </div>
                  </div>
                </v-col>
                <v-col class="d-flex justify-start align-start gap-3" cols="12">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Userinfo />
                  </span>
                  <div>
                    <div class="info-label">{{ t('companies.profile.licenseNumber') }}</div>
                    <div class="info-value">
                      {{ companyDetails.licenseNo }}
                    </div>
                  </div>
                </v-col>
                <v-col class="d-flex justify-start align-start gap-3" cols="12">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Call />
                  </span>
                  <div>
                    <div class="info-label">{{ t('companies.profile.contactNumber') }}</div>
                    <div class="info-value">{{ companyDetails.phone }}</div>
                  </div>
                </v-col>
                <v-col class="d-flex justify-start align-start gap-3" cols="12">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Mail />
                  </span>
                  <div>
                    <div class="info-label">{{ t('companies.profile.email') }}</div>
                    <div class="info-value">{{ companyDetails.email }}</div>
                  </div>
                </v-col>
                <v-col class="d-flex justify-start align-start gap-3" cols="12">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Globmaps />
                  </span>
                  <div>
                    <div class="info-label">{{ t('companies.profile.city') }}</div>
                    <div class="info-value">{{ companyDetails.city }}</div>
                  </div>
                </v-col>
                <v-col class="d-flex justify-start align-start gap-3" cols="12">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Locationicon />
                  </span>
                  <div>
                    <div class="info-label">{{ t('companies.profile.address') }}</div>
                    <div class="info-value">{{ companyDetails.address }}</div>
                  </div>
                </v-col>
              </v-row>
            </VCardText>
          </VCard>
        </VCol>
      </VRow>
    </v-container>

    <!-- Add Company Dialog -->
    <VDialog v-model="isAddDialogVisible" max-width="600px">
      <VCard rounded="lg">
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">{{ t('companies.profile.editDialog.title') }}</h5>
              <p class="text-sm text-secondary mb-0">
                {{ t('companies.profile.editDialog.subtitle') }}
              </p>
            </div>
            <button @click="isAddDialogVisible = false">
              <v-icon size="sm">mdi-close</v-icon>
            </button>
          </div>
        </VCardTitle>
        <VCardText>
          <VForm ref="form">
            <VRow>
              <VCol cols="12">
                <label class="mb-2 d-block" for="">{{ t('companies.dialog.companyNameLabel') }}</label>
                <VTextField
                  v-model="formData.companyName"
                  :placeholder="t('companies.dialog.companyNamePlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>

              <VCol cols="12">
                <label class="mb-2 d-block" for="">{{ t('companies.dialog.licenseLabel') }}</label>
                <VTextField
                  v-model="formData.licenseNo"
                  :placeholder="t('companies.dialog.licensePlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>

              <VCol cols="12">
                <label class="mb-2 d-block" for="">{{ t('companies.dialog.cityLabel') }}</label>
                <VSelect
                  v-model="formData.city"
                  :items="['الرياض', 'جدة']"
                  :placeholder="t('companies.dialog.cityPlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>

              <VCol cols="12">
                <label class="mb-2 d-block" for="">{{ t('companies.dialog.phoneLabel') }}</label>
                <VTextField
                  v-model="formData.phone"
                  :placeholder="t('companies.dialog.phonePlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>

              <VCol cols="12">
                <label class="mb-2 d-block" for="">{{ t('companies.dialog.emailLabel') }}</label>
                <VTextField
                  v-model="formData.email"
                  :placeholder="t('companies.dialog.emailPlaceholder')"
                  required
                  rounded="lg"
                  type="email"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block" for="">{{ t('companies.dialog.addressLabel') }}</label>
                <VTextField
                  v-model="formData.address"
                  :placeholder="t('companies.dialog.addressPlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>
            </VRow>
          </VForm>
        </VCardText>

        <VCardActions class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between">
          <VBtn class="text-black rounded-xl bg-white" variant="outlined" @click="isAddDialogVisible = false">
            {{ t('common.cancel') }}
          </VBtn>
          <VBtn class="rounded-xl px-8" color="primary" variant="flat" @click="updateCompanyDetails">
            {{ t('common.save') }}
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>

    <!-- Success Dialog -->
    <SuccessDialog
      v-model="successDialogOpen"
      :action-text="t('companies.profile.editDialog.successAction')"
      :cancel-text="t('companies.profile.editDialog.successCancel')"
      :subtitle="t('companies.profile.editDialog.successSubtitle')"
      :title="t('companies.profile.editDialog.successTitle')"
      @action="successDialogOpen = false"
      @cancel="cancelActionSuccessDialog"
    />
  </div>
</template>
<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import Call from '@/components/icons/call.vue'
import Company from '@/components/icons/company.vue'
import Editicon from '@/components/icons/editicon.vue'
import Globmaps from '@/components/icons/globmaps.vue'
import Locationicon from '@/components/icons/locationicon.vue'
import Mail from '@/components/icons/mail.vue'
import Userinfo from '@/components/icons/userinfo.vue'
import SuccessDialog from '@/components/SuccessDialog.vue'

const { t } = useI18n()

const companyDetails = ref({
  companyName: 'شركة راية لخدمات الحج والعمرة',
  licenseNo: '2990029183121',
  city: 'الرياض',
  phone: '+99629987897',
  email: ' your.username@example.com',
  address: 'الرياض ، بجوار برج الرياض',
})
const isAddDialogVisible = ref(false)
const successDialogOpen = ref(false)

const formData = ref({ ...companyDetails.value })

const updateCompanyDetails = () => {
  companyDetails.value = { ...formData.value }
  isAddDialogVisible.value = false
  successDialogOpen.value = true
}

const cancelActionSuccessDialog = () => {
  successDialogOpen.value = false
  isAddDialogVisible.value = true
}
</script>
<style scoped>
.info-label {
  margin-bottom: 4px;
  color: #667178;
}

.info-value {
  font-weight: bold;
  color: #25343e;
}
</style>
