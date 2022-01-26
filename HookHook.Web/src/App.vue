<template>
  <div :class="{ dark: darkMode }">
    <component :is='layout' @ToggleDarkMode="ToggleDarkMode">
      <router-view />
    </component>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "App",
  data() {
    return {
      darkMode: window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches,
    };
  },
  methods: {
    ToggleDarkMode() {
      this.darkMode = !this.darkMode;
    },
  },
  computed: {
    layout() {
      return this.$route.meta?.layout || 'DefaultLayout';
    }
  }
});
</script>