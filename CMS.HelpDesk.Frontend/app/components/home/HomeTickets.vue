<script setup lang="ts">
import { h, resolveComponent } from "vue";
import type { TableColumn } from "@nuxt/ui";
import { storeToRefs } from "pinia";
import { useTicketsStore } from "../../../stores/tickets";
import type {
  Ticket,
  TicketSummary,
  TicketPriority,
  TicketStatus,
} from "../../types";

const UBadge = resolveComponent("UBadge");

// Priority sort weights: high -> medium -> low
const priorityWeight: Record<TicketPriority, number> = {
  urgent: -1,
  high: 0,
  medium: 1,
  low: 2,
  unknown: 3,
};

const ticketsStore = useTicketsStore();
const { items: tickets } = storeToRefs(ticketsStore);

// Map store tickets to a compact summary table
const data = computed<TicketSummary[]>(() =>
  tickets.value.map((t: Ticket) => ({
    id: `#${t.id}`,
    subject: t.subject,
    customer: {
      firstName: t.requester.firstName,
      lastName: t.requester.lastName,
    },
    status: t.status,
    priority: t.priority,
    createdAt: t.createdAt,
  }))
);

// Sorted view: by priority (high/urgent first), then by createdAt desc
const sortedData = computed(() =>
  [...data.value].sort((a, b) => {
    const pw = priorityWeight[a.priority] - priorityWeight[b.priority];
    if (!Number.isNaN(pw) && pw !== 0) return pw;
    return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
  })
);

const columns: TableColumn<TicketSummary>[] = [
  {
    accessorKey: "id",
    header: "ID",
    cell: ({ row }) => String(row.getValue("id")),
  },
  {
    accessorKey: "subject",
    header: "Subject",
    cell: ({ row }) => String(row.getValue("subject")),
  },
  {
    accessorKey: "customer",
    header: "Customer",
    cell: ({ row }) => {
      const c = row.getValue("customer") as TicketSummary["customer"];
      return `${c.firstName} ${c.lastName}`;
    },
  },
  {
    accessorKey: "status",
    header: "Status",
    cell: ({ row }) => {
      const color = {
        open: "warning" as const,
        pending: "info" as const,
        resolved: "success" as const,
        closed: "neutral" as const,
      }[row.getValue("status") as string];
      return h(UBadge, { class: "capitalize", variant: "subtle", color }, () =>
        String(row.getValue("status"))
      );
    },
  },
  {
    accessorKey: "priority",
    header: "Priority",
    cell: ({ row }) => {
      const val = String(row.getValue("priority"));
      const color =
        (
          {
            low: "neutral",
            medium: "info",
            high: "warning",
            urgent: "error",
            unknown: "neutral",
          } as const
        )[val as keyof typeof priorityWeight] || "neutral";
      return h(
        UBadge,
        { class: "capitalize", variant: "subtle", color },
        () => val
      );
    },
  },
  {
    accessorKey: "createdAt",
    header: "Created At",
    cell: ({ row }) => {
      const dt = new Date(String(row.getValue("createdAt")));
      const diffMs = Date.now() - dt.getTime();
      const sec = Math.floor(diffMs / 1000);
      const min = Math.floor(sec / 60);
      const hrs = Math.floor(min / 60);
      const days = Math.floor(hrs / 24);
      if (days > 0) return `${days} day${days > 1 ? "s" : ""} ago`;
      if (hrs > 0) return `${hrs} hour${hrs > 1 ? "s" : ""} ago`;
      if (min > 0) return `${min} minute${min > 1 ? "s" : ""} ago`;
      return `${sec} second${sec !== 1 ? "s" : ""} ago`;
    },
  },
];
</script>

<template>
  <UTable
    :data="sortedData"
    :columns="columns"
    class="shrink-0"
    :ui="{
      base: 'table-fixed border-separate border-spacing-0',
      thead: '[&>tr]:bg-elevated/50 [&>tr]:after:content-none',
      tbody:
        '[&>tr]:last:[&>td]:border-b-0 [&>tr:hover]:bg-elevated/60 [&>tr]:cursor-pointer',
      th: 'first:rounded-l-lg last:rounded-r-lg border-y border-default first:border-l last:border-r',
      td: 'border-b border-default',
    }"
  />
</template>
