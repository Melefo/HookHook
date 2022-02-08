<template>
  <a href="/login" @click.prevent="handleTwitch">
    <img class="h-10" alt="twitch" src="@/assets/img/twitch.svg" />
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
    ...mapActions("user", ["twitch"]),
    async handleTwitch() {
      window.removeEventListener("message", this.receiveTwitch);

      var scopes = "";
      scopes += "channel:read:subscriptions";
      scopes += " user:edit";
      scopes += " user:read:email";
      scopes += " user:read:follows"

      const url = `https://id.twitch.tv/oauth2/authorize?client_id=${
        process.env.VUE_APP_TWITCH_CLIENTID
      }&redirect_uri=${
        process.env.VUE_APP_TWITCH_REDIRECT
      }&state=${Math.random().toString(36).slice(2)}&response_type=code&scope=${scopes}`;
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

      // * on devrait cross check le state ?

      let data = Object.fromEntries(new URLSearchParams(event.data));
      if (!data.code) {
        return;
      }
      window.removeEventListener("message", this.receiveTwitch);
      const { errors, error } = await this.twitch(data.code);
      this.errors = errors || null;
      this.error = error || null;
      if (!this.error && !this.errors) {
        this.$router.push("/dashboard");
      }
    },
  },
});
</script>