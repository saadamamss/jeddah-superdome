<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { logger } from '@/@core/utils/logger'
import AppBreadcrumb from '@/components/AppBreadcrumb.vue'
import { useAppStore } from '@/stores/app.store'

import DataTable from '@/components/dataTable.vue'
import Pagination from '@/@core/components/Pagination.vue'
import Repeaticon from '@/components/icons/repeaticon.vue'
import SuccessDialog from '@/components/SuccessDialog.vue'
import DeleteDialog from '@/components/DeleteDialog.vue'
import { notificationsService } from '@/services/notifications.service'
import { usersService } from '@/services/users.service'
import { useTablePage } from '@/composables/useTablePage'
import { useConfirmDialog } from '@/composables/useConfirmDialog'
import type { User } from '@/types'

const { t } = useI18n()
const appStore = useAppStore()
const isSending = ref(false)
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const form = ref<InstanceType<any> | null>(null)
const isValid = ref(false)

const targetItems = ['شركات', 'حجاج', 'مستخدمين']
const typeItems = ['تذكير', 'إعلان', 'تنبيه']
const methodItems = ['إشعار داخل النظام', 'بريد إلكتروني', 'كلاهما']

// Table state via composable
const {
  items: notifications,
  isLoading,
  currentPage,
  perPage,
  totalItems,
  fetchData: fetchNotifications,
} = useTablePage((params) => notificationsService.getAll(params), {
  entityName: t('notifications.title'),
  fetchOnMount: false,
})

// Data
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
const selectedNotification = ref<{ id: number } | null>(null)

const tableHeaders = [
  { title: t('notifications.headers.title'), value: 'notificationTitle' },
  { title: t('notifications.headers.body'), value: 'notificationText' },
  { title: t('notifications.headers.target'), value: 'targetCategory' },
  { title: t('notifications.headers.type'), value: 'notificationType' },
  { title: t('notifications.headers.method'), value: 'transmissionMethod' },
  { title: '', value: 'actions' },
]

const fetchUsers = async () => {
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

const closeDialog = () => {
  dialog.value = false
  selectedUsers.value = []
  newNotification.value = { title: '', body: '', target: '', type: '', method: '' }
  if (form.value) {
    form.value.reset()
  }
}

const handleSubmit = async () => {
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
      appStore.showSnackbar({
        message: t('notifications.messages.sendSuccess'),
        color: 'success',
      })
      closeDialog()
      successDialogOpen.value = true
      await fetchNotifications()
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
  close,
  confirm,
} = useConfirmDialog({
  onConfirm: async (id: number) => {
    try {
      await notificationsService.delete(id)
      appStore.showSnackbar({ message: t('notifications.delete.success'), color: 'success' })
      selectedNotification.value = null
      await fetchNotifications()
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

// Lifecycle
onMounted(async () => {
  await fetchNotifications()
})

watch(dialog, (open: boolean) => {
  if (open && users.value.length === 0) {
    fetchUsers()
  }
})
</script>

<template>
  <v-container class="px-0 px-sm-4 px-xl-0 py-2 pt-0" fluid>
    <AppBreadcrumb
      :items="[
        { title: t('notifications.breadcrumbHome'), to: '/', disabled: false },
        { title: t('notifications.breadcrumbTitle') },
      ]"
    />

    <VCard class="rounded-lg px-4 bg-light-white" elevation="0">
      <v-card-title class="pt-4 pb-2">
        <div>
          <div class="d-flex align-center gap-2 mb-2">
            <div class="d-flex align-center justify-space-between w-100">
              <h2 class="text-h6 font-weight-bold">{{ t('notifications.title') }}</h2>

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

      <v-row v-if="isLoading">
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
      <v-card v-else elevation="0" rounded="lg">
        <DataTable
          class="notifications"
          :headers="tableHeaders"
          :items="notifications"
          :items-per-page="perPage"
          :loading="isLoading"
          :page="currentPage"
          :total-items="totalItems"
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
                @click="openDeleteDialog(item.id)"
              >
                {{ t('notifications.deleteButton') }}
              </v-btn>
            </div>
          </template>
        </DataTable>
        <Pagination v-model:page="currentPage" v-model:per-page="perPage" :total-items="totalItems" />
      </v-card>
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
      @cancel="close"
    />
  </v-container>
</template>

<style lang="scss" scoped>
.bg-light-white {
  background-color: rgba(255, 255, 255, 1);
  border-radius: 16px;
}
</style>
