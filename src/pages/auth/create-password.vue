<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useAuthStore } from '@/stores/auth.store'
import { useAppStore } from '@/stores/app.store'
import { requiredValidator } from '@validators'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const isPasswordVisible = ref(false)

const formState = reactive({
  password: null as string | null,
  passwordConfirmation: null as string | null,
})

const auth = useAuthStore()
const appStore = useAppStore()
const router = useRouter()
const route = useRoute()

const isSubmitting = ref(false)
const isCreatingNewPassword = computed(() => route.query.new === '1')
const token = computed(() => (route.query.token as string) || '')
const email = computed(() => (route.query.email as string) || '')

const updatePassword = async () => {
  try {
    isSubmitting.value = true
    await auth.resetPassword(email.value, token.value, formState.password || '')

    appStore.showSnackbar({ message: t('auth.createPassword.success'), color: 'success' })

    router.replace({
      name: 'login',
    })
  } catch (error: unknown) {
    logger.error(error)

    const err = error as { response?: { data?: { message?: string } } }
    if (err?.response?.data?.message) {
      appStore.showSnackbar({ message: err.response.data.message, color: 'error' })
      return
    }

    appStore.showSnackbar({ message: t('auth.createPassword.error'), color: 'error' })
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
      <div class="w-100 d-flex flex-column align-center justify-center pb-6 pt-3 px-4">
        <img alt="جدة سوبردوم" src="/jeddah-superdome.svg" width="140" />
        <h1 class="text-h5 text-sm-h4 font-weight-semibold mb-1 pt-10">
          <span v-if="isCreatingNewPassword">{{ t('auth.createPassword.title') }}</span>
          <span v-else>{{ t('auth.createPassword.subtitle') }}</span>
        </h1>
        <p v-if="!isCreatingNewPassword" class="mb-4 text-lg" style="color: #75797c">
          {{ t('auth.createPassword.description') }}
        </p>
      </div>
      <VCardText class="w-100 app-logo py-4 px-2 px-sm-6 px-xl-12 rounded-xl" text>
        <VCardText class="px-4" style="flex: 0">
          <VForm class="mb-8" @submit.prevent="updatePassword">
            <VRow>
              <VCol class="py-1 mb-4" cols="12">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  {{ t('auth.createPassword.newPasswordLabel') }}
                </label>
                <VTextField
                  v-model="formState.password"
                  bg-color="white"
                  density="comfortable"
                  :placeholder="t('auth.createPassword.newPasswordPlaceholder')"
                  :append-inner-icon="isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'"
                  rounded="lg"
                  :rules="[requiredValidator]"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  variant="outlined"
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
              </VCol>
              <VCol class="mb-4" cols="12">
                <label class="d-block text-subtitle-1 mb-2 text-dark">
                  {{ t('auth.createPassword.confirmPasswordLabel') }}
                </label>
                <VTextField
                  v-model="formState.passwordConfirmation"
                  bg-color="white"
                  density="comfortable"
                  :placeholder="t('auth.createPassword.confirmPasswordPlaceholder')"
                  :append-inner-icon="isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'"
                  rounded="lg"
                  :rules="[requiredValidator]"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  variant="outlined"
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
              </VCol>
              <VCol cols="12">
                <VBtn
                  class="w-100"
                  :class="{ 'px-10': isCreatingNewPassword }"
                  height="50"
                  :loading="isSubmitting"
                  rounded="pill"
                  size="large"
                  type="submit"
                >
                  <span v-if="isCreatingNewPassword">{{ t('auth.createPassword.confirmButton') }}</span>
                  <span v-else>{{ t('auth.createPassword.changeButton') }}</span>
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

<style lang="scss">
@use '@core/scss/template/pages/page-auth.scss';

.text-danger {
  color: red;
}

.auth-wrapper {
  background-size: cover;
}
</style>
