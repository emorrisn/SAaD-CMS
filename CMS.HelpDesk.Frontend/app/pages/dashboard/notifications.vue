<template>
  <UDashboardPanel id="notifications" resizable>
    <template #header>
      <UDashboardNavbar
        title="Notifications"
        class="bg-elevated/25"
        :ui="{ right: 'gap-3' }"
      >
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body> aa </template>
  </UDashboardPanel>
</template>

<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth: true,
});
</script>
