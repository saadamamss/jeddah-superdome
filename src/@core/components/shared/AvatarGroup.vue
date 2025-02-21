<script setup>
const props = defineProps({
  avatars: [],
  size: {
    type: String,
    default: "45",
  },
});

const visibleItems = computed(() => {
  if (props.avatars?.length > 4) {
    return props.avatars.slice(0, 4);
  }

  return props.avatars;
});
</script>

<template>
  <div class="d-flex pa-2 rounded-pill bg-background">
    <template v-for="(item, index) in visibleItems" :key="index">
      <v-avatar
        :size="size"
        style="border: 4px solid #fff"
        :class="{ 'ms-n3': index > 0 }"
      >
        <v-tooltip activator="parent" location="top">{{ item.name }}</v-tooltip>
        <v-img :alt="item.name" :src="item.avatar"></v-img>
      </v-avatar>
    </template>
    <v-avatar
      v-if="avatars?.length > 4"
      :size="size"
      color="gray"
      style="border: 4px solid #fff"
      class="ms-n3"
    >
      <span class="text-primary">
        <span>{{ avatars?.length - 4 }}</span>
        <span>+</span>
      </span>
    </v-avatar>
  </div>
</template>
