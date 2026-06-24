<template>
  <div>
    <v-container class="px-0 px-sm-4 px-xl-0 py-0" fluid>
      <AppBreadcrumb
        :items="[{ title: t('pilgrims.breadcrumbHome'), to: '/', disabled: false }, { title: t('pilgrims.title') }]"
      />

      <VCard class="rounded-lg">
        <VCardText>
          <!-- Header Section -->
          <div class="d-flex flex-wrap align-center justify-space-between mb-6 ga-4">
            <h2 class="text-h5 font-weight-bold flex-grow-1 flex-md-grow-0">
              {{ t('pilgrims.title') }}
            </h2>

            <div class="d-flex flex-wrap align-center justify-md-end ga-2" style="flex-grow: 1; min-width: 0">
              <v-text-field
                v-model="search"
                class="bg-primary_2 border-none flex-grow-1"
                clearable
                density="compact"
                height="48"
                hide-details
                :placeholder="t('pilgrims.searchPlaceholder')"
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
                  v-if="$can('create', 'pilgrims')"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  color="primary_2"
                  elevation="0"
                  height="48"
                  :prepend-icon="Fileadd"
                  @click="uploadDialog = true"
                >
                  {{ t('pilgrims.addFile') }}
                </VBtn>

                <VBtn
                  v-if="$can('create', 'pilgrims')"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  elevation="0"
                  height="48"
                  @click="isAddDialogVisible = true"
                >
                  {{ t('pilgrims.addPilgrim') }}
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
                :items="items"
                :items-per-page="perPage"
                :loading="isLoading"
              >
                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn class="text-sm text-black rounded-lg" color="#fff" size="small" @click="openDetails(item)">
                      <template #prepend>
                        <EyeIcon />
                      </template>
                    </v-btn>
                    <v-btn
                      v-if="$can('edit', 'pilgrims')"
                      class="text-sm text-black rounded-lg"
                      color="#fff"
                      :prepend-icon="Editicon"
                      size="small"
                      @click="handleEdit(item)"
                    >
                      {{ t('pilgrims.editButton') }}
                    </v-btn>
                    <v-btn
                      v-if="$can('delete', 'pilgrims')"
                      class="text-sm text-error rounded-lg"
                      color="#fff"
                      :prepend-icon="Trash"
                      size="small"
                      @click="openDeleteDialog(item.id)"
                    >
                      {{ t('pilgrims.deleteButton') }}
                    </v-btn>
                  </div>
                </template>
              </DataTable>

              <Pagination v-model:page="currentPage" v-model:per-page="perPage" :total-items="totalItems" />
            </v-col>
          </v-row>
        </VCardText>
      </VCard>

      <!-- Add Pilgrim Dialog -->
      <VDialog v-model="isAddDialogVisible" max-width="600px">
        <VCard rounded="lg">
          <VCardTitle class="bg-primary_2 py-3 rounded-lg">
            <div class="d-flex align-center justify-space-between">
              <div>
                <h5 class="mb-2">{{ isEdit ? t('pilgrims.dialog.editTitle') : t('pilgrims.dialog.addTitle') }}</h5>
                <p class="text-sm text-secondary mb-0">
                  {{ t('pilgrims.dialog.subtitle') }}
                </p>
              </div>
              <button @click="isAddDialogVisible = false">
                <v-icon size="sm">mdi-close</v-icon>
              </button>
            </div>
          </VCardTitle>

          <VCardText style="max-height: 60vh; overflow-y: auto">
            <VForm ref="form" @submit.prevent="handleSubmit">
              <VRow>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('pilgrims.dialog.nameLabel') }}</label>
                  <VTextField
                    v-model="formData.name"
                    :placeholder="t('pilgrims.dialog.namePlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('pilgrims.dialog.identityLabel') }}</label>
                  <VTextField
                    v-model="formData.identity"
                    :placeholder="t('pilgrims.dialog.identityPlaceholder')"
                    :rules="[validationRules.required, validationRules.saId]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('pilgrims.dialog.companyLabel') }}</label>
                  <v-select
                    v-model="formData.companyId"
                    item-title="companyName"
                    item-value="id"
                    :items="companiesList"
                    :placeholder="t('common.select')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('pilgrims.dialog.nationalityLabel') }}</label>
                  <v-select
                    v-model="formData.nationality"
                    :items="['سعودي', 'مصري', 'باكستاني', 'إندونيسي', 'هندي', 'بنغالي', 'نيجيري', 'تركي']"
                    :label="t('common.select')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12" md="6">
                  <label class="mb-2 d-block">{{ t('pilgrims.dialog.genderLabel') }}</label>
                  <v-select
                    v-model="formData.gender"
                    :items="['ذكر', 'أنثى']"
                    :label="t('common.select')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12" md="6">
                  <label class="mb-2 d-block">{{ t('pilgrims.dialog.birthDateLabel') }}</label>
                  <v-text-field
                    v-model="formData.birthDate"
                    :placeholder="t('pilgrims.dialog.birthDatePlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                    type="date"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('pilgrims.dialog.phoneLabel') }}</label>
                  <VTextField
                    v-model="formData.phone"
                    :placeholder="t('pilgrims.dialog.phonePlaceholder')"
                    :rules="[validationRules.required, validationRules.phone]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('pilgrims.dialog.emailLabel') }}</label>
                  <VTextField
                    v-model="formData.email"
                    :placeholder="t('pilgrims.dialog.emailPlaceholder')"
                    :rules="[validationRules.required, validationRules.email]"
                    required
                    rounded="lg"
                    type="email"
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
              {{ isEdit ? t('common.save') : t('common.add') }}
            </VBtn>
          </VCardActions>
        </VCard>
      </VDialog>

      <!-- Upload pilgrim file Dialog -->
      <VDialog v-model="uploadDialog" max-width="700px">
        <VCard class="rounded-lg" rounded="lg">
          <VCardTitle class="bg-primary_2 py-3 rounded-lg">
            <div class="d-flex align-center justify-space-between">
              <div>
                <h5 class="mb-2">{{ t('pilgrims.upload.title') }}</h5>
                <p class="text-sm text-secondary">
                  {{ t('pilgrims.upload.subtitle') }}
                </p>
              </div>
              <button @click="uploadDialog = false">
                <v-icon size="sm">mdi-close</v-icon>
              </button>
            </div>
          </VCardTitle>

          <UploadComponent
            v-model="fileToUpload"
            :btn-text="t('pilgrims.upload.btnText')"
            :download-url="`${API_BASE_URL}/pilgrims/template`"
            :is-uploading="isUploading"
            :title="t('pilgrims.upload.fileName')"
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
        :action-text="t('pilgrims.success.addAction')"
        :cancel-text="t('pilgrims.success.addCancel')"
        :subtitle="t('pilgrims.success.addSubtitle')"
        :title="t('pilgrims.success.addTitle')"
        @action="successDialogOpen = false"
        @cancel="successDialogOpen = false"
      />

      <!-- Delete Confirmation Dialog -->
      <DeleteDialog
        v-model="deleteDialogOpen"
        :action-text="t('pilgrims.delete.action')"
        :cancel-text="t('pilgrims.delete.cancel')"
        :subtitle="t('pilgrims.delete.subtitle')"
        :title="t('pilgrims.delete.title')"
        @action="confirm"
        @cancel="close"
      />
      <!-- Pilgrim Details Dialog -->
      <PilgrimDetailsDialog v-model="detailsDialogVisible" :pilgrim-id="selectedPilgrimId" />
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { logger } from '@/@core/utils/logger'
import AppBreadcrumb from '@/components/AppBreadcrumb.vue'
import UploadComponent from '@/components/UploadComponent.vue'
import SuccessDialog from '@/components/SuccessDialog.vue'
import DeleteDialog from '@/components/DeleteDialog.vue'
import Pagination from '@/@core/components/Pagination.vue'
import DataTable from '@/components/dataTable.vue'
import Editicon from '@/components/icons/editicon.vue'
import EyeIcon from '@/components/icons/eyeicon.vue'
import Trash from '@/components/icons/trash.vue'
import Fileadd from '@/components/icons/fileadd.vue'
import Magnifier from '@/components/icons/magnifier.vue'
import { pilgrimsService } from '@/services/pilgrims.service'
import { companiesService } from '@/services/companies.service'
import { useAppStore } from '@/stores/app.store'
import { useTablePage } from '@/composables/useTablePage'
import { useConfirmDialog } from '@/composables/useConfirmDialog'
import { useFileUpload } from '@/composables/useFileUpload'
import { API_BASE_URL } from '@/config'
import type { Company } from '@/types'
import { validationRules } from '@/@core/utils/validators'
import PilgrimDetailsDialog from './pilgrim-details.vue'

