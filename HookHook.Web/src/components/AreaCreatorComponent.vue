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
        ...mapActions("about", ["get"]),
        ...mapActions("area", ["getServices", "createAreaRequest"]),
        ...mapActions("service", ["getAccounts"]),
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

            // ! solution temporaire avant de mettre un dropdown pour choisir le compte
            // * find in the accounts of service the first one and attach it to this action
            for (let i = 0; i < this.connectedServiceAccounts.length; i++) {
                const connectedAccount = this.connectedServiceAccounts[i];

                if (connectedAccount.actions.filter((act:any) => act.name === type).length > 0) {
                    this.action['accountID'] = connectedAccount.accounts[0].userId;
                    console.log("Got account id", this.action['accountID']);
                    break;
                } else if (connectedAccount.reactions.filter((react:any) => react.name === type).length > 0) {
                    this.action['accountID'] = connectedAccount.accounts[0].userId;
                    console.log("Got account id", this.action['accountID']);
                    break;
                }
            }
        },
        updateReaction({type, params, index} : any) {
            this.reactions[index]['type'] = type;
            this.reactions[index]['arguments'] = [...params];

            // * find in the accounts of service the first one and attach it to this reaction
            // ! solution temporaire avant de mettre un dropdown pour choisir le compte
            // * find in the accounts of service the first one and attach it to this action
            for (let i = 0; i < this.connectedServiceAccounts.length; i++) {
                const connectedAccount = this.connectedServiceAccounts[i];

                if (connectedAccount.actions.filter((act:any) => act.name === type).length > 0) {
                    this.reactions[index]['accountID'] = connectedAccount.accounts[0].userId;
                    console.log("Got account id", this.reactions[index]['accountID']);
                    break;
                } else if (connectedAccount.reactions.filter((react:any) => react.name === type).length > 0) {
                    this.reactions[index]['accountID'] = connectedAccount.accounts[0].userId;
                    console.log("Got account id", this.reactions[index]['accountID']);
                    break;
                }
            }

        },
        async createArea() {
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
            connectedServiceAccounts: [] as any[],
            actionAccount: {} as any,
            reactionAccounts: [] as any[],
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

        const { server: { services } } = await this.get();
        this.connectedServiceAccounts = await Promise.all(services.map(async (s: any) => {
            const accounts = await this.getAccounts(s.name);

            return {...s, accounts: accounts };
        }));
        console.log(this.connectedServiceAccounts);
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