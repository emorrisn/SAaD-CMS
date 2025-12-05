<template>
  <UDashboardPanel id="new-ticket" resizable>
    <template #header>
      <UDashboardNavbar
        title="New Ticket"
        class="bg-elevated/25"
        :ui="{ right: 'gap-3' }"
      >
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <UContainer class="py-4">
        <UForm
          :state="form"
          :validate="validate"
          @submit="onSubmit"
          @reset="onReset"
        >
          <div class="grid grid-cols-1 xl:grid-cols-3 gap-4">
            <div class="xl:col-span-2 space-y-4">
              <UCard>
                <template #header>
                  <div class="flex items-center justify-between">
                    <span>Customer Details</span>
                  </div>
                </template>
                <div class="grid grid-cols-1 md:grid-cols-6 gap-3">
                  <UFormField
                    label="Email"
                    name="customerEmail"
                    class="md:col-span-3"
                  >
                    <UInputMenu
                      v-model="form.customerEmail"
                      :items="customerEmailItems"
                      value-key="value"
                      label-key="label"
                      placeholder="Select or type customer email address"
                      class="w-full"
                      create-item="always"
                      @update:model-value="
                        (item) => {
                          const info = customerDirectory[item];
                          if (info) {
                            form.customerFirstName = info.firstName;
                            form.customerLastName = info.lastName;
                            form.customerPhone = info.phone;
                          }
                        }
                      "
                      @create="(item) => addCustomerEmail(item)"
                    />
                  </UFormField>
                  <UFormField
                    label="First Name"
                    name="customerFirstName"
                    class="md:col-span-3"
                  >
                    <UInput
                      v-model="form.customerFirstName"
                      placeholder="Enter customer first name"
                      class="w-full"
                    />
                  </UFormField>
                  <UFormField
                    label="Last Name"
                    name="customerLastName"
                    class="md:col-span-3"
                  >
                    <UInput
                      v-model="form.customerLastName"
                      placeholder="Enter customer last name"
                      class="w-full"
                    />
                  </UFormField>
                  <UFormField
                    label="Phone"
                    name="customerPhone"
                    class="md:col-span-3"
                  >
                    <UInput
                      v-model="form.customerPhone"
                      placeholder="Enter customer phone number"
                      class="w-full"
                    />
                  </UFormField>
                </div>
              </UCard>

              <UCard>
                <template #header>Problem Classification</template>
                <div class="grid grid-cols-1 md:grid-cols-6 gap-3">
                  <UFormField
                    label="Category"
                    name="category"
                    class="md:col-span-3"
                  >
                    <UInputMenu
                      v-model="form.category"
                      :items="categoryItems"
                      value-key="value"
                      label-key="label"
                      create-item="always"
                      placeholder="Select ticket category"
                      class="w-full"
                      @create="(item) => addCategoryItem(item)"
                    />
                  </UFormField>
                  <UFormField
                    label="Priority"
                    name="priority"
                    class="md:col-span-3"
                  >
                    <UInputMenu
                      v-model="form.priority"
                      :items="priorityItems"
                      value-key="value"
                      label-key="label"
                      placeholder="Select ticket priority"
                      class="w-full"
                      create-item="always"
                      @create="(item) => addPriorityItem(item)"
                    />
                  </UFormField>
                  <UFormField
                    label="Status"
                    name="status"
                    class="md:col-span-3"
                  >
                    <UInputMenu
                      v-model="form.status"
                      :items="statusItems"
                      value-key="value"
                      label-key="label"
                      placeholder="Select ticket status"
                      class="w-full"
                      create-item="always"
                      @create="(item) => addStatusItem(item)"
                    />
                  </UFormField>
                  <UFormField
                    label="Source"
                    name="source"
                    class="md:col-span-3"
                  >
                    <UInputMenu
                      v-model="form.source"
                      :items="sourceItems"
                      value-key="value"
                      label-key="label"
                      placeholder="Select ticket source"
                      class="w-full"
                      create-item="always"
                      @create="(item) => addSourceItem(item)"
                    />
                  </UFormField>
                </div>
              </UCard>

              <UCard>
                <template #header>Description</template>
                <div class="space-y-3">
                  <UFormField label="Subject" name="subject">
                    <UInput
                      v-model="form.subject"
                      placeholder="Enter a short summary of the issue"
                      class="w-full"
                      @change="onSubjectChange"
                    />
                  </UFormField>
                  <UFormField label="Detailed Description" name="description">
                    <UTextarea
                      v-model="form.description"
                      :rows="6"
                      placeholder="Provide detailed description of the problem"
                      class="w-full"
                    />
                  </UFormField>
                </div>
              </UCard>
            </div>

            <div class="space-y-4">
              <UCard>
                <template #header>Suggested Solutions</template>
                <div class="grid grid-cols-1 gap-3">
                  <div
                    v-if="suggestions.length === 0"
                    class="text-sm text-muted"
                  >
                    Start typing a description to see suggestions.
                  </div>

                  <UModal
                    v-for="s in suggestions"
                    :key="s.id"
                    title="Suggested Solution Details"
                    :close="{
                      color: 'primary',
                      variant: 'outline',
                      class: 'rounded-full',
                    }"
                  >
                    <UButton
                      label="Open"
                      color="neutral"
                      variant="link"
                      class="p-0"
                    >
                      <UCard
                        class="border-default cursor-pointer hover:bg-elevated/50 text-left"
                      >
                        <div class="flex items-start gap-3">
                          <div class="flex-1">
                            <p class="font-bold">{{ s.title }}</p>
                            <p class="text-sm text-muted font-normal">
                              {{ s.summary }}
                            </p>
                            <div class="flex flex-wrap gap-1 mt-1">
                              <UBadge
                                v-for="t in s.tags"
                                :key="t"
                                color="neutral"
                                variant="soft"
                                >{{ t }}</UBadge
                              >
                            </div>
                          </div>
                        </div>
                      </UCard>
                    </UButton>

                    <template #body>
                      <h3 class="text-lg font-semibold mb-2">{{ s.title }}</h3>
                      <p class="mb-4">{{ s.summary }}</p>
                      <div class="flex flex-wrap gap-2">
                        <UBadge
                          v-for="t in s.tags"
                          :key="t"
                          color="neutral"
                          variant="soft"
                          >{{ t }}</UBadge
                        >
                      </div>
                    </template>
                  </UModal>
                </div>
              </UCard>

              <UCard>
                <template #header>Ticket Assignment</template>
                <div class="space-y-3">
                  <UFormField label="Assigned To" name="assignedTo">
                    <UInputMenu
                      v-model="form.assignedTo"
                      :items="assigneeItems"
                      value-key="value"
                      label-key="label"
                      multiple
                      placeholder="Select one or more agents"
                      class="w-full"
                    />
                  </UFormField>
                </div>
              </UCard>
            </div>
          </div>
        </UForm>
      </UContainer>
    </template>

    <template #footer>
      <UDashboardToolbar
        class="h-(--ui-header-height) justify-end border-t border-default shrink-0 flex items-center border-b px-4 sm:px-6 gap-1.5 bg-elevated/25"
      >
        <UButton color="secondary" class="cursor-pointer" @click="saveDraft">
          <Icon name="i-lucide-save" /> Save as Draft
        </UButton>
        <UButton color="primary" class="cursor-pointer" @click="onSubmit">
          <Icon name="i-lucide-send" /> Publish Ticket
        </UButton>
      </UDashboardToolbar>
    </template>
  </UDashboardPanel>
