<template>
  <div>
    <VCard class="rounded-lg">
      <VCardText>
        <div class="d-flex justify-space-between align-center mb-6">
          <h2 class="text-h5 font-weight-bold">تفاصيل الشركة</h2>
          <VBtn color="primary" :loading="isSubmitting" @click="handleSubmit">
            حفظ التغييرات
          </VBtn>
        </div>

        <VForm ref="form" @submit.prevent="handleSubmit">
          <VRow>
            <VCol cols="12" md="6">
              <VTextField
                v-model="formData.name"
                label="اسم الشركة"
                required
                :rules="[(v) => !!v || 'اسم الشركة مطلوب']"
              />
            </VCol>

            <VCol cols="12" md="6">
              <VTextField
                v-model="formData.license_no"
                label="رقم الترخيص"
                required
                :rules="[(v) => !!v || 'رقم الترخيص مطلوب']"
              />
            </VCol>

            <VCol cols="12" md="6">
              <VTextField
                v-model="formData.center_no"
                label="رقم المركز"
                required
                :rules="[(v) => !!v || 'رقم المركز مطلوب']"
              />
            </VCol>

            <VCol cols="12" md="6">
              <VTextField
                v-model="formData.phone"
                label="رقم الهاتف"
                required
                :rules="[(v) => !!v || 'رقم الهاتف مطلوب']"
              />
            </VCol>

            <VCol cols="12" md="6">
              <VTextField
                v-model="formData.email"
                label="البريد الإلكتروني"
                type="email"
                required
                :rules="[
                  (v) => !!v || 'البريد الإلكتروني مطلوب',
                  (v) => /.+@.+\..+/.test(v) || 'البريد الإلكتروني غير صحيح',
                ]"
              />
            </VCol>

            <VCol cols="12" md="6">
              <VTextField
                v-model="formData.contact_name"
                label="اسم جهة الاتصال"
                required
                :rules="[(v) => !!v || 'اسم جهة الاتصال مطلوب']"
              />
            </VCol>

            <VCol cols="12" md="6">
              <VTextField
                v-model="formData.pligrims_nationality"
                label="جنسية الحجاج"
                required
                :rules="[(v) => !!v || 'جنسية الحجاج مطلوبة']"
              />
            </VCol>

            <VCol cols="12">
              <label class="text-subtitle-2 mb-2 d-block"
                >الموقع على الخريطة</label
              >
              <LocationPicker
                v-model="selectedLocation"
                class="rounded-lg overflow-hidden"
                style="height: 300px"
                @update:model-value="handleLocationChange"
              />
              <!-- Coordinates display -->
              <div class="d-flex gap-4 mt-2">
                <VTextField
                  v-model="formData.latitude"
                  label="خط العرض"
                  readonly
                  variant="outlined"
                  density="comfortable"
                  class="rounded-lg"
                  hide-details
                />
                <VTextField
                  v-model="formData.longitude"
                  label="خط الطول"
                  readonly
                  variant="outlined"
                  density="comfortable"
                  class="rounded-lg"
                  hide-details
                />
              </div>
            </VCol>

            <VCol cols="12">
              <VTextField
                v-model="formData.address"
                label="العنوان"
                required
                :rules="[(v) => !!v || 'العنوان مطلوب']"
              />
            </VCol>
          </VRow>
        </VForm>
      </VCardText>
    </VCard>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "@axios";
import { useAppStore } from "@/stores/app";
import LocationPicker from "@/components/maps/LocationPicker.vue";

const route = useRoute();
const router = useRouter();
const appStore = useAppStore();

const form = ref(null);
const isSubmitting = ref(false);
const selectedLocation = ref({ lat: 21.4225, lng: 39.8262 }); // Default Makkah coordinates

const formData = ref({
  name: "",
  license_no: "",
  center_no: "",
  phone: "",
  email: "",
  contact_name: "",
  pligrims_nationality: "",
  latitude: "21.4225",
  longitude: "39.8262",
  address: "",
  created_by_id: 1,
});

const handleLocationChange = (newLocation) => {
  if (newLocation) {
    formData.value.latitude = newLocation.lat.toString();
    formData.value.longitude = newLocation.lng.toString();
  }
};

const fetchCompanyDetails = async () => {
  try {
    const response = await axios.get("/companies/" + route.params.id);
    if (response.data.status.success) {
      formData.value = {
        ...response.data.data,
        created_by_id: 1,
      };
    }
  } catch (error) {
    appStore.showSnackbar({
      message: "حدث خطأ أثناء تحميل بيانات الشركة",
      color: "error",
    });
    console.error(error);
    router.push("/company");
  }
};

const handleSubmit = async () => {
  const { valid } = await form.value.validate();

  if (!valid) {
    appStore.showSnackbar({
      message: "يرجى التحقق من صحة جميع الحقول",
      color: "error",
    });
    return;
  }

  try {
    isSubmitting.value = true;
    await axios.put("/companies/" + route.params.id, formData.value);

    appStore.showSnackbar({
      message: "تم تحديث بيانات الشركة بنجاح",
      color: "success",
    });
  } catch (error) {
    appStore.showSnackbar({
      message: "حدث خطأ أثناء تحديث بيانات الشركة",
      color: "error",
    });
    console.error(error);
  } finally {
    isSubmitting.value = false;
  }
};

onMounted(() => {
  fetchCompanyDetails();
});
</script>

<style lang="scss" scoped>
:deep(.v-field) {
  border-radius: 8px;
  background-color: rgb(250, 250, 250);
}
</style>
