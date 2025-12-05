<script setup lang="ts">
import * as z from "zod";
import type { FormSubmitEvent } from "@nuxt/ui";
import type { PasswordForm } from "../../../types";

definePageMeta({
  layout: "dashboard",
  auth: true,
});

useSeoMeta({
  title: "Security Settings",
});

const { signOut } = useAuth();

const passwordSchema = z
  .object({
    newPassword: z.string().min(8, "Password must be at least 8 characters"),
    confirmPassword: z
      .string()
      .min(8, "Password must be at least 8 characters"),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "Passwords do not match",
  });

type PasswordSchema = z.output<typeof passwordSchema>;

const passwordState = reactive<Partial<PasswordForm>>({
  newPassword: "",
  confirmPassword: "",
});

const toast = useToast();
async function onSubmit(event: FormSubmitEvent<PasswordSchema>) {
  // TODO: call your API to change password
  console.log(event.data);

  toast.add({
    title: "Success",
    description: "Your password has been updated.",
    icon: "i-lucide-check",
    color: "success",
  });
}

async function handleLogout() {
  await signOut({ callbackUrl: "/" });
}

async function handleDeleteAccount() {
  // TODO: implement delete account flow (confirmation + API call)
  toast.add({
    title: "Not implemented",
    description: "Account deletion is not yet available.",
    color: "warning",
  });
}
</script>

<template>
  <UForm
    id="security-settings"
    :schema="passwordSchema"
    :state="passwordState"
    @submit="onSubmit"
  >
    <UPageCard
      title="Change password"
      description="Update the password used to access your account."
      variant="naked"
      orientation="horizontal"
      class="mb-4"
    />

    <UPageCard variant="subtle" class="mb-6">
      <UFormField
        name="newPassword"
        label="New password"
        description="Use at least 8 characters with a mix of letters, numbers, and symbols."
        required
        class="flex max-sm:flex-col justify-between items-start gap-4"
      >
        <UInput
          v-model="passwordState.newPassword"
          type="password"
          autocomplete="new-password"
          class="w-full"
        />
      </UFormField>

      <USeparator />

      <UFormField
        name="confirmPassword"
        label="Confirm password"
        description="Enter the same password again to confirm."
        required
        class="flex max-sm:flex-col justify-between items-start gap-4"
      >
        <UInput
          v-model="passwordState.confirmPassword"
          type="password"
          autocomplete="new-password"
          class="w-full"
        />
      </UFormField>
    </UPageCard>

    <!-- Actions section -->
    <UPageCard
      title="Actions"
      description="Manage critical account actions."
      variant="naked"
      orientation="horizontal"
      class="mb-4"
    />

    <UPageCard variant="subtle">
      <UFormField
        name="logout"
        label="Log out"
        description="Log out of your account on this device."
        class="flex max-sm:flex-col justify-between items-start gap-4"
      >
        <UButton
          color="neutral"
          variant="outline"
          icon="i-lucide-log-out"
          @click="handleLogout"
        >
          Logout
        </UButton>
      </UFormField>
      <USeparator />
      <UFormField
        name="deleteAccount"
        label="Delete account"
        description="Permanently delete your account and all associated data."
        class="flex max-sm:flex-col justify-between items-start gap-4"
      >
        <UButton
          color="error"
          variant="outline"
          icon="i-lucide-trash-2"
          @click="handleDeleteAccount"
        >
          Delete account
        </UButton>
      </UFormField>
    </UPageCard>
  </UForm>
</template>
