/**
 * Auth Service
 * Handles all authentication-related API calls
 * Tokens are stored in HttpOnly cookies set by the backend — never in localStorage.
 */
import BaseService from './base.service'
import type {
  CompanyInfo,
  LoginCredentials,
  OtpPayload,
  ResetPasswordPayload,
  UpdatePasswordPayload,
  User,
} from '@/types'

interface LoginResponse {
  user: User
}

class AuthService extends BaseService {
  private readonly BASE = ''

  /** Check if user is authenticated by calling /me */
  async isAuthenticated(): Promise<boolean> {
    try {
      await this.getMe()
      return true
    } catch {
      return false
    }
  }

  /** Login — backend sets access_token and refresh_token as HttpOnly cookies */
  async login(credentials: LoginCredentials): Promise<LoginResponse> {
    return this.post<LoginResponse>(`${this.BASE}/login`, credentials)
  }

  /** Logout — backend clears auth cookies */
  async logout(): Promise<void> {
    await this.post<void>(`${this.BASE}/logout`)
  }

  /** Refresh tokens — backend reads refresh_token cookie and sets new cookies */
  async refresh(): Promise<void> {
    await this.post<void>(`${this.BASE}/refresh`)
  }

  async resetPassword(data: ResetPasswordPayload): Promise<{ status: { success: boolean; message: string } }> {
    return this.post(`${this.BASE}/auth/app/reset-password`, data)
  }

  async verifyOtp(data: OtpPayload): Promise<{ status: { success: boolean } }> {
    return this.post(`${this.BASE}/auth/app/verify-otp`, data)
  }

  async updatePassword(data: UpdatePasswordPayload): Promise<{ status: { success: boolean } }> {
    return this.post(`${this.BASE}/auth/app/update-password`, data)
  }

  /** Get current user — backend returns user with company info if role is company */
  async getMe(): Promise<{ data: User }> {
    return this.get<{ data: User }>(`${this.BASE}/me`)
  }

  /** Update company profile */
  async updateCompanyProfile(data: Partial<CompanyInfo>): Promise<{ data: CompanyInfo }> {
    return this.put<{ data: CompanyInfo }>(`${this.BASE}/company/profile`, data)
  }
}

export const authService = new AuthService()
