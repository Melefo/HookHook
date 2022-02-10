<template>
  <Listbox v-model="selectedPerson">
    <ListboxButton>{{ selectedPerson.name }}</ListboxButton>
    <ListboxOptions>
      <!-- Use the `active` state to conditionally style the active option. -->
      <!-- Use the `selected` state to conditionally style the selected option. -->
      <ListboxOption
        as="template"
        v-slot="{ active, selected }"
        v-for="person in people"
        :key="person"
        :value="person"
      >
        <li
          :class="{
            'bg-blue-500 text-white': active,
            'bg-white text-black': !active
          }"
        >
          <CheckIcon v-show="selected" />
          {{ person.name }}
        </li>
      </ListboxOption>
    </ListboxOptions>
  </Listbox>
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