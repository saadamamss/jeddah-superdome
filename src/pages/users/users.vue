<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import AppBreadcrumb from '@/components/AppBreadcrumb.vue'
import { useAppStore } from '@/stores/app.store'
import { usersService } from '@/services/users.service'
import { useTablePage } from '@/composables/useTablePage'
import { useFileUpload } from '@/composables/useFileUpload'
import { API_BASE_URL } from '@/config'
import type { Company, User } from '@/types'
import DataTable from '@/components/dataTable.vue'
import Pagination from '@/@core/components/Pagination.vue'
import CustomeSwitch from '@/components/CustomeSwitch.vue'
import Editicon from '@/components/icons/editicon.vue'
import Trash from '@/components/icons/trash.vue'
import Fileadd from '@/components/icons/fileadd.vue'
import Magnifier from '@/components/icons/magnifier.vue'
import UploadComponent from '@/components/UploadComponent.vue'
import SuccessDialog from '@/components/SuccessDialog.vue'
import DeleteOperatorConfirmationDialog from './components/DeleteOperatorConfirmationDialog.vue'
import { rolesService } from '@/services/roles.service'
import { companiesService } from '@/services/companies.service'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const appStore = useAppStore()

// Table state via composable
const {
  items: users,
  isLoading,
  search,
  currentPage,
  perPage,
  totalItems: totalUsers,
  fetchData: fetchUsers,
} = useTablePage((params) => usersService.getAll(params), { entityName: t('users.title'), fetchOnMount: false })

const headers = [
  { title: t('users.headers.identity'), value: 'identityNumber', align: 'start' },
  { title: t('users.headers.name'), value: 'name' },
  { title: t('users.headers.role'), value: 'role' },
  { title: t('users.headers.company'), value: 'company' },
  { title: t('users.headers.phone'), value: 'phone' },
  { title: t('users.headers.status'), value: 'status' },
  { title: '', value: 'actions' },
]

// Dialogs
const isAddDialogVisible = ref(false)
const isEditDialogVisible = ref(false)
const uploadDialog = ref(false)
const successDialogOpen = ref(false)
const isSubmitting = ref(false)
const editingUserId = ref<number | null>(null)
const deletingUserId = ref<number | null>(null)

// Form
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const form = ref<InstanceType<any> | null>(null)
const formData = ref({
  name: '',
  email: '',
  phone: '',
  password: '',
  identityNumber: '',
  status: '' as 'active' | 'inactive' | '',
  roleId: null as number | null,
  companyId: null as number | null,
})

const {
  selectedFile: fileToUpload,
  isUploading,
  uploadError,
  uploadProgress,
  selectFile,
  upload,
  reset,
} = useFileUpload({
  uploadService: (file, onProgress) => usersService.upload(file, onProgress),
  successMessage: t('users.upload.success'),
  errorMessage: t('users.upload.error'),
})

// Available roles and companies for selection — fetched from API
const roles = ref<{ id: number; name: string }[]>([])
const companiesList = ref<Company[]>([])

const fetchRoles = async () => {
  try {
    const response = await rolesService.getAll({ perPage: 100 })
    if (response.data) {
      roles.value = response.data.map((r: { id: number; name: string }) => ({ id: r.id, name: r.name }))
    }
  } catch {
    logger.error('Failed to fetch roles')
  }
}

const fetchCompaniesList = async () => {
  try {
    const response = await companiesService.getAll({ perPage: 100 })
    if (response?.status?.success) {
      companiesList.value = response.data || []
    }
  } catch (error) {
    logger.error('Error fetching companies list:', error)
  }
}

const handleStatusChange = async (item: User, newStatus: string) => {
  const previousStatus = item.status
  item.status = newStatus as 'active' | 'inactive'
  try {
    await usersService.update(item.id, { status: newStatus as 'active' | 'inactive' })
    appStore.showSnackbar({ message: t('users.messages.editSuccess'), color: 'success' })
  } catch (error: unknown) {
    item.status = previousStatus
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('users.messages.updateError'),
      color: 'error',
    })
  }
}

const handleUploadSubmit = async () => {
  const success = await upload()
  if (success) {
    uploadDialog.value = false
    successDialogOpen.value = true
    await fetchUsers()
  }
}

