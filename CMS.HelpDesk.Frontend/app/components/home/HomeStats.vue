<script setup lang="ts">
import { computed } from "vue";
import { storeToRefs } from "pinia";
import { useCustomersStore } from "../../../stores/customers";
import { useStatisticsStore } from "../../../stores/statistics";
import { useTicketsStore } from "../../../stores/tickets";
import type { HomeStat, DateRange } from "../../types";

const props = defineProps<{
  range: DateRange;
}>();

function formatCurrency(value: number | string): string {
  const numeric = typeof value === "number" ? value : Number(value);
  return numeric.toLocaleString("en-GB", {
    style: "currency",
    currency: "GBP",
    maximumFractionDigits: 0,
  });
}

function formatNumber(value: number | string): string {
  const numeric = typeof value === "number" ? value : Number(value);
  return numeric.toLocaleString("en-GB");
}

function formatTime(value: number | string): string {
  const numeric = typeof value === "number" ? value : Number(value);
  const minutes = Math.floor(numeric / 60);
  const seconds = numeric % 60;
  return `${minutes}m ${seconds}s`;
}
const statisticsStore = useStatisticsStore();
const customersStore = useCustomersStore();
const ticketsStore = useTicketsStore();

const { summary, trends } = storeToRefs(statisticsStore);
const { items: customers } = storeToRefs(customersStore);
const { items: tickets } = storeToRefs(ticketsStore);

function percentChange(current: number, previous: number): number {
  if (!previous) return 0;
  return Math.round(((current - previous) / previous) * 100);
}

const latestTrend = computed(() => trends.value.at(-1)?.value ?? 0);
const previousTrend = computed(
  () => trends.value.at(-2)?.value ?? latestTrend.value
);
const volumeVariation = computed(() =>
  percentChange(latestTrend.value, previousTrend.value)
);

const stats = computed<HomeStat[]>(() => {
  const resolved = summary.value.resolved ?? 0;
  const avgResponseSeconds = summary.value.avgResponseSeconds ?? 0;
  const resolutionChange = percentChange(
    resolved,
    previousTrend.value || resolved
  );

  return [
    {
      title: "Customers",
      icon: "i-lucide-users",
      value: customers.value.length,
      variation: volumeVariation.value,
      formatter: formatNumber,
    },
    {
      title: "Tickets",
      icon: "i-lucide-chart-pie",
      value: tickets.value.length,
      variation: volumeVariation.value,
      link: "/dashboard/tickets",
      formatter: formatNumber,
    },
    {
      title: "Average Response Time",
      icon: "i-lucide-clock",
      value: avgResponseSeconds,
      variation: -volumeVariation.value,
      formatter: formatTime,
    },
    {
      title: "Resolved",
      icon: "i-lucide-check-circle",
      value: resolved,
      variation: resolutionChange,
      formatter: formatNumber,
    },
  ];
});
</script>

<template>
  <UPageGrid class="lg:grid-cols-4 gap-4 sm:gap-6 lg:gap-px">
    <UPageCard
      v-for="(stat, index) in stats"
      :key="index"
      :icon="stat.icon"
      :title="stat.title"
      :to="stat.link ?? '/dashboard/reports'"
      variant="subtle"
      :ui="{
        container: 'gap-y-1.5',
        wrapper: 'items-start',
        leading:
          'p-2.5 rounded-full bg-primary/10 ring ring-inset ring-primary/25 flex-col',
        title: 'font-normal text-muted text-xs uppercase',
      }"
      class="lg:rounded-none first:rounded-l-lg last:rounded-r-lg hover:z-1"
    >
      <div class="flex items-center gap-2">
        <span class="text-2xl font-semibold text-highlighted">
          {{ stat.value }}
        </span>

        <UBadge
          :color="stat.variation > 0 ? 'success' : 'error'"
          variant="subtle"
          class="text-xs"
        >
          {{ stat.variation > 0 ? "+" : "" }}{{ stat.variation }}%
        </UBadge>
      </div>
    </UPageCard>
  </UPageGrid>
</template>
