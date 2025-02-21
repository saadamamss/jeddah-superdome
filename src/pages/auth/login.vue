<script setup>
import { useAuthStore } from "@/stores/Auth";
import { requiredValidator } from "@validators";
import { toast } from "vue3-toastify";

const isPasswordVisible = ref(false);

const formState = reactive({
  sa_id: null, // "05423533223"
  password: null, // "1234po12"
});

const auth = useAuthStore();
const router = useRouter();

const isSubmitting = ref(false);

const loginUser = async () => {
  try {
    isSubmitting.value = true;
    const { data } = await auth.login(formState);

    auth.updateUser(data?.data || null);
    auth.saveToken(data?.token || null);

    // router.push({ name: "home" });
    location.href = "/";
  } catch (error) {
    if (error?.response?.message) {
      toast(error.response.message, { rtl: true, hideProgressBar: true });
      return;
    }

    toast("بيانات تسجيل الدخول خاطئة", { rtl: true, hideProgressBar: true });
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
            تسجيل الدخول!
          </h1>
          <p class="mb-4 text-lg" style="color: #75797c">
            أدخل بياناتك لتسجيل الدخول إلى حسابك.
          </p>
        </VCardTitle>
        <v-divider class="my-4" />
        <VCardText style="flex: 0" class="px-4">
          <VForm @submit.prevent="loginUser" class="mb-8">
            <VRow>
              <VCol cols="12" class="py-1">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  رقم الهوية الوطنية أو الإقامة*
                </label>
                <VTextField
                  v-model="formState.sa_id"
                  type="number"
                  bg-color="white"
                  density="comfortable"
                  placeholder="ادخل رقم الهوية الوطنية أو الإقامة"
                  rounded="lg"
                  :rules="[requiredValidator]"
                />
              </VCol>

              <VCol cols="12" class="py-1">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  كلمة المرور
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

                <div class="d-flex align-center justify-space-between mt-6">
                  <v-btn
                    :to="{ name: 'forgot-password' }"
                    variant="text"
                    class="text-primary text-base px-0"
                  >
                    نسيت كلمة المرور؟
                  </v-btn>

                  <VBtn
                    rounded="pill"
                    :loading="isSubmitting"
                    type="submit"
                    size="large"
                    height="50"
                  >
                    تسجيل الدخول
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
</style>
