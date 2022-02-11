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
      window.removeEventListener("message", this.receiveGoogle);

      let scopes = "openid";
      scopes += " email";
      scopes += " profile";

      const url = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${
        process.env.VUE_APP_GOOGLE_CLIENTID
      }&redirect_uri=${
        window.location.origin + '/oauth'
      }&state=${Math.random().toString(36).slice(2)}
      &response_type=code
      &scope=${scopes}
      &access_type=offline&nonce=${Math.random().toString(36).slice(2)}`;
      let popup = window.open(
        url,
        "_blank",
        "width=500, height=750, left=20, top=20, popup=true"
      );
      if (popup == null) {
        return;
      }
      popup.focus();
      window.addEventListener("message", this.receiveGoogle, false);
    },
    async receiveGoogle(event: any) {
      if (event.origin !== window.location.origin) {
        return;
      }
      let data = Object.fromEntries(new URLSearchParams(event.data));
      if (!data.code) {
        return;
      }
      window.removeEventListener("message", this.receiveGoogle);
      const { errors, error } = await this.google(data.code);
      this.errors = errors || null;
      this.error = error || null;
      if (!this.error && !this.errors) {
        this.$router.push("/dashboard");
      }
    },
  },
});
</script>