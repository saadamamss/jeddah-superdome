<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { logger } from '@/@core/utils/logger'
import { useAppStore } from '@/stores/app.store'
import { requiredValidator } from '@validators'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const appStore = useAppStore()

const email = route.query.email
const code = route.query.code

if (!email || !code) {
  // router.replace({ name: "forgot-password" });
}

const formState = reactive({
  email,
  code,
  password: '',
  passwordConfirmation: '',
})

const isSubmitting = ref(false)
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const isCreatingNewPassword = ref(true)

const passwordValidator = (value: string) => {
  const regExp = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&*()]).{8,}/
  return regExp.test(value) || t('validators.passwordStrength')
}

const confirmPasswordValidator = (value: string, target: string) => value === target || t('validators.passwordMismatch')

const submitNewPassword = async () => {
  try {
    isSubmitting.value = true
    await auth.updatePassword({
      phone: String(formState.email || ''),
      code: String(formState.code || ''),
      password: formState.password,
      passwordConfirmation: formState.passwordConfirmation,
    })

    appStore.showSnackbar({ message: t('auth.resetPassword.success'), color: 'success' })
    router.replace({ name: 'login' })
  } catch (error: unknown) {
    logger.error(error)

    const err = error as Record<string, unknown>
    const response = err?.response as Record<string, unknown> | undefined
    if (response?.message) {
      appStore.showSnackbar({ message: String(response.message), color: 'error' })
      return
    }

    appStore.showSnackbar({ message: t('auth.resetPassword.error'), color: 'error' })
  } finally {
    isSubmitting.value = false
  }
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
      <div class="w-100 d-flex justify-center pb-2pt-3 px-4">
        <div class="w-100 d-flex flex-column align-center justify-center pb-4 pt-3 px-4">
          <img alt="جدة سوبردوم" src="/jeddah-superdome.svg" width="140" />
          <h1 class="text-h5 text-sm-h4 font-weight-semibold mb-1 pt-10">
            <span v-if="isCreatingNewPassword">{{ t('auth.resetPassword.title') }}</span>
            <span v-else>{{ t('auth.resetPassword.subtitle') }}</span>
          </h1>
          <p v-if="!isCreatingNewPassword" class="mb-4 text-lg" style="color: #75797c">
            {{ t('auth.resetPassword.description') }}
          </p>
        </div>
      </div>
      <VCardText class="w-100 app-logo py-4 px-2 px-sm-6 px-xl-12 rounded-xl" flat>
        <VCardText class="px-4" style="flex: 0">
          <VForm class="mb-8" @submit.prevent="submitNewPassword">
            <VRow>
              <VCol class="py-1 mb-4" cols="12">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  {{ t('auth.resetPassword.newPasswordLabel') }}
                </label>
                <VTextField
                  v-model="formState.password"
                  bg-color="white"
                  density="comfortable"
                  dir="ltr"
                  :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                  placeholder="********"
                  rounded="lg"
                  :rules="[requiredValidator, passwordValidator]"
                  :type="showPassword ? 'text' : 'password'"
                  @click:append-inner="showPassword = !showPassword"
                />
              </VCol>

              <VCol class="py-1 mb-10" cols="12">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  {{ t('auth.resetPassword.confirmPasswordLabel') }}
                </label>
                <VTextField
                  v-model="formState.passwordConfirmation"
                  bg-color="white"
                  density="comfortable"
                  dir="ltr"
                  :append-inner-icon="showConfirmPassword ? 'mdi-eye-off' : 'mdi-eye'"
                  placeholder="********"
                  rounded="lg"
                  :rules="[requiredValidator, (val: string) => confirmPasswordValidator(val, formState.password)]"
                  :type="showConfirmPassword ? 'text' : 'password'"
                  @click:append-inner="showConfirmPassword = !showConfirmPassword"
                />
              </VCol>

              <VCol class="py-1" cols="12">
                <VBtn class="px-10 w-100" height="50" :loading="isSubmitting" rounded="pill" size="large" type="submit">
                  {{ t('auth.resetPassword.saveButton') }}
                </VBtn>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCardText>
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
.auth-wrapper {
  background-size: cover;
}
</style>
