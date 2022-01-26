
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
        <img class="h-10" alt="google" src="@/assets/img/google.svg" />
        <img class="h-10" alt="twitch" src="@/assets/img/twitch.svg" />

        <button @click="registerGithub">
            <a :href="githubAuthLink">
                <img class="h-10" alt="github" src="@/assets/img/github.svg" />
            </a>
        </button>
        <img class="h-10" alt="spotify" src="@/assets/img/spotify.svg" />
        <img class="h-10" alt="twitter" src="@/assets/img/twitter.svg" />
        <img class="h-10" alt="discord" src="@/assets/img/discord.svg" />
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
import { defineComponent } from "vue";
import { mapActions } from "vuex";

export default defineComponent({
    components: { Bloc, HookHook, Register, Login },
    data() {
        return {
            githubAuthLink: `https://github.com/login/oauth/authorize?client_id=${process.env.VUE_APP_GITHUB_CLIENT_ID}&response_type=code&redirect_uri=http://localhost:1024`
        };
    },
    methods: {
        ...mapActions("user", ["registerWithGithub"]),
        async registerGithub()
        {
            
        }
    }
});
</script>