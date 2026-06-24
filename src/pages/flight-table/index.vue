<template>
  <div>
    <v-container class="px-0 px-sm-4 px-xl-0 py-0" fluid>
      <AppBreadcrumb
        :items="[
          { title: t('flights.breadcrumbHome'), to: '/', disabled: false },
          { title: t('flights.breadcrumbTitle') },
        ]"
      />

      <VCard class="rounded-lg">
        <VCardText>
          <!-- Header Section -->
          <div class="d-flex flex-wrap align-center justify-space-between mb-6 ga-4">
            <h2 class="text-h5 font-weight-bold">{{ t('flights.title') }}</h2>

            <div class="d-flex flex-wrap align-center justify-md-end ga-2" style="flex-grow: 1; min-width: 0">
              <v-text-field
                v-model="search"
                class="bg-primary_2 border-none flex-grow-1"
                clearable
                density="compact"
                height="48"
                hide-details
                :placeholder="t('flights.searchPlaceholder')"
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
                  v-if="$can('create', 'flights')"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  color="primary_2"
                  elevation="0"
                  height="48"
                  :prepend-icon="Fileadd"
                  @click="uploadDialog = true"
                >
                  {{ t('flights.addFile') }}
                </VBtn>
                <VBtn
                  v-if="$can('create', 'flights')"
                  class="rounded-lg flex-grow-1 flex-md-grow-0"
                  elevation="0"
                  height="48"
                  @click="isAddDialogVisible = true"
                >
                  {{ t('flights.addFlight') }}
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
                    <v-btn
                      v-if="$can('edit', 'flights')"
                      class="text-sm text-black rounded-lg"
                      color="#fff"
                      :prepend-icon="Editicon"
                      size="small"
                      @click="handleEdit(item)"
                    >
                      {{ t('flights.editButton') }}
                    </v-btn>
                    <v-btn
                      v-if="$can('delete', 'flights')"
                      class="text-sm text-error rounded-lg"
                      color="#fff"
                      :prepend-icon="Trash"
                      size="small"
                      @click="openDeleteDialog(item.id)"
                    >
                      {{ t('flights.deleteButton') }}
                    </v-btn>
                  </div>
                </template>
              </DataTable>

              <Pagination v-model:page="currentPage" v-model:per-page="perPage" :total-items="totalItems" />
            </v-col>
          </v-row>
        </VCardText>
      </VCard>

      <!-- Add Flight Dialog -->
      <VDialog v-model="isAddDialogVisible" max-width="600px">
        <VCard rounded="lg">
          <VCardTitle class="bg-primary_2 py-3 rounded-lg">
            <div class="d-flex align-center justify-space-between">
              <div>
                <h5 class="mb-2">{{ isEdit ? t('flights.dialog.editTitle') : t('flights.dialog.addTitle') }}</h5>
                <p class="text-sm text-secondary mb-0">
                  {{ t('flights.dialog.subtitle') }}
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
                  <label class="mb-2 d-block">{{ t('flights.dialog.flightNumberLabel') }}</label>
                  <VTextField
                    v-model="formData.flightNumber"
                    :placeholder="t('flights.dialog.flightNumberPlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12" md="6">
                  <label class="mb-2 d-block">{{ t('flights.dialog.timeLabel') }}</label>
                  <VTextField
                    v-model="formData.time"
                    :placeholder="t('flights.dialog.timePlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                    type="time"
                  />
                </VCol>
                <VCol cols="12" md="6">
                  <label class="mb-2 d-block">{{ t('flights.dialog.dateLabel') }}</label>
                  <VTextField
                    v-model="formData.date"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                    type="date"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('flights.dialog.companyLabel') }}</label>
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
                  <label class="mb-2 d-block">{{ t('flights.dialog.destinationLabel') }}</label>
                  <VTextField
                    v-model="formData.destination"
                    :placeholder="t('flights.dialog.destinationPlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('flights.dialog.transportLabel') }}</label>
                  <v-select
                    v-model="formData.transport"
                    :items="['باص', 'سيارة', 'شاحنة']"
                    :placeholder="t('common.select')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                  />
                </VCol>
                <VCol cols="12">
                  <label class="mb-2 d-block">{{ t('flights.dialog.pilgrimsCountLabel') }}</label>
                  <VTextField
                    v-model="formData.noOfHuj"
                    :placeholder="t('flights.dialog.pilgrimsCountPlaceholder')"
                    :rules="[validationRules.required]"
                    required
                    rounded="lg"
                    type="number"
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

      <!-- Upload flight file Dialog -->
      <VDialog v-model="uploadDialog" max-width="700px">
        <VCard class="rounded-lg" rounded="lg">
          <VCardTitle class="bg-primary_2 py-3 rounded-lg">
            <div class="d-flex align-center justify-space-between">
              <div>
                <h5 class="mb-2">{{ t('flights.upload.title') }}</h5>
                <p class="text-sm text-secondary">
                  {{ t('flights.upload.subtitle') }}
                </p>
              </div>
              <button @click="uploadDialog = false">
                <v-icon size="sm">mdi-close</v-icon>
              </button>
            </div>
          </VCardTitle>

          <UploadComponent
            v-model="fileToUpload"
            :btn-text="t('flights.upload.btnText')"
            :download-url="`${API_BASE_URL}/flights/template`"
            :is-uploading="isUploading"
            :title="t('flights.upload.fileName')"
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
        :action-text="t('flights.success.addAction')"
        :cancel-text="t('flights.success.addCancel')"
        :subtitle="t('flights.success.addSubtitle')"
        :title="t('flights.success.addTitle')"
        @action="successDialogOpen = false"
        @cancel="successDialogOpen = false"
      />

      <!-- Delete Confirmation Dialog -->
      <DeleteDialog
        v-model="deleteDialogOpen"
        :action-text="t('flights.delete.action')"
        :cancel-text="t('flights.delete.cancel')"
        :subtitle="t('flights.delete.subtitle')"
        :title="t('flights.delete.title')"
        @action="confirm"
        @cancel="close"
      />
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
import Trash from '@/components/icons/trash.vue'
import Fileadd from '@/components/icons/fileadd.vue'
import Magnifier from '@/components/icons/magnifier.vue'
import { flightsService } from '@/services/flights.service'
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

