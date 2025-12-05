<script setup lang="ts">
import { format } from "date-fns";
import type { InboxNotification } from "../../types";

defineProps<{
  notification: InboxNotification;
}>();

const emits = defineEmits(["close"]);

const dropdownItems = [
  [
    {
      label: "Mark as unread",
      icon: "i-lucide-check-circle",
    },
    {
      label: "Mark as important",
      icon: "i-lucide-triangle-alert",
    },
  ],
  [
    {
      label: "Star notification",
      icon: "i-lucide-star",
    },
    {
      label: "Mute notifications like this",
      icon: "i-lucide-circle-pause",
    },
  ],
];
</script>

<template>
  <UDashboardPanel
    id="inbox-item"
    :default-size="50"
    :min-size="40"
    :max-size="70"
    class="grow flex flex-col"
  >
    <UDashboardNavbar :title="notification.title" :toggle="false">
      <template #leading>
        <UButton
          icon="i-lucide-x"
          color="neutral"
          variant="ghost"
          class="-ms-1.5"
          @click="emits('close')"
        />
      </template>

      <template #right>
        <UDropdownMenu :items="dropdownItems">
          <UButton
            icon="i-lucide-ellipsis-vertical"
            color="neutral"
            variant="ghost"
          />
        </UDropdownMenu>
      </template>
    </UDashboardNavbar>

    <div
      class="flex flex-col sm:flex-row justify-between gap-1 p-4 sm:px-6 border-b border-default"
    >
      <div class="flex items-start gap-4 sm:my-1.5">
        <div class="min-w-0">
          <p class="font-semibold text-highlighted">
            {{ notification.sender.name }}
          </p>
          <p class="text-muted">
            {{ notification.sender.email }}
          </p>
        </div>
      </div>

      <p class="max-sm:pl-16 text-muted text-sm sm:mt-2">
        {{ format(new Date(notification.createdAt), "dd MMM HH:mm") }}
      </p>
    </div>

    <div class="flex-1 p-4 sm:p-6 overflow-y-auto">
      <p class="whitespace-pre-wrap">
        {{ notification.message }}
      </p>
    </div>
  </UDashboardPanel>
</template>
