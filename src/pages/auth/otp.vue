<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useAuthStore } from '@/stores/auth.store'
import { useAppStore } from '@/stores/app.store'
import { requiredValidator } from '@validators'
import VOtpInput from 'vue3-otp-input'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const route = useRoute()
const auth = useAuthStore()
const appStore = useAppStore()

const email = route.query.email
if (!email) {
  // router.replace({ name: "forgot-password" });
}

const formState = reactive({
  code: '',
})

const isSubmitting = ref(false)
const otpInput = ref<InstanceType<typeof VOtpInput> | null>(null)

const submitOTP = async () => {
  try {
    isSubmitting.value = true
    // await auth.verifyOTP({ email, code: formState.code });

    // router.replace({
    //   name: "reset-password",
    //   query: { email, code: formState.code },
    // });
  } catch (error: unknown) {
    logger.error(error)

    const err = error as { response?: { message?: string } }
    if (err?.response?.message) {
      appStore.showSnackbar({ message: err.response.message, color: 'error' })
      return
    }

    appStore.showSnackbar({ message: t('auth.otp.error'), color: 'error' })
  } finally {
    isSubmitting.value = false
  }
}

const resendOTP = async () => {
  try {
    await auth.resetPassword({ phone: (email as string) || '' })
    appStore.showSnackbar({ message: t('auth.otp.resentSuccess'), color: 'success' })
    // Reset OTP input after resending
    otpInput.value?.clearInput()
  } catch (error) {
    logger.error(error)
    appStore.showSnackbar({ message: t('auth.otp.resentError'), color: 'error' })
  }
}

const handleOnComplete = (value: string) => {
  formState.code = value
  submitOTP()
}

const handleOnChange = (value: string) => {
  formState.code = value
}
</script>

<template>
  <VRow class="auth-wrapper text-black" no-gutters>
    <div class="overlay" />
    <VCol
      class="content d-flex flex-column align-center justify-center py-4 pa-0 mx-auto"
      cols="12"
      lg="5"
      md="8"
      sm="10"
    >
      <div class="w-100 d-flex flex-column align-center justify-center pb-6 pt-3 px-4">
        <img alt="جدة سوبردوم" src="/jeddah-superdome.svg" width="140" />
        <div>
          <h1 class="text-h5 text-sm-h4 font-weight-semibold mb-1 text-center pb-3 pt-10">
            {{ t('auth.otp.title') }}
          </h1>
          <p class="mb-4 text-lg text-center" style="color: #75797c">
            {{ t('auth.otp.subtitle') }}
          </p>
        </div>
      </div>
      <VCard class="w-100 app-logo py-4 px-2 px-sm-6 px-xl-12 rounded-xl" flat>
        <VCardText class="px-4" style="flex: 0">
          <VForm class="mb-8" @submit.prevent="submitOTP">
            <VRow>
              <VCol class="py-1" cols="12">
                <div class="mb-4" dir="ltr">
                  <VOtpInput
                    ref="otpInput"
                    v-model="formState.code"
                    :disabled="isSubmitting"
                    :input-classes="'otp-input'"
                    input-type="number"
                    :length="4"
                    :num-inputs="4"
                    :rules="[requiredValidator]"
                    separator=" "
                    should-auto-focus
                    @on-change="handleOnChange"
                    @on-complete="handleOnComplete"
                  />
                </div>
              </VCol>

              <VCol class="py-1 d-flex justify-center" cols="12">
                <VBtn
                  class="px-10 w-100"
                  height="50"
                  :loading="isSubmitting"
                  rounded="pill"
                  size="large"
                  style="max-width: 280px"
                  type="submit"
                >
                  {{ t('auth.otp.verifyButton') }}
                </VBtn>
              </VCol>
              <VCol class="py-1 d-flex justify-center mt-5" cols="12">
                <VBtn :disabled="isSubmitting" variant="text" @click="resendOTP">
                  {{ t('auth.otp.resendButton') }}
                </VBtn>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCard>
    </VCol>

    <VCol class="content py-8 mt-10" cols="12">
      <div class="text-center mb-10">
        <img alt="zimam" src="/zimam-qwa.svg" />
      </div>
      <p class="text-secondary text-center">
        {{ t('common.copyright') }}
      </p>
    </VCol>
  </VRow>
</template>

<style lang="scss" scoped>
:deep(.vue-otp-input) {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
}

:deep(.otp-input) {
  width: 60px !important;
  height: 60px !important;
  padding: 0.5rem;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
  background-color: white;
  font-size: 1.5rem;
  text-align: center;

  &:focus {
    outline: none;
    border-color: rgb(var(--v-theme-primary));
  }
}

.auth-wrapper {
  background-size: cover;
}
.otp-input-container {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
}
</style>
