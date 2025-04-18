<script setup>
import { injectionKeyIsVerticalNavHovered, useLayouts } from "@layouts";
import {
  VerticalNavGroup,
  VerticalNavLink,
  VerticalNavSectionTitle,
} from "@layouts/components";
import { config } from "@layouts/config";
import { PerfectScrollbar } from "vue3-perfect-scrollbar";
import { useAuthStore } from "@/stores/Auth";
import { LOCAL_STORAGE_USER_KEY } from "@/config";
import { toast } from "vue3-toastify";
import { authService } from "@/services/auth.service";
import { useRouter, useRoute } from "vue-router";
import { hasPermission } from "@core/utils/helpers";

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

const auth = useAuthStore();
const router = useRouter();
const isLoggingOut = ref(false);

provide(injectionKeyIsVerticalNavHovered, isHovered);

const {
  isVerticalNavCollapsed: isCollapsed,
  isLessThanOverlayNavBreakpoint,
  isVerticalNavMini,
  isAppRtl,
} = useLayouts();

// const hideTitleAndIcon = isVerticalNavMini(windowWidth, isHovered);

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

const handleLogout = async () => {
  try {
    isLoggingOut.value = true;
    // First clear the auth store
    auth.logout();
    // Then clear any remaining session data
    authService.logout();
    // Finally redirect to login
    await router.push("/auth/login");
  } catch (error) {
    console.error("Logout error:", error);
    toast("حدث خطأ غير متوقع!", { rtl: true, hideProgressBar: true });
  } finally {
    isLoggingOut.value = false;
  }
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
      name="nav-items position-relative"
      :update-is-vertical-nav-scrolled="updateIsVerticalNavScrolled"
    >
      <PerfectScrollbar
        :key="isAppRtl"
        tag="ul"
        class="nav-items pb-4"
        :options="{ wheelPropagation: false }"
        @ps-scroll-y="handleNavScroll"
      >
        <template v-for="(item, index) in navItems">
          <Component
            :is="resolveNavItemComponent(item)"
            :key="index"
            v-if="
              (typeof item.permission == 'boolean' && item.permission) ||
              hasPermission(item.permission)
            "
            :item="item"
            :class="item.permission"
          />
        </template>
      </PerfectScrollbar>
      <div class="nav-link py-3 px-2 pe-6">
        <v-btn
          @click="handleLogout"
          :loading="isLoggingOut"
          block
          variant="link"
          color="white"
          class="rounded-pill justify-space-between"
        >
          <svg
            width="26"
            height="26"
            viewBox="0 0 26 26"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              opacity="0.4"
              d="M1 13C1 19.6274 6.37258 25 13 25C13.9165 25 14.809 24.8973 15.6667 24.7026V1.29736C14.809 1.10275 13.9165 1 13 1C6.37258 1 1 6.37258 1 13Z"
              fill="white"
            />
            <path
              d="M2 13C2 6.98468 7.22126 2 13.8 2C14.3886 2 14.9665 2.04012 15.531 2.11744C16.0781 2.19239 16.5825 1.80958 16.6574 1.2624C16.7324 0.715225 16.3496 0.210891 15.8024 0.135939C15.1481 0.0463223 14.4795 0 13.8 0C6.24025 0 0 5.76048 0 13C0 20.2395 6.24025 26 13.8 26C14.4795 26 15.1481 25.9537 15.8024 25.8641C16.3496 25.7891 16.7324 25.2848 16.6574 24.7376C16.5825 24.1904 16.0781 23.8076 15.531 23.8826C14.9665 23.9599 14.3886 24 13.8 24C7.22126 24 2 19.0153 2 13Z"
              fill="white"
            />
            <path
              d="M22.3637 8.94959C21.9676 8.56465 21.3345 8.57364 20.9496 8.96968C20.5646 9.36571 20.5736 9.99881 20.9697 10.3837C21.1851 10.5932 21.528 10.8628 21.8543 11.1191L21.9302 11.1787C22.2578 11.436 22.6082 11.711 22.9397 11.9919L22.9492 12H11.6667C11.1144 12 10.6667 12.4477 10.6667 13C10.6667 13.5523 11.1144 14 11.6667 14H22.9492L22.9397 14.0081C22.6082 14.289 22.2579 14.564 21.9303 14.8212L21.8543 14.8809C21.528 15.1372 21.1851 15.4068 20.9697 15.6163C20.5736 16.0012 20.5646 16.6343 20.9496 17.0303C21.3345 17.4264 21.9676 17.4354 22.3637 17.0504C22.4853 16.9321 22.7278 16.738 23.0897 16.4537L23.1696 16.391C23.4924 16.1375 23.8712 15.8403 24.2327 15.534C24.6201 15.2057 25.0225 14.8382 25.3353 14.471C25.4921 14.2869 25.6467 14.0794 25.7661 13.8562C25.8813 13.641 26 13.3444 26 13C26 12.6556 25.8813 12.359 25.7661 12.1438C25.6467 11.9206 25.4921 11.7131 25.3353 11.5291C25.0225 11.1618 24.6201 10.7943 24.2327 10.466C23.8712 10.1597 23.4924 9.86245 23.1696 9.60903L23.0897 9.54628C22.7278 9.26202 22.4853 9.06785 22.3637 8.94959Z"
              fill="white"
            />
          </svg>

          <span class="nav-item-title ms-3 text-base"> تسجيل الخروج </span>
        </v-btn>
      </div>
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
