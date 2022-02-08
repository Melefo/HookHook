<template>
  <a href="/login" @click.prevent="handleGoogle">
    <img class="h-10" alt="google" src="@/assets/img/google.svg" />
  </a>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { mapActions } from "vuex";

export default defineComponent({
  data() {
    return {
      error: null,
      errors: null,
    };
  },
  methods: {
    ...mapActions("user", ["google"]),
    async handleGoogle() {

        try {
            // console.log(Vue3GoogleOauth)
            // const code = await this.$gAuth.getAuthCode();
            // const { error, errors } = await this.google(code);
            // this.error = error;
            // this.errors = errors;
            // if (!this.error && !this.errors) {
            //     this.$router.push("/dashboard");
            // }
        } catch {
            console.error("Problemo largo con el google");
            // this.error = "Error with Google OAuth";
        }
    }
  },
});
</script>