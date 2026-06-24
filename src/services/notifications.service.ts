/**
 * Notifications Service
 * Handles all notification-related API calls
 */
import BaseService from './base.service'
import type { Notification, PaginatedResponse, SendNotificationPayload } from '@/types'

class NotificationsService extends BaseService {
  private readonly BASE = '/notifications'

  async getAll(params?: {
    page?: number
    perPage?: number
    search?: string
  }): Promise<PaginatedResponse<Notification>> {
    return this.get<PaginatedResponse<Notification>>(this.BASE, { params })
  }

  async send(data: SendNotificationPayload): Promise<{ status: { success: boolean; message: string } }> {
    return this.post(`${this.BASE}/send`, data)
  }

  async resend(id: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.post(`${this.BASE}/${id}/resend`)
  }

  async delete(id: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.deleteRequest(`${this.BASE}/${id}`)
  }
}

export const notificationsService = new NotificationsService()
