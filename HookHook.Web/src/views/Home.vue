
<template>
  <div class="bg-[#F9F9F9] dark:bg-[#181A1E] fixed h-screen w-screen p-10">
    <div class="flex justify-between">
      <div>
        <h1 class="font-bold text-5xl text-black dark:text-white">
          H<span class="text-[#F09113] inline-block">oo<img class="m-auto mt-[-10px]" :src="Orange" /></span>kH<span class="text-[#A3E7EE]">oo</span>k
        </h1>
      </div>
      <div>
        <Switch @ToggleDarkMode="this.$emit('ToggleDarkMode')" />
      </div>
    </div>
  </div>
</template>

<script>
import Switch from "@/components/Switch.vue";
import Orange from "@/assets/img/orangeBeak.svg";

export default {
  components: { Switch },
};
</script>