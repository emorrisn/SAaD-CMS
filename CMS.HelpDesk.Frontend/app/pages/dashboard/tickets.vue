<script setup lang="ts">
import type { TableColumn } from "@nuxt/ui";
// removed unused upperFirst
import { getPaginationRowModel } from "@tanstack/table-core";
import type { Row } from "@tanstack/table-core";
import { storeToRefs } from "pinia";
import { useTicketsStore } from "../../../stores/tickets";
import type { Ticket, TicketPriority, TicketStatus } from "../../types";

definePageMeta({
  layout: "dashboard",
  auth: true,
});

useSeoMeta({
  title: "Tickets",
});

const UButton = resolveComponent("UButton");
const UBadge = resolveComponent("UBadge");
const UDropdownMenu = resolveComponent("UDropdownMenu");
const UCheckbox = resolveComponent("UCheckbox");

const toast = useToast();
const table = useTemplateRef("table");

function formatColumnLabel(id: string) {
  const pretty = id.replace(/[._]/g, " ");
  return pretty
    .split(" ")
    .filter(Boolean)
    .map((w) => w.charAt(0).toUpperCase() + w.slice(1))
    .join(" ");
}

const columnFilters = ref([]);
const columnVisibility = ref({
  "requester.email": false,
  updatedAt: false,
});
const rowSelection = ref<Record<string | number, boolean>>({});

const status = ref<"idle" | "pending" | "success" | "error">("idle");

const ticketsStore = useTicketsStore();
const { items: data } = storeToRefs(ticketsStore);

function timeAgo(iso: string) {
  const dt = new Date(iso);
  const now = new Date();
  const diffMs = now.getTime() - dt.getTime();
  const sec = Math.floor(diffMs / 1000);
  const min = Math.floor(sec / 60);
  const hrs = Math.floor(min / 60);
  const days = Math.floor(hrs / 24);
  if (days > 0) return `${days} day${days > 1 ? "s" : ""} ago`;
  if (hrs > 0) return `${hrs} hour${hrs > 1 ? "s" : ""} ago`;
  if (min > 0) return `${min} minute${min > 1 ? "s" : ""} ago`;
  return `${sec} second${sec !== 1 ? "s" : ""} ago`;
}

function getRowItems(row: Row<Ticket>) {
  return [
    {
      type: "label",
      label: "Ticket Actions",
    },
    {
      label: "Copy ticket ID",
      icon: "i-lucide-copy",
      onSelect() {
        navigator.clipboard.writeText(row.original.id.toString());
        toast.add({
          title: "Copied to clipboard",
          description: "Ticket ID copied to clipboard",
        });
      },
    },
    {
      type: "separator",
    },
    {
      label: "View ticket",
      icon: "i-lucide-list",
    },
    {
      label: "Assign to me",
      icon: "i-lucide-user-plus",
    },
    {
      type: "separator",
    },
    {
      label: "Delete ticket",
      icon: "i-lucide-trash",
      color: "error",
      onSelect() {
        toast.add({
          title: "Ticket deleted",
          description: "The ticket has been deleted.",
        });
      },
    },
  ];
}

