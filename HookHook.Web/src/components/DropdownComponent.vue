<template>
  <div>
    <p class="text-white p-2">AREA name : {{ selectedPerson.name }}</p>
    <Listbox v-model="selectedPerson">
      <div class="relative mt-1">
        <span class="text-white">When : </span>
        <ListboxButton class="relative p-2 text-left text-white rounded-lg">
          <span class="block truncate underline decoration-[#FD9524] underline-offset-4">{{ selectedPerson.name }}</span>
          <span class="absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none"/>
        </ListboxButton>
        <transition
          leave-active-class="transition duration-100 ease-in"
          leave-from-class="opacity-100"
          leave-to-class="opacity-0"
        >
          <ListboxOptions class="absolute py-1 mt-1 ml-[8%] overflow-auto text-base bg-white rounded-md shadow-lg max-h-60 ring-1 ring-black ring-opacity-5 focus:outline-none sm:text-sm">
            <ListboxOption
              v-slot="{ active, selected }"
              v-for="person in people"
              :key="person.name"
              :value="person"
              as="template"
            >
              <li
                :class="[
                  active ? 'text-black bg-[#FD9524]' : 'text-gray-900',
                  'cursor-default select-none relative py-2 pl-4 pr-4',
                ]"
              >
                <span
                  :class="[
                    selected ? 'font-medium' : 'font-normal',
                    'block truncate',
                  ]"
                  >{{ person.name }}</span
                >
              </li>
            </ListboxOption>
          </ListboxOptions>
        </transition>
      </div>
    </Listbox>
    <p>test</p>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { Listbox, ListboxButton, ListboxOptions, ListboxOption } from '@headlessui/vue';

export default defineComponent({
  name: 'DropdownComponent',
  components: { Listbox, ListboxButton, ListboxOptions, ListboxOption },
  methods: {
  },
  computed: {
  },
  setup() {
    const people = [
      { id: 1, name: 'Durward Reynolds', unavailable: false },
      { id: 2, name: 'Kenton Towne', unavailable: false },
      { id: 3, name: 'Therese Wunsch', unavailable: false },
      { id: 4, name: 'Benedict Kessler', unavailable: true },
      { id: 5, name: 'Katelyn Rohan', unavailable: false },
    ]
    const selectedPerson = ref(people[0])
    return {
      people,
      selectedPerson,
    }
    },
});
</script>