<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { logger } from '@/@core/utils/logger'
import AppBreadcrumb from '@/components/AppBreadcrumb.vue'
import { useAppStore } from '@/stores/app.store'
import { userNotificationsService } from '@/services/user-notifications.service'
import { notificationsService } from '@/services/notifications.service'
import { usersService } from '@/services/users.service'

import DataTable from '@/components/dataTable.vue'
import Pagination from '@/@core/components/Pagination.vue'
import Repeaticon from '@/components/icons/repeaticon.vue'
import SuccessDialog from '@/components/SuccessDialog.vue'
import DeleteDialog from '@/components/DeleteDialog.vue'
import { useConfirmDialog } from '@/composables/useConfirmDialog'
import type { Notification, User, UserNotification } from '@/types'
import { getAbility } from '@/plugins/casl/ability'

const { t } = useI18n()
const appStore = useAppStore()
const canManage = getAbility().can('create', 'admin_notifications')

// ─── Personal notifications (for current user) ────────────────
const notifications = ref<UserNotification[]>([])
const isLoading = ref(false)
const currentPage = ref(1)
const perPage = ref(20)
const totalItems = ref(0)
const unreadCount = ref(0)

const personalHeaders = [
  { title: '', value: 'status', sortable: false, width: '40px' },
  { title: t('notifications.headers.title'), value: 'title' },
  { title: t('notifications.headers.body'), value: 'body' },
  { title: t('notifications.headers.type'), value: 'type' },
  { title: t('notifications.headers.date'), value: 'date' },
]

async function fetchUserNotifications(page = currentPage.value) {
  isLoading.value = true
  try {
    const response = await userNotificationsService.getAll({ page, perPage: perPage.value })
    notifications.value = response.data
    unreadCount.value = response.unreadCount
    if (response.pagination) {
      totalItems.value = response.pagination.totalObjects
      currentPage.value = response.pagination.currentPage
      perPage.value = response.pagination.perPage
    }
  } catch (error) {
    logger.error('Error fetching notifications:', error)
    appStore.showSnackbar({ message: t('notifications.messages.loadUsersError'), color: 'error' })
    notifications.value = []
    totalItems.value = 0
  } finally {
    isLoading.value = false
  }
}

async function markAsRead(id: number) {
  try {
    await userNotificationsService.markAsRead(id)
    const notif = notifications.value.find((n: UserNotification) => n.id === id)
    if (notif) {
      notif.isRead = true
      notif.readAt = new Date().toISOString()
    }
    if (unreadCount.value > 0) unreadCount.value--
  } catch {
    // silently fail
  }
}

async function markAllAsRead() {
  try {
    await userNotificationsService.markAllAsRead()
    notifications.value.forEach((n: UserNotification) => {
      n.isRead = true
    })
    unreadCount.value = 0
  } catch {
    // silently fail
  }
}

function formatDate(dateStr: string) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  return d.toLocaleDateString('ar-SA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

// ─── Admin: Sent notifications management ────────────────────
const sentNotifications = ref<Notification[]>([])
const sentLoading = ref(false)
const sentPage = ref(1)
const sentPerPage = ref(12)
const sentTotal = ref(0)

const sentHeaders = [
  { title: t('notifications.headers.title'), value: 'notificationTitle' },
  { title: t('notifications.headers.body'), value: 'notificationText' },
  { title: t('notifications.headers.target'), value: 'targetCategory' },
  { title: t('notifications.headers.type'), value: 'notificationType' },
  { title: t('notifications.headers.method'), value: 'transmissionMethod' },
  { title: t('notifications.headers.recipients'), value: 'recipientCount' },
  { title: t('notifications.headers.read'), value: 'readCount' },
  { title: '', value: 'actions' },
]

const isSending = ref(false)
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const form = ref<InstanceType<any> | null>(null)
const isValid = ref(false)

const targetItems = [
  t('notifications.targets.companies'),
  t('notifications.targets.pilgrims'),
  t('notifications.targets.users'),
]
const typeItems = [
  t('notifications.types.reminder'),
  t('notifications.types.announcement'),
  t('notifications.types.alert'),
]
const methodItems = [
  t('notifications.methods.inApp'),
  t('notifications.methods.email'),
  t('notifications.methods.both'),
]

const users = ref<(User & { fullName: string })[]>([])
const dialog = ref(false)
const successDialogOpen = ref(false)
const selectedUsers = ref<number[]>([])
const newNotification = ref({
  title: '',
  body: '',
  target: '',
  type: '',
  method: '',
})

async function fetchSentNotifications(page = sentPage.value) {
  sentLoading.value = true
  try {
    const response = await notificationsService.getAll({ page, perPage: sentPerPage.value })
    sentNotifications.value = response.data
    if (response.pagination) {
      sentTotal.value = response.pagination.totalObjects
      sentPage.value = response.pagination.currentPage
      sentPerPage.value = response.pagination.perPage
    }
  } catch (error) {
    logger.error('Error fetching sent notifications:', error)
    sentNotifications.value = []
    sentTotal.value = 0
  } finally {
    sentLoading.value = false
  }
}

async function fetchUsers() {
  try {
    const response = await usersService.getAll()
    if (response?.status?.success) {
      users.value = (response.data || []).map((user: User) => ({
        ...user,
        fullName: `${user.name}`,
      }))
    }
  } catch (error: unknown) {
    logger.error('Error fetching users:', error)
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('notifications.messages.loadUsersError'),
      color: 'error',
    })
  }
}

