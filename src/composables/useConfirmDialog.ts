/**
 * useConfirmDialog - Shared composable for delete/confirm dialog patterns
 * Manages dialog visibility, selected item ID, and delete confirmation flow
 */
import { ref } from 'vue'

interface UseConfirmDialogOptions {
  onConfirm: (id: number) => Promise<void>
}

export function useConfirmDialog({ onConfirm }: UseConfirmDialogOptions) {
  const isOpen = ref(false)
  const selectedId = ref<number | null>(null)
  const isProcessing = ref(false)

  const open = (id: number) => {
    selectedId.value = id
    isOpen.value = true
  }

  const close = () => {
    selectedId.value = null
    isOpen.value = false
  }

  const confirm = async () => {
    if (!selectedId.value) return

    isProcessing.value = true
    try {
      await onConfirm(selectedId.value)
      close()
    } finally {
      isProcessing.value = false
    }
  }

  return {
    isOpen,
    selectedId,
    isProcessing,
    open,
    close,
    confirm,
  }
}
