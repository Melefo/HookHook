<template>
    <div>
        <div class="text-black text-2xl mt-8 dark:text-white text-left">
            <p class="welcomingMessage">Hello <span class="userName">{{ firstName }}</span>, welcome back !</p>
        </div>
        <div class="text-black dark:text-white mt-8">
            <p>Services</p>
        </div>
        <!-- // todo centrer et colorier les svg pliz -->
        <OAuthButtonsComponent/>
        <p class="creatorTitle">Creator</p>
        <p class="urAreaTitle">Your AREAs</p>
        <!--AREA CREATOR-->
        <div class="creatorBG rounded-xl bg-[#3B3F43] text-black overflow-y-scroll">
            <!-- TEST DROPDOWN HEADLESS UI-->
            <AreaCreatorComponent/>
            <CarouselComponent class="col-span-3 row-start-2 sm:grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-4 overflow-auto sm:max-h-[75vh]" />
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import CarouselComponent from "@/components/CarouselComponent.vue";
import AreaCreatorComponent from '@/components/AreaCreatorComponent.vue'
import OAuthButtonsComponent from "@/components/OAuthButtonsComponent.vue";

export default defineComponent({
  components: { CarouselComponent, OAuthButtonsComponent, AreaCreatorComponent },
  methods: {

  },
  computed: {
  },
  data() {
    return {
      filters: ["Use a tweet reaction", "Retweet a tweet from a specific user"],
      selectedValue: null,
    }
  },
});
</script>