<template>
  <span class="p-1 mr-2 bg-primary/20 rounded text-primary flex">
    <Icon name="fe:headphone" size="25" class="mb-0" />
  </span>
  <NuxtLink
    :to="auth.data.value?.user ? '/dashboard/home' : '/'"
    class="font-bold"
  >
    <span v-if="!collapsed">Help Desk</span>
  </NuxtLink>
</template>

<script setup lang="ts">
const auth = useAuth();

defineProps({
  collapsed: {
    type: Boolean,
    required: false,
    default: false,
  },
});
</script>
