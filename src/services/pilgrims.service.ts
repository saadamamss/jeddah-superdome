/**
 * Pilgrims Service
 * Handles all pilgrim-related API calls
 */
import BaseService from './base.service'
import type { PaginatedResponse, Pilgrim } from '@/types'

class PilgrimsService extends BaseService {
  private readonly BASE = '/pilgrims'

  async getAll(params?: {
    page?: number
    perPage?: number
    search?: string
    companyId?: number
    excludeFlightId?: number
  }): Promise<PaginatedResponse<Pilgrim>> {
    return this.get<PaginatedResponse<Pilgrim>>(this.BASE, { params })
  }

  async getById(id: number): Promise<{ data: Pilgrim }> {
    return this.get<{ data: Pilgrim }>(`${this.BASE}/${id}`)
  }

  async create(data: Partial<Pilgrim>): Promise<{ data: Pilgrim; status: { success: boolean; message: string } }> {
    return this.post(`${this.BASE}`, data)
  }

  async update(
    id: number,
    data: Partial<Pilgrim>,
  ): Promise<{ data: Pilgrim; status: { success: boolean; message: string } }> {
    return this.put(`${this.BASE}/${id}`, data)
  }

  async delete(id: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.deleteRequest(`${this.BASE}/${id}`)
  }

  async upload(
    file: File,
    onProgress?: (percent: number) => void,
  ): Promise<{ data: { created: number; errors: string[] }; status: { success: boolean; message: string } }> {
    if (onProgress) {
      return this.uploadWithProgress<{
        data: { created: number; errors: string[] }
        status: { success: boolean; message: string }
      }>(`${this.BASE}/upload`, file, onProgress)
    }

    const formData = new FormData()
    formData.append('file', file)
    return this.post(`${this.BASE}/upload`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  }
}

export const pilgrimsService = new PilgrimsService()
