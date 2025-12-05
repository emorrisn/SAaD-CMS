import { defineStore } from "pinia";
import type { InboxItem } from "../app/types";

type InboxState = {
  items: InboxItem[];
  loading: boolean;
  error?: string;
};

export const useInboxStore = defineStore("inbox", {
  state: (): InboxState => ({
    items: [
      {
        id: "msg-1",
        subject: "Re: Cannot login to portal",
        from: "alice.johnson@example.com",
        receivedAt: "2025-12-01T10:10:00Z",
        ticketId: "1001",
        unread: true,
      },
      {
        id: "msg-2",
        subject: "Payment page error screenshot",
        from: "carol.nguyen@example.com",
        receivedAt: "2025-11-25T11:30:00Z",
        ticketId: "1003",
        unread: false,
      },
    ],
    loading: false,
    error: undefined,
  }),
  getters: {
    unreadCount: (state) => state.items.filter((m) => m.unread).length,
  },
  actions: {
    setItems(items: InboxItem[]) {
      this.items = items;
    },
    markRead(id: string) {
      const item = this.items.find((m) => m.id === id);
      if (item) item.unread = false;
    },
    removeItem(id: string) {
      this.items = this.items.filter((m) => m.id !== id);
    },
    setLoading(val: boolean) {
      this.loading = val;
    },
    setError(message?: string) {
      this.error = message;
    },
  },
});
