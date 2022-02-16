<template>
    <div class="mt-8 gridCreator text-black dark:text-white">
      <p class="creatorTitle">Creator</p>
      <p class="urAreaTitle">Your AREAs</p>
      <!--AREA CREATOR-->
      <Bloc class="creatorBG rounded-xl text-black">
        <!-- TEST DROPDOWN HEADLESS UI-->
        <DropdownComponent/>
      </Bloc>
      <!--MY AREA-->
      <CarouselComponent/>
    </div>
</template>

<style>
    .button {
      border-radius: 10px;
      width: 60px;
      margin: 0px 10px 5px 10px;
      padding: 10px 0;
      transition-duration: 1s;
    }
    .button:hover {
      transform: scale(1.05);
    }
    .gridCreator {
      display: grid;
      height: 750px;
      grid-template-columns: 40% 60%;
      grid-template-rows: 4% 47% 47%;
      gap: 1%;
    }
    .creatorTitle {
      grid-column: 1 / span 1;
      grid-row: 1 / span 1;
    }
    .urAreaTitle {
      grid-column: 2 / span 1;
      grid-row: 1 / span 1;
    }
    .creatorBG {
      grid-column: 1 / span 1;
      grid-row: 2 / span 2;
    }
    .urAreaBG {
      grid-column: 2 / span 1;
      grid-row: 2 / span 2;
      flex-flow: column wrap;
    }
</style>

<script lang="ts">
import { defineComponent } from "vue";

import CarouselComponent from "@/components/CarouselComponent.vue";
import DropdownComponent from "@/components/DropdownComponent.vue";
import Bloc from "@/components/BlocComponent.vue";

export default defineComponent({
  components: { CarouselComponent, DropdownComponent, Bloc },
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