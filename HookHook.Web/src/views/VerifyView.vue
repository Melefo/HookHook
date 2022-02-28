<template>
    <div class="text-black dark:text-white m-4">
        <p>Please wait...</p>
        <p>Your email is being validated</p>
        <p>You will be redirected when finished</p>
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { mapActions } from 'vuex'

export default defineComponent({
    methods: {
        ...mapActions("signIn", ["verify"])
    },
    async created() {
        const res = await this.verify(this.id);
        if (res.error === undefined && res.errors === undefined) {
            this.$router.push('/dashboard');
        }
        else {
            this.$router.push('/');
        }
    },
    props: {
        id: {
            type: String,
            required: true,
        }
    }
})
</script>
