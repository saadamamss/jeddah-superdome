<script setup lang="ts">
interface AvatarItem {
  name: string
  avatar?: string
}

const props = defineProps<{
  avatars?: AvatarItem[]
  size?: string
  maxAvatars?: number
}>()

const fallbackAvatar = 'https://placehold.co/400'

const visibleItems = computed(() => {
  if (props.avatars && props.avatars.length > (props.maxAvatars ?? 4)) {
    return props.avatars.slice(0, props.maxAvatars ?? 4)
  }

  return props.avatars ?? []
})

const handleImageError = () => {
  // Image error handled by v-img fallback
}
</script>

<template>
  <div class="d-flex pa-2 rounded-pill bg-background align-center gap-2">
    <template v-for="(item, index) in visibleItems" :key="index">
      <v-avatar :class="{ 'ms-n3': index > 0 }" :size="size ?? '45'" style="border: 4px solid #fff">
        <v-tooltip activator="parent" location="top">{{ item.name }}</v-tooltip>
        <v-img :alt="item.name" :src="item.avatar || fallbackAvatar" @error="handleImageError" />
      </v-avatar>
    </template>
    <v-avatar
      v-if="avatars && avatars.length > (maxAvatars ?? 4)"
      class="ms-n3"
      color="gray"
      :size="size ?? '45'"
      style="border: 4px solid #fff"
    >
      <span class="text-primary">
        <span>{{ (avatars?.length ?? 0) - (maxAvatars ?? 4) }}</span>
        <span>+</span>
      </span>
    </v-avatar>

    <slot name="additionData" />
  </div>
</template>
