<template>
  <a href="/login" @click.prevent="handleGithub">
    <img v-if="oauth" class="h-10" alt="github" src="@/assets/img/github.svg" />
    <div v-else>
      <slot />
    </div>
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
  props: {
    oauth: {
      type: Boolean,
      default: true
    }
  },
  methods: {
    ...mapActions("signIn", ["github"]),
    ...mapActions("service", ["addGitHub"]),
    async handleGithub() {
      window.removeEventListener("message", this.receiveGitHub);

      const url = `https://github.com/login/oauth/authorize?client_id=${
        process.env.VUE_APP_GITHUB_CLIENTID
      }&redirect_uri=${
        window.location.origin + '/oauth'
      }&state=${Math.random().toString(36).slice(2)}&response_type=code&scope=user%20repo`;
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
      const info = this.oauth ? await this.github(data.code) : await this.addGitHub(data.code);
      this.errors = info.errors || null;
      this.error = info.error || null;
      if (this.oauth) {
        if (!this.error && !this.errors) {
          this.$router.push("/dashboard");
        }
      }
      else {
        this.$emit('addAccount', info);
      }
    },
  },
});
</script>