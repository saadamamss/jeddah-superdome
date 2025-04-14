<script setup>
import { useAuthStore } from "@/stores/Auth";
import { requiredValidator } from "@validators";
import { toast } from "vue3-toastify";
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";

const isPasswordVisible = ref(false);
const form = ref(null);

const formState = reactive({
  email: "",
  password: "",
});

const auth = useAuthStore();
const router = useRouter();

const isSubmitting = ref(false);

const loginUser = async () => {
  const { valid } = await form.value.validate();

  if (!valid) return;

  try {
    isSubmitting.value = true;
    const { token, user } = await auth.login(formState);

    if (token) {
      location.href = "/";
      // location.href = "/rating";
    } else {
      throw new Error("بيانات تسجيل الدخول غير صحيحة");
    }
  } catch (error) {
    console.error("Login error:", error);

    if (error.response?.status == 422) {
      toast.error("بيانات الدخول غير صحيحة", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
      return;
    }

    if (error?.response?.data?.message) {
      toast.error(error.response.data.message, {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
    } else if (error?.message) {
      toast.error(error.message, {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
    } else {
      toast.error("حدث خطأ أثناء تسجيل الدخول", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
    }
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
          <VForm ref="form" @submit.prevent="loginUser" class="mb-8">
            <VRow>
              <VCol cols="12" class="py-1">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  البريد الإلكتروني
                </label>
                <VTextField
                  v-model="formState.email"
                  type="email"
                  bg-color="white"
                  density="comfortable"
                  placeholder="ادخل البريد الإلكتروني"
                  rounded="lg"
                  :rules="[(v) => !!v || 'البريد الإلكتروني مطلوب']"
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
                  :rules="[(v) => !!v || 'كلمة المرور مطلوبة']"
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
                    color="primary"
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
