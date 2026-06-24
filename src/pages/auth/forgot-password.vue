<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useAuthStore } from '@/stores/auth.store'
import { useAppStore } from '@/stores/app.store'
import { emailValidator, requiredValidator } from '@validators'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const formState = reactive({
  email: null as string | null,
})

const auth = useAuthStore()
const appStore = useAppStore()
const router = useRouter()

const isSubmitting = ref(false)

const submitEmail = async () => {
  try {
    isSubmitting.value = true
    await auth.resetPassword({ phone: formState.email || '' })

    router.replace({ name: 'otp', query: { email: formState.email || '' } })
  } catch (error: unknown) {
    logger.error(error)

    const err = error as { response?: { message?: string } }
    if (err?.response?.message) {
      appStore.showSnackbar({ message: err.response.message, color: 'error' })
      return
    }

    appStore.showSnackbar({ message: t('auth.forgotPassword.error'), color: 'error' })
    router.replace({ name: 'otp', query: { email: formState.email || '' } })
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <VRow class="auth-wrapper text-black" no-gutters>
    <div class="overlay" />
    <VCol
      class="d-flex content flex-column align-center justify-center py-4 pa-0 mx-auto"
      cols="12"
      lg="5"
      md="8"
      sm="10"
    >
      <div class="w-100 d-flex flex-column align-center justify-center pb-6 pt-3 px-4">
        <img alt="جدة سوبردوم" src="/jeddah-superdome.svg" width="140" />
        <div>
          <h1 align="center" class="text-5xl pt-12 py-4">
            {{ t('auth.forgotPassword.title') }}
          </h1>
          <p class="text-secondary text-center text-lg">
            {{ t('auth.forgotPassword.subtitle') }}
          </p>
        </div>
      </div>

      <VCardText class="w-100 app-logo py-4 px-5 px-sm-6 px-xl-12" style="max-width: 500px">
        <VForm class="mb-8" @submit.prevent="submitEmail">
          <VRow>
            <VCol class="py-1" cols="12">
              <label class="d-block text-subtitle-1 mb-2 text-dark">
                {{ t('auth.forgotPassword.emailLabel') }}
              </label>
              <VTextField
                v-model="formState.email"
                bg-color="white"
                density="comfortable"
                dir="ltr"
                :placeholder="t('auth.forgotPassword.emailPlaceholder')"
                rounded="lg"
                :rules="[requiredValidator, emailValidator]"
                type="email"
              />
            </VCol>

            <VCol class="py-1 mt-8" cols="12">
              <VBtn class="px-10 w-100" height="50" :loading="isSubmitting" rounded="pill" size="large" type="submit">
                {{ t('auth.forgotPassword.sendButton') }}
              </VBtn>
            </VCol>
          </VRow>
        </VForm>
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
