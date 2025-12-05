<script setup lang="ts">
import { format, isToday } from "date-fns";
import type { InboxNotification } from "../../types";

const props = defineProps<{
  notifications: InboxNotification[];
}>();

const notificationRefs = ref<Element[]>([]);

const selectedNotification = defineModel<InboxNotification | null>();

watch(selectedNotification, () => {
  if (!selectedNotification.value) {
    return;
  }
  const ref = notificationRefs.value[selectedNotification.value.id];
  if (ref) {
    ref.scrollIntoView({ block: "nearest" });
  }
});

defineShortcuts({
  arrowdown: () => {
    const index = props.notifications.findIndex(
      (notification) => notification.id === selectedNotification.value?.id
    );

    if (index === -1) {
      selectedNotification.value = props.notifications[0];
    } else if (index < props.notifications.length - 1) {
      selectedNotification.value = props.notifications[index + 1];
    }
  },
  arrowup: () => {
    const index = props.notifications.findIndex(
      (notification) => notification.id === selectedNotification.value?.id
    );

    if (index === -1) {
      selectedNotification.value =
        props.notifications[props.notifications.length - 1];
    } else if (index > 0) {
      selectedNotification.value = props.notifications[index - 1];
    }
  },
});
</script>

<template>
  <div class="overflow-y-auto divide-y divide-default">
    <div
      v-for="(notification, index) in notifications"
      :key="index"
      :ref="el => { notificationRefs[notification.id] = el as Element }"
    >
      <div
        class="p-4 sm:px-6 text-sm cursor-pointer border-l-2 transition-colors"
        :class="[
          notification.unread ? 'text-highlighted' : 'text-toned',
          selectedNotification && selectedNotification.id === notification.id
            ? 'border-primary bg-primary/10'
            : 'border-bg hover:border-primary hover:bg-primary/5',
        ]"
        @click="selectedNotification = notification"
      >
        <div
          class="flex items-center justify-between"
          :class="[notification.unread && 'font-semibold']"
        >
          <div class="flex items-center gap-3">
            <span class="font-medium">{{ notification.sender.name }}</span>

            <UChip v-if="notification.unread" />
          </div>

          <span>{{
            isToday(new Date(notification.createdAt))
              ? format(new Date(notification.createdAt), "HH:mm")
              : format(new Date(notification.createdAt), "dd MMM")
          }}</span>
        </div>
        <p class="truncate" :class="[notification.unread && 'font-semibold']">
          {{ notification.title }}
        </p>
        <p class="text-dimmed line-clamp-1">
          {{ notification.message }}
        </p>
      </div>
    </div>
  </div>
</template>