</template>

<script setup lang="ts">
import { storeToRefs } from "pinia";
import type { Customer, FormError, MenuItem, Suggestion } from "../../types";
import { useCustomersStore } from "../../../stores/customers";
import { useSuggestedSolutionsStore } from "../../../stores/suggestedSolutions";
import { useSystemStore } from "../../../stores/system";
import { useUsersStore } from "../../../stores/users";

definePageMeta({
  layout: "dashboard",
  auth: true,
});

useSeoMeta({
  title: "New Ticket",
});

const form = reactive({
  customerFirstName: "",
  customerLastName: "",
  customerEmail: "",
  customerPhone: "",
  category: "",
  priority: "Medium",
  status: "Open",
  source: "Phone",
  subject: "",
  description: "",
  stepsTaken: "",
  attachments: "",
  assignedTo: [],
  dueBy: "",
});

const systemStore = useSystemStore();
const customersStore = useCustomersStore();
const suggestedSolutionsStore = useSuggestedSolutionsStore();
const usersStore = useUsersStore();

const { categories, priorities, statuses, sources } = storeToRefs(systemStore);
const { items: customers } = storeToRefs(customersStore);
const { items: suggestionCatalog } = storeToRefs(suggestedSolutionsStore);
const { items: users } = storeToRefs(usersStore);

