<template>
  <div>
    <Switch
        @click="this.$emit('ToggleDarkMode')"
      v-model="enabled"
      class="bg-[#3B3F43] relative inline-flex flex-shrink-0 h-[38px] w-[90px] border-2 border-transparent rounded-xl cursor-pointer"
    >
      <span class="sr-only">Use setting</span>
      <span
        aria-hidden="true"
        :class="enabled ? 'translate-x-12' : 'translate-x-1'"
        class="pointer-events-none h-[34px] w-[34px] text-white transition ease-in-out duration-200"
      >
      <SunIcon v-if="!enabled" />
      <MoonIcon v-else />
      </span>
    </Switch>
  </div>
</template>

<script lang="ts">
import { ref } from 'vue'
import { Switch } from '@headlessui/vue'
import { SunIcon, MoonIcon } from '@heroicons/vue/solid'
import { defineComponent } from 'vue'

export default defineComponent({
    components: { Switch, SunIcon, MoonIcon },
    setup() {
        const enabled = ref(false)

        return { enabled }
    }
})
</script>
