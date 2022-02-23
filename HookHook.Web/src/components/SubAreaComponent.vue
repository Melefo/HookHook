<template>
  <div class="mt-2">
    <span class="dark:text-white text-black">{{ verb }}: </span>
    <div class="flex flex-row items-center">
      <ActionComponent @actionChange="changeOptions" />
      <Listbox v-if="seelctedService !== null" v-model="selectedPerson">
        <div class="">
          <ListboxButton
            class="relative p-2 text-left text-black dark:text-white"
          >
            <span
              class="block border-0 border-b-2 border-[#A3E7EE]"
              v-if="selectedPerson !== null"
              >{{ selectedPerson.username }}</span
            >
            <span class="block border-0 border-b-2 border-[#FD9524]" v-else-if="people.length > 0"
              >Select an account</span>
            <span v-else class="block border-0 border-b-2 border-[#A3E7EE]">
              No account
            </span>
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
                v-slot="{ selected }"
                v-for="person in people"
                :key="person.userId"
                :value="person"
                as="template"
              >
              <li class="cursor-pointer select-none relative py-2 pl-4 pr-4 hover:bg-[#EEEEEE]">
                <span :class="[selected ? 'text-[#F09113]' : '']" class="relative py-2 pl-4 pr-4 hover:text-[#A3E7EE]">{{ person.username }}</span>
              </li>
              </ListboxOption>
            </ListboxOptions>
          </transition>
        </div>
      </Listbox>
      <Listbox v-if="selectedPerson != null" v-model="currentService">
        <div>
        <ListboxButton class="relative p-2 text-left text-black dark:text-white">
          <span
            v-if='currentService !== null && currentService.description !== ""'
            class="block border-0 border-b-2 border-[#A3E7EE]"
            >{{ currentService.description }}</span>
            <span v-else-if="possibleServices.length > 0" class="block border-0 border-b-2 border-[#FD9524]">Select a {{ areaType }}</span>
            <span v-else class="block border-0 border-b-2 border-[#A3E7EE]">No {{ areaType }}</span>
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
            v-slot="{ selected }"
            v-for="possibleService in possibleServices"
            :key="possibleService.name"
            :value="possibleService"
            as="template"
            @click="serviceSelected(possibleService)"
            >
              <li class="cursor-pointer select-none relative py-2 pl-4 pr-4 hover:bg-[#EEEEEE]">
                <span :class="[ selected ? 'text-[#F09113]' : '']" class="relative py-2 pl-4 pr-4 hover:text-[#A3E7EE]">{{ possibleService.description }}</span>
              </li>
          </ListboxOption>
        </ListboxOptions>
        </div>
      </Listbox>
    </div>
    <div v-for="(parameter, i) in currentParameters" :key="parameter">
      <div>
      <label :for="i" class="dark:text-white text-black">{{ parameter }}:</label>
      <input
        :id="i"
        :class="[
          paramsToSend[i] != null && paramsToSend[i].length > 0
            ? 'border-[#A3E7EE]'
            : 'border-[#FD9524]',
        ]"
        v-model="paramsToSend[i]"
        class="
          dark:text-white
          text-black
          appearance-none
          bg-transparent
          border-0 border-b-2
          w-1/2
          py-1
          px-2
          focus:outline-none
        "
        type="text"
        :placeholder="parameter"
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
      this.currentParameters = [];
      this.paramsToSend = [];
      this.currentService = null;
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
      this.seelctedService = newAction;
    },
    serviceSelected(service: any) {
      for (let i = 0; i < this.serviceDetails.length; i++) {
        const serviceDetail = this.serviceDetails[i];

        if (serviceDetail["className"] === service.name) {
          this.currentParameters = [...serviceDetail["parameterNames"]];
          this.paramsToSend = [...serviceDetail["parameterNames"].map(() => { return null })];
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
      currentService: null as any|null,
      accounts: [] as any[],
      people: [],
      selectedPerson: null as any|null,
      seelctedService: null as any|null
    };
  },
  setup() {},
  created: async function () {},
});
</script>