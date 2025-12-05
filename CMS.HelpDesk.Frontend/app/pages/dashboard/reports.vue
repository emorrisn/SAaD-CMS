<template>
  <UDashboardPanel id="reports" resizable>
    <template #header>
      <UDashboardNavbar
        title="Reports"
        class="bg-elevated/25"
        :ui="{ right: 'gap-3' }"
      >
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <UContainer class="py-4">
        <UCard variant="soft" class="mb-4 border border-default">
          <template #header>
            <h2 class="text-lg font-semibold">Columns</h2>
          </template>
          <UForm :state="form" class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <UFormField label="Columns">
              <USelectMenu
                v-model="selectedColumns"
                :items="columnOptions"
                option-attribute="label"
                value-attribute="value"
                by="value"
                multiple
                class="w-full"
              />
            </UFormField>

            <!-- Status Filter -->
            <UFormField label="Ticket Status">
              <USelectMenu
                v-model="selectedStatuses"
                :items="statusOptions"
                option-attribute="label"
                value-attribute="value"
                by="value"
                multiple
                class="w-full"
              />
            </UFormField>

            <div class="md:col-span-3">
              <UFormField label="Date Range">
                <UInputDate ref="inputDateRef" v-model="calendarModel" range>
                  <template #trailing>
                    <UPopover :reference="inputDateRef?.inputsRef[0]?.$el">
                      <UButton
                        color="neutral"
                        variant="link"
                        size="sm"
                        icon="i-lucide-calendar"
                        aria-label="Select a date range"
                        class="px-0 w-full"
                      />

                      <template #content>
                        <UCalendar
                          v-model="calendarModel"
                          class="p-2 w-full"
                          :number-of-months="2"
                          range
                        />
                      </template>
                    </UPopover>
                  </template>
                </UInputDate>
              </UFormField>
            </div>
          </UForm>
          <template #footer>
            <div class="flex flex-wrap gap-2 justify-end">
              <UDropdownMenu :items="generateItems" :disabled="!isValid">
                <UButton color="primary">
                  Generate Report
                  <Icon name="i-lucide-chevron-down" class="ms-1" />
                </UButton>
              </UDropdownMenu>
            </div>
          </template>
        </UCard>

        <UTable
          :data="archiveRows"
          :columns="archiveColumns"
          :ui="{
            base: 'table-fixed border-separate border-spacing-0',
            thead: '[&>tr]:bg-elevated/50 [&>tr]:after:content-none',
            tbody: '[&>tr]:last:[&>td]:border-b-0 [&>tr:hover]:bg-elevated/60',
            th: 'first:rounded-l-lg last:rounded-r-lg border-y border-default first:border-l last:border-r',
            td: 'border-b border-default',
          }"
        />
      </UContainer>

      <!-- Export options handled via dropdown menu above -->
    </template>
  </UDashboardPanel>
</template>

<script setup lang="ts">
import { sub } from "date-fns";
import { CalendarDate } from "@internationalized/date";
import { h, resolveComponent } from "vue";
import { storeToRefs } from "pinia";
import type { TableColumn } from "@nuxt/ui";
import { useReportsStore } from "../../../stores/reports";
import type { ArchiveRow, ExportMode } from "../../types";
import type { DateRange } from "../../types";
definePageMeta({
  layout: "dashboard",
  auth: true,
});

useSeoMeta({
  title: "Reports",
});

const generateItems = [
  {
    label: "Export as PDF",
    icon: "i-lucide-file-text",
    onSelect: () => onGenerate("pdf"),
  },
  {
    label: "Export as CSV",
    icon: "i-lucide-table",
    onSelect: () => onGenerate("csv"),
  },
];

// Ticket table columns; all selected by default
const columnOptions = [
  { label: "ID", value: "id" },
  { label: "Subject", value: "subject" },
  { label: "Requester First Name", value: "requester.firstName" },
  { label: "Requester Last Name", value: "requester.lastName" },
  { label: "Requester Email", value: "requester.email" },
  { label: "Priority", value: "priority" },
  { label: "Status", value: "status" },
  { label: "Created At", value: "createdAt" },
  { label: "Updated At", value: "updatedAt" },
  { label: "Location", value: "location" },
];

