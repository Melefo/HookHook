
<template>
  <h2
    class="
      my-5
      sm:my-10
      font-medium
      text-3xl text-black
      dark:text-white
      transition-colors
      duration-200
    "
  >
    Welcome to <HookHook />, the best way to automate your work and make it more
    relaxing.
  </h2>
  <section
    class="
      grid grid-rows-none grid-rows-2
      xl:grid-rows-none xl:grid-cols-2
      gap-10
      xl:gap-40 xl:mx-40
    "
  >
    <Bloc class="flex flex-col justify-center items-center">
      <Login />
      <div class="grid grid-cols-3 sm:grid-cols-6 gap-4 mt-8">
        <component v-for="(item, key) in services" :key="key" :is="(item.name === 'youtube' ? 'google' : item.name) + 'Oauth'" />
      </div>
    </Bloc>
    <Bloc class="flex justify-center items-center">
      <Register />
    </Bloc>
  </section>
  <div class="flex flex-col xl:flex-row justify-evenly mt-20 hidden">
    <img
      class="object-contain w-40 h-40"
      src="@/assets/pinguin/breakdance.gif"
    />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/dance.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/clap.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/mop.gif" />
    <img class="object-contain w-40 h-v" src="@/assets/pinguin/maracas.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/warp.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/photo.gif" />
  </div>
</template>

<script lang="ts">
import Bloc from "@/components/BlocComponent.vue";
import HookHook from "@/components/HookHookComponent.vue";
import Register from "@/components/User/RegisterComponent.vue";
import Login from "@/components/User/LoginComponent.vue";
import DiscordOauth from "@/components/OAuth/DiscordOAuthComponent.vue";
import GithubOauth from "@/components/OAuth/GitHubOAuthComponent.vue";
import SpotifyOauth from "@/components/OAuth/SpotifyOAuthComponent.vue";
import TwitchOauth from "@/components/OAuth/TwitchOAuthComponent.vue";
import TwitterOauth from '@/components/OAuth/TwitterOAuthComponent.vue';
import GoogleOauth from "@/components/OAuth/GoogleOAuthComponent.vue"

import { defineComponent } from "vue";
import { mapActions } from "vuex";

export default defineComponent({
  components: { Bloc, HookHook, Register, Login, DiscordOauth, GithubOauth, SpotifyOauth, TwitchOauth, TwitterOauth, GoogleOauth },
  methods: {
    ...mapActions("about", ["get"])
  },
  data: function() {
    return {
      services: []
    }
  },
  created: async function() {
    const { server: { services } } = await this.get();
    this.services = services;
  }
});
</script>