// Table state via composable
const { items, isLoading, search, currentPage, perPage, totalItems, fetchData } = useTablePage(
  (params) => flightsService.getAll(params),
  { entityName: t('flights.title'), fetchOnMount: false },
)

const headers = ref([
  { title: t('flights.headers.flightNumber'), value: 'flightNumber' },
  { title: t('flights.headers.company'), value: 'company' },
  { title: t('flights.headers.date'), value: 'date' },
  { title: t('flights.headers.time'), value: 'time' },
  { title: t('flights.headers.destination'), value: 'destination' },
  { title: t('flights.headers.pilgrimsCount'), value: 'noOfHuj' },
  { title: t('flights.headers.transport'), value: 'transport' },
  { title: '', value: 'actions' },
])

const companiesList = ref<Company[]>([])
const isAddDialogVisible = ref(false)
const uploadDialog = ref(false)
const successDialogOpen = ref(false)
const isSubmitting = ref(false)
const form = ref(null)
const isEdit = ref(false)
const editingFlightId = ref<number | null>(null)

const formData = ref({
  flightNumber: '',
  time: '',
  date: '',
  companyId: null as number | null,
  destination: '',
  transport: '',
  noOfHuj: '' as string | number,
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
  uploadService: (file, onProgress) => flightsService.upload(file, onProgress),
  successMessage: t('flights.upload.success'),
  errorMessage: t('flights.upload.error'),
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
  editingFlightId.value = item.id as number
  formData.value = {
    flightNumber: (item.flightNumber as string) || '',
    time: (item.time as string) || '',
    date: (item.date as string) || '',
    companyId: (item.companyId as number) || null,
    destination: (item.destination as string) || '',
    transport: (item.transport as string) || '',
    noOfHuj: (item.noOfHuj as string) || '',
  }
  isAddDialogVisible.value = true
}

const handleSubmit = async () => {
  const { valid } = (await form.value?.validate()) ?? { valid: false }
  if (!valid) return

  try {
    isSubmitting.value = true
    if (isEdit.value && editingFlightId.value) {
      await flightsService.update(editingFlightId.value, {
        flightNumber: formData.value.flightNumber,
        time: formData.value.time,
        date: formData.value.date,
        companyId: formData.value.companyId ?? undefined,
        destination: formData.value.destination,
        transport: formData.value.transport,
        noOfHuj: Number(formData.value.noOfHuj),
      })
      appStore.showSnackbar({ message: t('flights.messages.editSuccess'), color: 'success' })
    } else {
      await flightsService.create({
        flightNumber: formData.value.flightNumber,
        time: formData.value.time,
        date: formData.value.date,
        companyId: formData.value.companyId ?? undefined,
        destination: formData.value.destination,
        transport: formData.value.transport,
        noOfHuj: Number(formData.value.noOfHuj),
      })
      appStore.showSnackbar({ message: t('flights.messages.addSuccess'), color: 'success' })
    }
    isAddDialogVisible.value = false
    await fetchData(currentPage.value)
    formData.value = {
      flightNumber: '',
      time: '',
      date: '',
      companyId: null,
      destination: '',
      transport: '',
      noOfHuj: '',
    }
  } catch (error: unknown) {
    const axiosError = error as { response?: { data?: { message?: string } } }
    appStore.showSnackbar({
      message: axiosError.response?.data?.message || t('flights.messages.error'),
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
      await flightsService.delete(id)
      appStore.showSnackbar({ message: t('flights.delete.success'), color: 'success' })
      await fetchData(currentPage.value)
    } catch (error: unknown) {
      const axiosError = error as { response?: { data?: { message?: string } } }
      appStore.showSnackbar({
        message: axiosError.response?.data?.message || t('flights.delete.error'),
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
