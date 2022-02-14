<template>
  <div class="text-black mt-2 justify-start">
    <DialogComponent text="Twitter" bgColor="#A3E7EE" src="twitter.svg"/>
    <DialogComponent text="Spotify" bgColor="#B4E1DC" src="spotify.svg"/>
    <DialogComponent text="Discord" bgColor="#D9D1EA" src="discord.svg"/>
    <DialogComponent text="Github" bgColor="#F5CDCB" src="github.svg"/>
    <DialogComponent text="Google" bgColor="#F8CBAA" src="google.svg"/>
    <DialogComponent text="Twitch" bgColor="#FFFFC7" src="twitch.svg"/>
  </div>
</template>

<script lang="ts">
  import { defineComponent } from "vue";
  import DialogComponent from "@/components/DialogComponent.vue";

  export default defineComponent ({
    name: 'OAuthButtonsComponent',
    components: { DialogComponent },
    props: ['text', 'bgColor', 'src'],
  });
</script>