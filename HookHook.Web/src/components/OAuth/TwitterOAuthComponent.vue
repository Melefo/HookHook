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
    }
  },
  methods: {
    ...mapActions("signIn", ["twitter", "authorize"]),
    ...mapActions("service", ["addTwitter"]),
    async handleTwitter() {
      window.removeEventListener("message", this.receiveTwitter);

      var { url, error, errors } = await this.authorize("Twitter");
      if (url === null) {
        this.errors = errors || null;
        this.error = error || null;
        return;
      }

      let popup = window.open(
        url,
        "_blank",
        "width=500, height=750, left=20, top=20, popup=true"
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
      const { errors, error } = this.oauth ? await this.twitter({token: data.oauth_token, verifier: data.oauth_verifier }) : await this.addTwitter({token: data.oauth_token, verifier: data.oauth_verifier });
      this.errors = errors || null;
      this.error = error || null;
      if (this.oauth) {
        if (!this.error && !this.errors) {
          this.$router.push("/dashboard");
        }
      }
    },
  },
});
</script>