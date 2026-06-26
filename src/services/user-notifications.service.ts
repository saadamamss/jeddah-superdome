import BaseService from './base.service'
import type { UnreadCountResponse, UserNotificationsResponse } from '@/types'

class UserNotificationsService extends BaseService {
  private readonly BASE = '/user-notifications'

  async getAll(params?: { page?: number; perPage?: number; unreadOnly?: boolean }): Promise<UserNotificationsResponse> {
    return this.get<UserNotificationsResponse>(this.BASE, { params })
  }

  async getUnreadCount(): Promise<UnreadCountResponse> {
    return this.get<UnreadCountResponse>(`${this.BASE}/unread-count`)
  }

  async markAsRead(id: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.put(`${this.BASE}/${id}/read`)
  }

  async markAllAsRead(): Promise<{ status: { success: boolean; message: string } }> {
    return this.put(`${this.BASE}/read-all`)
  }
}

export const userNotificationsService = new UserNotificationsService()
