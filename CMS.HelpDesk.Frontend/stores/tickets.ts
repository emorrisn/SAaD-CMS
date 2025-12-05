import { defineStore } from "pinia";
import type { Ticket } from "../app/types";

type TicketsState = {
  items: Ticket[];
  loading: boolean;
  error?: string;
};

export const useTicketsStore = defineStore("tickets", {
  state: (): TicketsState => ({
    items: [
      {
        id: 1001,
        subject: "Cannot login to portal",
        requester: {
          firstName: "Alice",
          lastName: "Johnson",
          email: "alice.johnson@example.com",
          avatar: { src: "/avatars/alice.png", alt: "Alice" },
        },
        priority: "high",
        status: "open",
        createdAt: "2025-12-01T09:15:00Z",
        updatedAt: "2025-12-01T10:05:00Z",
      },
      {
        id: 1002,
        subject: "Email notifications not received",
        requester: {
          firstName: "Bob",
          lastName: "Smith",
          email: "bob.smith@example.com",
          avatar: { src: "/avatars/bob.png", alt: "Bob" },
        },
        priority: "medium",
        status: "pending",
        createdAt: "2025-11-28T14:30:00Z",
        updatedAt: "2025-11-29T08:45:00Z",
      },
      {
        id: 1003,
        subject: "Payment page throws error",
        requester: {
          firstName: "Carol",
          lastName: "Nguyen",
          email: "carol.nguyen@example.com",
          avatar: { src: "/avatars/carol.png", alt: "Carol" },
        },
        priority: "urgent",
        status: "resolved",
        createdAt: "2025-11-25T11:00:00Z",
        updatedAt: "2025-11-26T16:22:00Z",
      },
      {
        id: 1004,
        subject: "Request for feature: bulk upload",
        requester: {
          firstName: "Daniel",
          lastName: "Lee",
          email: "daniel.lee@example.com",
          avatar: { src: "/avatars/daniel.png", alt: "Daniel" },
        },
        priority: "unknown",
        status: "closed",
        createdAt: "2025-11-20T08:10:00Z",
        updatedAt: "2025-11-21T12:00:00Z",
      },
    ],
    loading: false,
    error: undefined,
  }),
  getters: {
    byId: (state) => (id: string | number) =>
      state.items.find((t) => t.id === id),
    openTickets: (state) => state.items.filter((t) => t.status === "open"),
  },
  actions: {
    setTickets(tickets: Ticket[]) {
      this.items = tickets;
    },
    upsertTicket(ticket: Ticket) {
      const idx = this.items.findIndex((t) => t.id === ticket.id);
      if (idx >= 0) this.items.splice(idx, 1, ticket);
      else this.items.push(ticket);
    },
    removeTicket(id: string | number) {
      const targetId = String(id);
      this.items = this.items.filter((t) => String(t.id) !== targetId);
    },
    setLoading(val: boolean) {
      this.loading = val;
    },
    setError(message?: string) {
      this.error = message;
    },
  },
});
