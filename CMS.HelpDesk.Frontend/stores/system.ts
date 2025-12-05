import { defineStore } from "pinia";
import type { SystemSettings, ThemeChoice } from "../app/types";

export const useSystemStore = defineStore("system", {
  state: (): SystemSettings => ({
    categories: [
      "Authentication",
      "Billing",
      "Connectivity",
      "Device",
      "Other",
    ],
    priorities: ["Low", "Medium", "High", "Urgent"],
    statuses: ["Open", "In Progress", "Blocked", "Resolved", "Closed"],
    sources: ["Phone", "Email", "Web", "Chat"],
    theme: "system",
    isNavbarCollapsed: false,
    isNavbarVisible: true,
  }),
  actions: {
    setCategories(values: string[]) {
      this.categories = values;
    },
    setPriorities(values: string[]) {
      this.priorities = values;
    },
    setStatuses(values: string[]) {
      this.statuses = values;
    },
    setSources(values: string[]) {
      this.sources = values;
    },
    setTheme(theme: ThemeChoice) {
      this.theme = theme;
    },
    setNavbarCollapsed(collapsed: boolean) {
      this.isNavbarCollapsed = collapsed;
    },
    setNavbarVisible(visible: boolean) {
      this.isNavbarVisible = visible;
    },
  },
});
