<template>
    <div class="text-black text-2xl mt-8 dark:text-white text-left">
      <p class="welcomingMessage">Hello <span class="userName">{{ firstName }}</span>, welcome back !</p>
    </div>
    <div class="text-black dark:text-white mt-8">
      <p>Services</p>
    </div>
    <!-- // todo centrer et colorier les svg pliz -->
    <OAuthButtonsComponent/>
    <div class="gridCreator text-black dark:text-white">
      <p class="creatorTitle">Creator</p>
      <p class="urAreaTitle">Your AREAs</p>
      <!--AREA CREATOR-->
      <div class="creatorBG rounded-xl bg-[#3B3F43] text-black">
        <!-- TEST DROPDOWN 2 -->
        <div id="app">
          <select v-model="selectedValue">
            <option disabled value="">Please select one</option>
            <option v-bind:key="item" v-for="item in filters" :value="item">{{item}}</option>
          </select>
        </div>
        <!-- TEST DROPDOWN HEADLESS UI-->
        <DropdownComponent/>
      </div>
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
    }
</style>

<script lang="ts">
import { defineComponent } from "vue";
import { parseJwt } from "@/router";
import { mapGetters } from "vuex";

import CarouselComponent from "@/components/CarouselComponent.vue";
import DropdownComponent from "@/components/DropdownComponent.vue";
import OAuthButtonsComponent from "@/components/OAuthButtonsComponent.vue";

export default defineComponent({
  components: { CarouselComponent, DropdownComponent, OAuthButtonsComponent },
  methods: {

  },
  computed: {
    ...mapGetters("signIn", ["token"]),
    firstName: function() {
        const jwt = parseJwt(this.token);
        return jwt !== null ? jwt.given_name : "";
    }
  },
  data() {
    return {
      filters: ["Tweeter: 'j'ai juré c'est une dinguerie'", "Envoyer un message privé à Kev Adams"],
      selectedValue: null,
    }
  },
});
</script>