const handleSubmit = async () => {
  try {
    isSubmitting.value = true
    await usersService.create({
      name: formData.value.name,
      email: formData.value.email,
      phone: formData.value.phone,
      password: formData.value.password || undefined,
      identityNumber: formData.value.identityNumber || undefined,
      status: (formData.value.status || undefined) as 'active' | 'inactive' | undefined,
      roleId: formData.value.roleId ?? undefined,
      companyId: formData.value.companyId ?? undefined,
    })
    appStore.showSnackbar({ message: t('users.messages.addSuccess'), color: 'success' })
    isAddDialogVisible.value = false
    await fetchUsers()
    formData.value = {
      name: '',
      email: '',
      phone: '',
      password: '',
      identityNumber: '',
      status: '',
      roleId: null,
      companyId: null,
    }
  } catch (error: unknown) {
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('users.messages.updateError'),
      color: 'error',
    })
  } finally {
    isSubmitting.value = false
  }
}

const deleteDialogOpen = ref(false)
const isDeleteLoading = ref(false)

const openDeleteDialog = (id: number) => {
  deletingUserId.value = id
  deleteDialogOpen.value = true
}

const handleDelete = async (newSupervisorId: number | null) => {
  if (!deletingUserId.value) return
  try {
    isDeleteLoading.value = true
    await usersService.delete(deletingUserId.value, { newSupervisorId: newSupervisorId ?? undefined })
    appStore.showSnackbar({ message: t('users.delete.success'), color: 'success' })
    deleteDialogOpen.value = false
    deletingUserId.value = null
    await fetchUsers()
  } catch (error: unknown) {
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('users.delete.error'),
      color: 'error',
    })
  } finally {
    isDeleteLoading.value = false
  }
}

const openEditDialog = (user: User) => {
  editingUserId.value = user.id
  formData.value = {
    name: user.name,
    email: user.email,
    phone: user.phone || '',
    password: '',
    identityNumber: user.identityNumber || '',
    status: user.status,
    roleId: user.role?.id ?? null,
    companyId: user.parentCompanies?.[0]?.id ?? null,
  }
  isEditDialogVisible.value = true
}

const handleEditSubmit = async () => {
  if (!editingUserId.value) return
  try {
    isSubmitting.value = true
    await usersService.update(editingUserId.value, {
      name: formData.value.name,
      email: formData.value.email,
      phone: formData.value.phone,
      password: formData.value.password || undefined,
      identityNumber: formData.value.identityNumber || undefined,
      status: formData.value.status as 'active' | 'inactive' | undefined,
      roleId: formData.value.roleId ?? undefined,
      companyId: formData.value.companyId ?? undefined,
    })
    appStore.showSnackbar({ message: t('users.messages.editSuccess'), color: 'success' })
    isEditDialogVisible.value = false
    editingUserId.value = null
    formData.value = {
      name: '',
      email: '',
      phone: '',
      password: '',
      identityNumber: '',
      status: '',
      roleId: null,
      companyId: null,
    }
    await fetchUsers()
  } catch (error: unknown) {
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('users.messages.updateError'),
      color: 'error',
    })
  } finally {
    isSubmitting.value = false
  }
}

onMounted(() => {
  fetchUsers()
  fetchRoles()
  fetchCompaniesList()
})
</script>

