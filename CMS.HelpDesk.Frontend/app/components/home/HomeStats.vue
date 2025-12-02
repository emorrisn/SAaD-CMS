<script setup lang="ts">
interface Range {
  start: Date;
  end: Date;
}

interface Stat {
  title: string;
  icon: string;
  value: number | string;
  variation: number;
  link?: string;
  formatter?: (value: number) => string;
}

const props = defineProps<{
  range: Range;
}>();

function formatCurrency(value: number): string {
  return value.toLocaleString("en-GB", {
    style: "currency",
    currency: "GBP",
    maximumFractionDigits: 0,
  });
}

function formatNumber(value: number): string {
  return value.toLocaleString("en-GB");
}

function formatTime(value: number): string {
  const minutes = Math.floor(value / 60);
  const seconds = value % 60;
  return `${minutes}m ${seconds}s`;
}

const baseStats = [
  {
    title: "Customers",
    icon: "i-lucide-users",
    value: 3500,
    variation: 3000,
    formatter: formatNumber,
  },
  {
    title: "Tickets",
    icon: "i-lucide-chart-pie",
    value: 1200,
    variation: 8,
    link: "/dashboard/tickets",
    formatter: formatNumber,
  },
  {
    title: "Average Response Time",
    icon: "i-lucide-clock",
    value: 3000,
    variation: -12,
    formatter: formatTime,
  },
  {
    title: "Resolved",
    icon: "i-lucide-check-circle",
    value: 750,
    variation: 15,
    formatter: formatNumber,
  },
];

const { data: stats } = await useAsyncData<Stat[]>(
  "stats",
  async () => {
    return baseStats.map((stat) => {
      const value = stat.value;

      return {
        title: stat.title,
        icon: stat.icon,
        value: stat.formatter ? stat.formatter(value) : value,
        variation: stat.variation,
        link: stat.link,
      };
    });
  },
  {
    watch: [() => props.range],
    default: () => [],
  }
);
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
