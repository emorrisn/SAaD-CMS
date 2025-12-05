import { defineStore } from "pinia";
import type { UserRecord } from "../app/types";

type UsersState = {
  items: UserRecord[];
  activeUserId?: string;
  loading: boolean;
  error?: string;
};

export const useUsersStore = defineStore("users", {
  state: (): UsersState => ({
    items: [
      {
        id: "daniel.hughes",
        firstName: "Daniel",
        lastName: "Hughes",
        email: "daniel.hughes@example.com",
        role: "Agent",
      },
      {
        id: "priya.patel",
        firstName: "Priya",
        lastName: "Patel",
        email: "priya.patel@example.com",
        role: "Agent",
      },
      {
        id: "omar.hassan",
        firstName: "Omar",
        lastName: "Hassan",
        email: "omar.hassan@example.com",
        role: "Senior Agent",
      },
    ],
    activeUserId: "daniel.hughes",
    loading: false,
    error: undefined,
  }),
  getters: {
    activeUser: (state) => state.items.find((u) => u.id === state.activeUserId),
  },
  actions: {
    setUsers(users: UserRecord[]) {
      this.items = users;
    },
    upsertUser(user: UserRecord) {
      const idx = this.items.findIndex((u) => u.id === user.id);
      if (idx >= 0) this.items.splice(idx, 1, user);
      else this.items.push(user);
    },
    removeUser(userId: string) {
      this.items = this.items.filter((u) => u.id !== userId);
    },
    setActiveUser(userId?: string) {
      this.activeUserId = userId;
    },
    setLoading(val: boolean) {
      this.loading = val;
    },
    setError(message?: string) {
      this.error = message;
    },
  },
});
