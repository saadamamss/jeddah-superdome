/// <reference types="vite/client" />

/* eslint-disable @typescript-eslint/no-explicit-any */

declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<{}, {}, any>
  export default component
}

declare module 'vue3-toastify' {
  export const toast: {
    success(message: string, options?: any): void
    error(message: string, options?: any): void
    info(message: string, options?: any): void
    warning(message: string, options?: any): void
  }
}

declare module '@axios' {
  import type { AxiosInstance } from 'axios'
  const axiosIns: AxiosInstance
  export default axiosIns
}

declare module '@validators' {
  export const requiredValidator: (value: any) => true | string
  export const emailValidator: (value: any) => true | string
}
