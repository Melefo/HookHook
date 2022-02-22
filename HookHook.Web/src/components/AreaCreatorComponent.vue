<template>
    <div class="p-2">
        <div>
            <label for="name" class="dark:text-white text-black">AREA name :</label>
            <input id="name" class="dark:text-white text-black appearance-none bg-transparent border-0 border-b-2 border-[#FD9524] w-1/2 py-1 px-2 focus:outline-none" type="text" placeholder="My new area" />
        </div>
        <SubAreaComponent @updateInfo="updateAction" reaction-index="-1" :service-details="serviceDetails" verb="When" area-type="Action"/>

        <div
            v-for="(reaction, index) in reactions"
            :key="index"
            class="mt-4"
            >
            <SubAreaComponent
                :service-details="serviceDetails"
                @updateInfo="updateReaction"
                :reaction-index="index"
                verb="Do"
                area-type="Reaction"/>
            <button class="text-white pt-2" @click="removeReaction(index)">
                Remove
            </button>
        </div>
        <button class="text-white pt-6" @click="addReaction()">
            Add Reaction
        </button>
        <br/>
        <div class="text-white pt-6">
            Trigger every:
            <input class="text-black" min="1" @keyup="validateMinutes()" v-model="minutes" type="number"/>
            minute(s)
        </div>
        <button class="text-white pt-6" @click="createArea()">
            Create
        </button>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { mapActions } from "vuex";
import SubAreaComponent from "@/components/SubAreaComponent.vue";

export default defineComponent({
    name: 'DropdownComponent',
    components: { SubAreaComponent },
    methods: {
        ...mapActions("area", ["getServices", "createAreaRequest"]),
        validateMinutes() {
            if (this.minutes < 0) {
                this.minutes *= -1;
            }
            if (this.minutes < 1) {
                this.minutes = 1;
            }
        },
        updateAction({type, params} : any) {
            this.action['type'] = type;
            this.action['arguments'] = [...params];
        },
        updateReaction({type, params, index} : any) {
            this.reactions[index]['type'] = type;
            this.reactions[index]['arguments'] = [...params];
        },
        async createArea() {
            // todo call the store, check for errors
            const { error } = await this.createAreaRequest({
                "action": this.action,
                "reactions": this.reactions,
                "minutes": this.minutes
            });
            this.error = error || null;
        },
        addReaction() {
            this.reactions.push({
                type:"",
                arguments: []
            });
        },
        removeReaction(index: number) {
            this.reactions = this.reactions.splice(index, 1);
        }
    },
    computed: {
    },
    data: function() {
        return {
            serviceDetails: [] as any[],
            action: {} as any,
            reactions: [] as any[],
            error: null as any,
            minutes: 1 as number
        }
    },
    setup() {
    },
    created: async function() {
        // * fetch the services with the service arguments
        const serviceDetails = await this.getServices();
        this.serviceDetails = serviceDetails;
    },
    mounted: function() {
        this.action.type = "";
        this.action.arguments = []

        this.reactions = [{
            type: "",
            arguments: []
        }]
    }
});
</script>