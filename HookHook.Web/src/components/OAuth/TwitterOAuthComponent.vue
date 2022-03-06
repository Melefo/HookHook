<template>
  <a href="/login" @click.prevent="handleTwitter">
    <img v-if="oauth" class="h-10" alt="twitter" src="@/assets/img/twitter.svg" />
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
    },
    list: {
      type: null as any[]|null
    }
  },
  methods: {
    ...mapActions("signIn", ["twitter", "authorize"]),
    ...mapActions("service", ["addTwitter"]),
    async handleTwitter() {
      window.removeEventListener("message", this.receiveTwitter);

      var { url, error, errors } = await this.authorize({ provider: "Twitter", redirect: process.env.VUE_APP_TWITTER_REDIRECT });
      if (url === null) {
        this.errors = errors || null;
        this.error = error || null;
        return;
      }

      let popup = window.open(
        url,
        "oauthWindow",
        "width=500, height=750, left=20, top=20"
      );
      if (popup == null) {
        return;
      }
      popup.focus();

      window.addEventListener("message", this.receiveTwitter, false);
    },
    async receiveTwitter(event: any) {
      if (event.origin !== window.location.origin) {
        return;
      }
      let data = Object.fromEntries(new URLSearchParams(event.data));
      if (!data.oauth_token || !data.oauth_verifier) {
        return;
      }
      window.removeEventListener("message", this.receiveTwitter);
      const info = this.oauth ? await this.twitter({token: data.oauth_token, verifier: data.oauth_verifier }) : await this.addTwitter({token: data.oauth_token, verifier: data.oauth_verifier });
      this.errors = info.errors || null;
      this.error = info.error || null;
      if (this.oauth) {
        if (!this.error && !this.errors) {
          this.$router.push("/dashboard");
        }
      }
    },
  }
});
</script>