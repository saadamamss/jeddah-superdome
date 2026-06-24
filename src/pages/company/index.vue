<template>
  <div>
    <v-container class="px-0 px-sm-4 px-xl-0 py-0" fluid>
      <AppBreadcrumb
        :items="[{ title: t('companies.breadcrumbHome'), to: '/', disabled: false }, { title: t('companies.title') }]"
      />

      <VCard class="rounded-lg">
        <VCardText>
          <!-- Header Section -->
          <div class="d-flex flex-wrap align-center justify-space-between mb-6 ga-4">
            <h2 class="text-h5 font-weight-bold flex-grow-1 flex-md-grow-0">
              {{ t('companies.title') }}
            </h2>

            <div class="d-flex flex-wrap align-center justify-md-end ga-2" style="flex-grow: 1; min-width: 0">
              <v-text-field
                v-model="search"
                class="bg-primary_2 border-none flex-grow-1"
                clearable
                density="compact"
                height="48"
                hide-details
                :placeholder="t('companies.searchPlaceholder')"
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
                  v-if="$can('create', 'companies')"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  color="primary_2"
                  elevation="0"
                  height="48"
                  :prepend-icon="Fileadd"
                  @click="uploadDialog = true"
                >
                  <span class="d-none d-sm-inline">{{ t('companies.addFile') }}</span>
                  <span class="d-inline d-sm-none">{{ t('companies.addFileShort') }}</span>
                </VBtn>

                <VBtn
                  v-if="$can('create', 'companies')"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  elevation="0"
                  height="48"
                  @click="openAddForm"
                >
                  <span class="d-none d-sm-inline">{{ t('companies.addCompany') }}</span>
                  <span class="d-inline d-sm-none">{{ t('companies.addShort') }}</span>
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
                :items="companies"
                :items-per-page="perPage"
                :loading="isLoading"
              >
                <template #[`item.licenseNumber`]="{ item }">
                  <span class="text-nowrap">{{ item.licenseNumber }}</span>
                </template>

                <template #[`item.companyName`]="{ item }">
                  <span class="text-nowrap">{{ item.companyName }}</span>
                </template>
                <template #[`item.contactNumber`]="{ item }">
                  <span class="text-nowrap">{{ item.contactNumber }}</span>
                </template>

                <template #[`item.actions`]="{ item }">
                  <div class="d-flex gap-2">
                    <v-btn
                      v-if="$can('edit', 'companies')"
                      class="text-black text-sm rounded-lg"
                      color="#fff"
                      :prepend-icon="Editicon"
                      size="small"
                      @click="handleEdit(item)"
                    >
                      {{ t('companies.editButton') }}
                    </v-btn>
                    <v-btn
                      v-if="$can('delete', 'companies')"
                      class="text-error text-sm rounded-lg"
                      color="#fff"
                      :prepend-icon="Trash"
                      size="small"
                      @click="openDeleteDialog(item.id)"
                    >
                      {{ t('companies.deleteButton') }}
                    </v-btn>
                  </div>
                </template>
              </DataTable>

              <Pagination v-model:page="currentPage" v-model:per-page="perPage" :total-items="totalItems" />
            </v-col>
          </v-row>
        </VCardText>
      </VCard>

      <!-- Add Company Dialog -->
      <VDialog v-model="isAddDialogVisible" max-width="600px">
        <VCard rounded="lg">
          <VCardTitle class="bg-primary_2 py-3 rounded-lg">
            <div class="d-flex align-center justify-space-between">
              <div>
                <h5 class="mb-2">{{ isEdit ? t('companies.dialog.editTitle') : t('companies.dialog.addTitle') }}</h5>
                <p class="text-sm text-secondary mb-0">
                  {{ t('companies.dialog.subtitle') }}
                </p>
              </div>
            </div>
          </VCardTitle>
          <VCardText style="max-height: 60vh; overflow-y: auto">
            <VForm ref="form" @submit.prevent="handleSubmit">
              <VRow>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('companies.dialog.companyNameLabel') }}</label>
                  <VTextField
                    v-model="formData.companyName"
                    :placeholder="t('companies.dialog.companyNamePlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('companies.dialog.licenseLabel') }}</label>
                  <VTextField
                    v-model="formData.licenseNo"
                    :placeholder="t('companies.dialog.licensePlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('companies.dialog.cityLabel') }}</label>
                  <VTextField
                    v-model="formData.city"
                    :placeholder="t('companies.dialog.cityPlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('companies.dialog.phoneLabel') }}</label>
                  <VTextField
                    v-model="formData.phone"
                    :placeholder="t('companies.dialog.phonePlaceholder')"
                    :rules="[validationRules.required, validationRules.phone]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('companies.dialog.emailLabel') }}</label>
                  <VTextField
                    v-model="formData.email"
                    :placeholder="t('companies.dialog.emailPlaceholder')"
                    :rules="[validationRules.required, validationRules.email]"
                    required
                    rounded="lg"
                    type="email"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('companies.dialog.addressLabel') }}</label>
                  <VTextField
                    v-model="formData.address"
                    :label="t('companies.dialog.addressPlaceholder')"
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
              {{ isEdit ? t('common.save') : t('common.add') }}
            </VBtn>
          </VCardActions>
        </VCard>
      </VDialog>

      <!-- Upload company file Dialog -->
      <VDialog v-model="uploadDialog" max-width="700px">
        <VCard class="rounded-lg" rounded="lg">
          <VCardTitle class="bg-primary_2 py-3 rounded-lg">
            <div class="d-flex align-center justify-space-between">
              <div>
                <h5 class="mb-2">{{ t('companies.upload.title') }}</h5>
                <p class="text-sm text-secondary">
                  {{ t('companies.upload.subtitle') }}
                </p>
              </div>
              <button @click="uploadDialog = false">
                <v-icon size="sm">mdi-close</v-icon>
              </button>
            </div>
          </VCardTitle>

          <UploadComponent
            v-model="fileToUpload"
            :btn-text="t('companies.upload.btnText')"
            :download-url="`${API_BASE_URL}/companies/template`"
            :is-uploading="isUploading"
            :title="t('companies.upload.fileName')"
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
        :action-text="t('companies.success.addAction')"
        :cancel-text="t('companies.success.addCancel')"
        :subtitle="t('companies.success.addSubtitle')"
        :title="t('companies.success.addTitle')"
        @action="successDialogOpen = false"
        @cancel="successDialogOpen = false"
      />

      <!-- Delete Confirmation Dialog -->
      <DeleteDialog
        v-model="deleteDialogOpen"
        :action-text="t('companies.delete.action')"
        :cancel-text="t('companies.delete.cancel')"
        :subtitle="t('companies.delete.subtitle')"
        :title="t('companies.delete.title')"
        @action="confirm"
        @cancel="close"
      />
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import AppBreadcrumb from '@/components/AppBreadcrumb.vue'
import UploadComponent from '@/components/UploadComponent.vue'
import SuccessDialog from '@/components/SuccessDialog.vue'
import DeleteDialog from '@/components/DeleteDialog.vue'
import Pagination from '@/@core/components/Pagination.vue'
import DataTable from '@/components/dataTable.vue'
import Editicon from '@/components/icons/editicon.vue'
import Trash from '@/components/icons/trash.vue'
import Fileadd from '@/components/icons/fileadd.vue'
import Magnifier from '@/components/icons/magnifier.vue'
import { companiesService } from '@/services/companies.service'
import { useAppStore } from '@/stores/app.store'
import { useTablePage } from '@/composables/useTablePage'
import { useConfirmDialog } from '@/composables/useConfirmDialog'
import { useFileUpload } from '@/composables/useFileUpload'
import { API_BASE_URL } from '@/config'
import type { Company } from '@/types'
import { validationRules } from '@/@core/utils/validators'

