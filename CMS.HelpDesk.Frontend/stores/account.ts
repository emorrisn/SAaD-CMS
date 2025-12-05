import { defineStore } from "pinia";
import type { AccountPreferences, AccountProfile } from "../app/types";

type AccountState = {
  profile?: AccountProfile;
  preferences: AccountPreferences;
  loading: boolean;
  error?: string;
};

export const useAccountStore = defineStore("account", {
  state: (): AccountState => ({
    profile: {
      id: "daniel.hughes",
      email: "daniel.hughes@example.com",
      firstName: "Daniel",
      lastName: "Hughes",
      avatarUrl: "/avatars/daniel.png",
      language: "en",
      timezone: "UTC",
    },
    preferences: { theme: "system" },
    loading: false,
    error: undefined,
  }),
  actions: {
    setProfile(profile: AccountProfile | undefined) {
      this.profile = profile;
    },
    updatePreferences(prefs: AccountPreferences) {
      this.preferences = { ...this.preferences, ...prefs };
    },
    setLoading(val: boolean) {
      this.loading = val;
    },
    setError(message?: string) {
      this.error = message;
    },
  },
});
