/**
 * plugins/index.js
 *
 * Automatically included in `./src/main.js`
 */

// Plugins
import type { App } from 'vue'
import vuetify from './vuetify'
import pinia from '@/stores'
import router from '@/router'
import googleMaps from './google-maps'
import i18n from './i18n'
import VueTelInput from 'vue-tel-input'
import 'vue-tel-input/vue-tel-input.css'
import { abilitiesPlugin } from '@casl/vue'
import { initialAbility } from '@/plugins/casl/ability'

export function registerPlugins(app: App) {
  app
    .use(vuetify)
    .use(VueTelInput)
    .use(pinia)
    .use(router)
    .use(googleMaps)
    .use(i18n)
    .use(abilitiesPlugin, initialAbility)

  app.config.globalProperties.$can = (...args: [string, string]) => {
    return initialAbility.can(...args)
  }
}
