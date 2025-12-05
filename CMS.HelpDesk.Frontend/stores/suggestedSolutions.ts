import { defineStore } from "pinia";
import type { Suggestion } from "../app/types";

type SuggestedSolutionsState = {
  items: Suggestion[];
  loading: boolean;
  error?: string;
};

export const useSuggestedSolutionsStore = defineStore("suggestedSolutions", {
  state: (): SuggestedSolutionsState => ({
    items: [
      {
        id: "d1",
        title: "Account Locked",
        summary:
          "Unlock account via admin panel; ensure recent failed logins noted.",
        tags: ["Auth", "Lockout"],
        category: "auth",
      },
      {
        id: "d2",
        title: "Wi-Fi Intermittent",
        summary: "Check router logs; advise switching to wired for testing.",
        tags: ["Network"],
        category: "network",
      },
      {
        id: "s1",
        title: "Reset Credentials",
        summary: "Guide customer through password reset; verify 2FA status.",
        tags: ["Auth", "2FA", "Password", "Login"],
        category: "auth",
      },
      {
        id: "s2",
        title: "Check Network Health",
        summary: "Run connectivity checks and review endpoint status.",
        tags: ["Network", "Performance", "Latency", "Timeout", "Slow"],
        category: "network",
      },
      {
        id: "s3",
        title: "Verify Billing",
        summary: "Cross-check invoices and subscription status.",
        tags: ["Billing", "Invoice", "Charge"],
        category: "billing",
      },
    ],
    loading: false,
    error: undefined,
  }),
  getters: {
    byCategory: (state) => (category?: string) =>
      category
        ? state.items.filter((s) => s.category === category)
        : state.items,
  },
  actions: {
    setSolutions(items: Suggestion[]) {
      this.items = items;
    },
    upsertSolution(item: Suggestion) {
      const idx = this.items.findIndex((s) => s.id === item.id);
      if (idx >= 0) this.items.splice(idx, 1, item);
      else this.items.push(item);
    },
    removeSolution(id: string) {
      this.items = this.items.filter((s) => s.id !== id);
    },
    setLoading(val: boolean) {
      this.loading = val;
    },
    setError(message?: string) {
      this.error = message;
    },
  },
});
