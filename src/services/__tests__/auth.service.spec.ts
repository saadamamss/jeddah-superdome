import { beforeEach, describe, expect, it, vi } from 'vitest'

import { authService } from '../auth.service'

// Mock only the axios plugin to prevent real HTTP calls
vi.mock('@/plugins/axios', () => ({
  default: {
    get: vi.fn().mockResolvedValue({ data: {} }),
    post: vi.fn().mockResolvedValue({ data: {} }),
    put: vi.fn().mockResolvedValue({ data: {} }),
    patch: vi.fn().mockResolvedValue({ data: {} }),
    delete: vi.fn().mockResolvedValue({ data: {} }),
    interceptors: {
      request: { use: vi.fn() },
      response: { use: vi.fn() },
    },
  },
}))

// Mock router to prevent import issues
vi.mock('@/router', () => ({
  default: { push: vi.fn() },
}))

describe('AuthService', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('login', () => {
    it('posts credentials and returns user', async () => {
      const mockUser = { id: 1, name: 'Admin', email: 'admin@test.com' }
      const mockPost = vi.fn().mockResolvedValue({ user: mockUser })
      ;(authService as unknown as { post: typeof mockPost }).post = mockPost

      const result = await authService.login({ email: 'admin@test.com', password: 'pass' })
      expect(result).toEqual({ user: mockUser })
    })
  })

  describe('getMe', () => {
    it('calls GET /me', async () => {
      const mockGet = vi.fn().mockResolvedValue({ data: { id: 1, name: 'Admin' } })
      ;(authService as unknown as { get: typeof mockGet }).get = mockGet

      const result = await authService.getMe()
      expect(result.data).toEqual({ id: 1, name: 'Admin' })
    })
  })

  describe('logout', () => {
    it('calls POST /logout', async () => {
      const mockPost = vi.fn().mockResolvedValue(undefined)
      ;(authService as unknown as { post: typeof mockPost }).post = mockPost

      await authService.logout()
      expect(mockPost).toHaveBeenCalledWith('/logout')
    })
  })

  describe('refresh', () => {
    it('calls POST /refresh', async () => {
      const mockPost = vi.fn().mockResolvedValue(undefined)
      ;(authService as unknown as { post: typeof mockPost }).post = mockPost

      await authService.refresh()
      expect(mockPost).toHaveBeenCalledWith('/refresh')
    })
  })
})
