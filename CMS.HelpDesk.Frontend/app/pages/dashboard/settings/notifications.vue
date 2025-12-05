<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth: true,
});

useSeoMeta({
  title: "Notification Settings",
});

const state = reactive<{ [key: string]: boolean }>({
  email: true,
  sms: false,
  desktop: true,
  new_ticket_assigned: true,
  ticket_status_updates: true,
  customer_replies: true,
  sla_breaches: true,
  weekly_summary: false,
});

const sections = [
  {
    title: "Notification channels",
    description: "Where can we notify you?",
    fields: [
      {
        name: "email",
        label: "Email",
        description: "Receive a daily email digest.",
      },
      {
        name: "sms",
        label: "SMS",
        description: "Receive SMS notifications.",
      },
      {
        name: "desktop",
        label: "Desktop",
        description: "Receive desktop notifications.",
      },
    ],
  },
  {
    title: "Notification types",
    description: "What kind of notifications would you like to receive?",
    fields: [
      {
        name: "new_ticket_assigned",
        label: "New ticket assigned",
        description: "Notify me when a new ticket is assigned to me.",
      },
      {
        name: "ticket_status_updates",
        label: "Ticket status updates",
        description: "Notify me when the status of my tickets changes.",
      },
      {
        name: "customer_replies",
        label: "Customer replies",
        description: "Notify me when a customer replies to a ticket I own.",
      },
      {
        name: "sla_breaches",
        label: "SLA breaches",
        description:
          "Notify me when a ticket I own is close to or has breached SLA.",
      },
      {
        name: "weekly_summary",
        label: "Weekly summary",
        description: "Send me a weekly summary of my ticket activity.",
      },
    ],
  },
];

async function onChange() {
  // Do something with data
  console.log(state);
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <div v-for="(section, index) in sections" :key="index">
      <UPageCard
        :title="section.title"
        :description="section.description"
        variant="naked"
        class="mb-4"
      />

      <UPageCard
        variant="subtle"
        :ui="{ container: 'divide-y divide-default' }"
      >
        <UFormField
          v-for="field in section.fields"
          :key="field.name"
          :name="field.name"
          :label="field.label"
          :description="field.description"
          class="flex items-center justify-between gap-4 last:pb-0 pb-4"
        >
          <USwitch v-model="state[field.name]" @update:model-value="onChange" />
        </UFormField>
      </UPageCard>
    </div>
  </div>
</template>