function closeDialog() {
  dialog.value = false
  selectedUsers.value = []
  newNotification.value = { title: '', body: '', target: '', type: '', method: '' }
  if (form.value) {
    form.value.reset()
  }
}

async function handleSubmit() {
  if (!form.value) return
  const { valid } = await form.value.validate()
  if (!valid) return

  try {
    isSending.value = true
    const response = await notificationsService.send({
      userIds: selectedUsers.value,
      title: newNotification.value.title,
      body: newNotification.value.body,
      target: newNotification.value.target,
      type: newNotification.value.type,
      method: newNotification.value.method,
    })

    if (response?.status?.success) {
      appStore.showSnackbar({ message: t('notifications.messages.sendSuccess'), color: 'success' })
      closeDialog()
      successDialogOpen.value = true
      await fetchSentNotifications()
    }
  } catch (error: unknown) {
    logger.error('Error sending notification:', error)
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('notifications.messages.sendError'),
      color: 'error',
    })
  } finally {
    isSending.value = false
  }
}

const {
  isOpen: deleteDialog,
  open: openDeleteDialog,
  close: closeDelete,
  confirm,
} = useConfirmDialog({
  onConfirm: async (id: number) => {
    try {
      await notificationsService.delete(id)
      appStore.showSnackbar({ message: t('notifications.delete.success'), color: 'success' })
      await fetchSentNotifications()
    } catch (error: unknown) {
      const axiosError = error as { response?: { data?: { message?: string } } }
      appStore.showSnackbar({
        message: axiosError.response?.data?.message || t('notifications.delete.error'),
        color: 'error',
      })
      throw error
    }
  },
})

const selectedNotification = ref<{ id: number } | null>(null)

watch(dialog, (open: boolean) => {
  if (open && users.value.length === 0) {
    fetchUsers()
  }
})

watch(sentPage, (page: number) => {
  fetchSentNotifications(page)
})

// ─── Lifecycle ────────────────────────────────────────────────
onMounted(async () => {
  await fetchUserNotifications()
  await fetchSentNotifications()
})
</script>

