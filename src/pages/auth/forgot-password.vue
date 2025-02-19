<script setup>
import { useAuthStore } from "@/stores/Auth";
import { phoneValidator, requiredValidator } from "@validators";
import { toast } from "vue3-toastify";

const formState = reactive({
  phone: null,
});

const auth = useAuthStore();
const router = useRouter();

const isSubmitting = ref(false);

const submitPhoneNumber = async () => {
  try {
    isSubmitting.value = true;
    const { data } = await auth.resetPassword(formState);

    console.log(data?.code);

    router.replace({ name: "otp", query: { phone: formState.phone } });
  } catch (error) {
    console.error(error);

    if (error?.response?.message) {
      toast(error.response.message, { rtl: true, hideProgressBar: true });
      return;
    }

    toast("خطأ غير متوقع!", { rtl: true, hideProgressBar: true });
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<template>
  <VRow no-gutters class="auth-wrapper text-black">
    <VCol
      cols="12"
      sm="10"
      md="8"
      lg="5"
      class="d-flex flex-column align-center justify-center py-4 pa-0 mx-auto"
    >
      <div class="w-100 d-flex justify-center pb-6 pt-3 px-4">
        <img width="280" src="/logo-colored.svg" alt="امتثال" />
      </div>
      <VCard flat class="w-100 app-logo py-4 px-2 px-sm-6 px-xl-12 rounded-xl">
        <VCardTitle style="flex: 0">
          <h1 class="text-h5 text-sm-h4 font-weight-semibold mb-1">
            هل نسيت كلمة المرور؟
          </h1>
          <p class="mb-4 text-lg" style="color: #75797c">
            أدخل رقم هاتفك المسجل لإرسال رابط إعادة تعيين كلمة المرور.
          </p>
        </VCardTitle>
        <v-divider class="my-3" />
        <VCardText style="flex: 0" class="px-4">
          <VForm @submit.prevent="submitPhoneNumber" class="mb-8">
            <VRow>
              <VCol cols="12" class="py-1">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  رقم الجوال*
                </label>
                <VTextField
                  v-model="formState.phone"
                  type="number"
                  bg-color="white"
                  density="comfortable"
                  placeholder="05XXXXXXX"
                  rounded="lg"
                  :rules="[requiredValidator, phoneValidator]"
                  dir="ltr"
                />
              </VCol>

              <VCol cols="12" class="py-1">
                <div class="d-flex align-center justify-space-between mt-6">
                  <div></div>

                  <VBtn
                    rounded="pill"
                    :loading="isSubmitting"
                    type="submit"
                    size="large"
                    height="50"
                    class="px-10"
                  >
                    التالي
                  </VBtn>
                </div>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCard>
    </VCol>
  </VRow>
</template>

<style lang="scss">
@use "@core/scss/template/pages/page-auth.scss";

.text-danger {
  color: red;
}

.auth-wrapper {
  background: url(/bg.webp) no-repeat center;
  background-size: cover;
}
</style>
