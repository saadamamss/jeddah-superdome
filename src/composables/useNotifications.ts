import { HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr'
import { userNotificationsService } from '@/services/user-notifications.service'
import { useAuthStore } from '@/stores/auth.store'
import { API_BASE_URL } from '@/config'
import type { UserNotification } from '@/types'

function getCookie(name: string): string | null {
  const match = document.cookie.match(new RegExp(`(?:^|;\\s*)${name}=([^;]*)`))
  return match ? decodeURIComponent(match[1]) : null
}

let connection: ReturnType<typeof createConnection> | null = null
const notifications = ref<UserNotification[]>([])
const unreadCount = ref(0)
const loading = ref(false)

function createConnection() {
  const baseOrigin = new URL(API_BASE_URL).origin
  const hubUrl = `${baseOrigin}/hubs/notifications`

  return new HubConnectionBuilder()
    .withUrl(hubUrl, {
      accessTokenFactory: () => getCookie('access_token') ?? '',
      withCredentials: true,
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build()
}

async function fetchNotifications() {
  try {
    const response = await userNotificationsService.getAll({ page: 1, perPage: 50, unreadOnly: true })
    notifications.value = response.data
    unreadCount.value = response.unreadCount
  } catch {
    // silently fail
  }
}

async function fetchUnreadCount() {
  try {
    const response = await userNotificationsService.getUnreadCount()
    unreadCount.value = response.data.unreadCount
  } catch {
    // silently fail
  }
}

async function markAsRead(id: number) {
  try {
    await userNotificationsService.markAsRead(id)
    notifications.value = notifications.value.filter((n: UserNotification) => n.id !== id)
    if (unreadCount.value > 0) unreadCount.value--
  } catch {
    // silently fail
  }
}

async function markAllAsRead() {
  try {
    await userNotificationsService.markAllAsRead()
    notifications.value = []
    unreadCount.value = 0
  } catch {
    // silently fail
  }
}

function playNotificationSound() {
  try {
    const audio = new Audio('/sounds/notification.mp3')
    audio.volume = 0.5
    audio.play().catch(() => {
      // autoplay blocked by browser
    })
  } catch {
    // silently fail
  }
}

export function useNotifications() {
  const authStore = useAuthStore()

  async function connect() {
    if (connection && connection.state !== HubConnectionState.Disconnected) {
      return
    }

    connection = createConnection()

    connection.on(
      'ReceiveNotification',
      (payload: {
        id: number
        notificationId: number
        title: string
        body: string
        target: string
        type: string
        method: string
        createdAt: string
      }) => {
        const notification: UserNotification = {
          id: payload.id,
          notificationId: payload.notificationId,
          title: payload.title,
          body: payload.body,
          target: payload.target,
          type: payload.type,
          isRead: false,
          deliveredAt: payload.createdAt,
          createdAt: payload.createdAt,
        }
        notifications.value.unshift(notification)
        unreadCount.value++
        playNotificationSound()
      },
    )

    connection.onreconnecting(() => {
      // Will automatically reconnect
    })

    connection.onreconnected(async () => {
      if (authStore.user) {
        await connection?.invoke('JoinGroup', `user_${authStore.user.identityId}`)
      }
    })

    try {
      await connection.start()
      if (authStore.user) {
        await connection.invoke('JoinGroup', `user_${authStore.user.identityId}`)
      }
    } catch {
      // SignalR connection failed; polling will be used as fallback
    }
  }

  async function disconnect() {
    if (connection && connection.state !== HubConnectionState.Disconnected) {
      try {
        await connection.stop()
      } catch {
        // ignore
      }
    }
  }

  async function init() {
    loading.value = true
    try {
      await fetchNotifications()
      await connect()
    } finally {
      loading.value = false
    }
  }

  return {
    notifications: readonly(notifications),
    unreadCount: readonly(unreadCount),
    loading: readonly(loading),
    init,
    connect,
    disconnect,
    fetchUnreadCount,
    fetchNotifications,
    markAsRead,
    markAllAsRead,
  }
}
