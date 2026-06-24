<script setup lang="ts">
import { injectionKeyIsVerticalNavHovered, useLayouts } from '@layouts'
import { VerticalNavGroup, VerticalNavLink, VerticalNavSectionTitle } from '@layouts/components'
import { config } from '@layouts/config'
import { logger } from '@/@core/utils/logger'
import LogoutIcon from '@/components/icons/logouticon.vue'
import { PerfectScrollbar } from 'vue3-perfect-scrollbar'
import { useAuthStore } from '@/stores/auth.store'
import { useAppStore } from '@/stores/app.store'
import { useRoute, useRouter } from 'vue-router'
import type { NavItem, NavSection } from '@/navigation/vertical'
import axios from '@/plugins/axios'

const props = defineProps({
  tag: {
    type: [String, null],
    required: false,
    default: 'aside',
  },
  navItems: {
    type: null,
    required: true,
  },
  isOverlayNavActive: {
    type: Boolean,
    required: true,
  },
  toggleIsOverlayNavActive: {
    type: Function,
    required: true,
  },
})

const refNav = ref()
const { width: windowWidth } = useWindowSize()
const isHovered = useElementHover(refNav)

const auth = useAuthStore()
const appStore = useAppStore()
const router = useRouter()
const isLoggingOut = ref(false)

provide(injectionKeyIsVerticalNavHovered, isHovered)

const {
  isVerticalNavCollapsed: isCollapsed,
  isLessThanOverlayNavBreakpoint,
  isVerticalNavMini,
  isAppRtl,
} = useLayouts()

const isCompanyUser = computed(() => auth.isCompany)

const resolveNavItemComponent = (item: Record<string, unknown>) => {
  if ('heading' in item) return VerticalNavSectionTitle
  if ('children' in item) return VerticalNavGroup

  return VerticalNavLink
}

/** Filter nav items based on role */
const filteredNavItems = computed(() => {
  return (props.navItems as NavSection[]).reduce<NavSection[]>((acc, section) => {
    if (section.divider) {
      acc.push(section)
      return acc
    }

    if (!section.children) return acc

    const filteredChildren = section.children.filter((item: NavItem) => {
      if (isCompanyUser.value && item.adminOnly) return false
      return true
    })

    if (filteredChildren.length > 0) {
      acc.push({ ...section, children: filteredChildren })
    }

    return acc
  }, [])
})

const route = useRoute()

watch(
  () => route.name,
  () => {
    props.toggleIsOverlayNavActive(false)
  },
)

const isVerticalNavScrolled = ref(false)
const updateIsVerticalNavScrolled = (val: boolean) => (isVerticalNavScrolled.value = val)

const handleNavScroll = (evt: Event) => {
  const target = evt.target as HTMLElement
  isVerticalNavScrolled.value = target.scrollTop > 0
}

const handleLogout = async () => {
  isLoggingOut.value = true
  try {
    await axios.post('/logout')
    auth.logout()
    await router.push('/auth/login')
  } catch (error) {
    logger.error('Logout error:', error)
    appStore.showSnackbar({ message: 'حدث خطأ غير متوقع!', color: 'error' })
  } finally {
    isLoggingOut.value = false
  }
}
</script>