<template>
  <div>
    <v-container class="px-0 px-sm-4 px-xl-0 py-0" fluid>
      <AppBreadcrumb
        :items="[{ title: t('common.home'), to: '/', disabled: false }, { title: t('users.breadcrumbTitle') }]"
      />

      <VCard class="rounded-lg">
        <VCardText>
          <!-- Header Section -->
          <div class="d-flex flex-wrap align-center justify-space-between mb-6 ga-4">
            <h2 class="text-h5 font-weight-bold">{{ t('users.breadcrumbTitle') }}</h2>

            <div class="d-flex flex-wrap align-center justify-md-end ga-2" style="flex-grow: 1; min-width: 0">
              <v-text-field
                v-model="search"
                class="bg-primary_2 border-none flex-grow-1"
                clearable
                density="compact"
                height="48"
                hide-details
                :placeholder="t('users.searchPlaceholder')"
                rounded="lg"
                :style="{
                  'min-width': '200px',
                  'max-width': '300px',
                  'margin-inline-end': '12px',
                }"
              >
                <template #prepend-inner>
                  <Magnifier />
                </template>
              </v-text-field>

              <div class="d-flex flex-wrap ga-2" style="min-width: fit-content">
                <VBtn
                  v-if="$can('create', 'users')"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  color="primary_2"
                  elevation="0"
                  height="48"
                  :prepend-icon="Fileadd"
                  @click="uploadDialog = true"
                >
                  {{ t('users.addFile') }}
                </VBtn>
                <VBtn
                  v-if="$can('create', 'users')"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  elevation="0"
                  height="48"
                  @click="isAddDialogVisible = true"
                >
                  {{ t('users.addUser') }}
                </VBtn>
              </div>
            </div>
          </div>

          <v-row>
            <v-col cols="12">
              <DataTable
                v-model:page="currentPage"
                class="elevation-0"
                :headers="headers"
                :hide-default-footer="true"
                :items="users"
                :items-per-page="perPage"
                :loading="isLoading"
              >
                <template #[`item.status`]="{ item }">
                  <CustomeSwitch
                    :_key="item.identityNumber"
                    :model-value="item.status"
                    @update:model-value="(val: string) => handleStatusChange(item, val)"
                  />
                </template>

                <template #[`item.role`]="{ item }">
                  <v-chip v-if="item.role" color="primary" size="small" variant="tonal">
                    {{ item.role.name }}
                  </v-chip>
                  <span v-else>-</span>
                </template>

                <template #[`item.company`]="{ item }">
                  <span v-if="item.parentCompanies?.length">{{ item.parentCompanies[0].companyName }}</span>
                  <span v-else>-</span>
                </template>

                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn
                      v-if="$can('edit', 'users')"
                      class="text-black text-sm rounded-lg"
                      color="#fff"
                      :prepend-icon="Editicon"
                      size="small"
                      @click="openEditDialog(item)"
                    >
                      {{ t('common.edit') }}
                    </v-btn>
                    <v-btn
                      v-if="$can('delete', 'users')"
                      class="text-error text-sm rounded-lg"
                      color="#fff"
                      :prepend-icon="Trash"
                      size="small"
                      @click="openDeleteDialog(item.id)"
                    >
                      {{ t('common.delete') }}
                    </v-btn>
                  </div>
                </template>
              </DataTable>

              <Pagination v-model:page="currentPage" v-model:per-page="perPage" :total-items="totalUsers" />
            </v-col>
          </v-row>
        </VCardText>
      </VCard>
    </v-container>

    <!-- Add User Dialog -->
    <VDialog v-model="isAddDialogVisible" max-width="600px">
      <VCard rounded="lg">
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">{{ t('users.dialog.addTitle') }}</h5>
              <p class="text-sm text-secondary mb-0">
                {{ t('users.dialog.subtitle') }}
              </p>
            </div>
          </div>
        </VCardTitle>
        <VCardText style="max-height: 60vh; overflow-y: auto">
          <VForm ref="form">
            <VRow>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.firstNameLabel') }} *</label>
                <VTextField
                  v-model="formData.name"
                  :placeholder="t('users.dialog.namePlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.emailLabel') }}</label>
                <VTextField
                  v-model="formData.email"
                  :placeholder="t('users.dialog.emailPlaceholder')"
                  required
                  rounded="lg"
                  type="email"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.phoneLabel') }}</label>
                <VTextField
                  v-model="formData.phone"
                  :placeholder="t('users.dialog.phonePlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">
                  {{ t('users.dialog.passwordLabel') }}
                  <small class="text-medium-emphasis ms-1">({{ t('common.optional') }})</small>
                </label>
                <VTextField
                  v-model="formData.password"
                  :hint="t('users.dialog.passwordHint')"
                  :placeholder="t('users.dialog.passwordPlaceholder')"
                  rounded="lg"
                  type="password"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.identityLabel') }}</label>
                <VTextField
                  v-model="formData.identityNumber"
                  :placeholder="t('users.dialog.identityLabel')"
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.statusLabel') }}</label>
                <v-select
                  v-model="formData.status"
                  :items="['active', 'inactive']"
                  :label="t('common.select')"
                  required
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.roleLabel') }}</label>
                <v-select
                  v-model="formData.roleId"
                  item-title="name"
                  item-value="id"
                  :items="roles"
                  :label="t('common.select')"
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.companyLabel') }}</label>
                <v-select
                  v-model="formData.companyId"
                  item-title="companyName"
                  item-value="id"
                  :items="companiesList"
                  :placeholder="t('common.select')"
                  clearable
                  rounded="lg"
                />
              </VCol>
            </VRow>
          </VForm>
        </VCardText>

        <VCardActions class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between">
          <VBtn class="text-black rounded-xl bg-white" variant="outlined" @click="isAddDialogVisible = false">
            {{ t('common.cancel') }}
          </VBtn>
          <VBtn class="rounded-xl px-8" color="primary" :loading="isSubmitting" variant="flat" @click="handleSubmit">
            {{ t('common.add') }}
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>

    <!-- Edit User Dialog -->
    <VDialog v-model="isEditDialogVisible" max-width="600px">
      <VCard rounded="lg">
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">{{ t('users.dialog.editTitle') }}</h5>
              <p class="text-sm text-secondary mb-0">
                {{ t('users.dialog.subtitle') }}
              </p>
            </div>
          </div>
        </VCardTitle>
        <VCardText style="max-height: 60vh; overflow-y: auto">
          <VForm ref="form">
            <VRow>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.firstNameLabel') }} *</label>
                <VTextField
                  v-model="formData.name"
                  :placeholder="t('users.dialog.namePlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.emailLabel') }}</label>
                <VTextField
                  v-model="formData.email"
                  :placeholder="t('users.dialog.emailPlaceholder')"
                  required
                  rounded="lg"
                  type="email"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.phoneLabel') }}</label>
                <VTextField
                  v-model="formData.phone"
                  :placeholder="t('users.dialog.phonePlaceholder')"
                  required
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.identityLabel') }}</label>
                <VTextField
                  v-model="formData.identityNumber"
                  :placeholder="t('users.dialog.identityLabel')"
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">
                  {{ t('users.dialog.passwordLabel') }}
                  <small class="text-medium-emphasis ms-1">({{ t('common.optional') }})</small>
                </label>
                <VTextField
                  v-model="formData.password"
                  :hint="t('users.dialog.passwordHint')"
                  :placeholder="t('users.dialog.passwordPlaceholder')"
                  rounded="lg"
                  type="password"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.statusLabel') }}</label>
                <v-select
                  v-model="formData.status"
                  :items="['active', 'inactive']"
                  :label="t('common.select')"
                  required
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.roleLabel') }}</label>
                <v-select
                  v-model="formData.roleId"
                  item-title="name"
                  item-value="id"
                  :items="roles"
                  :label="t('common.select')"
                  rounded="lg"
                />
              </VCol>
              <VCol cols="12">
                <label class="mb-2 d-block">{{ t('users.dialog.companyLabel') }}</label>
                <v-select
                  v-model="formData.companyId"
                  item-title="companyName"
                  item-value="id"
                  :items="companiesList"
                  :placeholder="t('common.select')"
                  clearable
                  rounded="lg"
                />
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
        <VCardActions class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between">
          <VBtn class="text-black rounded-xl bg-white" variant="outlined" @click="isEditDialogVisible = false">
            {{ t('common.cancel') }}
          </VBtn>
          <VBtn
            class="rounded-xl px-8"
            color="primary"
            :loading="isSubmitting"
            variant="flat"
            @click="handleEditSubmit"
          >
            {{ t('common.save') }}
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>

    <!-- Upload user file Dialog -->
    <VDialog v-model="uploadDialog" max-width="700px">
      <VCard class="rounded-lg" rounded="lg">
        <VCardTitle class="bg-primary_2 py-3 rounded-lg">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h5 class="mb-2">{{ t('users.upload.title') }}</h5>
              <p class="text-sm text-secondary">
                {{ t('users.upload.subtitle') }}
              </p>
            </div>
            <button @click="uploadDialog = false">
              <v-icon size="sm">mdi-close</v-icon>
            </button>
          </div>
        </VCardTitle>

        <UploadComponent
          v-model="fileToUpload"
          :btn-text="t('users.upload.btnText')"
          :download-url="`${API_BASE_URL}/users/template`"
          :is-uploading="isUploading"
          :title="t('users.upload.fileName')"
          :upload-error="uploadError"
          :upload-progress="uploadProgress"
          @file-selected="selectFile"
          @reupload-file="reset"
        />

        <VCardActions class="bg-primary_2 mt-6 rounded-lg py-3 justify-space-between">
          <VBtn class="text-black rounded-xl bg-white" variant="outlined" @click="uploadDialog = false">
            {{ t('common.cancel') }}
          </VBtn>
          <VBtn
            class="rounded-xl"
            color="primary"
            :disabled="!fileToUpload || isUploading"
            :loading="isUploading"
            variant="flat"
            @click="handleUploadSubmit"
          >
            {{ t('common.add') }}
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>

    <!-- Success Dialog -->
    <SuccessDialog
      v-model="successDialogOpen"
      :action-text="t('users.success.addAction')"
      :cancel-text="t('users.success.addCancel')"
      :subtitle="t('users.success.addSubtitle')"
      :title="t('users.success.addTitle')"
      @action="successDialogOpen = false"
      @cancel="successDialogOpen = false"
    />

    <DeleteOperatorConfirmationDialog
      v-model="deleteDialogOpen"
      :is-deleting="isDeleteLoading"
      @delete="handleDelete"
      @close="deleteDialogOpen = false"
    />
  </div>
</template>

<style lang="scss" scoped>
.ga-2 {
  gap: 0.5rem;
}
</style>
