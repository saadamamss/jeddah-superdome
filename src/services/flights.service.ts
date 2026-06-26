/**
 * Flights Service
 * Handles all flight-related API calls
 */
import BaseService from './base.service'
import type { Flight, FlightPilgrimAssignment, PaginatedResponse } from '@/types'

class FlightsService extends BaseService {
  private readonly BASE = '/flights'

  async getAll(params?: {
    page?: number
    perPage?: number
    search?: string
    companyId?: number
  }): Promise<PaginatedResponse<Flight>> {
    return this.get<PaginatedResponse<Flight>>(this.BASE, { params })
  }

  async getById(id: number): Promise<{ data: Flight }> {
    return this.get<{ data: Flight }>(`${this.BASE}/${id}`)
  }

  async create(data: Partial<Flight>): Promise<{ data: Flight; status: { success: boolean; message: string } }> {
    return this.post(`${this.BASE}`, data)
  }

  async update(
    id: number,
    data: Partial<Flight>,
  ): Promise<{ data: Flight; status: { success: boolean; message: string } }> {
    return this.put(`${this.BASE}/${id}`, data)
  }

  async delete(id: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.deleteRequest(`${this.BASE}/${id}`)
  }

  async assignPilgrim(flightId: number, pilgrimId: number): Promise<{ status: { success: boolean; message: string } }> {
    return this.post(`${this.BASE}/${flightId}/pilgrims/${pilgrimId}`)
  }

  async unassignPilgrim(
    flightId: number,
    pilgrimId: number,
  ): Promise<{ status: { success: boolean; message: string } }> {
    return this.deleteRequest(`${this.BASE}/${flightId}/pilgrims/${pilgrimId}`)
  }

  async getAssignedPilgrims(
    flightId: number,
    params?: { page?: number; perPage?: number },
  ): Promise<{
    data: {
      items: FlightPilgrimAssignment[]
      pagination: {
        currentPage: number
        perPage: number
        totalObjects: number
        totalPages: number
        itemsOnPage: number
      }
    }
    status: { success: boolean; message: string }
  }> {
    return this.get(`${this.BASE}/${flightId}/pilgrims`, { params })
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

export const flightsService = new FlightsService()
