<script setup>
import { useAuthStore } from "@/stores/Auth";
import { requiredValidator } from "@validators";
import { toast } from "vue3-toastify";

const isPasswordVisible = ref(false);

const formState = reactive({
  password: null,
  password_confirmation: null,
});

const auth = useAuthStore();
const router = useRouter();
const route = useRoute();

const isSubmitting = ref(false);
const isCreatingNewPassword = computed(() => route.query.new == 1);
const code = computed(() => route.query.code || null);
const phoneNumber = computed(() => route.query.phone || null);

const updatePassword = async () => {
  try {
    isSubmitting.value = true;
    await auth.updatePassword({
      phone: phoneNumber.value,
      code: code.value,
      ...formState,
    });

    toast("تم تحديث كلمة المرور بنجاح", { rtl: true, hideProgressBar: true });

    router.replace({
      name: "login",
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
      <div
        class="w-100 d-flex flex-column align-center justify-center pb-6 pt-3 px-4"
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
      <VCardText
        text
        class="w-100 app-logo py-4 px-2 px-sm-6 px-xl-12 rounded-xl"
      >
        <VCardText style="flex: 0" class="px-4">
          <VForm @submit.prevent="updatePassword" class="mb-8">
            <VRow>
              <VCol cols="12" class="py-1 mb-4">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  كلمة المرور الجديدة
                </label>
                <VTextField
                  v-model="formState.password"
                  variant="outlined"
                  bg-color="white"
                  density="comfortable"
                  placeholder="أدخل كلمة المرور"
                  rounded="lg"
                  :rules="[requiredValidator]"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="
                    isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
                  "
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
              </VCol>
              <VCol cols="12" class="mb-4">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  تأكيد كلمة المرور الجديدة
                </label>
                <VTextField
                  v-model="formState.password_confirmation"
                  variant="outlined"
                  bg-color="white"
                  density="comfortable"
                  placeholder="تأكيد كلمة المرور"
                  rounded="lg"
                  :rules="[requiredValidator]"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="
                    isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
                  "
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
              </VCol>
              <VCol cols="12">
                <VBtn
                  rounded="pill"
                  :loading="isSubmitting"
                  type="submit"
                  size="large"
                  height="50"
                  class="w-100"
                  :class="{ 'px-10': isCreatingNewPassword }"
                >
                  <span v-if="isCreatingNewPassword">تأكيد</span>
                  <span v-else>تغيير كلمة المرور</span>
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

<style lang="scss">
@use "@core/scss/template/pages/page-auth.scss";

.text-danger {
  color: red;
}

.auth-wrapper {
  background-size: cover;
}
</style>
