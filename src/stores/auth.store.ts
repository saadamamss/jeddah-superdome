import { defineStore } from 'pinia'
import { authService } from '@/services/auth.service'
import { defineAbilityFromUser } from '@/plugins/casl/ability'
import type { CompanyInfo, LoginCredentials, User } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const isLoading = ref(false)
  const initialized = ref(false)

  const isAuthenticated = computed(() => !!user.value)
  const isCompany = computed(() => !!user.value?.company)

  /** Get the company data if the logged-in user is a company user */
  const company = computed<CompanyInfo | null>(() => user.value?.company ?? null)

  /** Validate cookie against backend and load fresh user data */
  const init = async (): Promise<boolean> => {
    isLoading.value = true
    try {
      const response = await authService.getMe()
      const userData = response.data

      if (!userData) {
        user.value = null
        defineAbilityFromUser(null)
        return false
      }

      user.value = userData
      defineAbilityFromUser(userData)
      return true
    } catch {
      user.value = null
      defineAbilityFromUser(null)
      return false
    } finally {
      isLoading.value = false
      initialized.value = true
    }
  }

  const setUser = (newUser: User | null): void => {
    user.value = newUser
    if (newUser) {
      defineAbilityFromUser(newUser)
    }
  }

  /** Update company data within the user object */
  const setCompany = (companyData: CompanyInfo): void => {
    if (user.value) {
      user.value = { ...user.value, company: companyData }
    }
  }

  /** Refresh user data and permissions from backend */
  const refreshPermissions = async (): Promise<void> => {
    try {
      const response = await authService.getMe()
      const userData = response.data
      if (userData) {
        user.value = userData
        defineAbilityFromUser(userData)
      }
    } catch {
      // silent — don't logout on refresh failure
    }
  }

  const logout = async (): Promise<void> => {
    try {
      await authService.logout()
    } catch {
      // Cookie may already be expired — still clear local state
    } finally {
      user.value = null
      defineAbilityFromUser(null)
    }
  }

  /** Login — works for both admin and company users (same endpoint) */
  const login = async (credentials: LoginCredentials): Promise<void> => {
    const response = await authService.login(credentials)
    if (!response) {
      throw new Error('No data received from server')
    }
    setUser(response.user)
  }

  const forgotPassword = async (email: string): Promise<unknown> => {
    return authService.forgotPassword({ email })
  }

  const resetPassword = async (email: string, token: string, newPassword: string): Promise<unknown> => {
    return authService.resetPassword({ email, token, newPassword })
  }

  return {
    user,
    isLoading,
    initialized,
    isAuthenticated,
    isCompany,
    company,
    init,
    setUser,
    setCompany,
    refreshPermissions,
    logout,
    login,
    forgotPassword,
    resetPassword,
  }
})
