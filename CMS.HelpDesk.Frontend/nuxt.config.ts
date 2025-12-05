// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2025-07-15",
  devtools: { enabled: true },
  ssr: false,
  app: {
    head: {
      titleTemplate: "%s · Help Desk",
      meta: [
        { name: "viewport", content: "width=device-width, initial-scale=1" },
        { name: "theme-color", content: "#0ea5e9" },
      ],
    },
  },
  modules: [
    "@nuxt/eslint",
    "@nuxt/ui",
    "@nuxt/icon",
    "@pinia/nuxt",
    "@sidebase/nuxt-auth",
  ],
  css: ["~/assets/css/main.css"],
  icon: {
    mode: "css",
    componentName: "Icon",
    cssLayer: "base",
  },
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE,
    },
  },
  auth: {
    isEnabled: true,
    disableServerSideAuth: false,
    globalAppMiddleware: true,
    baseURL: process.env.NUXT_PUBLIC_API_BASE || "http://localhost:7122",
    provider: {
      type: "local",
      endpoints: {
        signIn: { path: "/auth/login", method: "post" },
        signOut: { path: "/auth/logout", method: "post" },
        getSession: { path: "/auth/session", method: "get" },
        signUp: false,
      },
      session: {
        dataType: {
          token: "string",
          sessionId: "string",
          user: {
            username: "string",
            normalizedUsername: "string",
            tenantId: "string",
            role: "string",
            isActive: "boolean",
          },
        },
      },
    },
    token: {
      type: "Bearer",
    },
    cookie: {
      options: {
        secure: process.env.NODE_ENV === "production",
        sameSite: "lax",
      },
    },
  },
});