const categoryItems = computed(() =>
  categories.value.map((c) => ({ label: c, value: c }))
);
const priorityItems = computed(() =>
  priorities.value.map((p) => ({ label: p, value: p }))
);
const statusItems = computed(() =>
  statuses.value.map((s) => ({ label: s, value: s }))
);
const sourceItems = computed(() =>
  sources.value.map((s) => ({ label: s, value: s }))
);

const assigneeItems = computed(() =>
  users.value.map((u) => ({
    label: `${u.firstName} ${u.lastName}`,
    value: u.id,
  }))
);

const customerDirectory = computed<Record<string, Customer>>(() => {
  const map: Record<string, Customer> = {};
  customers.value.forEach((c) => {
    map[c.email] = {
      firstName: c.firstName,
      lastName: c.lastName,
      phone: c.phone ?? "",
    };
  });
  return map;
});

const customerEmailItems = computed<MenuItem[]>(() =>
  customers.value.map((c) => ({ label: c.email, value: c.email }))
);

// Reactive list shown in the UI, populated by matcher
const suggestions = ref<Suggestion[]>([]);

function addCategoryItem(item: string) {
  if (!categories.value.includes(item)) {
    systemStore.setCategories([...categories.value, item]);
  }
  form.category = item;
}

function addPriorityItem(item: string) {
  if (!priorities.value.includes(item)) {
    systemStore.setPriorities([...priorities.value, item]);
  }
  form.priority = item;
}

function addStatusItem(item: string) {
  if (!statuses.value.includes(item)) {
    systemStore.setStatuses([...statuses.value, item]);
  }
  form.status = item;
}

function addSourceItem(item: string) {
  if (!sources.value.includes(item)) {
    systemStore.setSources([...sources.value, item]);
  }
  form.source = item;
}

function addCustomerEmail(item: string) {
  const existing = customers.value.find((c) => c.email === item);
  if (!existing) {
    customersStore.upsertCustomer({
      id: `cust-${Date.now()}`,
      email: item,
      firstName: "",
      lastName: "",
      phone: "",
    });
  }
  form.customerEmail = item;
}

function onSubjectChange() {
  const pool: Suggestion[] = [];

  for (const s of suggestionCatalog.value) {
    const matches = s.tags.some((t) =>
      form.subject.toLowerCase().includes(t.toLowerCase())
    );
    if (matches) pool.push(s);
  }

  // De-duplicate by id in case multiple tags match the same suggestion
  const unique = new Map(pool.map((p) => [p.id, p]));
  suggestions.value = Array.from(unique.values());
}

function validate(): FormError[] {
  const errors: FormError[] = [];
  if (!form.subject)
    errors.push({ path: "subject", message: "Subject is required" });
  if (!form.description)
    errors.push({ path: "description", message: "Description is required" });
  if (!form.category)
    errors.push({ path: "category", message: "Select a category" });
  if (!form.status) errors.push({ path: "status", message: "Select a status" });
  return errors;
}

async function onSubmit() {
  const payload = { ...form, createdAt: new Date().toISOString() };
  await $fetch("/api/tickets", { method: "POST", body: payload }).catch(
    () => {}
  );
}

async function saveDraft() {
  const payload = {
    ...form,
    isDraft: true,
    savedAt: new Date().toISOString(),
  };
  await $fetch("/api/tickets/drafts", { method: "POST", body: payload }).catch(
    () => {}
  );
}

function onReset() {
  Object.assign(form, {
    customerFirstName: "",
    customerLastName: "",
    customerEmail: "",
    customerPhone: "",
    category: "",
    priority: "Medium",
    status: "Open",
    source: "Phone",
    subject: "",
    description: "",
    stepsTaken: "",
    attachments: "",
    assignedTo: [],
    dueBy: "",
  });
  suggestions.value = [];
}
</script>
