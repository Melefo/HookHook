
<template>
  <section>
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
      Welcome to <HookHook />, the best way to automate your work and make it
      more relaxing.
    </h2>
  </section>
  <section>
    <div
      class="
        grid grid-rows-none grid-rows-2
        xl:grid-rows-none xl:grid-cols-2
        gap-10
        xl:gap-40 xl:mx-40
      "
    >
      <Bloc class="flex flex-col justify-center items-center">
        <form class="flex flex-col">
          <Input
            class="pb-5"
            type="text"
            label="Username/Email"
            :reverse="true"
          />
          <Input
            class="pb-5"
            type="password"
            label="Password"
            :reverse="true"
          />
          <Input type="submit" value="Se connecter" :reverse="true" />
        </form>
        <div class="grid grid-cols-3 sm:grid-cols-6 gap-4 mt-8">
          <img class="h-10" alt="google" src="@/assets/img/google.svg" />
          <img class="h-10" alt="twitch" src="@/assets/img/twitch.svg" />
          <img class="h-10" alt="github" src="@/assets/img/github.svg" />
          <img class="h-10" alt="spotify" src="@/assets/img/spotify.svg" />
          <img class="h-10" alt="twitter" src="@/assets/img/twitter.svg" />
          <img class="h-10" alt="discord" src="@/assets/img/discord.svg" />
        </div>
      </Bloc>
      <Bloc class="flex justify-center items-center">
        <form class="flex flex-col">
          <div class="grid grid-cols-2 gap-4">
            <Input
              class="pb-5"
              type="text"
              label="First name"
              :reverse="true"
            />
            <Input class="pb-5" type="text" label="Last name" :reverse="true" />
          </div>
          <Input class="pb-5" type="email" label="Email" :reverse="true" />
          <Input class="pb-5" type="text" label="Username" :reverse="true" />
          <div class="grid grid-cols-2 gap-4">
            <Input
              class="pb-5"
              type="password"
              label="Password"
              :reverse="true"
            />
            <Input
              class="pb-5"
              type="password"
              label="Confirm password"
              :reverse="true"
            />
          </div>
          <Input type="submit" value="S'enregistrer" :reverse="true" />
        </form>
      </Bloc>
    </div>
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
import Input from "@/components/InputComponent.vue";
import Bloc from "@/components/BlocComponent.vue";
import HookHook from "@/components/HookHookComponent.vue";

import { defineComponent } from "vue";

export default defineComponent({
  components: { Input, Bloc, HookHook },
});
</script>