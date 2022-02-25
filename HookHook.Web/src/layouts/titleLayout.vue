<template>
  <div
    class="
      transition-colors
      duration-200
      bg-[#f0f0f0]
      dark:bg-[#181A1E]
      min-h-screen min-w-screen
      p-10
    "
  >
    <header class="flex flex-row justify-between">
      <div class="w-[90px] sm:flex hidden"/>
      <RouterLink to="/">
        <HookHook class="lg:text-9xl md:text-7xl sm:text-6xl text-4xl" />
      </RouterLink>
      <div class="mt-0">
        <Switch @ToggleDarkMode="this.$emit('ToggleDarkMode')" />
      </div>
    </header>
    <slot />
  </div>
</template>

<script lang="ts">
import Switch from "@/components/SwitchComponent.vue";
import HookHook from "@/components/HookHookComponent.vue";

import { defineComponent } from "vue";

export default defineComponent({
  components: { Switch, HookHook },
});
</script>