const { t } = useI18n()
const appStore = useAppStore()

const headers = ref([
  { title: t('companies.headers.licenseNumber'), value: 'licenseNumber' },
  { title: t('companies.headers.companyName'), value: 'companyName' },
  { title: t('companies.headers.connectedHujjaj'), value: 'connectedHujjaj', align: 'start' },
  { title: t('companies.headers.contactNumber'), value: 'contactNumber' },
  { title: '', value: 'actions' },
])

// Use composable for table state — handles pagination, search, loading, error snackbar
const {
  items: companies,
  isLoading,
  search,
  currentPage,
  perPage,
  totalItems,
  fetchData: fetchCompanies,
} = useTablePage<Company>((params) => companiesService.getAll(params), {
  entityName: t('companies.title'),
  defaultPerPage: 12,
  fetchOnMount: false,
})

// State
const isAddDialogVisible = ref(false)
const uploadDialog = ref(false)
const successDialogOpen = ref(false)
const isSubmitting = ref(false)
const form = ref(null)
const isEdit = ref(false)
const editingCompanyId = ref<number | null>(null)

onMounted(() => {
  fetchCompanies()
})

const formData = ref({
  companyName: '',
  licenseNo: '',
  city: '',
  phone: '',
  email: '',
  address: '',
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
  uploadService: (file, onProgress) => companiesService.upload(file, onProgress),
  successMessage: t('companies.upload.success'),
  errorMessage: t('companies.upload.error'),
})

