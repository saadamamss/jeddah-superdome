<script setup>
import { useAuthStore } from "@/stores/Auth";
import { requiredValidator } from "@validators";
import { toast } from "vue3-toastify";

const formState = reactive({
  code: "",
});

const auth = useAuthStore();
const router = useRouter();
const route = useRoute();

const isSubmitting = ref(false);
const phoneNumber = computed(() => route.query?.phone || null);

const submitCode = async () => {
  try {
    isSubmitting.value = true;
    await auth.verifyOTP({
      phone: phoneNumber.value,
      code: formState.code,
    });

    router.replace({
      name: "create-password",
      query: { new: 1, code: formState.code, phone: phoneNumber.value },
    });
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

const resendCode = () => {};
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
            تحقق من رقم الجوال
          </h1>
          <p class="mb-4 text-lg" style="color: #75797c">
            <span>تم إرسال رمز التحقق إلى رقم هاتفك </span>
            <!-- <span dir="ltr">+966 12** **** ***</span> -->
            <span>، يرجى إدخال الرمز لتأكيد العملية</span>
          </p>
        </VCardTitle>
        <v-divider class="my-3" />
        <VCardText style="flex: 0" class="px-4">
          <VForm @submit.prevent="submitCode" class="mb-8">
            <VRow>
              <VCol cols="12" class="py-1">
                <div dir="ltr" class="mb-4">
                  <v-otp-input
                    v-model="formState.code"
                    :disabled="isSubmitting"
                    autofocus
                    length="4"
                    width="500"
                    min-height="80"
                    :rules="[requiredValidator]"
                    @finish="submitCode"
                  ></v-otp-input>
                </div>

                <div class="d-flex flex-column align-center">
                  <p class="mb-0">لم يصلك رمز التحقق؟</p>
                  <v-btn
                    @click="resendCode"
                    variant="link"
                    :disabled="isSubmitting"
                  >
                    إعادة الإرسال خلال <span>59</span> ثانية ...
                  </v-btn>
                </div>
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
                    تأكيد رقم الجوال
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
