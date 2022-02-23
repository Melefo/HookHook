<template>
  <a href="/" @click.prevent="handleDiscord">
    <img v-if="oauth" class="h-10" alt="discord" src="@/assets/img/discord.svg" />
    <div v-else>
      <slot />
    </div>
  </a>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { mapActions } from "vuex";
import DiscordOauth2 from "discord-oauth2";

export default defineComponent({
  components: {},
  props: {
    oauth: {
      type: Boolean,
      default: true
    }
  },
  data() {
      return {
          error: null,
          errors: null
      }
  },
  methods: {
    ...mapActions("signIn", ["discord"]),
    ...mapActions("service", ["addDiscord"]),
    async handleDiscord() {
      window.removeEventListener("message", this.receiveDiscord);

      const oauth = new DiscordOauth2({
        clientId: process.env.VUE_APP_DISCORD_CLIENTID,
        redirectUri: window.location.origin + '/oauth',
      });

      const url = oauth.generateAuthUrl({
        scope: ["identify", "guilds", "email"],
        state: Math.random().toString(36).slice(2),
      });

      let popup = window.open(
        url,
        "_blank",
        "width=500, height=750, left=20, top=20, popup=true"
      );

      if (popup == null) {
        return;
      }
      popup.focus();
      window.addEventListener("message", this.receiveDiscord, false);
    },
    async receiveDiscord(event: any) {
      if (event.origin !== window.location.origin) {
        return;
      }
      let data = Object.fromEntries(new URLSearchParams(event.data));
      if (!data.code) {
        return;
      }
      window.removeEventListener("message", this.receiveDiscord);
      const info = this.oauth ? await this.discord(data.code) : await this.addDiscord(data.code);
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