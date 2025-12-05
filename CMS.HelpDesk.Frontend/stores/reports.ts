import { defineStore } from "pinia";
import type { ReportEntry } from "../app/types";

type ReportsState = {
  items: ReportEntry[];
  loading: boolean;
  error?: string;
};

export const useReportsStore = defineStore("reports", {
  state: (): ReportsState => ({
    items: [
      {
        id: "rpt-2025-11-30",
        name: "Monthly Service Level Report",
        createdAt: "2025-11-30",
        payload: {
          params: "All Customers · 2025-11-01 → 2025-11-30 · Resolved",
        },
      },
      {
        id: "rpt-2025-10-31",
        name: "Agent Performance Summary",
        createdAt: "2025-10-31",
        payload: {
          params: "All Customers · 2025-10-01 → 2025-10-31 · Closed",
        },
      },
      {
        id: "rpt-2025-09-30",
        name: "Quarterly Ticket Volume Overview",
        createdAt: "2025-09-30",
        payload: {
          params: "All Customers · 2025-07-01 → 2025-09-30 · Any",
        },
      },
    ],
    loading: false,
    error: undefined,
  }),
  actions: {
    setReports(reports: ReportEntry[]) {
      this.items = reports;
    },
    upsertReport(report: ReportEntry) {
      const idx = this.items.findIndex((r) => r.id === report.id);
      if (idx >= 0) this.items.splice(idx, 1, report);
      else this.items.push(report);
    },
    removeReport(id: string) {
      this.items = this.items.filter((r) => r.id !== id);
    },
    setLoading(val: boolean) {
      this.loading = val;
    },
    setError(message?: string) {
      this.error = message;
    },
  },
});
