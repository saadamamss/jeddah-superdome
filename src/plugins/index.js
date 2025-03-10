/**
 * plugins/index.js
 *
 * Automatically included in `./src/main.js`
 */

// Plugins
import vuetify from "./vuetify";
import pinia from "@/stores";
import router from "@/router";
import googleMaps from "./google-maps";

export function registerPlugins(app) {
  app.use(vuetify).use(pinia).use(router).use(googleMaps);
}
