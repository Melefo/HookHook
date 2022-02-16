<template>
  <div>
    <Bloc v-for="(slide, key) in blocs" :key="key" class="dark:text-white text-black">
      <div>{{ slide.name }}</div>
      <div class="flex flex-row">
        <img class="w-10 h-10 m-auto" :src="`@/assets/img/coloredsvg/${slide.from}.svg`"/>
        <ArrowNarrowRightIcon class="h-8 text-white" />
        <div class="flex flex-col">
        <img v-for="(to, key) in slide.to" :key="key" class="w-10 h-10 m-auto" :src="`@/assets/img/coloredsvg/${to}.svg`"/>
        </div>
      </div>
      <div>{{ slide.date }}</div>
      <div class="flex flex-row">
        <RefreshIcon class="h-8 text-white" />
        <PencilIcon class="h-8 text-white" />
        <TrashIcon class="h-8 text-white" />
      </div>
    </Bloc>
  </div>
</template>

<script>
import { defineComponent } from 'vue';
import Bloc from "@/components/BlocComponent.vue";
import { RefreshIcon, ArrowNarrowRightIcon } from "@heroicons/vue/outline"
import { PencilIcon, TrashIcon } from "@heroicons/vue/solid"

export default defineComponent({
  name: 'CarouselComponent',
  components: {
    Bloc, RefreshIcon, PencilIcon, TrashIcon, ArrowNarrowRightIcon
  },
  data: () => ({
      blocs: Array(100).fill(
        { name: "Twitter on Push", from: "GitHub", to: [ "Twitter", "Twitter" ], date: "25 decembre 10h43" }
      )
  }),
});
</script>