export function useApi() {
  const config = useRuntimeConfig();
  const base = (config.public.apiBase || "").replace(/\/$/, "");

  async function get<T>(
    path: string,
    params?: Record<string, any>
  ): Promise<T> {
    const url = new URL(base + path);
    if (params) {
      Object.entries(params).forEach(([k, v]) => {
        if (v !== undefined && v !== null) url.searchParams.set(k, String(v));
      });
    }
    return await $fetch<T>(url.toString());
  }

  async function post<T>(path: string, body: any): Promise<T> {
    const url = base + path;
    return await $fetch<T>(url, { method: "POST", body });
  }

  async function patch<T>(path: string, body: any): Promise<T> {
    const url = base + path;
    return await $fetch<T>(url, { method: "PATCH", body });
  }

  return { get, post, patch };
}
