
<template>
  <div>
    <div class="flex flex-row justify-center items-center whitespace-nowrap">
    <img class="w-20 h-20" src="@/assets/pinguin/breakdance.gif"/>
    <h2 class="
        p-2
        font-medium
        text-3xl text-black
        dark:text-white
        transition-colors
        duration-200
      "
    >Welcome to H<span class="blink">oo</span>kH<span class="blink">oo</span>k</h2>
    <img class="w-20 h-20" src="@/assets/pinguin/breakdance.gif"/>
  </div>
  <h2 class="
    flex
    flex-row
    justify-center
    items-center
    p-5
    text-2xl text-black
    dark:text-white
    "
  >Wanna try our service?</h2>
    <section
      class="
        flex flex-col justify-center items-center
        gap-10
      "
    >
      <Bloc class="flex flex-col justify-center items-center xl:w-[40%] lg:w-[60%] md:w-[80%] w-[90%] ">
        <Register />
        <div class="grid grid-cols-3 sm:grid-cols-6 gap-4 mt-8">
            <component v-for="(item, key) in services" :key="key" :is="(item.name === 'youtube' ? 'google' : item.name) + 'Oauth'" />
        </div>
      </Bloc>
    </section>
  </div>
</template>

<script lang="ts">
import Bloc from "@/components/BlocComponent.vue";
import HookHook from "@/components/HookHookComponent.vue";
import Register from "@/components/User/RegisterComponent.vue";
import DiscordOauth from "@/components/OAuth/DiscordOAuthComponent.vue";
import GitHubOauth from "@/components/OAuth/GitHubOAuthComponent.vue";
import SpotifyOauth from "@/components/OAuth/SpotifyOAuthComponent.vue";
import TwitchOauth from "@/components/OAuth/TwitchOAuthComponent.vue";
import TwitterOauth from '@/components/OAuth/TwitterOAuthComponent.vue';
import GoogleOauth from "@/components/OAuth/GoogleOAuthComponent.vue";

import { defineComponent } from "vue";
import { mapActions } from "vuex";

export default defineComponent({
  components: { Bloc, HookHook, Register, DiscordOauth, GitHubOauth, SpotifyOauth, TwitchOauth, TwitterOauth, GoogleOauth },
  methods: {
    ...mapActions("about", ["get"])
  },
  data: function() {
    return {
      services: this.$store.state.about.info?.server?.services || []
    }
  },
  created: async function() {
    await this.get();
  }
});
</script>