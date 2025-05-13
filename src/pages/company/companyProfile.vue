<template>
  <div>
    <v-container fluid>
      <VRow>
        <VCol cols="12">
          <VCard rounded="lg" class="px-6 py-6">
            <VCardTitle class="d-flex justify-space-between px-0 py-0 mb-8">
              <h2 class="text-h5">بيانات الشركة</h2>
              <VBtn
                :prepend-icon="Editicon"
                variant="flat"
                class="bg-primary_2"
                @click="isAddDialogVisible = true"
              >
                تعديل
              </VBtn>
            </VCardTitle>

            <VCardText class="bg-primary_2 rounded-lg py-6">
              <v-row>
                <v-col cols="12" class="d-flex justify-start align-start gap-3">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Company />
                  </span>
                  <div>
                    <div class="info-label">اسم الشركة</div>
                    <div class="info-value">
                      {{ companyDetails.companyName }}
                    </div>
                  </div>
                </v-col>
                <v-col cols="12" class="d-flex justify-start align-start gap-3">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Userinfo />
                  </span>
                  <div>
                    <div class="info-label">رقم الترخيص</div>
                    <div class="info-value">
                      {{ companyDetails.license_no }}
                    </div>
                  </div>
                </v-col>
                <v-col cols="12" class="d-flex justify-start align-start gap-3">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Call />
                  </span>
                  <div>
                    <div class="info-label">رقم التواصل</div>
                    <div class="info-value">{{ companyDetails.phone }}</div>
                  </div>
                </v-col>
                <v-col cols="12" class="d-flex justify-start align-start gap-3">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Mail />
                  </span>
                  <div>
                    <div class="info-label">البريد الإلكترونى</div>
                    <div class="info-value">{{ companyDetails.email }}</div>
                  </div>
                </v-col>
                <v-col cols="12" class="d-flex justify-start align-start gap-3">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Globmaps />
                  </span>
                  <div>
                    <div class="info-label">المدينة</div>
                    <div class="info-value">{{ companyDetails.city }}</div>
                  </div>
                </v-col>
                <v-col cols="12" class="d-flex justify-start align-start gap-3">
                  <span class="px-3 py-1 bg-white rounded-lg">
                    <Locationicon />
                  </span>
                  <div>
                    <div class="info-label">العنوان</div>
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
              <h5 class="mb-2">تعديل بيانات الشركة</h5>
              <p class="text-sm text-secondary mb-0">
                يرجى تعديل بيانات الشركة بدقة. الحقول الإلزامية محددة بعلامة
                (*).
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
                <label for="" class="mb-2 d-block">اسم الشركة *</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.companyName"
                  placeholder="الإسم الكامل الرسمى"
                  required
                />
              </VCol>

              <VCol cols="12">
                <label for="" class="mb-2 d-block">رقم الترخيص*</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.license_no"
                  placeholder="رقم الترخيص"
                  required
                />
              </VCol>

              <VCol cols="12">
                <label for="" class="mb-2 d-block">المدينة*</label>
                <VSelect
                  rounded="lg"
                  :items="['الرياض', 'جدة']"
                  v-model="formData.city"
                  placeholder="الرياض، مكة، جدة..."
                  required
                />
              </VCol>

              <VCol cols="12">
                <label for="" class="mb-2 d-block">رقم التواصل*</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.phone"
                  placeholder="رقم التواصل"
                  required
                />
              </VCol>

              <VCol cols="12">
                <label for="" class="mb-2 d-block">البريد الإلكتروني*</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.email"
                  placeholder="البريد الإلكتروني"
                  type="email"
                  required
                />
              </VCol>
              <VCol cols="12">
                <label for="" class="mb-2 d-block">العنوان</label>
                <VTextField
                  rounded="lg"
                  v-model="formData.address"
                  placeholder="العنوان التفصيلى إن وجد"
                  required
                />
              </VCol>
            </VRow>
          </VForm>
        </VCardText>

        <VCardActions
          class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between"
        >
          <VBtn
            variant="outlined"
            class="text-black rounded-xl bg-white"
            @click="isAddDialogVisible = false"
          >
            إلغاء
          </VBtn>
          <VBtn
            color="primary"
            variant="flat"
            class="rounded-xl px-8"
            @click="updateCompanyDetails"
          >
            حفظ
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>

    <!-- Success Dialog -->
    <SuccessDialog
      title="تم تعديل بيانات الشركة بنجاح!"
      subtitle="يمكنك الآن العودة إلى بيانات الشركة أو متابعة التعديل إن لزم الأمر."
      cancel-text="تعديل بيانات أخرى"
      action-text="العودة إلى بيانات الشركة"
      v-model="successDialogOpen"
      @cancel="cancelActionSuccessDialog"
      @action="successDialogOpen = false"
    />
  </div>
</template>
<script setup>
import Call from "@/components/icons/call.vue";
import Company from "@/components/icons/company.vue";
import Editicon from "@/components/icons/editicon.vue";
import Globmaps from "@/components/icons/globmaps.vue";
import Locationicon from "@/components/icons/locationicon.vue";
import Mail from "@/components/icons/mail.vue";
import Userinfo from "@/components/icons/userinfo.vue";
import SuccessDialog from "@/components/SuccessDialog.vue";
const companyDetails = ref({
  companyName: "شركة راية لخدمات الحج والعمرة",
  license_no: "2990029183121",
  city: "الرياض",
  phone: "+99629987897",
  email: " your.username@example.com",
  address: "الرياض ، بجوار برج الرياض",
});
const isAddDialogVisible = ref(false);
const successDialogOpen = ref(false);

const formData = ref({ ...companyDetails.value });

const updateCompanyDetails = () => {
  companyDetails.value = { ...formData.value };
  isAddDialogVisible.value = false;
  successDialogOpen.value = true;
};

const cancelActionSuccessDialog = () => {
  successDialogOpen.value = false;
  isAddDialogVisible.value = true;
};
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
