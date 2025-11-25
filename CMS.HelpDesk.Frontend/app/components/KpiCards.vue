<template>
  <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
    <UCard>
      <div class="text-sm text-gray-500">Tickets in Queue</div>
      <div class="text-3xl font-semibold">{{ ticketsInQueue }}</div>
    </UCard>
    <UCard>
      <div class="text-sm text-gray-500">Avg Handle Time</div>
      <div class="text-3xl font-semibold">{{ avgHandleTime }}</div>
    </UCard>
    <UCard>
      <div class="text-sm text-gray-500">Resolved Today</div>
      <div class="text-3xl font-semibold">{{ resolvedToday }}</div>
    </UCard>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  ticketsInQueue: number;
  avgHandleTime: string;
  resolvedToday: number;
}>();
</script>
