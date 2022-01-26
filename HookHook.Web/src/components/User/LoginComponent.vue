
<template>
  <form class="flex flex-col">
    <Input class="pb-5" type="text" label="Username/Email" :reverse="true" />
    <Input class="pb-5" type="password" label="Password" :reverse="true" />
    <Input type="submit" value="Se connecter" :reverse="true" />
  </form>
</template>

<script lang="ts">
import Input from "@/components/InputComponent.vue";
import { defineComponent } from "vue";

export default defineComponent({
  components: { Input },
});
</script>