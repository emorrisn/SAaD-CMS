<template>
  <div>
    <UHeader>
      <template #left>
        <NavLogo />
      </template>
      <template #right>
        <UNavigationMenu
          class="hidden sm:flex"
          :items="getMenuItems('horizontal')"
        />
        <UColorModeButton variant="link" style="cursor: pointer" />
      </template>
      <template #body>
        <UNavigationMenu
          :items="getMenuItems('vertical')"
          orientation="vertical"
          class="sm:hidden"
        />
      </template>
    </UHeader>
    <slot />
  </div>
</template>

<script setup lang="ts">
import type { NavigationMenuItem } from "@nuxt/ui";

const route = useRoute();
const auth = useAuth();

const getMenuItems = (
  orientation: "vertical" | "horizontal"
): NavigationMenuItem[] => {
  const menu: NavigationMenuItem[] = [];
  const isHamburger = orientation === "vertical";

  if (auth.data.value?.user) {
    const user = auth.data.value.user;

    menu.push({
      label: user.username,
      to: "/dashboard/settings",
      icon: "i-lucide-circle-user",
      active: route.path.startsWith("/dashboard/settings"),
    });

    menu.push({
      label: isHamburger ? "Inbox" : "",
      to: "/dashboard/inbox",
      icon: "i-lucide-bell",
      active: route.path.startsWith("/dashboard/inbox"),
    });
  } else {
    menu.push({
      label: "Account",
      to: "/login",
      icon: "i-lucide-circle-user",
      active: route.path.startsWith("/login"),
    });
  }

  return menu;
};
</script>
