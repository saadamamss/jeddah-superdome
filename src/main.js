/**
 * main.js
 *
 * Bootstraps Vuetify and other plugins then mounts the App`
 */

// Plugins
import { registerPlugins } from "@/plugins";
import VueApexCharts from "vue3-apexcharts";
// Components
import App from "./App.vue";

// Composables
import { createApp } from "vue";
// import router from "@/router";
// import { createPinia } from "pinia";
// import VueDraggable from "vue-draggable";

import "@core/scss/template/index.scss";
import "@styles/styles.scss";

const app = createApp(App);
// const pinia = createPinia();
app.component("apexchart", VueApexCharts);
registerPlugins(app);
// app.use(VueDraggable);

app.directive("can", {
  mounted(el, binding) {
    const requiredPermission = binding.value;
  },
});

app.mount("#app");
