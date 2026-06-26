<template>
  <v-card-text class="py-2">
    <!-- Title -->
    <v-card-title class="text-h6 justify-center">
      {{ title }}
    </v-card-title>
    <!-- Uploaded File Info -->
    <v-alert v-if="modelValue && !isUploading" class="mb-6 border rounded-lg">
      <div class="d-flex align-center justify-space-between">
        <div class="d-flex gap-2 align-center">
          <div>
            <Csvfileicon />
          </div>
          <div>
            <span class="text-lg">{{ modelValue.name }}</span>
            <p v-if="uploadError" class="text-error mb-0 text-sm">
              {{ uploadError }}
            </p>
            <p v-else class="mb-0 text-secondary text-sm">
              {{ formatFileSize(modelValue.size) }}
            </p>
          </div>
        </div>
        <div class="d-flex gap-2">
          <v-btn v-if="uploadError" color="#fff" size="small" small variant="text" @click="retryFile">
            <Repeaticon />
          </v-btn>
          <v-btn color="#fff" size="small" small variant="text" @click="removeFile">
            <Trash />
          </v-btn>
        </div>
      </div>
    </v-alert>
    <!-- Drag & Drop Zone -->
    <v-card
      v-if="!isUploading && !modelValue"
      class="text-center bg-primary_2 py-6 mb-6"
      :class="['drop-zone', { dragover: dragOver }]"
      @click="fileInput?.click()"
      @dragleave="dragOver = false"
      @dragover.prevent="dragOver = true"
      @drop.prevent="handleDrop"
    >
      <Uploadfile />
      <p class="mt-4 mb-2">{{ t('upload.dragDrop') }}</p>
      <p class="text-secondary text-xs">
        {{ t('upload.supportedFormats') }}
      </p>
      <v-btn class="mt-2 rounded-xl" color="#fff">{{ t('upload.browseFiles') }}</v-btn>
      <input ref="fileInput" accept=".csv" hidden type="file" @change="handleFileSelect" />
    </v-card>

    <v-alert v-if="isUploading" class="mb-6 border rounded-lg">
      <div class="d-flex flex-column">
        <div class="d-flex gap-2 align-center justify-space-between mb-3">
          <span class="text-sm"> {{ t('upload.uploading') }}</span>
          <span>{{ uploadProgress }}%</span>
        </div>
        <div style="flex: 1">
          <div class="bg-white rounded-pill" style="height: 10px; width: 100%">
            <div class="bg-primary rounded-pill h-100" :style="{ width: `${uploadProgress}%` }" />
          </div>
          <div class="mt-1 text-secondary" style="flex: 1">
            <p>
              {{ progressSize }} {{ t('upload.from') }}
              {{ formatFileSize(modelValue?.size ?? 0) }}
            </p>
          </div>
        </div>
      </div>
    </v-alert>

    <v-card-text class="rounded-xl py-2" style="background-color: #f8f5ef">
      <div class="d-flex align-center justify-space-between">
        <div class="d-flex gap-2">
          <Inforound />
          <p class="text-sm mb-0">
            {{ t('upload.formatWarning') }}
          </p>
        </div>
        <v-btn
          v-if="downloadUrl"
          class="text-black bg-white px-5 text-sm"
          color="#fff"
          rounded="xl"
          size="small"
          variant="text"
          @click="downloadTemplate"
          >{{ btnText }}
        </v-btn>
        <span v-else class="text-black bg-white px-5 text-sm">
          {{ btnText }}
        </span>
      </div>
    </v-card-text>
  </v-card-text>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import Repeaticon from '@/components/icons/repeaticon.vue'
import Trash from '@/components/icons/trash.vue'
import Uploadfile from '@/components/icons/uploadfile.vue'
import Csvfileicon from './icons/csvfileicon.vue'
import Inforound from './icons/inforound.vue'

const { t } = useI18n()

const props = withDefaults(
  defineProps<{
    modelValue?: File | null
    title?: string
    uploadError?: string | null
    btnText?: string
    isUploading?: boolean
    uploadProgress?: number
    downloadUrl?: string
  }>(),
  {
    modelValue: null,
    isUploading: false,
    uploadProgress: 0,
  },
)

const emit = defineEmits<{
  (e: 'update:modelValue', value: File | null): void
  (e: 'file-selected', value: File): void
  (e: 'reupload-file'): void
}>()

const dragOver = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)

const progressSize = computed(() => {
  if (!props.modelValue || !props.uploadProgress) return '0 Bytes'
  const bytes = (props.modelValue.size * props.uploadProgress) / 100
  return formatFileSize(bytes)
})

function formatFileSize(bytes: number) {
  if (bytes === 0) return '0 Bytes'

  const k = 1024
  const sizes = [
    t('upload.fileSizeUnits.byte'),
    t('upload.fileSizeUnits.kilobyte'),
    t('upload.fileSizeUnits.megabyte'),
    t('upload.fileSizeUnits.gigabyte'),
    t('upload.fileSizeUnits.terabyte'),
  ]
  const i = Math.floor(Math.log(bytes) / Math.log(k))

  return sizes[i] + ' ' + parseFloat((bytes / Math.pow(k, i)).toFixed(2))
}

async function handleFileSelect(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0] ?? null
  dragOver.value = false
  if (file) {
    emit('file-selected', file)
    emit('update:modelValue', file)
  }
}

function handleDrop(event: DragEvent) {
  const files = event.dataTransfer?.files
  if (files && files.length && files[0].name.endsWith('.csv')) {
    emit('file-selected', files[0])
    emit('update:modelValue', files[0])
  }
  dragOver.value = false
}

function removeFile() {
  emit('update:modelValue', null)
  emit('reupload-file')
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

function retryFile() {
  emit('update:modelValue', null)
  emit('reupload-file')
  nextTick(() => {
    fileInput.value?.click()
  })
}

function downloadTemplate() {
  if (!props.downloadUrl) return
  const link = document.createElement('a')
  link.href = props.downloadUrl
  link.download = ''
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}
</script>

<style scoped>
.drop-zone {
  border: 2px dashed #ddd;
  cursor: pointer;
  transition: background 0.3s;
}

.dragover {
  background: rgba(25, 118, 210, 0.1);
}
</style>
