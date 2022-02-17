<template>
    <div class="mt-8 gridCreator text-black dark:text-white">
      <p class="creatorTitle">Creator</p>
      <p class="urAreaTitle">Your AREAs</p>
      <div class="grid grid-cols-5 gap-4">
        <Bloc class="creatorBG rounded-xl text-black col-span-2 min-h-[75vh]">
          <DropdownComponent/>
        </Bloc>
        <CarouselComponent class="col-span-3 grid grid-cols-4 gap-4 overflow-auto max-h-[75vh]" />
      </div>
    </div>
</template>

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