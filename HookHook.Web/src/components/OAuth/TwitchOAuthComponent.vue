<template>
  <a href="/login" @click.prevent="handleTwitch">
    <img v-if="oauth" class="h-10" alt="twitch" src="@/assets/img/twitch.svg" />
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
    ...mapActions("signIn", ["twitch"]),
    ...mapActions("service", ["addTwitch"]),
    async handleTwitch() {
      window.removeEventListener("message", this.receiveTwitch);

      var scopes = [
        "channel:read:subscriptions",
        "user:edit",
        "user:read:email",
        "user:read:follows"
      ];

      const url = `https://id.twitch.tv/oauth2/authorize?client_id=${
        process.env.VUE_APP_TWITCH_CLIENTID
      }&redirect_uri=${
        process.env.VUE_APP_TWITCH_REDIRECT
      }&state=${Math.random().toString(36).slice(2)}&response_type=code&scope=${scopes.join(' ')}`;
      let popup = window.open(
        url,
        "_blank",
        "width=500, height=750, left=20, top=20, popup=true"
      );
      if (popup == null) {
        return;
      }
      popup.focus();
      window.addEventListener("message", this.receiveTwitch, false);
    },
    async receiveTwitch(event: any) {
      if (event.origin !== window.location.origin) {
        return;
      }

      let data = Object.fromEntries(new URLSearchParams(event.data));
      if (!data.code) {
        return;
      }
      window.removeEventListener("message", this.receiveTwitch);
      const info = this.oauth ? await this.twitch(data.code) : await this.addTwitch(data.code);
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