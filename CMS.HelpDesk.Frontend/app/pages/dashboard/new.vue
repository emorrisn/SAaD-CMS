<template>
  <UDashboardPanel id="new-ticket" resizable>
    <template #header>
      <UDashboardNavbar
        title="New Ticket"
        class="bg-elevated/25"
        :ui="{ right: 'gap-3' }"
      >
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body> aaa </template>

    <template #footer>
      <UDashboardToolbar
        class="h-(--ui-header-height) justify-end border-t border-default shrink-0 flex items-center border-b px-4 sm:px-6 gap-1.5 bg-elevated/25"
      >
        <UButton color="secondary"
          ><Icon name="i-lucide-save" /> Save Draft
        </UButton>
        <UButton color="primary"
          ><Icon name="i-lucide-send" /> Submit Ticket</UButton
        >
      </UDashboardToolbar>
    </template>
  </UDashboardPanel>
</template>

<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth: true,
});
</script>
