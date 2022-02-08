<template>
    <div>
        <p>Hello</p>
        <div v-for="(user, key) in users" :key="key">{{ user }}</div>
    </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { mapActions } from "vuex";

export default defineComponent({
    methods: {
        ...mapActions("user", ["all"])
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
        const { error, errors } = json;
        if (error !== null) {
            this.error = error;
        }
        if (errors !== null) {
            this.errors = errors;
        }
        if (this.error === null && this.errors === null) {
            this.users = json;
        }
    }
})
</script>