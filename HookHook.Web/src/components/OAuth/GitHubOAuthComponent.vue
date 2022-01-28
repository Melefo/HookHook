<template>
  <a href="/login" @click.prevent="handleGithub">
    <img class="h-10" alt="github" src="@/assets/img/github.svg" />
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
    ...mapActions("user", ["github"]),
    async handleGithub() {
      window.removeEventListener("message", this.receiveGitHub);

      const url = `https://github.com/login/oauth/authorize?client_id=${
        process.env.VUE_APP_GITHUB_CLIENTID
      }&redirect_uri=${
        process.env.VUE_APP_GITHUB_REDIRECT
      }&state=${Math.random().toString(36).slice(2)}&response_type=code`;
      let popup = window.open(
        url,
        "_blank",
        "width=500, height=750, left=20, top=20, popup=true"
      );
      if (popup == null) {
        return;
      }
      popup.focus();
      window.addEventListener("message", this.receiveGitHub, false);
    },
    async receiveGitHub(event: any) {
      if (event.origin !== window.location.origin) {
        return;
      }
      let data = Object.fromEntries(new URLSearchParams(event.data));
      if (!data.code) {
        return;
      }
      window.removeEventListener("message", this.receiveGitHub);
      const { errors, error } = await this.github(data.code);
      this.errors = errors || null;
      this.error = error || null;
      if (!this.error && !this.errors) {
        this.$router.push("/dashboard");
      }
    },
  },
});
</script>