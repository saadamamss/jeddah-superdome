<script setup>
import { useAuthStore } from "@/stores/Auth";
import { emailValidator, requiredValidator } from "@validators";
import { toast } from "vue3-toastify";

const formState = reactive({
  email: null,
});

const auth = useAuthStore();
const router = useRouter();

const isSubmitting = ref(false);

const submitEmail = async () => {
  try {
    isSubmitting.value = true;
    const { data } = await auth.resetPassword(formState);

    console.log(data?.code);

    router.replace({ name: "otp", query: { email: formState.email } });
  } catch (error) {
    console.error(error);

    if (error?.response?.message) {
      toast(error.response.message, { rtl: true, hideProgressBar: true });
      return;
    }

    toast("خطأ غير متوقع!", { rtl: true, hideProgressBar: true });
    router.replace({ name: "otp", query: { email: formState.email } });
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<template>
  <VRow no-gutters class="auth-wrapper text-black">
    <div class="overlay"></div>
    <VCol
      cols="12"
      sm="10"
      md="8"
      lg="5"
      class="d-flex content flex-column align-center justify-center py-4 pa-0 mx-auto"
    >
      <div
        class="w-100 d-flex flex-column align-center justify-center pb-6 pt-3 px-4"
      >
        <img width="140" src="/jeddah-superdome.svg" alt="جدة سوبردوم" />
        <div>
          <h1 align="center" class="text-5xl pt-12 py-4">
            هل نسيت كلمة المرور؟
          </h1>
          <p class="text-secondary text-center text-lg">
            أدخل بريدك الإلكتروني المسجل لإرسال رمز التحقق.
          </p>
        </div>
      </div>

      <VCardText
        style="max-width: 500px"
        class="w-100 app-logo py-4 px-5 px-sm-6 px-xl-12"
      >
        <VForm @submit.prevent="submitEmail" class="mb-8">
          <VRow>
            <VCol cols="12" class="py-1">
              <label class="d-block text-subtitle-1 mb-2 text-dark">
                البريد الإلكتروني*
              </label>
              <VTextField
                v-model="formState.email"
                type="email"
                bg-color="white"
                density="comfortable"
                placeholder="example@domain.com"
                rounded="lg"
                :rules="[requiredValidator, emailValidator]"
                dir="ltr"
              />
            </VCol>

            <VCol cols="12" class="py-1 mt-8">
              <VBtn
                rounded="pill"
                :loading="isSubmitting"
                type="submit"
                size="large"
                height="50"
                class="px-10 w-100"
              >
                إرسال
              </VBtn>
            </VCol>
          </VRow>
        </VForm>
      </VCardText>
    </VCol>
    <VCol cols="12" class="content py-8 mt-10">
      <div class="text-center mb-10">
        <img src="/zimam-qwa.svg" alt="zimam" />
      </div>
      <p class="text-secondary text-center">
        © جميع حقوق الطبع و النشر محفوظة 2025 - Jeddah Superdome
      </p>
    </VCol>
  </VRow>
</template>

<style lang="scss">
@use "@core/scss/template/pages/page-auth.scss";

.text-danger {
  color: red;
}

.auth-wrapper {
  background-size: cover;
}
</style>
