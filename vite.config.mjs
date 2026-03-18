// Plugins
import AutoImport from "unplugin-auto-import/vite";
// import Components from "unplugin-vue-components/vite";
// import Fonts from "unplugin-fonts/vite";
import Layouts from "vite-plugin-vue-layouts";
import Vue from "@vitejs/plugin-vue";
// import vueJsx from "@vitejs/plugin-vue-jsx";
import VueRouter from "unplugin-vue-router/vite";
import Vuetify, { transformAssetUrls } from "vite-plugin-vuetify";

// Utilities
import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    // port: 5000,
  },
  plugins: [
    VueRouter(),
    Layouts(),
    Vue({
      template: { transformAssetUrls },
    }),
    // vueJsx(),
    // https://github.com/vuetifyjs/vuetify-loader/tree/master/packages/vite-plugin#readme
    Vuetify({
      autoImport: true,
      styles: {
        configFile: "src/styles/variables/_vuetify.scss",
        // configFile: "src/@core/scss/template/libs/vuetify/variables",
      },
    }),
    // Components(),
    // Fonts({
    //   google: {
    //     families: [
    //       {
    //         name: "Roboto",
    //         styles: "wght@100;300;400;500;700;900",
    //       },
    //     ],
    //   },
    // }),
    AutoImport({
      imports: ["vue", "vue-router", "@vueuse/core", "@vueuse/math", "pinia"],
      eslintrc: {
        enabled: true,
      },
      vueTemplate: true,
    }),
  ],
  define: { "process.env": {} },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
      "@core": fileURLToPath(new URL("./src/@core", import.meta.url)),
      "@layouts": fileURLToPath(new URL("./src/@layouts", import.meta.url)),
      "@themeConfig": fileURLToPath(
        new URL("./themeConfig.js", import.meta.url)
      ),
      "@images": fileURLToPath(
        new URL("./src/assets/images/", import.meta.url)
      ),
      "@styles": fileURLToPath(new URL("./src/styles/", import.meta.url)),
      "@validators": fileURLToPath(
        new URL("./src/@core/utils/validators", import.meta.url)
      ),
      "@axios": fileURLToPath(new URL("./src/plugins/axios", import.meta.url)),
      "@configured-variables": fileURLToPath(
        new URL("./src/styles/variables/_template.scss", import.meta.url)
      ),
    },
    extensions: [".js", ".json", ".jsx", ".mjs", ".ts", ".tsx", ".vue"],
  },
  optimizeDeps: {
    exclude: ["vuetify"],
    entries: ["./src/**/*.vue"],
  },
  css: {
    preprocessorOptions: {
      scss: {
        api: "modern-compiler", // or "modern-compiler"
      },
    },
  },
});
