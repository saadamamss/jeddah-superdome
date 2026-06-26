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
  identityId: string
  name: string
  email: string
  phone: string
  identityNumber?: string
  role?: Role
  status: 'active' | 'inactive'
  companyId?: number
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
  capacity?: number
  status?: string
  transport: string
}

// ─── Flight Pilgrim Assignment ────────────────────────────────
export interface FlightPilgrimAssignment {
  pilgrimId: number
  name: string
  identity: string
  nationality: string
  gender: string
  phone: string
  email: string
  assignedAt: string
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
  flightsCount: number
  pilgrimsCount: number
  usersCount: number
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

// ─── User Notification (for bell icon) ─────────────────────────
export interface UserNotification {
  id: number
  notificationId: number
  title: string
  body: string
  target: string
  type: string
  isRead: boolean
  readAt?: string
  deliveredAt: string
  createdAt: string
}

export interface UserNotificationsResponse {
  data: UserNotification[]
  unreadCount: number
  pagination: Pagination
  status: {
    success: boolean
    message?: string
  }
}

export interface UnreadCountResponse {
  data: {
    unreadCount: number
  }
  status: {
    success: boolean
    message?: string
  }
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
