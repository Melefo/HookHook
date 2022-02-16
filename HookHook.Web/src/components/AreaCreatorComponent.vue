<template>
    <div>
        <!-- // todo pour le nom du area, un input modelé à une variable -->
        <p class="text-white p-2">AREA name : coucou</p>
        <!-- // todo bind (name, params) to variables -->
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
        <button class="text-white pt-6">
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
        ...mapActions("about", ["getServices"]),
        updateAction({type, params} : any) {
            console.log("Got new action type: ", type);
            console.log("Got new action params: ", params);

            this.action['type'] = type;
            this.action['arguments'] = [...params];
        },
        updateReaction({type, params, index} : any) {
            console.log("Got new reaction type: ", type);
            console.log("Got new reaction params: ", params);

            console.log("Reaction index", index);

            this.reactions[index]['type'] = type;
            this.reactions[index]['arguments'] = [...params];
        },
        createArea() {
            // todo call the store, check for errors
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
            serviceDetails: [],
            action: {},
            reactions: [],
            paramsToSend: [],
        }
    },
    setup() {
    },
    created: async function() {
        // * fetch the services with the service arguments
        const serviceDetails = await this.getServices();
        this.serviceDetails = serviceDetails;
        console.log("Got details = ", this.serviceDetails);

        this.action.type = "";
        this.action.arguments = []

        this.reactions = [{
            type: "",
            arguments: []
        }]
    },
});
</script>