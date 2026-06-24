/**
 * Users Service
 * Handles all user-related API calls
 */
import BaseService from './base.service'
import type { PaginatedResponse, User } from '@/types'

class UsersService extends BaseService {
  private readonly BASE = '/users'

  async getAll(params?: {
    page?: number
    perPage?: number
    search?: string
    roleId?: number
    district?: string
  }): Promise<PaginatedResponse<User>> {
    return this.get<PaginatedResponse<User>>(this.BASE, { params })
  }

  async getById(id: number): Promise<{ data: User }> {
    return this.get<{ data: User }>(`${this.BASE}/${id}`)
  }

  async create(
    data: Partial<User> & { password?: string; roleId?: number },
  ): Promise<{ data: User; status: { success: boolean; message: string } }> {
    return this.post(`${this.BASE}`, data)
  }

  async update(
    id: number,
    data: Partial<User> & { password?: string; roleId?: number },
  ): Promise<{ data: User; status: { success: boolean; message: string } }> {
    return this.put(`${this.BASE}/${id}`, data)
  }

  async delete(
    id: number,
    data?: { newSupervisorId?: number },
  ): Promise<{ status: { success: boolean; message: string } }> {
    return this.deleteRequest(`${this.BASE}/${id}`, { data })
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

export const usersService = new UsersService()
