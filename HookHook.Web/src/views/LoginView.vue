<template>
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
  >The best way to automate your work and make it more relaxing.</h2>
  <section
    class="
      flex flex-col justify-center items-center
      gap-10
    "
  >
    <Bloc class="flex flex-col justify-center items-center xl:w-[40%] lg:w-[60%] md:w-[80%] w-[90%]">
      <Login />
      <div class="grid grid-cols-3 sm:grid-cols-6 gap-4 mt-8">
        <component v-for="(item, key) in services" :key="key" :is="item.name + 'Oauth'" />
      </div>
    </Bloc>
  </section>
  <div class="flex flex-col xl:flex-row justify-evenly mt-20 hidden">
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/breakdance.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/dance.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/clap.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/mop.gif" />
    <img class="object-contain w-40 h-v" src="@/assets/pinguin/maracas.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/warp.gif" />
    <img class="object-contain w-40 h-40" src="@/assets/pinguin/photo.gif" />
  </div>
</template>

<style>
.blink {
  color:#A3E7EE;
  animation: blink 4s linear infinite;
}
@keyframes blink {  
  30% { color: #FD9524; }
}
</style>

<script lang="ts">
import Bloc from "@/components/BlocComponent.vue";
import HookHook from "@/components/HookHookComponent.vue";
import Login from "@/components/User/LoginComponent.vue";
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
  components: { Bloc, HookHook, Register, Login, DiscordOauth, GitHubOauth, SpotifyOauth, TwitchOauth, TwitterOauth, GoogleOauth },
  methods: {
    ...mapActions("about", ["get"])
  },
  computed: {
    services() {
      return this.$store.state.about.info?.server?.services || []
    }
  },
  created: async function() {
    await this.get();
  }
});
</script>