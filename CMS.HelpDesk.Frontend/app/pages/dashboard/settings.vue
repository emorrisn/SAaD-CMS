<template>
  <UDashboardPanel id="account" resizable>
    <template #header>
      <UDashboardNavbar
        title="Settings"
        class="bg-elevated/25"
        :ui="{ right: 'gap-3' }"
      >
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <UButton color="primary" @click="handleLogout"> Logout </UButton>
    </template>
  </UDashboardPanel>
</template>

<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth: true,
});

const { signOut } = useAuth();

const handleLogout = async () => {
  await signOut({ callbackUrl: "/" });
};
</script>
