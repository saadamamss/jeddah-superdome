/**
 * useFileUpload - Shared composable for file upload logic
 * Manages file selection, upload state, and error handling
 */
import { ref } from 'vue'
import { useAppStore } from '@/stores/app.store'

interface UseFileUploadOptions {
  uploadService: (file: File, onProgress?: (percent: number) => void) => Promise<unknown>
  successMessage?: string
  errorMessage?: string
}

export function useFileUpload({
  uploadService,
  successMessage = 'تم الرفع بنجاح',
  errorMessage = 'حدث خطأ أثناء الرفع',
}: UseFileUploadOptions) {
  const appStore = useAppStore()

  const selectedFile = ref<File | null>(null)
  const isUploading = ref(false)
  const uploadError = ref<string | null>(null)
  const showSuccess = ref(false)
  const uploadProgress = ref(0)

  const selectFile = (file: File) => {
    selectedFile.value = file
    uploadError.value = null
  }

  const upload = async (): Promise<boolean> => {
    if (!selectedFile.value) return false

    isUploading.value = true
    uploadError.value = null
    uploadProgress.value = 0

    try {
      const progressCallback = (percent: number) => {
        uploadProgress.value = percent
      }
      await uploadService(selectedFile.value, progressCallback)
      selectedFile.value = null
      showSuccess.value = true
      appStore.showSnackbar({ message: successMessage, color: 'success' })
      return true
    } catch (error: unknown) {
      const message = error instanceof Error ? error.message : errorMessage
      uploadError.value = message
      appStore.showSnackbar({ message, color: 'error' })
      return false
    } finally {
      isUploading.value = false
    }
  }

  const reset = () => {
    selectedFile.value = null
    uploadError.value = null
    showSuccess.value = false
  }

  return {
    selectedFile,
    isUploading,
    uploadError,
    showSuccess,
    uploadProgress,
    selectFile,
    upload,
    reset,
  }
}
