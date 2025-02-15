<script setup>
import { injectionKeyIsVerticalNavHovered, useLayouts } from "@layouts";
import {
  VerticalNavGroup,
  VerticalNavLink,
  VerticalNavSectionTitle,
} from "@layouts/components";
import { config } from "@layouts/config";
import { PerfectScrollbar } from "vue3-perfect-scrollbar";

const props = defineProps({
  tag: {
    type: [String, null],
    required: false,
    default: "aside",
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
});

const refNav = ref();
const { width: windowWidth } = useWindowSize();
const isHovered = useElementHover(refNav);

provide(injectionKeyIsVerticalNavHovered, isHovered);

const {
  isVerticalNavCollapsed: isCollapsed,
  isLessThanOverlayNavBreakpoint,
  isVerticalNavMini,
  isAppRtl,
} = useLayouts();

const hideTitleAndIcon = isVerticalNavMini(windowWidth, isHovered);

const resolveNavItemComponent = (item) => {
  if ("heading" in item) return VerticalNavSectionTitle;
  if ("children" in item) return VerticalNavGroup;

  return VerticalNavLink;
};

const route = useRoute();
const user_permissions = reactive([]);

watch(
  () => route.name,
  () => {
    props.toggleIsOverlayNavActive(false);
  }
);

onMounted(() => {
  user_permissions.value = JSON.parse(localStorage.getItem("SAR_TOKEN"));
});

// const userNavItems = computed(() => {
//   return props.navItems
// })

const isVerticalNavScrolled = ref(false);
const updateIsVerticalNavScrolled = (val) =>
  (isVerticalNavScrolled.value = val);

const handleNavScroll = (evt) => {
  isVerticalNavScrolled.value = evt.target.scrollTop > 0;
};
</script>

<template>
  <Component
    :is="props.tag"
    ref="refNav"
    class="layout-vertical-nav bg-primary"
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
          to="/"
          class="app-logo d-flex align-center justify-center"
        >
          <Transition name="fade" mode="out-in">
            <!-- <img
              v-if="isCollapsed && !isHovered"
              class="d-inline-block mx-auto"
              src="/logo-sm.svg"
              alt="امتثال"
              width="60"
            /> -->
            <img
              class="d-inline-block mx-auto"
              src="/logo.svg"
              alt="امتثال"
              width="125"
            />
          </Transition>
        </RouterLink>
        <!-- 👉 Vertical nav actions -->
        <!-- Show toggle collapsible in >md and close button in <md -->
        <template v-if="!isLessThanOverlayNavBreakpoint(windowWidth)">
          <!-- <VIcon
            v-show="isCollapsed && !hideTitleAndIcon"
            class="header-action"
            v-bind="config.icons.verticalNavUnPinned"
            @click="isCollapsed = !isCollapsed"
          ></VIcon>
          <VIcon
            v-show="!isCollapsed && !hideTitleAndIcon"
            class="header-action"
            v-bind="config.icons.verticalNavPinned"
            @click="isCollapsed = !isCollapsed"
          ></VIcon> -->
          <span
            class="header-action d-inline-flex"
            @click="isCollapsed = !isCollapsed"
            :style="`transition: transform 300ms ease-in-out;transform: rotate(${
              isCollapsed ? '180deg' : ''
            })`"
          >
            <svg
              width="32"
              height="32"
              viewBox="0 0 32 32"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M25.3333 24C25.3333 24.7363 25.9303 25.3333 26.6667 25.3333C27.403 25.3333 28 24.7363 28 24V7.99996C28 7.26358 27.403 6.66663 26.6667 6.66663C25.9303 6.66663 25.3333 7.26358 25.3333 7.99996L25.3333 24Z"
                fill="white"
              />
              <path
                d="M14.9263 9.8762C14.4897 10.4692 14.6165 11.3037 15.2094 11.7403L15.2218 11.7496L15.2633 11.7804C15.3002 11.8079 15.3552 11.849 15.4257 11.9023C15.5668 12.0088 15.7695 12.1633 16.0132 12.3535C16.502 12.7348 17.1493 13.2542 17.7927 13.8126C18.1206 14.0973 18.4398 14.3855 18.7324 14.6655L5.33333 14.6655C4.59695 14.6655 4 15.2624 4 15.9988C4 16.7352 4.59695 17.3321 5.33333 17.3321L18.7333 17.3321C18.4403 17.6126 18.1207 17.9013 17.7924 18.1864C17.149 18.7452 16.5017 19.2649 16.0129 19.6465C15.7692 19.8367 15.5665 19.9914 15.4254 20.098C15.3549 20.1512 15.2999 20.1924 15.263 20.2199L15.2216 20.2508L15.2116 20.2581L15.2091 20.26C14.6163 20.6968 14.4898 21.5314 14.9266 22.1242C15.3634 22.7171 16.1986 22.8432 16.7915 22.4064L16.7962 22.4029L16.8092 22.3934L16.8569 22.3579C16.8981 22.3272 16.9577 22.2826 17.033 22.2256C17.1836 22.1118 17.3976 21.9486 17.6538 21.7485C18.165 21.3495 18.851 20.799 19.541 20.1998C20.2243 19.6063 20.942 18.9389 21.4993 18.308C21.7766 17.9941 22.0424 17.6578 22.2461 17.3198C22.4277 17.0185 22.6667 16.5458 22.6667 15.9988C22.6667 15.4518 22.4277 14.979 22.246 14.6777C22.0423 14.3398 21.7764 14.0036 21.4991 13.6898C20.9417 13.0591 20.224 12.3919 19.5407 11.7988C18.8507 11.1999 18.1647 10.6498 17.6535 10.251C17.3972 10.051 17.1832 9.88783 17.0326 9.77411C16.9573 9.71722 16.8977 9.67264 16.8565 9.64194L16.8088 9.60648L16.7958 9.59692L16.7905 9.593C16.1975 9.15641 15.3629 9.2832 14.9263 9.8762Z"
                fill="white"
              />
            </svg>
          </span>
        </template>
        <template v-else>
          <VIcon
            class="header-action"
            v-bind="config.icons.close"
            @click="toggleIsOverlayNavActive(false)"
          ></VIcon>
        </template>
      </slot>
    </div>
    <slot name="before-nav-items">
      <!-- <div class="vertical-nav-items-shadow"/> -->
    </slot>
    <slot
      name="nav-items"
      :update-is-vertical-nav-scrolled="updateIsVerticalNavScrolled"
    >
      <PerfectScrollbar
        :key="isAppRtl"
        tag="ul"
        class="nav-items pb-4"
        :options="{ wheelPropagation: false }"
        @ps-scroll-y="handleNavScroll"
      >
        <Component
          :is="resolveNavItemComponent(item)"
          v-for="(item, index) in navItems"
          :key="index"
          v-can="item.permission"
          :item="item"
          :class="item.permission"
        />
      </PerfectScrollbar>
    </slot>
  </Component>
</template>

<style lang="scss">
@use "@configured-variables" as variables;
@use "@layouts/styles/mixins";

// .v-theme--dark .app-logo {
//   img {
//     filter: brightness(0) invert(1);
//   }
// }

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
  transition: transform 0.25s ease-in-out, inline-size 0.25s ease-in-out,
    box-shadow 0.25s ease-in-out;
  will-change: transform, inline-size;
  border-radius: 40px 0 0 40px;

  .nav-header {
    display: flex;
    align-items: center;
    border-block-end: 1px solid rgba(0, 0, 0, 5%);
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