const { t } = useI18n()
const appStore = useAppStore()

// Table state via composable
const { items, isLoading, search, currentPage, perPage, totalItems, fetchData } = useTablePage(
  (params) => pilgrimsService.getAll(params),
  { entityName: t('pilgrims.title'), fetchOnMount: false },
)

const headers = ref([
  { title: t('pilgrims.headers.name'), value: 'name' },
  { title: t('pilgrims.headers.identity'), value: 'identity' },
  { title: t('pilgrims.headers.nationality'), value: 'nationality' },
  { title: t('pilgrims.headers.gender'), value: 'gender' },
  { title: t('pilgrims.headers.phone'), value: 'phone' },
  { title: '', value: 'actions' },
])

const companiesList = ref<Company[]>([])
const isAddDialogVisible = ref(false)
const uploadDialog = ref(false)
const successDialogOpen = ref(false)
const isSubmitting = ref(false)
const form = ref(null)
const isEdit = ref(false)
const editingPilgrimId = ref<number | null>(null)

const formData = ref({
  name: '',
  identity: '',
  companyId: null as number | null,
  nationality: '',
  gender: '',
  birthDate: '',
  phone: '',
  email: '',
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
  uploadService: (file, onProgress) => pilgrimsService.upload(file, onProgress),
  successMessage: t('pilgrims.upload.success'),
  errorMessage: t('pilgrims.upload.error'),
})

