<script setup lang="ts">
import * as z from "zod";
import type { FormSubmitEvent } from "@nuxt/ui";
import type { ProfileForm } from "../../../types";

definePageMeta({
  layout: "dashboard",
  auth: true,
});

useSeoMeta({
  title: "General Settings",
});

const colorMode = useColorMode();

const profileSchema = z.object({
  name: z.string().min(2, "Too short"),
  email: z.string().email("Invalid email"),
  username: z.string().min(2, "Too short"),
  role: z.string(),
});

type ProfileSchema = z.output<typeof profileSchema>;

const profile = reactive<Partial<ProfileForm>>({
  // TODO: replace with actual user data once wired up
  name: "John Doe",
  email: "john.doe@example.com",
  username: "johndoe",
  role: "Helpdesk Agent",
});

const toast = useToast();
async function onSubmit(event: FormSubmitEvent<ProfileSchema>) {
  // TODO: call your API to persist changes
  console.log(event.data);

  toast.add({
    title: "Success",
    description: "Your settings have been updated.",
    icon: "i-lucide-check",
    color: "success",
  });
}
</script>

<template>
  <UForm
    id="settings"
    :schema="profileSchema"
    :state="profile"
    @submit="onSubmit"
  >
    <UPageCard
      title="General settings"
      description="Manage your basic account information."
      variant="naked"
      orientation="horizontal"
      class="mb-4"
    />

    <UPageCard variant="subtle" class="mb-6">
      <!-- existing fields... -->
      <UFormField
        name="name"
        label="Name"
        description="Will appear on receipts, invoices, and other communication."
        required
        class="flex max-sm:flex-col justify-between items-start gap-4"
      >
        <UInput v-model="profile.name" autocomplete="off" />
      </UFormField>

      <USeparator />

      <UFormField
        name="email"
        label="Email"
        description="Used to sign in and receive updates."
        required
        class="flex max-sm:flex-col justify-between items-start gap-4"
      >
        <UInput v-model="profile.email" type="email" autocomplete="off" />
      </UFormField>

      <USeparator />

      <UFormField
        name="username"
        label="Username"
        description="Your unique username for logging in and your profile URL."
        required
        class="flex max-sm:flex-col justify-between items-start gap-4"
      >
        <UInput v-model="profile.username" autocomplete="off" />
      </UFormField>

      <USeparator />

      <UFormField
        name="role"
        label="Role"
        description="Your current role. This cannot be changed."
        class="flex max-sm:flex-col justify-between items-start gap-4"
      >
        <UInput v-model="profile.role" disabled readonly />
      </UFormField>
    </UPageCard>

    <!-- Appearance section -->
    <UPageCard
      title="Appearance"
      description="Choose how the helpdesk interface looks."
      variant="naked"
      orientation="horizontal"
      class="mb-4"
    />

    <UPageCard variant="subtle" class="mb-6">
      <UFormField
        name="appearance"
        label="Theme"
        description="Switch between light, dark, or system preference."
        class="flex max-sm:flex-col justify-between items-start gap-4"
        :ui="{
          container: 'my-auto',
        }"
      >
        <URadioGroup
          v-model="colorMode.preference"
          :items="[
            { label: 'Light', value: 'light' },
            { label: 'Dark', value: 'dark' },
            { label: 'System', value: 'system' },
          ]"
          class="flex flex-col gap-2 my-auto"
          orientation="horizontal"
          variant="table"
        />
      </UFormField>
    </UPageCard>
  </UForm>
</template>
