/**
 * main.js
 *
 * Bootstraps Vuetify and other plugins then mounts the App`
 */

// Plugins
import { registerPlugins } from '@/plugins'
import { logger } from '@/@core/utils/logger'
// Components
import App from './App.vue'

// Composables
import { createApp } from 'vue'

import '@core/scss/template/index.scss'
import '@styles/styles.scss'

const app = createApp(App)
registerPlugins(app)

// Global error handler for unhandled promise rejections
window.addEventListener('unhandledrejection', (event) => {
  logger.error('Unhandled promise rejection:', event.reason)
  event.preventDefault()
})

// Global Vue error handler
app.config.errorHandler = (err, instance, info) => {
  logger.error('Vue error:', err, '\nComponent:', instance, '\nInfo:', info)
}

app.mount('#app')
