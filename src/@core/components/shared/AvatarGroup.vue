<script setup>
const props = defineProps({
  avatars: [],
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
        size="45"
        style="border: 4px solid #fff"
        :class="{ 'ms-n3': index > 0 }"
      >
        <v-tooltip activator="parent" location="top">{{ item.name }}</v-tooltip>
        <v-img :alt="item.name" :src="item.avatar"></v-img>
      </v-avatar>
    </template>
    <v-avatar
      v-if="avatars?.length > 4"
      size="45"
      color="gray"
      style="border: 4px solid #fff"
      class="ms-n3"
    >
      <span class="text-h6 text-primary">
        <span>{{ avatars?.length - 4 }}</span>
        <span>+</span>
      </span>
    </v-avatar>
  </div>
</template>
