/**
 * Roles Service
 * Handles all role-related API calls
 */
import BaseService from './base.service'
import type { PaginatedResponse, PermissionItem, Role } from '@/types'

class RolesService extends BaseService {
  private readonly BASE = '/roles'

  async getAll(params?: { page?: number; perPage?: number; search?: string }): Promise<PaginatedResponse<Role>> {
    return this.get<PaginatedResponse<Role>>(this.BASE, { params })
  }

  async getById(id: number): Promise<{ data: Role }> {
    return this.get<{ data: Role }>(`${this.BASE}/${id}`)
  }

  async create(data: {
    name: string
    description?: string
    permissions: string[]
  }): Promise<{ data: Role; status: { success: boolean; message: string } }> {
    return this.post('/add-role', data)
  }

  async update(
    id: number,
    data: {
      name: string
      description?: string
      permissions: string[]
    },
  ): Promise<{ data: Role; status: { success: boolean; message: string } }> {
    return this.put(`/edit-role/${id}`, data)
  }

  async delete(id: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.deleteRequest(`/delete-role/${id}`)
  }

  async getPermissions(): Promise<{ data: PermissionItem[] }> {
    return this.get<{ data: PermissionItem[] }>('/permissions')
  }
}

export const rolesService = new RolesService()
