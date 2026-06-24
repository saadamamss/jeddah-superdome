/**
 * Companies Service
 * Handles all company-related API calls
 */
import BaseService from './base.service'
import type { Company, PaginatedResponse } from '@/types'

type UploadProgressCallback = (percent: number) => void

class CompaniesService extends BaseService {
  private readonly BASE = '/companies'

  async getAll(params?: { page?: number; perPage?: number; search?: string }): Promise<PaginatedResponse<Company>> {
    return this.get<PaginatedResponse<Company>>(this.BASE, { params })
  }

  async getById(id: number): Promise<{ data: Company }> {
    return this.get<{ data: Company }>(`${this.BASE}/${id}`)
  }

  async create(data: Partial<Company>): Promise<{ data: Company; status: { success: boolean; message: string } }> {
    return this.post(`${this.BASE}`, data)
  }

  async update(
    id: number,
    data: Partial<Company>,
  ): Promise<{ data: Company; status: { success: boolean; message: string } }> {
    return this.put(`${this.BASE}/${id}`, data)
  }

  async delete(id: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.deleteRequest(`${this.BASE}/${id}`)
  }

  async upload(
    file: File,
    onProgress?: UploadProgressCallback,
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

export const companiesService = new CompaniesService()
