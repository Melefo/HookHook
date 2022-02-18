<template>
    <div class="mt-8 gridCreator text-black dark:text-white">
      <div class="sm:grid grid-cols-5 gap-4">
        <p class="col-span-2">Creator</p>
        <Bloc class="rounded-xl text-black col-span-2 min-h-[75vh] row-start-2">
          <DropdownComponent/>
        </Bloc>
        <p class="col-span-3">Your AREAs</p>
        <CarouselComponent class="col-span-3 row-start-2 sm:grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-4 overflow-auto sm:max-h-[75vh]" />
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