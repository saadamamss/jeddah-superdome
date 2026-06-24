/// <reference types="vite/client" />

declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<Record<string, unknown>, Record<string, unknown>, unknown>
  export default component
}

declare module 'vue-router/auto' {
  import type { RouteLocationNormalizedGeneric, RouteRecordRaw, Router as VueRouter } from 'vue-router'

  export function createRouter(options: {
    history: ReturnType<typeof import('vue-router').createWebHistory>
    routes: RouteRecordRaw[]
    scrollBehavior?: (
      to: RouteLocationNormalizedGeneric,
      from: RouteLocationNormalizedGeneric,
      savedPosition: { top: number } | null,
    ) => { top: number } | void
  }): VueRouter

  export function createWebHistory(base?: string): ReturnType<typeof import('vue-router').createWebHistory>

  export type RouteLocationNormalized = import('vue-router').RouteLocationNormalized
  export type NavigationGuardNext = import('vue-router').NavigationGuardNext
}

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $can: (action: string, subject: string) => boolean
  }
}
