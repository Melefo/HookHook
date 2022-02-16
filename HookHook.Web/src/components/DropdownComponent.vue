<template>
    <div>
        <!-- // todo pour le nom du area, un input modelé à une variable -->
        <p class="text-white p-2">AREA name : coucou</p>
        <Listbox>
        <div class="relative mt-1">
            <span class="text-white">When : </span>
            <ListboxButton class="relative p-2 text-left text-white rounded-lg">
            <span class="block truncate underline decoration-[#FD9524] underline-offset-4">{{ currentServiceDescription === "" ? "Action" : currentServiceDescription}}</span>
            <span class="absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none"/>
            </ListboxButton>
            <transition
            leave-active-class="transition duration-100 ease-in"
            leave-from-class="opacity-100"
            leave-to-class="opacity-0"
            >
                <ListboxOptions class="absolute py-1 mt-1 ml-[8%] overflow-auto z-10 text-base bg-white rounded-md shadow-lg max-h-60 ring-1 ring-black ring-opacity-5 focus:outline-none sm:text-sm">
                    <ListboxOption
                    v-slot="{ active, selected }"
                    v-for="possibleService in possibleServices"
                    :key="possibleService.name"
                    :value="possibleService.name"
                    as="template"
                    @click="serviceSelected(possibleService)"
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
                        >{{ possibleService.description }}</span
                        >
                    </li>
                    </ListboxOption>
                </ListboxOptions>
            </transition>

            <div
                v-for="(parameter, i) in currentParameters"
                :key="parameter"
                >
                <div class="text-white">
                    {{parameter + ":"}}
                </div>
                <input
                    v-model="paramsToSend[i]">
            </div>
        </div>
        </Listbox>
        <ActionComponent @actionChange="changeOptions"/>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { Listbox, ListboxButton, ListboxOptions, ListboxOption } from '@headlessui/vue';
import ActionComponent from '@/components/ActionComponent.vue';
import { mapActions } from "vuex";

export default defineComponent({
    name: 'DropdownComponent',
    components: { Listbox, ListboxButton, ListboxOptions, ListboxOption, ActionComponent },
    methods: {
        ...mapActions("about", ["getServices"]),
        changeOptions(newAction : any) {
            console.log(newAction.actions);

            this.possibleServices = newAction.actions;
        },
        serviceSelected(service : any) {
            console.log(service);
            this.currentServiceName = service.name;
            this.currentServiceDescription = service.description;

            for (let i = 0; i < this.serviceDetails.length; i++) {
                const serviceDetail = this.serviceDetails[i];

                if (serviceDetail['className'] === service.name) {
                    this.currentParameters = [...serviceDetail['parameterNames']];
                    this.paramsToSend = [...serviceDetail['parameterNames']]
                    break;
                }
            }
        }
    },
    computed: {
    },
    data: function() {
        return {
            serviceDetails: [],
            possibleServices: [],
            currentParameters: [],
            paramsToSend: [],
            currentServiceName: "",
            currentServiceDescription: ""
        }
    },
    setup() {

    },
    created: async function() {
        // * fetch the services with the service arguments
        const serviceDetails = await this.getServices();
        this.serviceDetails = serviceDetails;
        console.log("Got details = ", this.serviceDetails);
    },
});
</script>