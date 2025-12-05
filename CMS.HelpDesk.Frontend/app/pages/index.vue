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
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: "app",
  auth: false,
});

useSeoMeta({
  title: "Help Desk Portal",
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
