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
    <div class="overlay"></div>
    <VCol
      cols="12"
      sm="10"
      md="8"
      lg="5"
      class="content d-flex flex-column align-center justify-center py-4 pa-0 mx-auto"
    >
      <div style="width: 580px; max-width: 100%">
        <div
          class="w-100 d-flex flex-column align-center justify-center pb-6 pt-3 px-4"
        >
          <img width="140" src="/jeddah-superdome.svg" alt="جدة سوبردوم" />
          <div>
            <h1 align="center" class="text-5xl pt-8 py-4">تسجيل الدخول</h1>
            <p class="text-secondary text-lg">
              مرحبًا بك في نظام تفويج الحجاج - سوبر دوم جدة
            </p>
          </div>
        </div>
        <VCardText style="flex: 0" class="px-4">
          <VForm ref="form" @submit.prevent="loginUser" class="mb-8">
            <VRow>
              <VCol cols="12" class="py-1 mb-5">
                <label class="d-block text-subtitle-1 mb-2 text-black">
                  رقم الهوية / البريد الإلكتروني
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

              <VCol cols="12" class="py-1 mb-5">
                <label class="d-block text-subtitle-1 mb-2 text-black">
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
              </VCol>
              <VCol>
                <VBtn
                  class="w-100 mb-4"
                  rounded="lg"
                  :loading="isSubmitting"
                  type="submit"
                  size="large"
                  height="50"
                  color="primary"
                >
                  تسجيل الدخول
                </VBtn>
                <VBtn
                  class="w-100"
                  rounded="lg"
                  :loading="isSubmitting"
                  type="submit"
                  size="large"
                  height="50"
                  color="white"
                >
                  تسجيل دخول كشركة
                </VBtn>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </div>
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

<style lang="scss"></style>
