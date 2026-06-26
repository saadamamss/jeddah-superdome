<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useNotifications } from '@/composables/useNotifications'

const router = useRouter()
const { notifications, unreadCount, markAsRead, markAllAsRead, fetchNotifications } = useNotifications()
const isMenuOpen = ref(false)

function onMenuClose(val: boolean) {
  if (!val) {
    fetchNotifications()
  }
}
</script>

<template>
  <VBtn class="btn-icon" color="default" elevation="0" height="48">
    <VBadge color="error" :content="String(unreadCount)" :model-value="unreadCount > 0">
      <svg fill="none" height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg">
        <path
          clip-rule="evenodd"
          d="M10.5288 1.62434C10.9739 1.33296 11.5172 1.25 12.0002 1.25C12.4832 1.25 13.0265 1.33296 13.4716 1.62434C13.9654 1.94759 14.2502 2.47083 14.2502 3.125C14.2502 3.58918 14.109 4.06938 13.8791 4.48431C17.0316 5.28651 19.4124 8.06135 19.5908 11.4516C19.5999 11.6241 19.6066 11.7893 19.6131 11.9485C19.6353 12.49 19.6546 12.9615 19.7527 13.4133C19.87 13.9531 20.0869 14.3775 20.5281 14.7084C21.2974 15.2854 21.7502 16.191 21.7502 17.1527C21.7502 18.534 20.663 19.75 19.2002 19.75H15.6752C15.3277 21.4617 13.8144 22.75 12.0002 22.75C10.186 22.75 8.67266 21.4617 8.32521 19.75H4.8002C3.33739 19.75 2.2502 18.534 2.2502 17.1527C2.2502 16.191 2.70298 15.2854 3.47233 14.7084C3.91349 14.3775 4.13045 13.9531 4.24769 13.4133C4.3458 12.9615 4.36509 12.49 4.38725 11.9485C4.39376 11.7894 4.40052 11.6241 4.4096 11.4516C4.58804 8.06135 6.96877 5.28651 10.1213 4.48431C9.89145 4.06938 9.7502 3.58918 9.7502 3.125C9.7502 2.47083 10.035 1.94759 10.5288 1.62434Z"
          fill="#667178"
          fill-rule="evenodd"
        />
      </svg>
    </VBadge>

    <VMenu
      v-model="isMenuOpen"
      @update:model-value="onMenuClose"
      activator="parent"
      location="bottom end"
      offset="14px"
      width="380px"
    >
      <VList class="py-0 text-dark">
        <VListItem class="notification-section" height="48px">
          <template #title>
            <div class="d-flex align-center w-100">
              <span class="text-base font-weight-medium">التنبيهات</span>
              <VSpacer />
              <VBtn v-if="unreadCount > 0" variant="text" color="primary" size="small" @click="markAllAsRead">
                تحديد الكل كمقروء
              </VBtn>
            </div>
          </template>
        </VListItem>

        <VDivider />

        <template v-if="notifications.length === 0">
          <VListItem class="text-center text-medium-emphasis py-6">
            <VIcon icon="tabler-bell-off" size="40" class="mb-2" />
            <p>لا توجد تنبيهات</p>
          </VListItem>
        </template>

        <template v-for="notification in notifications" :key="notification.id">
          <VListItem class="text-dark" lines="two" link min-height="60px" @click="markAsRead(notification.id)">
            <template #title>
              <div class="d-flex align-center gap-2">
                <span class="font-weight-bold">
                  {{ notification.title }}
                </span>
                <span class="bg-primary rounded-circle" style="width: 8px; height: 8px" />
              </div>
            </template>
            <template #subtitle>
              <span class="text-body-2 text-medium-emphasis">{{ notification.body }}</span>
            </template>
          </VListItem>
          <VDivider />
        </template>

        <VListItem class="notification-section">
          <VBtn block color="dark" variant="text" @click="router.push('/notifications')"> عرض جميع التنبيهات </VBtn>
        </VListItem>
      </VList>
    </VMenu>
  </VBtn>
</template>
