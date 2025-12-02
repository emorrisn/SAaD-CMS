<script setup lang="ts">
import { h, resolveComponent } from "vue";
import type { TableColumn } from "@nuxt/ui";

interface Ticket {
  id: string;
  createdAt: string; // ISO string
  status: "open" | "in_progress" | "resolved" | "closed";
  requesterEmail: string;
  subject: string;
  priority: "low" | "medium" | "high";
}

const UBadge = resolveComponent("UBadge");

// Priority sort weights: high -> medium -> low
const priorityWeight: Record<Ticket["priority"], number> = {
  high: 0,
  medium: 1,
  low: 2,
};

// Sorted view: by priority (high first), then by createdAt desc
const sortedData = computed(() =>
  [...data].sort((a, b) => {
    const pw = priorityWeight[a.priority] - priorityWeight[b.priority];
    if (pw !== 0) return pw;
    return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
  })
);

// Hardcoded latest tickets (no randoms)
const data: Ticket[] = [
  {
    id: "#1025",
    createdAt: new Date().toISOString(),
    status: "open",
    requesterEmail: "alice.morgan@example.com",
    subject: "Unable to login to portal",
    priority: "high",
  },
  {
    id: "#1024",
    createdAt: new Date(Date.now() - 2 * 3600_000).toISOString(),
    status: "in_progress",
    requesterEmail: "ben.turner@example.com",
    subject: "Error 500 on reports page",
    priority: "medium",
  },
  {
    id: "#1023",
    createdAt: new Date(Date.now() - 6 * 3600_000).toISOString(),
    status: "resolved",
    requesterEmail: "chloe.smith@example.com",
    subject: "Password reset issue",
    priority: "low",
  },
  {
    id: "#1022",
    createdAt: new Date(Date.now() - 12 * 3600_000).toISOString(),
    status: "closed",
    requesterEmail: "david.lee@example.com",
    subject: "Billing discrepancy",
    priority: "medium",
  },
  {
    id: "#1021",
    createdAt: new Date(Date.now() - 24 * 3600_000).toISOString(),
    status: "open",
    requesterEmail: "eva.jones@example.com",
    subject: "Feature request: export to CSV",
    priority: "low",
  },
];

const columns: TableColumn<Ticket>[] = [
  {
    accessorKey: "id",
    header: "Ticket",
    cell: ({ row }) => {
      const rawId = String(row.getValue("id"));
      const id = rawId.replace(/^#/, "");
      const NuxtLink = resolveComponent("NuxtLink");
      return h(
        NuxtLink,
        {
          to: `/dashboard/ticket/${id}`,
          class: "text-primary hover:underline",
        },
        () => rawId
      );
    },
  },
  {
    accessorKey: "createdAt",
    header: "Created",
    cell: ({ row }) => {
      return new Date(row.getValue("createdAt")).toLocaleString("en-GB", {
        day: "numeric",
        month: "short",
        hour: "2-digit",
        minute: "2-digit",
        hour12: false,
      });
    },
  },
  {
    accessorKey: "status",
    header: "Status",
    cell: ({ row }) => {
      const color = {
        open: "warning" as const,
        in_progress: "info" as const,
        resolved: "success" as const,
        closed: "neutral" as const,
      }[row.getValue("status") as string];

      return h(UBadge, { class: "capitalize", variant: "subtle", color }, () =>
        String(row.getValue("status")).replace("_", " ")
      );
    },
  },
  {
    accessorKey: "subject",
    header: "Subject",
    cell: ({ row }) => {
      const id = String(row.getValue("id")).replace(/^#/, "");
      const NuxtLink = resolveComponent("NuxtLink");
      return h(
        NuxtLink,
        {
          to: `/dashboard/ticket/${id}`,
          class: "text-primary hover:underline",
        },
        () => row.getValue("subject")
      );
    },
  },
  {
    accessorKey: "requesterEmail",
    header: "Requester",
  },
  {
    accessorKey: "priority",
    header: () => h("div", { class: "text-right" }, "Priority"),
    cell: ({ row }) => {
      return h(
        "div",
        { class: "text-right font-medium capitalize" },
        row.getValue("priority")
      );
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
