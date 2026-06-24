/**
 * useTablePage - Shared composable for all table-based pages
 * Handles pagination, search, loading state, and data fetching
 */
import { onMounted, onUnmounted, ref, watch } from 'vue'
import { useDebounceFn } from '@vueuse/core'
import { useAppStore } from '@/stores/app.store'
import { logger } from '@/@core/utils/logger'

interface TablePageOptions {
  searchDelay?: number
  defaultPerPage?: number
  defaultPage?: number
  entityName?: string
  fetchOnMount?: boolean
}

interface Pagination {
  currentPage: number
  perPage: number
  totalObjects: number
  totalPages: number
}

interface ApiResponse<T> {
  data: T[]
  pagination: Pagination
  status: {
    success: boolean
    message?: string
  }
}

type FetchService<T> = (params: Record<string, unknown>) => Promise<ApiResponse<T>>

export function useTablePage<T = unknown>(fetchService: FetchService<T>, options: TablePageOptions = {}) {
  const {
    searchDelay = 500,
    defaultPerPage = 12,
    defaultPage = 1,
    entityName = 'البيانات',
    fetchOnMount = true,
  } = options

  const appStore = useAppStore()

  // Reactive state
  const items = ref<T[]>([])
  const isLoading = ref(false)
  const search = ref('')
  const currentPage = ref(defaultPage)
  const perPage = ref(defaultPerPage)
  const totalItems = ref(0)
  const totalPages = ref(1)

  // AbortController for request cancellation on unmount
  let abortController: AbortController | null = null

  // Fetch data with pagination and search
  const fetchData = async (page: number = currentPage.value): Promise<void> => {
    // Cancel any pending request
    abortController?.abort()
    abortController = new AbortController()

    try {
      isLoading.value = true
      const params: Record<string, unknown> = {
        page,
        perPage: perPage.value,
      }
      if (search.value) {
        params.search = search.value
      }

      const response = await fetchService(params)

      items.value = response.data || []
      if (response.pagination) {
        totalItems.value = response.pagination.totalObjects || 0
        totalPages.value = response.pagination.totalPages || 1
        perPage.value = response.pagination.perPage || defaultPerPage
        currentPage.value = response.pagination.currentPage || defaultPage
      }
    } catch (error) {
      logger.error(`Error fetching ${entityName}:`, error)
      appStore.showSnackbar({
        message: `حدث خطأ أثناء تحميل ${entityName}`,
        color: 'error',
      })
      items.value = []
      totalItems.value = 0
    } finally {
      isLoading.value = false
    }
  }

  // Debounced search
  const debouncedFetch = useDebounceFn(() => {
    currentPage.value = 1
    fetchData(1)
  }, searchDelay)

  // Watch search changes
  watch(search, () => {
    debouncedFetch()
  })

  // Watch page changes
  watch(currentPage, (newPage) => {
    if (newPage !== defaultPage || items.value.length === 0) {
      fetchData(newPage)
    }
  })

  // Watch per-page changes
  watch(perPage, () => {
    currentPage.value = 1
    fetchData(1)
  })

  // Fetch on mount
  if (fetchOnMount) {
    onMounted(() => {
      fetchData()
    })
  }

  // Cancel pending request on unmount
  onUnmounted(() => {
    abortController?.abort()
    abortController = null
  })

  return {
    items,
    isLoading,
    search,
    currentPage,
    perPage,
    totalItems,
    totalPages,
    fetchData,
    debouncedFetch,
  }
}
