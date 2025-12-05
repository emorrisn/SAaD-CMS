import { defineStore } from "pinia";
import type { StatPoint, StatisticsSummary } from "../app/types";

type StatisticsState = {
  summary: StatisticsSummary;
  trends: StatPoint[];
  loading: boolean;
  error?: string;
};

export const useStatisticsStore = defineStore("statistics", {
  state: (): StatisticsState => ({
    summary: {
      open: 12,
      pending: 5,
      resolved: 28,
      closed: 44,
      avgResponseSeconds: 3000,
      resolutionRate: 86,
    },
    trends: [
      { label: "Week -3", value: 22 },
      { label: "Week -2", value: 31 },
      { label: "Week -1", value: 28 },
      { label: "This Week", value: 30 },
    ],
    loading: false,
    error: undefined,
  }),
  actions: {
    setSummary(data: StatisticsSummary) {
      this.summary = data;
    },
    setTrends(points: StatPoint[]) {
      this.trends = points;
    },
    setLoading(val: boolean) {
      this.loading = val;
    },
    setError(message?: string) {
      this.error = message;
    },
  },
});
