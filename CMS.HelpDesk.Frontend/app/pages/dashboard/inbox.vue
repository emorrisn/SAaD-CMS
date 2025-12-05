<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { breakpointsTailwind, useBreakpoints } from "@vueuse/core";
import type { InboxNotification } from "../../types";

definePageMeta({
  layout: "dashboard",
  auth: true,
});

useSeoMeta({
  title: "Inbox",
});

const tabItems = [
  {
    label: "All",
    value: "all",
  },
  {
    label: "Unread",
    value: "unread",
  },
];

const selectedTab = ref("all");

const notifications = ref<InboxNotification[]>([
  {
    id: 0,
    unread: true,
    sender: {
      id: 1,
      name: "Support",
      email: "support@example.com",
      location: "Global",
    },
    title: "Ticket #1042 moved to In Progress",
    message:
      "Your ticket about the damaged order has been picked up by an agent and is now being worked on.",
    createdAt: "2024-06-20T10:15:00Z",
  },
  {
    id: 1,
    sender: {
      id: 2,
      name: "Reports",
      email: "reports@example.com",
      location: "Global",
    },
    title: "Monthly performance report is ready",
    message:
      "Your requested performance report has finished generating and is ready to download.",
    createdAt: "2024-06-20T09:05:00Z",
  },
  {
    id: 2,
    sender: {
      id: 3,
      name: "Account",
      email: "account@example.com",
      location: "Global",
    },
    title: "Account permissions updated",
    message:
      "Your account roles were updated to include Manager access. Please review your permissions.",
    createdAt: "2024-06-19T14:30:00Z",
  },
]);

// Filter notifications based on the selected tab
const filteredNotifications = computed<InboxNotification[]>(() => {
  if (selectedTab.value === "unread") {
    return notifications.value.filter(
      (notification: InboxNotification) => !!notification.unread
    );
  }

  return notifications.value;
});

const selectedNotification = ref<InboxNotification | null>();

const isNotificationPanelOpen = computed({
  get() {
    return !!selectedNotification.value;
  },
  set(value: boolean) {
    if (!value) {
      selectedNotification.value = null;
    }
  },
});

// Reset selected notification if it's not in the filtered list
watch(filteredNotifications, () => {
  if (
    !filteredNotifications.value.find(
      (notification: InboxNotification) =>
        notification.id === selectedNotification.value?.id
    )
  ) {
    selectedNotification.value = null;
  }
});

const breakpoints = useBreakpoints(breakpointsTailwind);
const isMobile = breakpoints.smaller("lg");
</script>

<template>
  <div class="flex w-full flex-1">
    <UDashboardPanel
      id="inbox"
      :default-size="25"
      :min-size="20"
      :max-size="30"
      resizable
    >
      <UDashboardNavbar title="Inbox">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
        <template #trailing>
          <UBadge :label="filteredNotifications.length" variant="subtle" />
        </template>

        <template #right>
          <UTabs
            v-model="selectedTab"
            :items="tabItems"
            :content="false"
            size="xs"
          />
        </template>
      </UDashboardNavbar>
      <InboxList
        v-model="selectedNotification"
        :notifications="filteredNotifications"
      />
    </UDashboardPanel>

    <InboxItem
      v-if="selectedNotification"
      :notification="selectedNotification"
      @close="selectedNotification = null"
    />
    <div v-else class="hidden lg:flex flex-1 items-center justify-center">
      <UIcon name="i-lucide-inbox" class="size-32 text-dimmed" />
    </div>

    <ClientOnly>
      <USlideover v-if="isMobile" v-model:open="isNotificationPanelOpen">
        <template #content>
          <InboxItem
            v-if="selectedNotification"
            :notification="selectedNotification"
            @close="selectedNotification = null"
          />
        </template>
      </USlideover>
    </ClientOnly>
  </div>
</template>
