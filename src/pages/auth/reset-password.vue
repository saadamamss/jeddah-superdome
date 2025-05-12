<script setup>
import { useAuthStore } from "@/stores/Auth";
import {
  requiredValidator,
  passwordValidator,
  confirmPasswordValidator,
} from "@validators";
import { toast } from "vue3-toastify";

const route = useRoute();
const router = useRouter();
const auth = useAuthStore();

const email = route.query.email;
const code = route.query.code;

if (!email || !code) {
  // router.replace({ name: "forgot-password" });
}

const formState = reactive({
  email,
  code,
  password: "",
  password_confirmation: "",
});

const isSubmitting = ref(false);
const showPassword = ref(false);
const showConfirmPassword = ref(false);

const submitNewPassword = async () => {
  try {
    isSubmitting.value = true;
    await auth.setNewPassword(formState);

    toast("تم تغيير كلمة المرور بنجاح", { rtl: true, hideProgressBar: true });
    router.replace({ name: "login" });
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
    <div class="overlay"></div>
    <VCol
      cols="12"
      sm="10"
      md="8"
      lg="5"
      class="content d-flex flex-column align-center justify-center py-4 pa-0 mx-auto"
    >
      <div class="w-100 d-flex justify-center pb-2pt-3 px-4">
        <div
          class="w-100 d-flex flex-column align-center justify-center pb-4 pt-3 px-4"
        >
          <img width="140" src="/jeddah-superdome.svg" alt="جدة سوبردوم" />
          <h1 class="text-h5 text-sm-h4 font-weight-semibold mb-1 pt-10">
            <span v-if="isCreatingNewPassword">كلمة المرور الجديدة</span>
            <span v-else>تغيير كلمة المرور!</span>
          </h1>
          <p
            v-if="!isCreatingNewPassword"
            class="mb-4 text-lg"
            style="color: #75797c"
          >
            يُرجى تغيير كلمة المرور عند تسجيل الدخول لأول مرة.
          </p>
        </div>
      </div>
      <VCardText
        flat
        class="w-100 app-logo py-4 px-2 px-sm-6 px-xl-12 rounded-xl"
      >
        <VCardText style="flex: 0" class="px-4">
          <VForm @submit.prevent="submitNewPassword" class="mb-8">
            <VRow>
              <VCol cols="12" class="py-1 mb-4">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  كلمة المرور الجديدة*
                </label>
                <VTextField
                  v-model="formState.password"
                  :type="showPassword ? 'text' : 'password'"
                  bg-color="white"
                  density="comfortable"
                  placeholder="********"
                  rounded="lg"
                  :rules="[requiredValidator, passwordValidator]"
                  dir="ltr"
                  :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                  @click:append-inner="showPassword = !showPassword"
                />
              </VCol>

              <VCol cols="12" class="py-1 mb-10">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  تأكيد كلمة المرور*
                </label>
                <VTextField
                  v-model="formState.password_confirmation"
                  :type="showConfirmPassword ? 'text' : 'password'"
                  bg-color="white"
                  density="comfortable"
                  placeholder="********"
                  rounded="lg"
                  :rules="[
                    requiredValidator,
                    (val) => confirmPasswordValidator(val, formState.password),
                  ]"
                  dir="ltr"
                  :append-inner-icon="
                    showConfirmPassword ? 'mdi-eye-off' : 'mdi-eye'
                  "
                  @click:append-inner="
                    showConfirmPassword = !showConfirmPassword
                  "
                />
              </VCol>

              <VCol cols="12" class="py-1">
                <VBtn
                  rounded="pill"
                  :loading="isSubmitting"
                  type="submit"
                  size="large"
                  height="50"
                  class="px-10 w-100"
                >
                  حفظ كلمة المرور
                </VBtn>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
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

<style lang="scss" scoped>
.auth-wrapper {
  background-size: cover;
}
</style>
