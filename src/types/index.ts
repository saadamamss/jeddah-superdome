/**
 * API Response Types
 * Standardized types for all API responses from the backend
 */

// ─── Pagination ───────────────────────────────────────────────
export interface Pagination {
  currentPage: number
  perPage: number
  totalObjects: number
  totalPages: number
  itemsOnPage: number
}

export interface PaginatedResponse<T> {
  data: T[]
  pagination: Pagination
  status: {
    success: boolean
    message?: string
  }
}

// ─── Role & Permission ───────────────────────────────────────
export interface Permission {
  id: number
  name: string
}

export interface Role {
  id: number
  name: string
  permissions?: Permission[]
}

// ─── Company ──────────────────────────────────────────────────
export interface Company {
  id: number
  companyName: string
  licenseNumber: string
  city: string
  contactNumber: string
  email: string
  password: string
  address?: string
  connectedHujjaj?: number
  latitude?: number
  longitude?: number
}

// ─── CompanyInfo (must be before User) ────────────────────────
export interface CompanyInfo {
  id: number
  companyName: string
  email: string
  phone?: string
  city?: string
  licenseNo?: string
  address?: string
}

// ─── User ─────────────────────────────────────────────────────
export interface User {
  id: number
  name: string
  email: string
  phone: string
  identityNumber?: string
  role?: Role
  status: 'active' | 'inactive'
  parentCompanies?: Company[]
  permissions?: string[]
  company?: CompanyInfo
}

// ─── Pilgrim ──────────────────────────────────────────────────
export interface Pilgrim {
  id: number
  name: string
  identity: string
  companyId?: number
  company?: string
  nationality: string
  gender: string
  birthDate: string
  phone: string
  email: string
}

// ─── Flight ───────────────────────────────────────────────────
export interface Flight {
  id: number
  flightNumber: string | number
  company: string
  companyId?: number
  date: string
  time: string
  destination: string
  noOfHuj: number
  transport: string
}

// ─── Notification ─────────────────────────────────────────────
export interface Notification {
  id: number
  title: string
  body: string
  target?: string
  type?: string
  method?: string
  createdAt?: string
}

// ─── Dashboard / Statistics ───────────────────────────────────
export interface DashboardStatistics {
  companiesCount: number
  campsCount: number
  locationsCount: number
  mainCompaniesCount: number
  pilgrimsCount: number
}

export interface DashboardData {
  statistics: DashboardStatistics
}

// ─── Auth ─────────────────────────────────────────────────────
export interface LoginCredentials {
  email: string
  password: string
}

export interface LoginResponse {
  user: User
}

// ─── OTP / Password Reset ─────────────────────────────────────
export interface OtpPayload {
  phone: string
  code: string
}

export interface ResetPasswordPayload {
  phone: string
}

export interface UpdatePasswordPayload {
  phone: string
  code: string
  password: string
  passwordConfirmation: string
}

// ─── Notification Send ────────────────────────────────────────
export interface SendNotificationPayload {
  title: string
  body: string
  target: string
  type: string
  method: string
  userIds?: number[]
}

// ─── .NET ProblemDetails ─────────────────────────────────────
export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  errors?: Record<string, string[]>
}

// ─── Permissions ─────────────────────────────────────────────
export interface PermissionItem {
  id: number
  name: string
  key: string
}

// ─── Common ───────────────────────────────────────────────────
export interface ApiError {
  response?: {
    status: number
    data: {
      message?: string
      errors?: Record<string, string[]>
    }
  }
  message?: string
}