const columns: TableColumn<Ticket>[] = [
  {
    id: "select",
    header: ({ table }) =>
      h(UCheckbox, {
        modelValue: table.getIsSomePageRowsSelected()
          ? "indeterminate"
          : table.getIsAllPageRowsSelected(),
        "onUpdate:modelValue": (value: boolean | "indeterminate") =>
          table.toggleAllPageRowsSelected(!!value),
        ariaLabel: "Select all",
      }),
    cell: ({ row }) =>
      h(UCheckbox, {
        modelValue: row.getIsSelected(),
        "onUpdate:modelValue": (value: boolean | "indeterminate") =>
          row.toggleSelected(!!value),
        ariaLabel: "Select row",
      }),
  },
  {
    accessorKey: "id",
    header: ({ column }) => {
      const isSorted = column.getIsSorted();
      return h(UButton, {
        color: "neutral",
        variant: "ghost",
        label: "ID",
        icon: isSorted
          ? isSorted === "asc"
            ? "i-lucide-arrow-up-narrow-wide"
            : "i-lucide-arrow-down-wide-narrow"
          : undefined,
        class: "-mx-2.5",
        onClick: () => column.toggleSorting(column.getIsSorted() === "asc"),
      });
    },
  },
  {
    accessorKey: "subject",
    header: ({ column }) => {
      const isSorted = column.getIsSorted();
      return h(UButton, {
        color: "neutral",
        variant: "ghost",
        label: "Subject",
        icon: isSorted
          ? isSorted === "asc"
            ? "i-lucide-arrow-up-narrow-wide"
            : "i-lucide-arrow-down-wide-narrow"
          : undefined,
        class: "-mx-2.5",
        onClick: () => column.toggleSorting(column.getIsSorted() === "asc"),
      });
    },
    cell: ({ row }) =>
      h("p", { class: "text-highlighted" }, row.original.subject),
  },

  {
    accessorKey: "requester.name",
    header: ({ column }) => {
      const isSorted = column.getIsSorted();
      return h(UButton, {
        color: "neutral",
        variant: "ghost",
        label: "Customer",
        icon: isSorted
          ? isSorted === "asc"
            ? "i-lucide-arrow-up-narrow-wide"
            : "i-lucide-arrow-down-wide-narrow"
          : undefined,
        class: "-mx-2.5",
        onClick: () => column.toggleSorting(column.getIsSorted() === "asc"),
      });
    },
    cell: ({ row }) =>
      h(
        "p",
        { class: "font-medium text-highlighted" },
        `${row.original.requester.firstName} ${row.original.requester.lastName}`
      ),
  },
  {
    accessorKey: "status",
    header: ({ column }) => {
      const isSorted = column.getIsSorted();
      return h(UButton, {
        color: "neutral",
        variant: "ghost",
        label: "Status",
        icon: isSorted
          ? isSorted === "asc"
            ? "i-lucide-arrow-up-narrow-wide"
            : "i-lucide-arrow-down-wide-narrow"
          : undefined,
        class: "-mx-2.5",
        onClick: () => column.toggleSorting(column.getIsSorted() === "asc"),
      });
    },
    filterFn: "equals",
    cell: ({ row }) => {
      const color = {
        open: "warning" as const,
        pending: "info" as const,
        resolved: "success" as const,
        closed: "neutral" as const,
      }[row.original.status];

      return h(
        UBadge,
        { class: "capitalize", variant: "subtle", color },
        () => row.original.status
      );
    },
  },
  {
    accessorKey: "priority",
    header: ({ column }) => {
      const isSorted = column.getIsSorted();
      return h(UButton, {
        color: "neutral",
        variant: "ghost",
        label: "Priority",
        icon: isSorted
          ? isSorted === "asc"
            ? "i-lucide-arrow-up-narrow-wide"
            : "i-lucide-arrow-down-wide-narrow"
          : undefined,
        class: "-mx-2.5",
        onClick: () => column.toggleSorting(column.getIsSorted() === "asc"),
      });
    },
    cell: ({ row }) => {
      const color = {
        low: "neutral" as const,
        medium: "info" as const,
        high: "warning" as const,
        urgent: "error" as const,
        unknown: "neutral" as const,
      }[row.original.priority];

      return h(
        UBadge,
        { class: "capitalize", variant: "subtle", color },
        () => row.original.priority
      );
    },
  },
  {
    accessorKey: "createdAt",
    header: ({ column }) => {
      const isSorted = column.getIsSorted();
      return h(UButton, {
        color: "neutral",
        variant: "ghost",
        label: "Created At",
        icon: isSorted
          ? isSorted === "asc"
            ? "i-lucide-arrow-up-narrow-wide"
            : "i-lucide-arrow-down-wide-narrow"
          : undefined,
        class: "-mx-2.5",
        onClick: () => column.toggleSorting(column.getIsSorted() === "asc"),
      });
    },
    cell: ({ row }) => timeAgo(row.original.createdAt),
  },

  {
    id: "actions",
    cell: ({ row }) => {
      return h(
        "div",
        { class: "text-right" },
        h(
          UDropdownMenu,
          {
            content: {
              align: "end",
            },
            items: getRowItems(row),
          },
          () =>
            h(UButton, {
              icon: "i-lucide-ellipsis-vertical",
              color: "neutral",
              variant: "ghost",
              class: "ml-auto",
            })
        )
      );
    },
  },
];

const statusFilter = ref("all");

watch(
  () => statusFilter.value,
  (newVal) => {
    if (!table?.value?.tableApi) return;

    const statusColumn = table.value.tableApi.getColumn("status");
    if (!statusColumn) return;

    if (newVal === "all") {
      statusColumn.setFilterValue(undefined);
    } else {
      statusColumn.setFilterValue(newVal);
    }
  }
);

// Optional: add an email filter input bound to table column

const pagination = ref({
  pageIndex: 0,
  pageSize: 10,
});

function handleRefresh() {
  status.value = "pending";
  setTimeout(() => {
    status.value = "success";
    toast.add({ title: "Refreshed", description: "Ticket list updated." });
  }, 500);
}
</script>

