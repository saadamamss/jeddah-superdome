import { defineStore } from 'pinia'

export const useSettingsStore = defineStore('settings', () => {
  const isAlertShow = ref(false)
  const alertColor = ref('')
  const alertMessage = ref('')

  return {
    isAlertShow,
    alertColor,
    alertMessage,
  }
})
