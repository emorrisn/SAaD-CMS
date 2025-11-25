<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <h1 class="text-2xl font-semibold">Agent Dashboard</h1>
      <NuxtLink to="/tickets/new">
        <UButton icon="i-heroicons-plus" color="primary">New Ticket</UButton>
      </NuxtLink>
    </div>

    <KpiCards
      :tickets-in-queue="kpis.queue"
      :avg-handle-time="kpis.aht"
      :resolved-today="kpis.resolvedToday"
    />
  </div>
</template>

<script setup lang="ts">
import KpiCards from "~/components/KpiCards.vue";

const kpis = reactive({ queue: 0, aht: "—", resolvedToday: 0 });
</script>

<style scoped></style>
