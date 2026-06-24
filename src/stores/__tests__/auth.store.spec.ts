import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'

import { useAuthStore } from '../auth.store'
import { authService } from '@/services/auth.service'

vi.mock('@/services/auth.service', () => ({
  authService: {
    login: vi.fn(),
    logout: vi.fn(),
    getMe: vi.fn(),
    refresh: vi.fn(),
  },
}))

vi.mock('@/plugins/casl/ability', () => ({
  defineAbilityFromUser: vi.fn(),
}))

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('initial state', () => {
    it('has null user initially', () => {
      const store = useAuthStore()
      expect(store.user).toBeNull()
    })

    it('is not authenticated initially', () => {
      const store = useAuthStore()
      expect(store.isAuthenticated).toBe(false)
    })
  })

  describe('init', () => {
    it('loads user from /me on success', async () => {
      const mockUser = {
        id: 1,
        name: 'Admin',
        email: 'admin@test.com',
        phone: '0512345678',
        status: 'active' as const,
        permissions: ['companies.view'],
      }
      vi.mocked(authService.getMe).mockResolvedValue({ data: mockUser })

      const store = useAuthStore()
      const result = await store.init()

      expect(result).toBe(true)
      expect(store.user).toEqual(mockUser)
      expect(store.isAuthenticated).toBe(true)
    })

    it('clears user on /me failure', async () => {
      vi.mocked(authService.getMe).mockRejectedValue(new Error('401'))

      const store = useAuthStore()
      const result = await store.init()

      expect(result).toBe(false)
      expect(store.user).toBeNull()
      expect(store.isAuthenticated).toBe(false)
    })
  })

  describe('logout', () => {
    it('clears user on successful logout', async () => {
      vi.mocked(authService.logout).mockResolvedValue(undefined)
      const store = useAuthStore()

      await store.logout()

      expect(store.user).toBeNull()
    })

    it('clears user even if logout API fails', async () => {
      vi.mocked(authService.logout).mockRejectedValue(new Error('Network error'))
      const store = useAuthStore()

      await store.logout()

      expect(store.user).toBeNull()
    })
  })

  describe('login', () => {
    it('sets user on successful login', async () => {
      const mockUser = { id: 1, name: 'Test', email: 'test@test.com', phone: '0512345678', status: 'active' as const }
      vi.mocked(authService.login).mockResolvedValue({ user: mockUser })

      const store = useAuthStore()
      await store.login({ email: 'test@test.com', password: 'password' })

      expect(store.user).toEqual(mockUser)
    })

    it('throws error when no data received', async () => {
      vi.mocked(authService.login).mockResolvedValue(null as never)

      const store = useAuthStore()

      await expect(store.login({ email: 'test@test.com', password: 'password' })).rejects.toThrow(
        'No data received from server',
      )
    })
  })
})
