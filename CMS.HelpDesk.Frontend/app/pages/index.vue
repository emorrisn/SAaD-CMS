<template>
  <div>
    <LandingLine />
    <div
      class="w-full max-w-(--ui-container) mx-auto px-4 sm:px-6 lg:px-8 flex flex-col lg:grid py-24 sm:py-32 lg:py-40 gap-16 sm:gap-y-24 md:pt-18 lg:pt-20"
      data-slot="container"
    >
      <!--[-->
      <div data-slot="wrapper" class="text-center">
        <div data-slot="header" class="">
          <h1
            data-slot="title"
            class="text-5xl sm:text-7xl text-pretty tracking-tight font-bold text-highlighted max-w-3xl mx-auto"
          >
            <div class="">Help Desk Portal</div>
          </h1>
          <div
            data-slot="description"
            class="text-lg sm:text-xl/8 text-muted text-balance mt-6"
          >
            A modern help desk solution to streamline your customer support
            experience.
          </div>
        </div>
        <div data-slot="footer" class="mt-10">
          <div
            data-slot="links"
            class="flex flex-wrap gap-x-6 gap-y-3 justify-center"
          >
            <NuxtLink
              href="/dashboard/home"
              rel="noopener noreferrer"
              data-slot="base"
              class="rounded-md font-medium inline-flex items-center disabled:cursor-not-allowed aria-disabled:cursor-not-allowed disabled:opacity-75 aria-disabled:opacity-75 transition-colors px-3 py-2 text-base gap-2 text-inverted bg-primary hover:bg-primary/75 active:bg-primary/75 disabled:bg-primary aria-disabled:bg-primary focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-primary"
            >
              <span data-slot="label" class="truncate">Go to Dashboard</span>
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: "app",
  auth: false,
});

const { status } = useAuth();

watch(
  () => status.value,
  (value) => {
    if (value === "authenticated") {
      return navigateTo("/dashboard/home");
    }
  },
  { immediate: true }
);
</script>