<template>
  <Component
    :is="props.tag"
    ref="refNav"
    class="layout-vertical-nav"
    :class="[
      {
        'overlay-nav': isLessThanOverlayNavBreakpoint(windowWidth),
        hovered: isHovered,
        visible: isOverlayNavActive,
        scrolled: isVerticalNavScrolled,
        'sidebar-collapsed': isCollapsed,
      },
    ]"
  >
    <!-- 👉 Header -->
    <div
      class="nav-header d-flex align-center justify-space-between ga-4 position-relative overflow-hidden px-4"
      :class="[isCollapsed ? 'pe-4' : 'px-4']"
    >
      <slot name="nav-header">
        <RouterLink
          v-if="!isCollapsed || isHovered"
          class="app-logo pt-5 mb-2 d-flex align-center justify-center"
          to="/"
        >
          <Transition mode="out-in" name="fade">
            <img alt="جدة سوبردوم" class="d-inline-block mx-auto" src="/jeddah-superdome.svg" width="90" />
          </Transition>
        </RouterLink>
        <!-- 👉 Vertical nav actions -->
        <!-- Show toggle collapsible in >md and close button in <md -->
        <template v-if="!isLessThanOverlayNavBreakpoint(windowWidth)" />
        <template v-else>
          <VIcon class="header-action" v-bind="config.icons.close" @click="toggleIsOverlayNavActive(false)" />
        </template>
      </slot>
    </div>
    <!--  -->
    <slot name="before-nav-items" />
    <!--  -->
    <slot name="nav-items position-relative" :update-is-vertical-nav-scrolled="updateIsVerticalNavScrolled">
      <PerfectScrollbar
        :key="isAppRtl.value ? 'rtl' : 'ltr'"
        class="nav-items pb-4"
        :options="{ wheelPropagation: false }"
        tag="ul"
        @ps-scroll-y="handleNavScroll"
      >
        <template v-for="(item, index) in filteredNavItems" :key="index">
          <v-divider v-if="item.divider" class="mt-10 mb-10" />
          <template v-else>
            <Component :is="resolveNavItemComponent(item as Record<string, unknown>)" :item="item" />
            <template v-for="(child, index) in item.children" :key="index">
              <Component :is="resolveNavItemComponent(child as unknown as Record<string, unknown>)" :item="child" />
            </template>
          </template>
        </template>
      </PerfectScrollbar>
      <div class="nav-link py-3 px-2 pe-6">
        <v-btn
          block
          class="rounded-pill justify-space-between"
          color="#667178"
          :disabled="isLoggingOut"
          :loading="isLoggingOut"
          variant="text"
          @click="handleLogout"
        >
          <LogoutIcon />
          <span class="nav-item-title ms-3 text-base"> تسجيل الخروج </span>
        </v-btn>
      </div>
    </slot>
  </Component>
</template>

<style lang="scss">
@use '@configured-variables' as variables;
@use '@layouts/styles/mixins';

.app-logo {
  img {
    filter: unset;
    max-inline-size: 300px;
  }
}

// 👉 Vertical Nav
.layout-vertical-nav {
  position: fixed;
  z-index: variables.$layout-vertical-nav-z-index;
  display: flex;
  flex-direction: column;
  block-size: 100%;
  inline-size: variables.$layout-vertical-nav-width;
  inset-block-start: 0;
  inset-inline-start: 0;
  transition:
    transform 0.25s ease-in-out,
    inline-size 0.25s ease-in-out,
    box-shadow 0.25s ease-in-out;
  will-change: transform, inline-size;
  border-radius: 40px 0 0 40px;

  .nav-header {
    display: flex;
    align-items: center;
    margin-block-end: 1rem;

    .header-action {
      // position: absolute;
      cursor: pointer;
      // inset-block-start: 50%;
      // inset-inline-start: 10px;
      // transform: translateY(-50%);
    }
  }

  .app-title-wrapper {
    margin-inline-end: auto;
  }

  .nav-items {
    block-size: 100%;

    // ℹ️ We no loner needs this overflow styles as perfect scrollbar applies it
    // overflow-x: hidden;

    // // ℹ️ We used `overflow-y` instead of `overflow` to mitigate overflow x. Revert back if any issue found.
    // overflow-y: auto;
  }

  .nav-item-title {
    overflow: hidden;
    margin-inline-end: auto;
    text-overflow: ellipsis;
    white-space: nowrap;
    line-height: 1.6;
  }

  // 👉 Collapsed
  .layout-vertical-nav-collapsed & {
    &:not(.hovered) {
      inline-size: variables.$layout-vertical-nav-collapsed-width;
    }
  }

  // 👉 Overlay nav
  &.overlay-nav {
    &:not(.visible) {
      transform: translateX(-#{variables.$layout-vertical-nav-width});

      @include mixins.rtl {
        transform: translateX(variables.$layout-vertical-nav-width);
      }
    }
  }
}
</style>
