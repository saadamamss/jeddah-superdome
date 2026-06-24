<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useAuthStore } from '@/stores/auth.store'
import { useAppStore } from '@/stores/app.store'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const isPasswordVisible = ref(false)
const form = ref<InstanceType<(typeof import('vuetify/components'))['VForm']> | null>(null)

const formState = reactive({
  email: '',
  password: '',
})

const auth = useAuthStore()
const appStore = useAppStore()
const router = useRouter()
const route = useRoute()

const isSubmitting = ref(false)

const handleLogin = async () => {
  if (!form.value) return
  const { valid } = await form.value.validate()
  if (!valid) return

  try {
    isSubmitting.value = true
    await auth.login(formState)
    const redirect = (route.query.redirect as string) || '/dashboard'
    router.push(redirect)
  } catch (error: unknown) {
    handleError(error)
  } finally {
    isSubmitting.value = false
  }
}

const handleError = (error: unknown) => {
  logger.error('Login error:', error)
  const err = error as { response?: { status?: number; data?: { message?: string } }; message?: string }

  if (err.response?.status === 422) {
    appStore.showSnackbar({ message: t('auth.login.invalidCredentials'), color: 'error' })
    return
  }

  const message = err?.response?.data?.message || err?.message || t('auth.login.loginError')
  appStore.showSnackbar({ message, color: 'error' })
}
</script>

<template>
  <VRow class="auth-wrapper text-black" no-gutters>
    <div class="overlay" />
    <VCol
      class="content d-flex flex-column align-center justify-center py-3 pa-0 mx-auto"
      cols="12"
      lg="5"
      md="8"
      sm="10"
    >
      <div style="width: 480px; max-width: 100%">
        <div class="w-100 d-flex flex-column align-center justify-center pb-3 pt-6 px-4">
          <img :alt="t('auth.login.title')" src="/jeddah-superdome.svg" width="140" />
          <div>
            <h1 align="center" class="text-5xl pt-8 py-4">{{ t('auth.login.title') }}</h1>
            <p class="text-secondary text-lg">
              {{ t('auth.login.subtitle') }}
            </p>
          </div>
        </div>
        <VCardText class="px-4" style="flex: 0">
          <VForm ref="form" class="mb-8" @submit.prevent="handleLogin">
            <VRow>
              <VCol class="py-1 mb-3" cols="12">
                <label class="d-block text-subtitle-1 mb-2 text-black">
                  {{ t('auth.login.emailLabel') }}
                </label>
                <VTextField
                  v-model="formState.email"
                  bg-color="white"
                  density="comfortable"
                  :placeholder="t('auth.login.emailPlaceholder')"
                  rounded="lg"
                  :rules="[(v: string) => !!v || t('auth.login.emailRequired')]"
                  type="email"
                />
              </VCol>

              <VCol class="py-1 mb-5" cols="12">
                <label class="d-block text-subtitle-1 mb-2 text-black">
                  {{ t('auth.login.passwordLabel') }}
                </label>
                <VTextField
                  v-model="formState.password"
                  bg-color="white"
                  density="comfortable"
                  :placeholder="t('auth.login.passwordPlaceholder')"
                  :append-inner-icon="isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'"
                  rounded="lg"
                  :rules="[(v: string) => !!v || t('auth.login.passwordRequired')]"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  variant="outlined"
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
              </VCol>
              <VCol>
                <VBtn
                  class="w-100"
                  color="primary"
                  height="50"
                  :loading="isSubmitting"
                  rounded="lg"
                  size="large"
                  type="submit"
                >
                  {{ t('auth.login.loginButton') }}
                </VBtn>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </div>
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

<style lang="scss"></style>
