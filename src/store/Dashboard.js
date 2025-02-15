import { defineStore } from 'pinia'
import axios from '@axios'

export const useDashboardStore = defineStore('DashboardStore', {
  actions: {
    // 👉 Fetch all Countries
    fetchDashboard(params) {
      return axios.get('dashboard', { params })
    },
  },
})
