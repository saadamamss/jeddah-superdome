/// <reference types="vite/client" />

declare module 'vuetify/styles' {
  const styles: void
  export default styles
}

declare module '@casl/vue' {
  import type { Plugin } from 'vue'

  export function useAbility(): {
    can: (action: string, subject: string) => boolean
    cannot: (action: string, subject: string) => boolean
  }

  export function abilitiesPlugin(): Plugin
}

declare module 'aos' {
  const AOS: {
    init: (options?: Record<string, unknown>) => void
    refresh: () => void
    refreshHard: () => void
  }
  export default AOS
}

declare module 'vue-tel-input' {
  import type { Plugin } from 'vue'
  const VueTelInput: Plugin
  export default VueTelInput
}

declare module '@layouts' {
  export const config: {
    app: {
      title: string
      logo: unknown
      contentWidth: { value: string }
      contentLayoutNav: { value: string }
      overlayNavFromBreakpoint: number
      enableI18n: boolean
      isRtl: { value: boolean }
      iconRenderer?: unknown
    }
    navbar: {
      type: { value: string }
      navbarBlur: { value: boolean }
    }
    footer: {
      type: { value: string }
    }
    verticalNav: {
      isVerticalNavCollapsed: { value: boolean }
      defaultNavItemIconProps: Record<string, unknown>
    }
    horizontalNav: {
      type: { value: string }
    }
    icons: {
      chevronDown: Record<string, string>
      chevronRight: Record<string, string>
      close: Record<string, string>
      verticalNavPinned: Record<string, string>
      verticalNavUnPinned: Record<string, string>
      sectionTitlePlaceholder: Record<string, string>
      [key: string]: Record<string, string>
    }
  }

  export function createLayouts(userConfig: Record<string, unknown>): () => void
  export const injectionKeyIsVerticalNavHovered: unique symbol

  export function useLayouts(): {
    navbarType: { value: string }
    isNavbarBlurEnabled: { value: boolean }
    footerType: { value: string }
    isVerticalNavCollapsed: { value: boolean }
    appContentWidth: { value: string }
    appContentLayoutNav: { value: string }
    horizontalNavType: { value: string }
    isLessThanOverlayNavBreakpoint: (windowWidth: number) => boolean
    _layoutClasses: { value: (windowWidth: number, windowScrollY: number) => string[] }
    switchToVerticalNavOnLtOverlayNavBreakpoint: (windowWidth: number) => void
    isVerticalNavMini: (windowWidth: { value: number }, isVerticalNavHovered?: { value: boolean }) => { value: boolean }
    dynamicI18nProps: (key: string, tag?: string) => { [key: string]: string }
    isAppRtl: { value: boolean }
    _setAppDir: (dir: string) => void
  }

  export * from './components'
}

declare module '@layouts/config' {
  export const config: {
    app: {
      title: string
      logo: unknown
      contentWidth: { value: string }
      contentLayoutNav: { value: string }
      overlayNavFromBreakpoint: number
      enableI18n: boolean
      isRtl: { value: boolean }
      iconRenderer: unknown
    }
    navbar: {
      type: { value: string }
      navbarBlur: { value: boolean }
    }
    footer: {
      type: { value: string }
    }
    verticalNav: {
      isVerticalNavCollapsed: { value: boolean }
      defaultNavItemIconProps: Record<string, unknown>
    }
    horizontalNav: {
      type: { value: string }
    }
    icons: {
      chevronDown: Record<string, string>
      chevronRight: Record<string, string>
      close: Record<string, string>
      verticalNavPinned: Record<string, string>
      verticalNavUnPinned: Record<string, string>
      sectionTitlePlaceholder: Record<string, string>
      [key: string]: Record<string, string>
    }
  }
}

declare module '@themeConfig' {
  export const themeConfig: {
    app: {
      title: string
      theme: { value: string }
      skin: { value: string }
      routeTransition: { value: string }
    }
    verticalNav: {
      isVerticalNavSemiDark: { value: boolean }
    }
    layout: {
      app: {
        title: string
        logo: unknown
        contentWidth: string
        contentLayoutNav: string
        overlayNavFromBreakpoint: number
        enableI18n: boolean
        isRtl: boolean
        iconRenderer: unknown
      }
      navbar: {
        type: string
        navbarBlur: boolean
      }
      footer: {
        type: string
      }
      verticalNav: {
        isVerticalNavCollapsed: boolean
        defaultNavItemIconProps: Record<string, unknown>
      }
      horizontalNav: {
        type: string
      }
      icons: Record<string, unknown>
    }
  }

  export const layoutConfig: Record<string, unknown>
}