const handleUploadSubmit = async () => {
  const success = await upload()
  if (success) {
    uploadDialog.value = false
    successDialogOpen.value = true
    await fetchCompanies()
  }
}

const openAddForm = () => {
  isEdit.value = false
  editingCompanyId.value = null
  formData.value = {
    companyName: '',
    licenseNo: '',
    city: '',
    phone: '',
    email: '',
    address: '',
  }
  isAddDialogVisible.value = true
}

const handleEdit = (item: Company) => {
  isEdit.value = true
  editingCompanyId.value = item.id
  formData.value = {
    companyName: item.companyName,
    licenseNo: item.licenseNumber,
    city: item.city || '',
    phone: item.contactNumber,
    email: item.email || '',
    address: item.address || '',
  }
  isAddDialogVisible.value = true
}

const handleSubmit = async () => {
  const { valid } = (await form.value?.validate()) ?? { valid: false }
  if (!valid) return

  try {
    isSubmitting.value = true
    if (isEdit.value && editingCompanyId.value) {
      await companiesService.update(editingCompanyId.value, formData.value)
      appStore.showSnackbar({ message: t('companies.messages.editSuccess'), color: 'success' })
    } else {
      await companiesService.create(formData.value)
      appStore.showSnackbar({ message: t('companies.messages.addSuccess'), color: 'success' })
    }
    isAddDialogVisible.value = false
    await fetchCompanies(currentPage.value)
    formData.value = { companyName: '', licenseNo: '', city: '', phone: '', email: '', address: '' }
  } catch (error: unknown) {
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('companies.messages.error'),
      color: 'error',
    })
  } finally {
    isSubmitting.value = false
  }
}

const {
  isOpen: deleteDialogOpen,
  open: openDeleteDialog,
  close,
  confirm,
} = useConfirmDialog({
  onConfirm: async (id: number) => {
    try {
      await companiesService.delete(id)
      appStore.showSnackbar({ message: t('companies.delete.success'), color: 'success' })
      await fetchCompanies(currentPage.value)
    } catch (error: unknown) {
      const axiosError = error as { response?: { data?: { message?: string } } }
      appStore.showSnackbar({
        message: axiosError.response?.data?.message || t('companies.delete.error'),
        color: 'error',
      })
      throw error
    }
  },
})
</script>

<style lang="scss" scoped>
.text-nowrap {
  white-space: nowrap;
}

.ga-2 {
  gap: 0.5rem;
}
</style>
