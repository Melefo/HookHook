<template>
    <div class="text-black text-2xl mt-8 dark:text-white text-left">
      <p class="welcomingMessage">Hello <span class="userName">{{ firstName }}</span>, welcome back !</p>
    </div>
    <div class="text-black dark:text-white mt-8">
      <p>Services</p>
    </div>
    <!-- // todo centrer et colorier les svg pliz -->
    <div class="text-black mt-2 justify-start">
        <button class="button bg-[#A3E7EE]">
            <img class="w-14 align-center" alt="twitter" src="@/assets/img/twitter.svg" />
        </button>
        <button class="button bg-[#B4E1DC]">
            <img class="w-14 align-center" alt="spotify" src="@/assets/img/spotify.svg" />
        </button>
        <button class="button bg-[#D9D1EA]">
            <img class="w-14 align-center" alt="discord" src="@/assets/img/discord.svg" />
        </button>
        <button class="button bg-[#F5CDCB]">
            <img class="w-14 align-center" alt="github" src="@/assets/img/github.svg" />
        </button>
        <button class="button bg-[#F8CBAA]">
            <img class="w-14 align-center" alt="google" src="@/assets/img/google.svg" />
        </button>
        <button class="button bg-[#FFFFC7]">
            <img class="w-14 align-center" alt="twitch" src="@/assets/img/twitch.svg" />
        </button>
    </div>
    <div class="gridCreator mt-8 text-black dark:text-white">
      <p class="creatorTitle">Creator</p>
      <p class="urAreaTitle">Your AREAs</p>
      <div class="creatorBG rounded-xl bg-[#3B3F43] p-5"/>
      <div class="urAreaBG rounded-xl bg-[#3B3F43] p-5"/>
    </div>
</template>

<style>
    .userName {
      color: red;
    }
    .button {
      border-radius: 10px;
      width: 12%;
      margin: 0 2% 0 0;
      padding: 5% 0;
      transition-duration: 1s;
    }
    .button:hover {
      transform: scale(1.05);
    }
    .gridCreator {
      display: grid;
      height: 400px;
      grid-template-columns: 40% 60%;
      grid-template-rows: 10% 45% 45%;
      gap: 1%;
    }
    .creatorTitle {
      grid-column: 1 / span 1;
      grid-row: 1 / span 1;
    }
    .urAreaTitle {
      grid-column: 2 / span 1;
      grid-row: 1 / span 1;
    }
    .creatorBG {
      grid-column: 1 / span 1;
      grid-row: 2 / span 2;
    }
    .urAreaBG {
      grid-column: 2 / span 1;
      grid-row: 2 / span 2;
    }
</style>

<script lang="ts">
import { defineComponent } from "vue";
import { parseJwt } from "@/router";
import { mapGetters } from "vuex";


export default defineComponent({
  components: {  },
  methods: {

  },
  computed: {
    ...mapGetters("user", ["token"]),
    firstName: function() {
        return parseJwt(this.token).given_name;
    }
  }
});
</script>