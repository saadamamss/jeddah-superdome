import { beforeEach, describe, expect, it, vi } from 'vitest'
import BaseService from '../base.service'

import axiosIns from '@/plugins/axios'

vi.mock('@/plugins/axios', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    patch: vi.fn(),
    delete: vi.fn(),
  },
}))

// Create a concrete subclass to test the abstract BaseService
class TestService extends BaseService {
  public async testGet<T>(url: string, config?: Parameters<BaseService['get']>[1]) {
    return this.get<T>(url, config)
  }

  public async testPost<T>(url: string, data?: unknown, config?: Parameters<BaseService['post']>[2]) {
    return this.post<T>(url, data, config)
  }

  public async testPut<T>(url: string, data?: unknown, config?: Parameters<BaseService['put']>[2]) {
    return this.put<T>(url, data, config)
  }

  public async testPatch<T>(url: string, data?: unknown, config?: Parameters<BaseService['patch']>[2]) {
    return this.patch<T>(url, data, config)
  }

  public async testDelete<T>(url: string, config?: Parameters<BaseService['deleteRequest']>[1]) {
    return this.deleteRequest<T>(url, config)
  }
}

describe('BaseService', () => {
  let service: TestService

  beforeEach(() => {
    vi.clearAllMocks()
    service = new TestService()
  })

  describe('get', () => {
    it('calls axiosIns.get and returns data', async () => {
      const mockData = { id: 1, name: 'test' }
      vi.mocked(axiosIns.get).mockResolvedValue({ data: mockData })

      const result = await service.testGet('/test')

      expect(axiosIns.get).toHaveBeenCalledWith('/test', undefined)
      expect(result).toEqual(mockData)
    })

    it('passes config to axiosIns.get', async () => {
      vi.mocked(axiosIns.get).mockResolvedValue({ data: null })
      const config = { params: { page: 1 } }

      await service.testGet('/test', config)

      expect(axiosIns.get).toHaveBeenCalledWith('/test', config)
    })
  })

  describe('post', () => {
    it('calls axiosIns.post and returns data', async () => {
      const mockData = { id: 1 }
      vi.mocked(axiosIns.post).mockResolvedValue({ data: mockData })

      const result = await service.testPost('/test', { name: 'test' })

      expect(axiosIns.post).toHaveBeenCalledWith('/test', { name: 'test' }, undefined)
      expect(result).toEqual(mockData)
    })
  })

  describe('put', () => {
    it('calls axiosIns.put and returns data', async () => {
      const mockData = { id: 1, name: 'updated' }
      vi.mocked(axiosIns.put).mockResolvedValue({ data: mockData })

      const result = await service.testPut('/test/1', { name: 'updated' })

      expect(axiosIns.put).toHaveBeenCalledWith('/test/1', { name: 'updated' }, undefined)
      expect(result).toEqual(mockData)
    })
  })

  describe('patch', () => {
    it('calls axiosIns.patch and returns data', async () => {
      const mockData = { id: 1, name: 'patched' }
      vi.mocked(axiosIns.patch).mockResolvedValue({ data: mockData })

      const result = await service.testPatch('/test/1', { name: 'patched' })

      expect(axiosIns.patch).toHaveBeenCalledWith('/test/1', { name: 'patched' }, undefined)
      expect(result).toEqual(mockData)
    })
  })

  describe('deleteRequest', () => {
    it('calls axiosIns.delete and returns data', async () => {
      const mockData = { status: { success: true } }
      vi.mocked(axiosIns.delete).mockResolvedValue({ data: mockData })

      const result = await service.testDelete('/test/1')

      expect(axiosIns.delete).toHaveBeenCalledWith('/test/1', undefined)
      expect(result).toEqual(mockData)
    })
  })
})
