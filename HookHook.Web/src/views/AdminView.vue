<template>
    <div>
        <table class="dark:text-white w-full transition-colors duration-200">
            <tr>
                <th>ID</th>
                <th>Username</th>
                <th>Email</th>
                <th>Name</th>
                <th>Role</th>
                <th>Area count</th>
                <th>Actions</th>
            </tr>
            <tr v-for="(user, key) in users" :key="key" class="m-5">
                <th>{{ user.id }}</th>
                <th>{{ user.username }}</th>
                <th>{{ user.email }}</th>
                <th>{{ user.firstName }} {{ user.lastName }}</th>
                <th>{{ user.role }}</th>
                <th>{{ user.areas !== null ? user.areas.length : 0 }}</th>
                <th>
                    <div class="grid grid-cols-3">
                        <button class="justify-self-center" @click.prevent="refresh(user.id)"><RefreshIcon class="transition-colors duration-200 h-8 dark:bg-[#F9F9F9] bg-[#3B3F43] dark:text-[#3B3F43] text-[#F9F9F9] rounded-md p-0.5 mx-1" /></button>
                        <button class="justify-self-center" v-if="user.role === 'Admin'" @click.prevent="promoteUser($event, user.id, key)"><ShieldExclamationIcon class="h-8 bg-[#B4E1DC] text-[#85B1AC] rounded-md p-0.5 mx-1" /></button>
                        <button class="justify-self-center" v-if="user.role === 'User'" @click.prevent="promoteUser($event, user.id, key)"><ShieldCheckIcon class="h-8 bg-[#FFFFC7] text-[#C6C791] rounded-md p-0.5 mx-1" /></button>
                        <button class="justify-self-center" v-if="user.role !== 'Admin'" @click.prevent="deleteUser($event, user.id, key)"><TrashIcon class="h-8 bg-[#F5CDCB] text-[#C49E9C] rounded-md p-0.5 mx-1" /></button>
                    </div>
                </th>
            </tr>
        </table>
    </div>
</template>

<style scoped>
table {
    border-spacing: 0 1em;
    border-collapse: initial;
}
</style>

<script lang="ts">
import { defineComponent } from "vue";
import { mapActions } from "vuex";
import { ShieldCheckIcon, ShieldExclamationIcon, TrashIcon, RefreshIcon } from "@heroicons/vue/outline";

export default defineComponent({
    components: { ShieldCheckIcon, ShieldExclamationIcon, TrashIcon, RefreshIcon },
    methods: {
        ...mapActions("user", ["all", "del", "promote", "refresh"]),
        promoteUser(_: any, id: string, key: number) {
            this.promote(id);
            this.users[key].role = this.users[key].role === "Admin" ? "User": "Admin";
        },
        deleteUser(_: any, id: string, key: number) {
            this.del(id);
            this.users.splice(key, 1);
        }
    },
    data: function() {
        return {
            users: [],
            error: null,
            errors: null
        }

    },
    async created() {
        const json = await this.all();
        if (this.error === null && this.errors === null) {
            this.users = json;
        }
        if (json.error !== null) {
            this.error = json.error;
        }
        if (json.errors !== null) {
            this.errors = json.errors;
        }
    }
})
</script>