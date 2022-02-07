<template>
  <div
    class="
      transition-colors
      duration-200
      bg-[#F9F9F9]
      dark:bg-[#181A1E]
      min-h-screen min-w-screen
      p-10
    "
  >
    <header class="flex flex-col sm:flex-row justify-between">
      <div class="w-[90px]"/>
      <RouterLink to="/">
        <HookHook class="text-9xl" />
      </RouterLink>
      <div class="mt-6 sm:mt-0">
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