<template>
  <UDashboardPanel
    id="tickets"
    resizable
    :ui="{
      body: 'p-0 sm:p-0',
    }"
  >
    <template #header>
      <UDashboardNavbar
        title="Tickets"
        class="bg-elevated/25"
        :ui="{ right: 'gap-3' }"
      >
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #right>
          <UButton
            variant="ghost"
            color="neutral"
            class="ring-default"
            aria-label="Refresh"
            :disabled="status === 'pending'"
            @click="handleRefresh()"
          >
            <UIcon
              name="i-lucide-refresh-ccw"
              :class="status === 'pending' ? 'animate-spin size-5' : 'size-5'"
            />
          </UButton>
          <div class="flex flex-wrap items-center gap-1.5">
            <CustomersDeleteModal
              :count="
                table?.tableApi?.getFilteredSelectedRowModel().rows.length
              "
            >
              <UButton
                v-if="
                  table?.tableApi?.getFilteredSelectedRowModel().rows.length
                "
                label="Delete"
                color="error"
                variant="subtle"
                icon="i-lucide-trash"
              >
                <template #trailing>
                  <UKbd>
                    {{
                      table?.tableApi?.getFilteredSelectedRowModel().rows.length
                    }}
                  </UKbd>
                </template>
              </UButton>
            </CustomersDeleteModal>

            <USelect
              v-model="statusFilter"
              :items="[
                { label: 'All', value: 'all' },
                { label: 'Open', value: 'open' },
                { label: 'Pending', value: 'pending' },
                { label: 'Resolved', value: 'resolved' },
                { label: 'Closed', value: 'closed' },
              ]"
              :ui="{
                trailingIcon:
                  'group-data-[state=open]:rotate-180 transition-transform duration-200',
              }"
              placeholder="Filter status"
              class="min-w-24 cursor-pointer"
              variant="none"
            />

            <UDropdownMenu
              class="group cursor-pointer"
              :items="
              table?.tableApi
                ?.getAllColumns()
                .filter((column: any) => column.getCanHide())
                .map((column: any) => ({
                  label: formatColumnLabel(column.id),
                  type: 'checkbox' as const,
                  checked: column.getIsVisible(),
                  onUpdateChecked(checked: boolean) {
                    table?.tableApi?.getColumn(column.id)?.toggleVisibility(!!checked)
                  },
                  onSelect(e?: Event) {
                    e?.preventDefault()
                  }
                }))
            "
              :content="{ align: 'end' }"
            >
              <UButton
                label="Display"
                color="neutral"
                variant="none"
                trailing-icon="i-lucide-chevron-down"
                :ui="{
                  trailingIcon:
                    'group-data-[state=open]:rotate-180 transition-transform duration-200 text-dimmed',
                }"
              />
            </UDropdownMenu>
          </div>
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <UTable
        ref="table"
        v-model:column-filters="columnFilters"
        v-model:column-visibility="columnVisibility"
        v-model:row-selection="rowSelection"
        v-model:pagination="pagination"
        :pagination-options="{
          getPaginationRowModel: getPaginationRowModel(),
        }"
        class="shrink-0"
        :data="data"
        :columns="columns"
        :loading="status === 'pending'"
        :ui="{
          base: 'table-fixed border-separate border-spacing-0',
          thead: '[&>tr]:bg-elevated/50 [&>tr]:after:content-none',
          tbody: '[&>tr]:last:[&>td]:border-b-0',
          th: 'py-2 border-b border-default',
          td: 'border-b border-default',
          separator: 'h-0',
        }"
      />
    </template>

    <template #footer>
      <UDashboardToolbar
        class="h-(--ui-header-height) border-t border-default shrink-0 flex items-center border-b px-4 sm:px-6 gap-1.5 bg-elevated/25"
      >
        <div class="text-sm text-muted">
          {{ table?.tableApi?.getFilteredSelectedRowModel().rows.length || 0 }}
          of
          {{ table?.tableApi?.getFilteredRowModel().rows.length || 0 }} row(s)
          selected.
        </div>

        <div class="flex items-center gap-1.5">
          <UPagination
            :default-page="
              (table?.tableApi?.getState().pagination.pageIndex || 0) + 1
            "
            :items-per-page="table?.tableApi?.getState().pagination.pageSize"
            :total="table?.tableApi?.getFilteredRowModel().rows.length"
            @update:page="(p: number) => table?.tableApi?.setPageIndex(p - 1)"
          />
        </div>
      </UDashboardToolbar>
    </template>
  </UDashboardPanel>
</template>
