// Utilities
import { defineStore } from 'pinia'
import { ref } from 'vue'

interface BreadcrumbItem {
  label: string
  to?: string
}

interface SnackbarState {
  show: boolean
  message: string
  color: string
  timeout: number
}

export const useAppStore = defineStore('app', () => {
  const appBreadcrumb = ref<BreadcrumbItem[]>([])
  const appPageTitle = ref('')

  // Add snackbar state
  const snackbar = ref<SnackbarState>({
    show: false,
    message: '',
    color: 'success',
    timeout: 3000,
  })

  const setAppBreadcrumb = (breadcrumb: BreadcrumbItem[] = [], isBaseRoute = false): void => {
    const baseHomeRoute: BreadcrumbItem[] = [{ label: 'الرئيسية', to: '/' }]

    if (isBaseRoute) {
      appBreadcrumb.value = [...baseHomeRoute]
      return
    }

    appBreadcrumb.value = [...baseHomeRoute, ...breadcrumb]
  }

  const updateAppPageTitle = (pageTitle = ''): void => {
    appPageTitle.value = pageTitle
  }

  // Add showSnackbar method
  const showSnackbar = ({
    message,
    color = 'success',
    timeout = 3000,
  }: {
    message: string
    color?: string
    timeout?: number
  }): void => {
    snackbar.value = {
      show: true,
      message,
      color,
      timeout,
    }
  }

  return {
    appBreadcrumb,
    setAppBreadcrumb,
    appPageTitle,
    updateAppPageTitle,
    snackbar,
    showSnackbar,
  }
})
