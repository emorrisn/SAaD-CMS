import { defineStore } from "pinia";
import type { CustomerRecord } from "../app/types";

type CustomersState = {
  items: CustomerRecord[];
  loading: boolean;
  error?: string;
};

export const useCustomersStore = defineStore("customers", {
  state: (): CustomersState => ({
    items: [
      {
        id: "cust-1",
        email: "daniel.hughes@example.com",
        firstName: "Daniel",
        lastName: "Hughes",
        phone: "+1 555-1010",
      },
      {
        id: "cust-2",
        email: "priya.patel@example.com",
        firstName: "Priya",
        lastName: "Patel",
        phone: "+1 555-2020",
      },
      {
        id: "cust-3",
        email: "omar.hassan@example.com",
        firstName: "Omar",
        lastName: "Hassan",
        phone: "+1 555-3030",
      },
    ],
    loading: false,
    error: undefined,
  }),
  getters: {
    byId: (state) => (id: string) => state.items.find((c) => c.id === id),
  },
  actions: {
    setCustomers(customers: CustomerRecord[]) {
      this.items = customers;
    },
    upsertCustomer(customer: CustomerRecord) {
      const idx = this.items.findIndex((c) => c.id === customer.id);
      if (idx >= 0) this.items.splice(idx, 1, customer);
      else this.items.push(customer);
    },
    removeCustomer(id: string) {
      this.items = this.items.filter((c) => c.id !== id);
    },
    setLoading(val: boolean) {
      this.loading = val;
    },
    setError(message?: string) {
      this.error = message;
    },
  },
});
