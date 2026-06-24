import { beforeEach, describe, expect, it, vi } from 'vitest'
import { useTablePage } from '../useTablePage'

vi.mock('@/stores/app.store', () => ({
  useAppStore: () => ({
    showSnackbar: vi.fn(),
  }),
}))

describe('useTablePage', () => {
  const mockService = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('initializes with default state', () => {
    mockService.mockResolvedValue({
      data: [],
      pagination: { currentPage: 1, perPage: 12, totalObjects: 0, totalPages: 0 },
    })
    const { items, isLoading, search, currentPage, perPage, totalItems } = useTablePage(mockService, {
      fetchOnMount: false,
    })

    expect(items.value).toEqual([])
    expect(isLoading.value).toBe(false)
    expect(search.value).toBe('')
    expect(currentPage.value).toBe(1)
    expect(perPage.value).toBe(12)
    expect(totalItems.value).toBe(0)
  })

  it('uses custom default values', () => {
    mockService.mockResolvedValue({
      data: [],
      pagination: { currentPage: 1, perPage: 10, totalObjects: 0, totalPages: 0 },
    })
    const { perPage, currentPage } = useTablePage(mockService, {
      defaultPerPage: 10,
      defaultPage: 2,
      fetchOnMount: false,
    })

    expect(perPage.value).toBe(10)
    expect(currentPage.value).toBe(2)
  })

  it('fetches data and updates items', async () => {
    const mockData = [
      { id: 1, name: 'Item 1' },
      { id: 2, name: 'Item 2' },
    ]
    mockService.mockResolvedValue({
      data: mockData,
      pagination: { currentPage: 1, perPage: 12, totalObjects: 50, totalPages: 5 },
    })

    const { items, fetchData } = useTablePage(mockService, { fetchOnMount: false })
    await fetchData()

    expect(items.value).toEqual(mockData)
    expect(items.value).toHaveLength(2)
  })

  it('updates totalItems from pagination', async () => {
    mockService.mockResolvedValue({
      data: [],
      pagination: { currentPage: 1, perPage: 12, totalObjects: 99, totalPages: 9 },
    })

    const { totalItems, fetchData } = useTablePage(mockService, { fetchOnMount: false })
    await fetchData()

    expect(totalItems.value).toBe(99)
  })

  it('sends search param when search has value', async () => {
    mockService.mockResolvedValue({
      data: [],
      pagination: { currentPage: 1, perPage: 12, totalObjects: 0, totalPages: 0 },
    })

    const { search, fetchData } = useTablePage(mockService, { fetchOnMount: false })
    search.value = 'test query'

    await fetchData()

    expect(mockService).toHaveBeenCalledWith({ page: 1, perPage: 12, search: 'test query' })
  })

  it('does not send search param when search is empty', async () => {
    mockService.mockResolvedValue({
      data: [],
      pagination: { currentPage: 1, perPage: 12, totalObjects: 0, totalPages: 0 },
    })

    const { fetchData } = useTablePage(mockService, { fetchOnMount: false })
    await fetchData()

    expect(mockService).toHaveBeenCalledWith({ page: 1, perPage: 12 })
  })

  it('handles errors gracefully', async () => {
    const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
    mockService.mockRejectedValue(new Error('Network error'))

    const { items, totalItems, fetchData } = useTablePage(mockService, { fetchOnMount: false })
    await fetchData()

    expect(items.value).toEqual([])
    expect(totalItems.value).toBe(0)
    consoleSpy.mockRestore()
  })

  it('handles response with status.success wrapper', async () => {
    const mockData = [{ id: 1 }]
    mockService.mockResolvedValue({
      data: mockData,
      status: { success: true },
      pagination: { currentPage: 1, perPage: 12, totalObjects: 1, totalPages: 1 },
    })

    const { items, fetchData } = useTablePage(mockService, { fetchOnMount: false })
    await fetchData()

    expect(items.value).toEqual(mockData)
  })

  it('fetchData passes explicit page parameter', async () => {
    mockService.mockResolvedValue({
      data: [],
      pagination: { currentPage: 2, perPage: 12, totalObjects: 0, totalPages: 0 },
    })

    const { fetchData } = useTablePage(mockService, { fetchOnMount: false })
    await fetchData(2)

    expect(mockService).toHaveBeenCalledWith({ page: 2, perPage: 12 })
  })

  it('exposes debouncedFetch function', () => {
    mockService.mockResolvedValue({
      data: [],
      pagination: { currentPage: 1, perPage: 12, totalObjects: 0, totalPages: 0 },
    })

    const { debouncedFetch } = useTablePage(mockService, { fetchOnMount: false })
    expect(typeof debouncedFetch).toBe('function')
  })
})
