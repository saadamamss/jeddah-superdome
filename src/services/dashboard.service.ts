/**
 * Dashboard Service
 * Handles dashboard statistics API calls
 */
import BaseService from './base.service'
import type { DashboardData } from '@/types'

class DashboardService extends BaseService {
  private readonly BASE = '/statistics'

  async getStatistics(params?: { page?: number }): Promise<{ data: DashboardData }> {
    return this.get<{ data: DashboardData }>(this.BASE, { params }, 30_000)
  }
}

export const dashboardService = new DashboardService()
