<template>
    <div>
        <!-- // todo pour le nom du area, un input modelé à une variable -->
        <p class="text-white p-2">AREA name : coucou</p>
        <SubAreaComponent @updateInfo="updateAction" reaction-index="-1" :service-details="serviceDetails" verb="when" area-type="Action"/>

        <div
            v-for="(reaction, index) in reactions"
            :key="index"
            >
            <SubAreaComponent
                :service-details="serviceDetails"
                @updateInfo="updateReaction"
                :reaction-index="index"
                verb="do"
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
        ...mapActions("about", ["getServices", "createArea"]),
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
            console.log("Action: ", this.action);
            console.log("Reactions: ", this.reactions);

            // const { error } = await this.createArea({
            //     "action": this.action,
            //     "reactions": this.reactions,
            //     "minutes": this.minutes
            // });
            // this.error = error || null;

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
            error: "" as string,
            minutes: 1 as number
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