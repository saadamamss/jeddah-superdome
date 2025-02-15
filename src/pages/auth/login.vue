<script setup>
import { useAuthStore } from "@/store/Auth";
import { emailValidator, requiredValidator } from "@validators";

const isPasswordVisible = ref(false);

const user = reactive({
  email: ref(""),
  password: ref(""),
});

const rememberMe = ref(false);

const auth = useAuthStore();
const router = useRouter();

const isLoading = ref(false);

const loginUser = () => {
  // isLoading.value = true
  // auth.login(user).then(res => {
  //   auth.updateUser(res.data.data);
  //   axiosIns.defaults.headers.Authorization = `Bearer ${res.data.data.api_token}`
  //   router.push({ name: 'index' })
  //   settingsListStore.alertColor = "success"
  //   settingsListStore.alertMessage = "تم تسجيل الدخول بنجاح"
  //   settingsListStore.isAlertShow = true
  //   setTimeout(() => {
  //     settingsListStore.isAlertShow = false
  //     settingsListStore.alertMessage = ""
  //   }, 3000)
  // }).catch(error => {
  //   isLoading.value = false
  //   settingsListStore.alertColor = "error"
  //   settingsListStore.alertMessage = "حدث خطأ ما !"
  //   if(error.response.status == 400) {
  //     isError.value = true
  //     alertMessage.value = "بيانات تسجيل الدخول خاطئة"
  //   }
  //   settingsListStore.isAlertShow = true
  //   setTimeout(() => {
  //     settingsListStore.isAlertShow = false
  //     settingsListStore.alertMessage = ""
  //   }, 3000)
  // })
};
</script>

<template>
  <VRow no-gutters class="auth-wrapper auth-bg-color text-dark">
    <VCol
      cols="12"
      sm="10"
      md="5"
      xl="4"
      class="d-flex align-center justify-center pa-0 mx-auto"
    >
      <VCard
        flat
        color="gray-light"
        class="app-logo py-0 px-0 px-sm-4 px-xl-12 w-100 d-flex flex-column rounded-0"
        style="height: 100vh; justify-content: center"
      >
        <VCardText style="flex: 0" class="pa-0 mb-4 mb-lg-6 mb-xl-10">
          <div class="d-flex justify-center pt-2">
            <img class="auth-logo-img" src="/logo.svg" alt="logo" width="350" />
          </div>
        </VCardText>
        <VCardText style="flex: 0">
          <h1 class="text-h5 font-weight-semibold mb-1">تسجيل الدخول</h1>
          <p class="mb-2">مرحبًا بك مرة أخرى! الرجاء إدخال تفاصيلك.</p>
        </VCardText>
        <VCardText style="flex: 0">
          <VForm @submit.prevent="loginUser" class="mb-8">
            <VRow>
              <VCol cols="12" class="py-1">
                <label class="d-block text-subtitle-1 mb-2 text-dark"
                  >البريد الإلكتروني</label
                >
                <VTextField
                  v-model="user.email"
                  type="email"
                  bg-color="white"
                  density="comfortable"
                  placeholder="ادخل البريد الإلكتروني"
                  rounded="lg"
                  :rules="[requiredValidator, emailValidator]"
                />
              </VCol>

              <VCol cols="12" class="py-1">
                <label class="d-block text-subtitle-1 mb-2 text-dark"
                  >الرقم السري</label
                >
                <VTextField
                  v-model="user.password"
                  variant="outlined"
                  bg-color="white"
                  density="comfortable"
                  placeholder="ادخل الرقم السري"
                  rounded="lg"
                  :rules="[requiredValidator]"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="
                    isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'
                  "
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />

                <div
                  class="d-flex align-center flex-wrap justify-space-between mt-2 mb-2 mb-xl-4"
                >
                  <VCheckbox
                    v-model="rememberMe"
                    label="تذكرني خلال ٣٠ يوم"
                    false-icon="system-uicons:checkbox-empty"
                    true-icon="mdi:check-box"
                  />
                  <a class="text-primary ms-2 mb-1" href="#">
                    نسيت الرقم السري
                  </a>
                </div>

                <VBtn
                  rounded="lg"
                  :loading="isLoading"
                  block
                  type="submit"
                  size="large"
                  height="50"
                >
                  تسجيل الدخول
                </VBtn>
              </VCol>
            </VRow>
          </VForm>
          <div class="py-3 py-xl-6 text-center">
            <a
              href="#"
              class="d-flex align-center justify-center gap-2 mb-4 py-2 px-4 rounded-lg bg-white elevation-4"
              style="border: 1px solid #d0d5dd"
            >
              <span class="text-dark font-weight-semibold"
                >تسجيل الدخول بواسطة</span
              >
              <span>
                <svg
                  width="1.5rem"
                  height="1.5rem"
                  viewBox="0 0 25 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <g clip-path="url(#clip0_20_48220)">
                    <path
                      d="M24.2663 12.2763C24.2663 11.4605 24.2001 10.6404 24.059 9.83789H12.7402V14.4589H19.222C18.953 15.9492 18.0888 17.2676 16.8233 18.1054V21.1037H20.6903C22.9611 19.0137 24.2663 15.9272 24.2663 12.2763Z"
                      fill="#4285F4"
                    />
                    <path
                      d="M12.7391 24.0013C15.9756 24.0013 18.705 22.9387 20.6936 21.1044L16.8266 18.106C15.7507 18.838 14.3618 19.2525 12.7435 19.2525C9.61291 19.2525 6.95849 17.1404 6.00607 14.3008H2.01562V17.3917C4.05274 21.4439 8.20192 24.0013 12.7391 24.0013Z"
                      fill="#34A853"
                    />
                    <path
                      d="M6.00277 14.3007C5.50011 12.8103 5.50011 11.1965 6.00277 9.70618V6.61523H2.01674C0.314734 10.006 0.314734 14.0009 2.01674 17.3916L6.00277 14.3007Z"
                      fill="#FBBC04"
                    />
                    <path
                      d="M12.7391 4.74966C14.4499 4.7232 16.1034 5.36697 17.3425 6.54867L20.7685 3.12262C18.5991 1.0855 15.7198 -0.034466 12.7391 0.000808666C8.20192 0.000808666 4.05274 2.55822 2.01562 6.61481L6.00166 9.70575C6.94967 6.86173 9.6085 4.74966 12.7391 4.74966Z"
                      fill="#EA4335"
                    />
                  </g>
                  <defs>
                    <clipPath id="clip0_20_48220">
                      <rect
                        width="24"
                        height="24"
                        fill="white"
                        transform="translate(0.5)"
                      />
                    </clipPath>
                  </defs>
                </svg>
              </span>
            </a>
            <p class="mb-0">
              ليس لديك حساب؟
              <a href="#" class="text-primary font-weight-semibold">سجل الآن</a>
            </p>
          </div>
        </VCardText>
      </VCard>
    </VCol>
    <VCol md="7" xl="8" class="d-none d-md-flex px-4">
      <div class="position-relative auth-bg w-100 px-4">
        <div
          class="app-logo d-flex align-center justify-center flex-column w-100 h-100"
        >
          <!-- <img
            class="auth-bg__vector"
            src="/imgs/login-vector.png"
            alt="logo"
            width="800"
          /> -->
        </div>
      </div>
    </VCol>
  </VRow>
</template>

<style lang="scss">
@use "@core/scss/template/pages/page-auth.scss";

.text-danger {
  color: red;
}
</style>