const handleUploadSubmit = async () => {
  const success = await upload()
  if (success) {
    uploadDialog.value = false
    successDialogOpen.value = true
    await fetchData()
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

const handleEdit = (item: Record<string, unknown>) => {
  isEdit.value = true
  editingPilgrimId.value = item.id as number
  formData.value = {
    name: (item.name as string) || '',
    identity: (item.identity as string) || '',
    companyId: (item.companyId as number) || null,
    nationality: (item.nationality as string) || '',
    gender: (item.gender as string) || '',
    birthDate: (item.birthDate as string) || '',
    phone: (item.phone as string) || '',
    email: (item.email as string) || '',
  }
  isAddDialogVisible.value = true
}

const handleSubmit = async () => {
  const { valid } = (await form.value?.validate()) ?? { valid: false }
  if (!valid) return

  try {
    isSubmitting.value = true
    if (isEdit.value && editingPilgrimId.value) {
      await pilgrimsService.update(editingPilgrimId.value, {
        name: formData.value.name,
        identity: formData.value.identity,
        companyId: formData.value.companyId ?? undefined,
        nationality: formData.value.nationality,
        gender: formData.value.gender,
        birthDate: formData.value.birthDate,
        phone: formData.value.phone,
        email: formData.value.email,
      })
      appStore.showSnackbar({ message: t('pilgrims.messages.editSuccess'), color: 'success' })
    } else {
      await pilgrimsService.create({
        name: formData.value.name,
        identity: formData.value.identity,
        companyId: formData.value.companyId ?? undefined,
        nationality: formData.value.nationality,
        gender: formData.value.gender,
        birthDate: formData.value.birthDate,
        phone: formData.value.phone,
        email: formData.value.email,
      })
      appStore.showSnackbar({ message: t('pilgrims.messages.addSuccess'), color: 'success' })
    }
    isAddDialogVisible.value = false
    await fetchData(currentPage.value)
    formData.value = {
      name: '',
      identity: '',
      companyId: null,
      nationality: '',
      gender: '',
      birthDate: '',
      phone: '',
      email: '',
    }
  } catch (error: unknown) {
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('pilgrims.messages.error'),
      color: 'error',
    })
  } finally {
    isSubmitting.value = false
  }
}

const selectedPilgrimId = ref<number>(0)
const detailsDialogVisible = ref(false)

const openDetails = (item: { id: number }) => {
  selectedPilgrimId.value = item.id
  detailsDialogVisible.value = true
}

const {
  isOpen: deleteDialogOpen,
  open: openDeleteDialog,
  close,
  confirm,
} = useConfirmDialog({
  onConfirm: async (id: number) => {
    try {
      await pilgrimsService.delete(id)
      appStore.showSnackbar({ message: t('pilgrims.delete.success'), color: 'success' })
      await fetchData(currentPage.value)
    } catch (error: unknown) {
      const axiosError = error as { response?: { data?: { message?: string } } }
      appStore.showSnackbar({
        message: axiosError.response?.data?.message || t('pilgrims.delete.error'),
        color: 'error',
      })
      throw error
    }
  },
})

watch(isAddDialogVisible, (open: boolean) => {
  if (open && companiesList.value.length === 0) {
    fetchCompaniesList()
  }
})

onMounted(() => {
  fetchData()
})
</script>

<style lang="scss" scoped>
.ga-2 {
  gap: 0.5rem;
}
</style>
