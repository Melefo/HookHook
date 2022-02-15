<template>
  <div>
    <Listbox v-model="selectedService">
      <div class="relative mt-1">
        <ListboxButton class="relative p-2 text-left text-white rounded-lg">
          <span class="block truncate underline decoration-[#FD9524] underline-offset-4">{{ selectedService !== null ? selectedService.name : 'Select a service' }}<img v-if="serviceChose === null" class="w-10 h-10 m-auto" :src="require(`@/assets/img/${serviceChose.name}.svg`)"/></span>
          <span class="absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none"/>
        </ListboxButton>
        <transition
          leave-active-class="transition duration-100 ease-in"
          leave-from-class="opacity-100"
          leave-to-class="opacity-0"
        >
          <ListboxOptions class="absolute py-1 mt-1 ml-[8%] overflow-auto text-base bg-white rounded-md shadow-lg max-h-60 ring-1 ring-black ring-opacity-5 focus:outline-none sm:text-sm">
            <ListboxOption
              v-slot="{ active }"
              v-for="serviceChose in service"
              :key="serviceChose.name"
              :value="serviceChose"
              as="template"
              class="bg-black"
              @click='$emit("actionChange", serviceChose)'
            >
              <li
                :class="[
                  active ? 'text-black bg-[#FD9524]' : 'text-gray-900',
                  'cursor-default select-none relative py-2 pl-4 pr-4 bg-black',
                ]"
              >
                <img class="w-10 h-10 m-auto" :src="require(`@/assets/img/${serviceChose.name}.svg`)"/>
              </li>
            </ListboxOption>
          </ListboxOptions>
        </transition>
      </div>
    </Listbox>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { Listbox, ListboxButton, ListboxOptions, ListboxOption } from '@headlessui/vue';
import { mapActions } from "vuex";

export default defineComponent({
  name: "ActionComponent",
  components: { Listbox, ListboxButton, ListboxOptions, ListboxOption },
  methods: {
  ...mapActions("about", ["get"]),
    color(name: string) {
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
    test() {
        console.log("testttt");
    }
  },
  computed: {
  },
  data: function() {
    return {
        service: [],
        serviceChose: "",
        selectedService: null,
    }
  },
  created: async function() {
    const { server: { services } } = await this.get();
    this.service = services;
  },
});
</script>