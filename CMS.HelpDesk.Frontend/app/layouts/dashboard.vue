<template>
  <UDashboardGroup>
    <UDashboardSidebar
      id="default"
      v-model:open="open"
      collapsible
      resizable
      class="bg-elevated/25"
      :ui="{ footer: 'lg:border-t lg:border-default' }"
    >
      <template #header="{ collapsed }">
        <NavLogo :collapsed="collapsed" />
      </template>

      <template #default="{ collapsed }">
        <UDashboardSearchButton :collapsed="collapsed" class="ring-default" />

        <UNavigationMenu
          :collapsed="collapsed"
          :items="links"
          orientation="vertical"
          tooltip
          popover
        />
      </template>

      <template #footer="{ collapsed }">
        <UNavigationMenu
          :collapsed="collapsed"
          :items="bottomLinks"
          orientation="vertical"
          class="w-full"
          tooltip
          popover
        />
      </template>
    </UDashboardSidebar>

    <UDashboardSearch :groups="groups" />

    <slot />
  </UDashboardGroup>
</template>

<script setup lang="ts">
import type { NavigationMenuItem } from "@nuxt/ui";

const open = ref(false);
const route = useRoute();

const links = [
  {
    label: "New Ticket",
    icon: "i-lucide-plus-circle",
    to: "/dashboard/new",
  },
  {
    label: "Dashboard",
    icon: "i-lucide-home",
    to: "/dashboard/home",
  },
  {
    label: "Tickets",
    icon: "i-lucide-ticket",
    to: "/dashboard/tickets",
  },
  {
    label: "Reports",
    icon: "i-lucide-bar-chart-2",
    to: "/dashboard/reports",
  },
] satisfies NavigationMenuItem[];

// groups moved below to merge links and bottomLinks

const auth = useAuth();
if (!auth.data.value?.user) {
  throw new Error("User is not authenticated");
}
const user = auth.data.value.user;

const bottomLinks = [
  {
    label: "Notifications",
    icon: "i-lucide-bell",
    to: "/dashboard/notifications",
  },
  {
    label: user.username,
    icon: "i-lucide-circle-user",
    to: "/dashboard/settings",
  },
] satisfies NavigationMenuItem[];

// Merge both navigation sets for search groups
const groups = computed(() => [
  {
    id: "links",
    label: "Navigate",
    // Combine top and bottom links
    items: [links, bottomLinks].flat(),
  },
  {
    id: "tickets",
    label: "Tickets",
    items: [
      {
        id: "source",
        label: "View page source",
        icon: "i-simple-icons-github",
        to: `https://github.com/nuxt-ui-templates/dashboard/blob/main/app/pages${
          route.path === "/" ? "/index" : route.path
        }.vue`,
      },
    ],
  },
  {
    id: "actions",
    label: "Actions",
    items: [
      {
        id: "refresh",
        label: "Refresh data",
        icon: "i-lucide-refresh-ccw",
        onClick: () => {
          console.log("Refresh action triggered");
        },
      },
    ],
  },
]);
</script>
