<template>
  <a href="/login" @click.prevent="handleTwitter">
    <img class="h-10" alt="twitter" src="@/assets/img/twitter.svg" />
  </a>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { mapActions } from "vuex";
// import twitterApi from 'twitter-api-v2';
// const twitterApi = require('twitter-api-v2').default;
import TwitterApi from 'twitter-api-v2';

const twitterClient = new TwitterApi({
    clientId: process.env.VUE_APP_TWITTER_CLIENTID,
    clientSecret: process.env.VUE_APP_TWITTER_SECRET
});

export default defineComponent({
  data() {
    return {
      error: null,
      errors: null,
    };
  },
  methods: {
    ...mapActions("user", ["twitter"]),
    async handleTwitch() {
      window.removeEventListener("message", this.receiveTwitter);

        // * essayons ceci:
        // * https://github.com/feross/login-with-twitter

      var scopes = "";

      const url = `https://api.twitter.com/oauth/request_token?oauth_token=${
        process.env.VUE_APP_TWITTER_CLIENTID
      }&callback_url=${
        process.env.VUE_APP_TWITTER_REDIRECT
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
      window.addEventListener("message", this.receiveTwitter, false);
    },
    async receiveTwitter(event: any) {
      if (event.origin !== window.location.origin) {
        return;
      }

      // * on devrait cross check le state ?

      let data = Object.fromEntries(new URLSearchParams(event.data));
      if (!data.code) {
        return;
      }
      console.log("About to call twitter");
      window.removeEventListener("message", this.receiveTwitter);
      const { errors, error } = await this.twitch(data.code);
      this.errors = errors || null;
      this.error = error || null;
      console.log(this.error);
      if (!this.error && !this.errors) {
        this.$router.push("/dashboard");
      }
    },
  },
});
</script>