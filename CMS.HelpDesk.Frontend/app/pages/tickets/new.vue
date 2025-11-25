<template>
  <UCard>
    <template #header>
      <div class="flex items-center justify-between">
        <h2 class="text-xl font-semibold">Create New Ticket</h2>
      </div>
    </template>

    <form class="space-y-6" @submit.prevent="submit">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <UFormGroup label="Consumer Username" required>
          <UInput
            v-model="form.consumerUsername"
            placeholder="e.g. john.doe@example.com"
          />
        </UFormGroup>
        <UFormGroup label="Tenant ID" required>
          <UInput v-model="form.tenantId" placeholder="tenant-demo" />
        </UFormGroup>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <UFormGroup label="Category">
          <UInput v-model="form.category" />
        </UFormGroup>
        <UFormGroup label="Priority">
          <USelect v-model="form.priority" :options="priorityOptions" />
        </UFormGroup>
      </div>

      <UFormGroup label="Subject">
        <UInput v-model="form.subject" />
      </UFormGroup>

      <UFormGroup label="Detailed Description" required>
        <UTextarea v-model="form.description" :rows="6" />
      </UFormGroup>

      <div class="flex justify-end gap-2">
        <NuxtLink to="/">
          <UButton variant="ghost">Cancel</UButton>
        </NuxtLink>
        <UButton type="submit" color="primary" :loading="loading"
          >Create Ticket</UButton
        >
      </div>
    </form>
  </UCard>
</template>

<script setup lang="ts">
import { useApi } from "@/composables/useApi";

const { post } = useApi();
const router = useRouter();

const priorityOptions = ["Low", "Medium", "High", "Critical"];

const form = reactive({
  tenantId: "tenant-demo",
  consumerUsername: "",
  category: "",
  priority: "Medium",
  subject: "",
  description: "",
});

const loading = ref(false);

async function submit() {
  loading.value = true;
  try {
    const payload = {
      tenantID: form.tenantId,
      consumerUsername: form.consumerUsername,
      category: form.category || null,
      priority: ["Low", "Medium", "High", "Critical"].indexOf(form.priority),
      subject: form.subject || null,
      description: form.description,
    };
    const res = await post<{ complaintId: string }>(`/api/complaints`, payload);
    await router.push(
      `/tickets/${res.complaintId}?tenantId=${encodeURIComponent(
        form.tenantId
      )}`
    );
  } finally {
    loading.value = false;
  }
}
</script>