const form = reactive({
  columns: columnOptions.map((c) => c.value) as string[],
  statuses: ["ANY"] as string[],
  range: {
    start: sub(new Date(), { days: 30 }),
    end: new Date(),
  } as DateRange,
});

// Accessibility: explicit labels and required checks
const isValid = computed(() => {
  return Boolean(
    form.columns &&
      form.columns.length > 0 &&
      form.statuses &&
      form.statuses.length > 0 &&
      form.range?.start &&
      form.range?.end
  );
});
// Map between select's objects and stored column values
const selectedColumns = computed<Array<{ label: string; value: string }>>({
  get() {
    return columnOptions.filter((opt) => form.columns.includes(opt.value));
  },
  set(vals) {
    form.columns = Array.isArray(vals) ? vals.map((v) => v.value) : [];
  },
});
// Bind UInputDate range to form.range using CalendarDate
const inputDateRef = useTemplateRef("inputDateRef");
const calendarModel = shallowRef<{ start: CalendarDate; end: CalendarDate }>({
  start: new CalendarDate(
    form.range.start.getUTCFullYear(),
    form.range.start.getUTCMonth() + 1,
    form.range.start.getUTCDate()
  ),
  end: new CalendarDate(
    form.range.end.getUTCFullYear(),
    form.range.end.getUTCMonth() + 1,
    form.range.end.getUTCDate()
  ),
});

watch(
  calendarModel,
  (val) => {
    if (val?.start) {
      form.range.start = new Date(
        val.start.year,
        val.start.month - 1,
        val.start.day
      );
    }
    if (val?.end) {
      form.range.end = new Date(val.end.year, val.end.month - 1, val.end.day);
    }
  },
  { deep: true }
);
const statusOptions = [
  { label: "Any", value: "ANY" },
  { label: "Resolved", value: "RESOLVED" },
  { label: "Closed", value: "CLOSED" },
  { label: "Escalated", value: "ESCALATED" },
];

// Map between select's objects and stored statuses strings
const selectedStatuses = computed<Array<{ label: string; value: string }>>({
  get() {
    return statusOptions.filter((opt) => form.statuses.includes(opt.value));
  },
  set(vals) {
    const ids = Array.isArray(vals) ? vals.map((v) => v.value) : [];
    // Mutual exclusivity: if ANY is selected with others, deselect ANY and keep specific
    if (ids.includes("ANY")) {
      const others = ids.filter((v) => v !== "ANY");
      form.statuses = others.length ? others : ["ANY"];
    } else {
      form.statuses = ids.length ? ids : ["ANY"]; // default to ANY when empty
    }
  },
});

// Report Archive table (match HomeTickets style)
const UButton = resolveComponent("UButton");

const archiveColumns: TableColumn<ArchiveRow>[] = [
  {
    accessorKey: "date",
    header: "Date Generated",
    cell: ({ row }) => String(row.getValue("date")),
  },
  {
    accessorKey: "name",
    header: "Report Name",
    cell: ({ row }) => String(row.getValue("name")),
  },
  {
    accessorKey: "params",
    header: "Parameters Used",
    cell: ({ row }) => String(row.getValue("params")),
  },
  {
    accessorKey: "actions",
    header: "Actions",
    cell: ({ row }) =>
      h(UButton, { color: "neutral", size: "sm" }, () =>
        String(row.getValue("actions"))
      ),
  },
];

const reportsStore = useReportsStore();
const { items: reports } = storeToRefs(reportsStore);

const archiveRows = computed<ArchiveRow[]>(() =>
  reports.value.map((r) => ({
    date: r.createdAt,
    name: r.name,
    params:
      (r.payload as { params?: string } | undefined)?.params ??
      "Parameters not specified",
    actions: "Download",
  }))
);

function onGenerate(mode: ExportMode) {
  if (!isValid.value) return;

  // In a real app, trigger API to generate/export or schedule
  const toast = useToast();
  toast.add({
    title: "Report request submitted",
    description: `Mode: ${mode} · Columns: ${
      form.columns.length
    } · Statuses: ${form.statuses.join(",")}`,
  });
}
</script>
