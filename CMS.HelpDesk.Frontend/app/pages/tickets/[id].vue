<template>
  <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
    <div class="lg:col-span-2 space-y-4">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <div>
              <div class="text-sm text-gray-500">Ticket</div>
              <div class="text-xl font-semibold">#{{ id }}</div>
            </div>
            <UBadge :color="statusColor(status)">{{
              statusLabel(status)
            }}</UBadge>
          </div>
        </template>
        <div>
          <div class="text-lg font-medium">
            {{ complaint?.subject || "No subject" }}
          </div>
          <p class="mt-2 whitespace-pre-wrap">{{ complaint?.description }}</p>
        </div>
      </UCard>

      <UCard>
        <template #header>
          <div class="text-lg font-semibold">Resolution Log & Timeline</div>
        </template>
        <ul class="space-y-2">
          <li v-for="(e, idx) in audit" :key="idx" class="text-sm">
            <span class="text-gray-500"
              >{{ new Date(e.timestamp).toLocaleString() }}:</span
            >
            <span class="ml-2">{{ e.action }}</span>
          </li>
          <li v-if="audit.length === 0" class="text-sm text-gray-500">
            No events yet.
          </li>
        </ul>
      </UCard>
    </div>

    <div class="space-y-4">
      <UCard>
        <template #header>
          <div class="text-lg font-semibold">Actions</div>
        </template>

        <UFormGroup label="Assign to Agent">
          <UInput v-model="assignAgentId" placeholder="Agent GUID" />
          <UButton class="mt-2" @click="assign">Assign</UButton>
        </UFormGroup>

        <UDivider class="my-4" />

        <UFormGroup label="Resolve with Notes">
          <UTextarea
            v-model="resolveNotes"
            :rows="4"
            placeholder="Resolution notes"
          />
          <UButton class="mt-2" color="green" @click="resolve">Resolve</UButton>
        </UFormGroup>
      </UCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useApi } from "@/composables/useApi";

const route = useRoute();
const { get, patch } = useApi();

const id = route.params.id as string;
const tenantId = (route.query.tenantId as string) || "tenant-demo";

const complaint = ref<any | null>(null);
const audit = ref<any[]>([]);
const status = ref<number>(0);

const assignAgentId = ref("");
const resolveNotes = ref("");

function statusLabel(s: number) {
  return ["New", "Assigned", "Resolved", "Closed"][s] ?? "New";
}
function statusColor(s: number) {
  return ["gray", "blue", "green", "gray"][s] || "gray";
}

async function load() {
  complaint.value = await get(`/api/complaints/${id}`, { tenantId });
  status.value = complaint.value.status;
  // For PoC: fetch all audit events from a flat read (no endpoint implemented), reusing complaint fields as fallback
  audit.value = [];
}

async function assign() {
  await patch(`/api/complaints/${id}/assign`, {
    tenantID: tenantId,
    agentID: assignAgentId.value,
  });
  await load();
}

async function resolve() {
  await patch(`/api/complaints/${id}/resolve`, {
    tenantID: tenantId,
    notes: resolveNotes.value,
  });
  await load();
}

onMounted(load);
</script>
