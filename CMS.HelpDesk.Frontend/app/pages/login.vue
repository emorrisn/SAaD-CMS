<template>
  <UContainer class="mt-20 max-w-md">
    <UCard variant="soft" class="shadow">
      <template #header>
        <h2 class="text-xl font-semibold">Welcome Back</h2>
      </template>

      <UFormField label="Workplace" class="mb-4">
        <USelectMenu
          v-model="selectedTenant"
          :items="tenantOptions"
          option-attribute="label"
          value-attribute="value"
          :loading="tenantsLoading"
          searchable
          placeholder="Select tenant"
          by="value"
          class="w-full"
        />
      </UFormField>

      <UFormField label="Username" class="mb-4">
        <UInput v-model="username" type="username" class="w-full" />
      </UFormField>

      <UFormField label="Password" class="mb-4">
        <UInput v-model="password" type="password" class="w-full" />
      </UFormField>

      <UButton :loading="loading" block @click="login" class="cursor-pointer">
        Login
      </UButton>

      <template v-if="error" #footer>
        <UAlert
          color="error"
          title="Something went wrong"
          :description="error"
        />
      </template>
    </UCard>
  </UContainer>
</template>

<script lang="ts" setup>
import type { Tenant } from "../types";

definePageMeta({
  layout: "app",
  auth: false,
});

useSeoMeta({
  title: "Login",
});

const username = ref("support1");
const password = ref("Passw0rd!");
const error = ref("");
const tenant = ref<string | null>(null);
const loading = ref(false);

const tenants = ref<Tenant[] | null>([]);
const tenantsLoading = ref(false);

const tenantOptions = computed(() =>
  Array.isArray(tenants.value)
    ? tenants.value.map((t) => ({ label: t.name, value: t.tenantId }))
    : []
);

// Map between the select's item object ({ label, value }) and the stored tenantId (string | null)
const selectedTenant = computed<{ label: string; value: string } | undefined>({
  get(): { label: string; value: string } | undefined {
    if (!tenant.value) return undefined;
    return tenantOptions.value.find((opt) => opt.value === tenant.value);
  },
  set(val: { label: string; value: string } | undefined) {
    tenant.value = val?.value ?? null;
  },
});

const apiBase =
  process.env.NUXT_PUBLIC_API_BASE || useRuntimeConfig().public?.apiBase;

async function loadTenants() {
  tenantsLoading.value = true;
  try {
    const data = await $fetch(`${apiBase}/tenants/list`);
    if (Array.isArray(data)) {
      tenants.value = data as Tenant[];
      if (!tenant.value && data.length) {
        tenant.value = data[0].tenantId;
      }
    } else {
      throw new Error("Unexpected tenants response shape");
    }
  } catch (e: any) {
    error.value = e?.data?.message || e?.message || "Failed to load tenants";
    tenants.value = []; // ensure array
  } finally {
    tenantsLoading.value = false;
  }
}

onMounted(() => {
  loadTenants();
});

const auth = useAuth();

async function login() {
  loading.value = true;
  error.value = "";

  try {
    await auth.signIn(
      {
        tenantId: tenant.value,
        username: username.value,
        password: password.value,
      },
      { callbackUrl: "/dashboard/home" }
    );
  } catch (err: any) {
    error.value = err?.data?.message || "Please try again later";
  } finally {
    loading.value = false;
  }
}
</script>
