import axios, { type InternalAxiosRequestConfig } from 'axios'
import { API_BASE_URL } from '@/config'
import { useAppStore } from '@/stores/app.store'

const axiosIns = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    Accept: 'application/json',
  },
})

let isRefreshing = false
let failedQueue: Array<{
  resolve: (value: unknown) => void
  reject: (reason?: unknown) => void
}> = []

const processQueue = (error: unknown): void => {
  failedQueue.forEach((promise) => {
    if (error) {
      promise.reject(error)
    } else {
      promise.resolve(undefined)
    }
  })
  failedQueue = []
}

// Endpoints that should NOT trigger token refresh on 401
const SKIP_REFRESH_PATHS = ['/login', '/refresh', '/logout']

// Response interceptor — handle 401 (token refresh) and 422 (validation)
axiosIns.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean
    }

    // Handle 401 Unauthorized
    if (error.response?.status === 401 && !originalRequest._retry) {
      const url = originalRequest.url || ''
      const shouldSkipRefresh = SKIP_REFRESH_PATHS.some((p) => url.includes(p))

      if (shouldSkipRefresh) {
        return Promise.reject(error)
      }

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject })
        })
          .then(() => axiosIns(originalRequest))
          .catch((err) => Promise.reject(err))
      }

      originalRequest._retry = true
      isRefreshing = true

      try {
        const { authService } = await import('@/services/auth.service')
        await authService.refresh()
        processQueue(null)
        return axiosIns(originalRequest)
      } catch (refreshError) {
        processQueue(refreshError)
        window.location.href = '/auth/login'
        return Promise.reject(refreshError)
      }
    }

    // Handle 403 Forbidden
    if (error.response?.status === 403) {
      const appStore = useAppStore()
      appStore.showSnackbar({ message: 'ليس لديك صلاحية للوصول إلى هذا المورد', color: 'error' })
      return Promise.reject(error)
    }

    // Handle 422 Validation Errors
    if (error.response?.status === 422) {
      const errorsObj = error.response?.data?.errors as Record<string, string[]> | undefined
      const appStore = useAppStore()

      if (errorsObj) {
        const messages = Object.values(errorsObj).flat()
        appStore.showSnackbar({ message: messages.join(' | '), color: 'error' })
      }
    }

    return Promise.reject(error)
  },
)

export default axiosIns
