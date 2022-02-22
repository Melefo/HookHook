<template>
  <div class="mt-2">
    <span class="dark:text-white text-black">{{ verb }} : </span>
    <div class="flex flex-row items-center">
      <ActionComponent @actionChange="changeOptions" />
      <Listbox v-if="possibleServices.length > 0" v-model="selectedPerson">
        <div class="">
          <ListboxButton
            class="relative p-2 text-left text-black dark:text-white"
          >
            <span
              class="block border-0 border-b-2 border-[#FD9524]"
              v-if="selectedPerson !== null"
              >{{ selectedPerson.username }}</span
            >
            <span class="block border-0 border-b-2 border-[#FD9524]" v-else
              >Select an account</span
            >
          </ListboxButton>

          <transition
            leave-active-class="transition duration-100 ease-in"
            leave-from-class="opacity-100"
            leave-to-class="opacity-0"
          >
            <ListboxOptions
              class="
                absolute
                py-1
                overflow-auto
                z-10
                text-base
                bg-white
                rounded-md
                shadow-lg
                max-h-60
                ring-1 ring-black ring-opacity-5
                focus:outline-none
                sm:text-sm
              "
            >
              <ListboxOption
                v-slot="{ active }"
                v-for="person in people"
                :key="person.userId"
                :value="person"
                as="template"
              >
              <li class="cursor-pointer select-none relative py-2 pl-4 pr-4 hover:bg-[#EEEEEE]">
                <span :class="[active ? 'text-[#F09113]' : '']" class="relative py-2 pl-4 pr-4 hover:text-[#A3E7EE]">{{ person.username }}</span>
              </li>
              </ListboxOption>
            </ListboxOptions>
          </transition>
        </div>
      </Listbox>
      <Listbox v-if="selectedPerson != null">
        <div>
        <ListboxButton class="relative p-2 text-left text-black dark:text-white">
          <span
            v-if='currentService !== null && currentService.description !== ""'
            class="block border-0 border-b-2 border-[#FD9524]"
            >{{ currentService.description }}</span>
            <span v-else class="block border-0 border-b-2 border-[#FD9524]">{{ areaType }}</span>
        </ListboxButton>
        <ListboxOptions
          class="
                absolute
                py-1
                overflow-auto
                z-10
                text-base
                bg-white
                rounded-md
                shadow-lg
                max-h-60
                ring-1 ring-black ring-opacity-5
                focus:outline-none
                sm:text-sm
          "
        >
          <ListboxOption
            v-slot="{ active }"
            v-for="possibleService in possibleServices"
            :key="possibleService.name"
            :value="possibleService.name"
            as="template"
            @click="serviceSelected(possibleService)"
            >
              <li class="cursor-pointer select-none relative py-2 pl-4 pr-4 hover:bg-[#EEEEEE]">
                <span :class="[ active ? 'text-[#F09113]' : '']" class="relative py-2 pl-4 pr-4 hover:text-[#A3E7EE]">{{ possibleService.description }}</span>
              </li>
          </ListboxOption>
        </ListboxOptions>
        </div>
      </Listbox>
    </div>
    <div v-for="(parameter, i) in currentParameters" :key="parameter">
      <div class="text-white">
        {{ parameter + ":" }}
      </div>
      <input
        v-model="paramsToSend[i]"
        @keyup="
          $emit('updateInfo', {
            type: currentService.name,
            params: paramsToSend,
            index: reactionIndex,
          })
        "
      />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import {
  Listbox,
  ListboxButton,
  ListboxOptions,
  ListboxOption,
} from "@headlessui/vue";
import ActionComponent from "@/components/ActionComponent.vue";
import { mapActions } from "vuex";

export default defineComponent({
  props: ["serviceDetails", "verb", "areaType", "reactionIndex"],
  name: "DropdownComponent",
  components: {
    Listbox,
    ListboxButton,
    ListboxOptions,
    ListboxOption,
    ActionComponent,
  },
  methods: {
    ...mapActions("service", ["getAccounts"]),
    ...mapActions("area", ["getServices"]),
    async changeOptions(newAction: any) {
      let appropriateActions =
        this.areaType === "Action" ? newAction.actions : newAction.reactions;
      this.possibleServices = [];

      for (let i = 0; i < appropriateActions.length; i++) {
        const action = appropriateActions[i];

        // * find the service in the service details
        const actionDetail = this.serviceDetails.find(
          (x: any) => x["className"] === action.name
        );

        // * if the types are the same, add to possibles

        if (
          this.areaType.toLowerCase() === actionDetail.areaType.toLowerCase()
        ) {
          this.possibleServices.push(action);
        }
      }
      this.selectedPerson = null;
      this.people = await this.getAccounts(newAction.name);
    },
    serviceSelected(service: any) {
      this.currentService = service;
      for (let i = 0; i < this.serviceDetails.length; i++) {
        const serviceDetail = this.serviceDetails[i];

        if (serviceDetail["className"] === service.name) {
          this.currentParameters = [...serviceDetail["parameterNames"]];
          this.paramsToSend = [...serviceDetail["parameterNames"]];
          break;
        }
      }
      this.$emit("updateInfo",
      {
        type: service.name,
        params: this.paramsToSend,
        index: this.reactionIndex,
      });
    },
  },
  computed: {},
  data: function () {
    return {
      possibleServices: [] as string[],
      currentParameters: [] as string[],
      paramsToSend: [] as string[],
      currentService: null,
      accounts: [] as any[],
      people: [],
      selectedPerson: null as any|null,
    };
  },
  setup() {},
  created: async function () {},
});
</script>