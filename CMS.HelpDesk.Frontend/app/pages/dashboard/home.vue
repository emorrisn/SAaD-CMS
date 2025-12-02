<template>
  <UDashboardPanel id="home" resizable>
    <template #header>
      <UDashboardNavbar
        title="Home"
        class="bg-elevated/25"
        :ui="{ right: 'gap-3' }"
      >
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
        <template #right>
          <UButton
            variant="ghost"
            color="neutral"
            class="ring-default"
            aria-label="Refresh"
            :disabled="refreshing"
            @click="onRefresh"
          >
            <UIcon
              name="i-lucide-refresh-ccw"
              :class="refreshing ? 'animate-spin size-5' : 'size-5'"
            />
          </UButton>
          <HomeDateRangePicker v-model="range" class="-ms-1" />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <HomeStats :range="range" />
      <HomeTickets :range="range" />
    </template>
  </UDashboardPanel>
</template>

<script setup lang="ts">
import { sub } from "date-fns";

const { status, data } = useAuth();

const range = shallowRef<{
  start: Date;
  end: Date;
}>({
  start: sub(new Date(), { days: 32 }),
  end: new Date(),
});

definePageMeta({
  layout: "dashboard",
  auth: true,
});

onMounted(() => {
  console.log("Auth Status:", status.value);
  console.log("User Data:", data.value);
});

const refreshing = ref(false);

async function onRefresh() {
  // Simulate a brief refresh; icon spins during this time
  if (refreshing.value) return;
  refreshing.value = true;
  try {
    await new Promise((r) => setTimeout(r, 600));
  } finally {
    refreshing.value = false;
  }
}
</script>
