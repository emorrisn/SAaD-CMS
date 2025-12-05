<script setup lang="ts">
import type { NavigationMenuItem } from "@nuxt/ui";

const links = [
  [
    {
      label: "General",
      icon: "i-lucide-user",
      to: "/dashboard/settings",
      exact: true,
    },
    {
      label: "Notifications",
      icon: "i-lucide-bell",
      to: "/dashboard/settings/notifications",
    },
    {
      label: "Security",
      icon: "i-lucide-shield",
      to: "/dashboard/settings/security",
    },
  ],
] satisfies NavigationMenuItem[][];
</script>

<template>
  <UDashboardPanel id="settings" :ui="{ body: 'lg:py-12' }">
    <template #header>
      <UDashboardNavbar title="Settings">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>

      <UDashboardToolbar>
        <UNavigationMenu :items="links" highlight class="-mx-1 flex-1" />
      </UDashboardToolbar>
    </template>

    <template #body>
      <div
        class="flex flex-col gap-4 sm:gap-6 lg:gap-12 w-full lg:max-w-2xl mx-auto"
      >
        <NuxtPage />
      </div>
    </template>

    <template #footer>
      <UDashboardToolbar
        class="h-(--ui-header-height) justify-start border-t border-default shrink-0 flex items-center border-b px-4 sm:px-6 gap-1.5 bg-elevated/25"
      >
        <UButton color="primary" class="cursor-pointer">
          <Icon name="i-lucide-refresh-cw" /> Sync Changes
        </UButton>
        <UButton color="neutral" class="cursor-pointer">
          <Icon name="i-lucide-x" /> Cancel Changes
        </UButton>
      </UDashboardToolbar>
    </template>
  </UDashboardPanel>
</template>