<template>
  <v-container class="px-0 px-sm-4 px-xl-0 py-2 pt-0" fluid>
    <AppBreadcrumb
      :items="[
        { title: t('notifications.breadcrumbHome'), to: '/', disabled: false },
        { title: canManage ? t('notifications.breadcrumbTitle') : t('nav.items.notificationsView') },
      ]"
    />

    <!-- ─── Personal Notifications ─────────────────────────────────── -->
    <VCard v-if="!canManage" class="rounded-lg px-4 bg-light-white mb-6" elevation="0">
      <v-card-title class="pt-4 pb-2">
        <div class="d-flex align-center justify-space-between w-100">
          <div class="d-flex align-center gap-3">
            <h2 class="text-h6 font-weight-bold">{{ t('nav.items.notificationsView') }}</h2>
            <VChip v-if="unreadCount > 0" color="primary" size="small" class="font-weight-medium">
              {{ unreadCount }} غير مقروء
            </VChip>
          </div>
          <div class="d-flex gap-2">
            <VBtn v-if="unreadCount > 0" variant="text" color="primary" size="small" @click="markAllAsRead">
              تحديد الكل كمقروء
            </VBtn>
          </div>
        </div>
      </v-card-title>

      <v-row v-if="isLoading">
        <v-col v-for="item in 6" :key="item" cols="12">
          <v-skeleton-loader type="list-item-two-line" />
        </v-col>
      </v-row>
      <template v-else-if="notifications.length > 0">
        <DataTable
          class="notifications"
          :headers="personalHeaders"
          :items="notifications"
          :items-per-page="perPage"
          :loading="isLoading"
          :page="currentPage"
          :total-items="totalItems"
          @click:row="(_: unknown, row: { item: UserNotification }) => markAsRead(row.item.id)"
        >
          <template #[`item.status`]="{ item }">
            <span
              v-if="!item.isRead"
              class="bg-primary rounded-circle d-inline-block"
              style="width: 8px; height: 8px"
            />
          </template>
          <template #[`item.title`]="{ item }">
            <span :class="!item.isRead ? 'font-weight-bold' : ''">{{ item.title }}</span>
          </template>
          <template #[`item.body`]="{ item }">
            <span :class="!item.isRead ? 'font-weight-bold' : ''" style="white-space: wrap">
              {{ item.body }}
            </span>
          </template>
          <template #[`item.type`]="{ item }">
            <VChip size="small" :color="!item.isRead ? 'primary' : 'grey'" variant="tonal">
              {{ item.type }}
            </VChip>
          </template>
          <template #[`item.date`]="{ item }">
            <span class="text-body-2 text-medium-emphasis">{{ formatDate(item.deliveredAt) }}</span>
          </template>
        </DataTable>
        <Pagination v-model:page="currentPage" v-model:per-page="perPage" :total-items="totalItems" />
      </template>
      <div v-else class="text-center text-medium-emphasis py-10">
        <VIcon icon="tabler-bell-off" size="40" class="mb-2" />
        <p>{{ t('notifications.emptyTitle') }}</p>
      </div>
    </VCard>

    <!-- ─── Admin: Notification Management ────────────────────────── -->
    <VCard v-if="canManage" class="rounded-lg px-4 bg-light-white" elevation="0">
      <v-card-title class="pt-4 pb-2">
        <div>
          <div class="d-flex align-center gap-2 mb-2">
            <div class="d-flex align-center justify-space-between w-100">
              <h3 class="text-h6 font-weight-bold">{{ t('notifications.managementTitle') }}</h3>

              <VBtn
                v-if="$can('create', 'admin_notifications')"
                class="rounded-lg"
                color="primary"
                height="48"
                @click="dialog = true"
              >
                {{ t('notifications.sendNew') }}
              </VBtn>
            </div>
          </div>
        </div>
      </v-card-title>

      <v-row v-if="sentLoading">
        <v-col v-for="item in 9" :key="item" cols="6" lg="4" sm="6">
          <v-card elevation="0">
            <v-card-text class="px-1 py-2">
              <v-skeleton-loader
                class="mx-auto"
                elevation="0"
                max-width="400"
                type="list-item-two-line, table-heading"
              />
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
      <template v-else-if="sentNotifications.length > 0">
        <DataTable
          class="notifications"
          :headers="sentHeaders"
          :items="sentNotifications"
          :items-per-page="sentPerPage"
          :loading="sentLoading"
          :page="sentPage"
          :total-items="sentTotal"
        >
          <template #[`item.notificationTitle`]="{ item }">
            <p style="white-space: wrap">
              {{ item.notificationTitle }}
            </p>
          </template>
          <template #[`item.notificationText`]="{ item }">
            <p style="white-space: wrap">
              {{ item.notificationText }}
            </p>
          </template>
          <template #[`item.actions`]="{ item }">
            <div class="d-flex gap-2">
              <v-btn
                v-if="$can('create', 'admin_notifications')"
                class="text-black rounded-lg"
                color="white"
                :prepend-icon="Repeaticon"
                size="small"
              >
                {{ t('notifications.resend') }}
              </v-btn>
              <v-btn
                v-if="$can('delete', 'admin_notifications')"
                class="text-error text-sm rounded-lg"
                color="#fff"
                size="small"
                @click="(selectedNotification = item) && openDeleteDialog(item.id)"
              >
                {{ t('notifications.deleteButton') }}
              </v-btn>
            </div>
          </template>
        </DataTable>
        <Pagination v-model:page="sentPage" v-model:per-page="sentPerPage" :total-items="sentTotal" />
      </template>
      <div v-else class="text-center text-medium-emphasis py-10">
        <p>{{ t('notifications.emptyTitle') }}</p>
      </div>
    </VCard>

    <!-- Send Notification Dialog -->
    <VDialog v-model="dialog" max-width="600" persistent>
      <VCard>
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">{{ t('notifications.dialog.title') }}</h5>
              <p class="text-sm text-secondary mb-0">
                {{ t('notifications.dialog.subtitle') }}
              </p>
            </div>
            <button @click="dialog = false">
              <v-icon size="sm">mdi-close</v-icon>
            </button>
          </div>
        </VCardTitle>
        <VDivider />
        <VForm ref="form" v-model="isValid" @submit.prevent="handleSubmit">
          <VCardText class="pa-4" style="max-height: 60vh; overflow-y: auto">
            <VRow>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('notifications.dialog.titleLabel') }}</label>
                <VTextField
                  v-model="newNotification.title"
                  :placeholder="t('notifications.dialog.titlePlaceholder')"
                  :rules="[(v: string) => !!v || t('notifications.dialog.titleRequired')]"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('notifications.dialog.bodyLabel') }}</label>
                <VTextarea
                  v-model="newNotification.body"
                  :placeholder="t('notifications.dialog.bodyPlaceholder')"
                  :rules="[(v: string) => !!v || t('notifications.dialog.bodyRequired')]"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('notifications.dialog.targetLabel') }}</label>
                <v-select
                  v-model="newNotification.target"
                  :items="targetItems"
                  :label="t('notifications.dialog.targetPlaceholder')"
                  :rules="[(v: string) => !!v || t('notifications.dialog.targetRequired')]"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('notifications.dialog.typeLabel') }}</label>
                <v-select
                  v-model="newNotification.type"
                  :items="typeItems"
                  :placeholder="t('notifications.dialog.typePlaceholder')"
                  :rules="[(v: string) => !!v || t('notifications.dialog.typeRequired')]"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('notifications.dialog.methodLabel') }}</label>
                <v-select
                  v-model="newNotification.method"
                  :items="methodItems"
                  :placeholder="t('notifications.dialog.methodPlaceholder')"
                  :rules="[(v: string) => !!v || t('notifications.dialog.methodRequired')]"
                />
              </VCol>
            </VRow>
          </VCardText>

          <VCardActions class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between">
            <VBtn class="text-black rounded-xl bg-white" variant="outlined" @click="closeDialog">
              {{ t('common.cancel') }}
            </VBtn>
            <VBtn class="rounded-xl px-8" color="primary" :loading="isSending" variant="flat" @click="handleSubmit">
              {{ t('notifications.dialog.sendButton') }}
            </VBtn>
          </VCardActions>
        </VForm>
      </VCard>
    </VDialog>

    <SuccessDialog
      v-model="successDialogOpen"
      :action-text="t('notifications.success.sendAction')"
      :cancel-text="t('notifications.success.sendCancel')"
      :subtitle="t('notifications.success.sendSubtitle')"
      :title="t('notifications.success.sendTitle')"
      @action="successDialogOpen = false"
      @cancel="successDialogOpen = false"
    />

    <!-- Delete Confirmation Dialog -->
    <DeleteDialog
      v-model="deleteDialog"
      :action-text="t('notifications.delete.action')"
      :cancel-text="t('notifications.delete.cancel')"
      :subtitle="t('notifications.delete.subtitle')"
      :title="t('notifications.delete.title')"
      @action="confirm"
      @cancel="closeDelete"
    />
  </v-container>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}
</style>
