<template>
  <div>
    <Bloc v-for="(slide, key) in blocs" :key="key" class="dark:text-white text-black flex max-h-full flex-col justify-between my-2 sm:my-0">
      <div>{{ slide.name }}</div>
      <div class="flex flex-row items-center my-2">
        <div class="flex w-[40px] h-[40px] rounded-xl" :style="{ 'background-color': color(slide.from) }">
          <img class="w-7 h-7 m-auto" :src="require(`@/assets/img/coloredsvg/${slide.from.toLowerCase()}.svg`)"/>
        </div>
        <ArrowNarrowRightIcon class="h-8 dark:text-white text-black mx-1" />
        <div class="gap-2 grid grid-cols-4">
          <div v-for="(to, keyy) in slide.to" :key="keyy" class="flex w-[40px] h-[40px] rounded-xl" :style="{ 'background-color': color(to) }">
            <img class="w-7 h-7 m-auto" :src="require(`@/assets/img/coloredsvg/${to.toLowerCase()}.svg`)"/>
          </div>
        </div>
      </div>
      <div>
        <div>{{ formatDate(slide.date * 1000) }}</div>
        <div class="flex flex-row justify-end">
          <button @click.prevent="async () => await trigger(slide.id)">
            <RefreshIcon class="h-10 dark:bg-[#181A1E] bg-[#f0f0f0] dark:text-[#f0f0f0] text-[#181A1E] rounded-md p-1.5 mx-2 duration-500 hover:scale-105" />
          </button>
          <button @click.prevent="async () => await deleteArea(slide.id, key)" >
            <TrashIcon class="h-10 dark:bg-[#181A1E] bg-[#f0f0f0] dark:text-[#f0f0f0] text-[#181A1E] rounded-md p-1.5 mx-2 duration-500 hover:scale-105" />
          </button>
        </div>
      </div>
    </Bloc>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import Bloc from "@/components/BlocComponent.vue";
import { RefreshIcon, ArrowNarrowRightIcon } from "@heroicons/vue/outline"
import { TrashIcon } from "@heroicons/vue/solid"
import dayjs from 'dayjs';
import { mapActions } from 'vuex';

export default defineComponent({
  name: 'CarouselComponent',
  components: {
    Bloc, RefreshIcon, TrashIcon, ArrowNarrowRightIcon
  },
  computed: {
    blocs() {
      const that: any = this;
      return that.$store.state.area.areas;
    }
  },
  methods: {
    ...mapActions("area", ["get", "delete", "trigger"]),
    formatDate(time: number) {
      return dayjs(time).format("D MMMM YYYY HH:mm");
    },
    color(name: string) {
      name = name.toLowerCase();
      switch (name) {
        case 'twitter':
          return "#A3E7EE";
        case 'spotify':
          return "#B4E1DC";
        case 'discord':
          return "#D9D1EA";
        case 'github':
          return "#F5CDCB";
        case 'google':
          return "#F8CBAA";
        case 'twitch':
          return "#FFFFC7";
      }
    },
    async deleteArea(id: string, key: number) {
      await this.delete(id);
      this.blocs.splice(key, 1);
    }
  },
  created: async function() {
    await this.get();
  }
});
</script>