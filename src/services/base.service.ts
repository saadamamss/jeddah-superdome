/**
 * Base API Service
 * Provides a typed wrapper around Axios with request/response interceptors and optional caching
 */
import axiosIns from '@/plugins/axios'
import type { AxiosRequestConfig, AxiosResponse } from 'axios'
import { apiCache } from './cache'

class BaseService {
  protected client = axiosIns

  protected async get<T>(url: string, config?: AxiosRequestConfig, cacheTTL?: number): Promise<T> {
    const cacheKey = `GET:${url}:${JSON.stringify(config?.params ?? {})}`

    if (cacheTTL !== undefined && cacheTTL > 0) {
      const cached = apiCache.get<T>(cacheKey)
      if (cached !== null) return cached
    }

    const response: AxiosResponse<T> = await this.client.get(url, config)
    const data = response.data

    if (cacheTTL !== undefined && cacheTTL > 0) {
      apiCache.set(cacheKey, data, cacheTTL)
    }

    return data
  }

  protected async post<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<T> {
    const response: AxiosResponse<T> = await this.client.post(url, data, config)
    apiCache.invalidate(url.split('/').slice(0, -1).join('/'))
    return response.data
  }

  protected async put<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<T> {
    const response: AxiosResponse<T> = await this.client.put(url, data, config)
    apiCache.invalidate(url.split('/').slice(0, -1).join('/'))
    return response.data
  }

  protected async patch<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<T> {
    const response: AxiosResponse<T> = await this.client.patch(url, data, config)
    apiCache.invalidate(url.split('/').slice(0, -1).join('/'))
    return response.data
  }

  protected async deleteRequest<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response: AxiosResponse<T> = await this.client.delete(url, config)
    apiCache.invalidate(url.split('/').slice(0, -1).join('/'))
    return response.data
  }

  protected async uploadWithProgress<T>(
    url: string,
    file: File,
    onProgress?: (percent: number) => void,
    additionalFields?: Record<string, string>,
  ): Promise<T> {
    const formData = new FormData()
    formData.append('file', file)

    if (additionalFields) {
      for (const [key, value] of Object.entries(additionalFields)) {
        formData.append(key, value)
      }
    }

    const response: AxiosResponse<T> = await this.client.post(url, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total)
          onProgress(percent)
        }
      },
    })

    apiCache.invalidate(url.split('/').slice(0, -1).join('/'))
    return response.data
  }
}

export default BaseService
