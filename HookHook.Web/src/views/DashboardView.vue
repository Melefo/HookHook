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
            <img class="w-20 h-20 m-auto" alt="twitter" src="@/assets/img/coloredsvg/twitter.svg" />
        </button>
        <button class="button bg-[#B4E1DC]">
            <img class="w-20 h-20 m-auto" alt="spotify" src="@/assets/img/coloredsvg/spotify.svg" />
        </button>
        <button class="button bg-[#D9D1EA]">
            <img class="w-20 h-20 m-auto" alt="discord" src="@/assets/img/coloredsvg/discord.svg" />
        </button>
        <button class="button bg-[#F5CDCB]">
            <img class="w-20 h-20 m-auto" alt="github" src="@/assets/img/coloredsvg/github.svg" />
        </button>
        <button class="button bg-[#F8CBAA]">
            <img class="w-20 h-20 m-auto" alt="google" src="@/assets/img/coloredsvg/google.svg" />
        </button>
        <button class="button bg-[#FFFFC7]">
            <img class="w-20 h-20 m-auto" alt="twitch" src="@/assets/img/coloredsvg/twitch.svg" />
        </button>
    </div>
    <div class="gridCreator text-black dark:text-white">
      <p class="creatorTitle">Creator</p>
      <p class="urAreaTitle">Your AREAs</p>
      <!--AREA CREATOR-->
      <div class="creatorBG rounded-xl bg-[#3B3F43]"/>
      <!--MY AREA-->
      <CarouselComponent/>
    </div>
</template>

<style>
    .userName {
      color: red;
    }
    .button {
      border-radius: 10px;
      width: 150px;
      margin: 0 25px 10px 0;
      padding: 35px 0;
      transition-duration: 1s;
    }
    .button:hover {
      transform: scale(1.05);
    }
    .gridCreator {
      display: grid;
      height: 475px;
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
      flex-flow: column wrap;
      display: grid;
      grid-template-columns: 25% 25% 25% 25%;
      grid-template-rows: 50% 50%;
    }
</style>

<script lang="ts">
import { defineComponent } from "vue";
import { parseJwt } from "@/router";
import { mapGetters } from "vuex";
import CarouselComponent from "@/components/CarouselComponent.vue";

export default defineComponent({
  components: { CarouselComponent },
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