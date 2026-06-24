/**
 * Simple TTL cache for API responses
 * Used to avoid redundant identical GET requests within a short time window
 */

interface CacheEntry<T> {
  data: T
  expiry: number
}

export class ApiCache {
  private store = new Map<string, CacheEntry<unknown>>()
  private defaultTTL: number

  constructor(defaultTTL = 60_000) {
    this.defaultTTL = defaultTTL
  }

  get<T>(key: string): T | null {
    const entry = this.store.get(key)
    if (!entry) return null
    if (Date.now() > entry.expiry) {
      this.store.delete(key)
      return null
    }
    return entry.data as T
  }

  set<T>(key: string, data: T, ttl?: number): void {
    this.store.set(key, {
      data,
      expiry: Date.now() + (ttl ?? this.defaultTTL),
    })
  }

  invalidate(pattern?: string): void {
    if (!pattern) {
      this.store.clear()
      return
    }
    for (const key of this.store.keys()) {
      if (key.includes(pattern)) {
        this.store.delete(key)
      }
    }
  }
}

export const apiCache = new ApiCache()
