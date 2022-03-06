<template>
  <div class="text-black justify-start">
    <DialogComponent v-for="(item, key) in services" :key="key" :text="item.name" :src="item.name.toLowerCase() + '.svg'" :bgColor="color(item.name)">
      <component :is="item.name + 'Oauth'" :oauth="false">
        <button class="button-74 mb-[20px]" :bgColor="color(item.name)">
          Add a user account
        </button>
      </component>
      <div v-for="(account, keyy) in accounts[item.name]" :key="account.userId" class="flex justify-between items-center mx-16">
        <p>{{ account.username }}</p>
        <button @click.prevent="async () => await deleteService(item.name, account.userId, keyy)">
          <XIcon class="h-8" />
        </button>
      </div>
    </DialogComponent>
  </div>
</template>

<style>
.button-74 {
  background-color: #ffffff;
  border: 2px solid #000000;
  border-radius: 30px;
  box-shadow: #000000 4px 4px 0 0;
  color: #000000;
  cursor: pointer;
  display: inline-block;
  font-weight: 600;
  font-size: 14px;
  padding: 0 14px;
  line-height: 40px;
  text-align: center;
  text-decoration: none;
  user-select: none;
  -webkit-user-select: none;
  touch-action: manipulation;
}

.button-74:hover {
  background-color: #f0f0f0;
}

.button-74:active {
  box-shadow: #000000 2px 2px 0 0;
  transform: translate(2px, 2px);
}

@media (min-width: 768px) {
  .button-74 {
    min-width: 120px;
    padding: 0 25px;
  }
}
</style>

<script lang="ts">
  import { defineComponent } from "vue";
  import DialogComponent from "@/components/DialogComponent.vue";
  import { mapActions } from "vuex";
  import DiscordOauth from "@/components/OAuth/DiscordOAuthComponent.vue";
  import GitHubOauth from "@/components/OAuth/GitHubOAuthComponent.vue";
  import SpotifyOauth from "@/components/OAuth/SpotifyOAuthComponent.vue";
  import TwitchOauth from "@/components/OAuth/TwitchOAuthComponent.vue";
  import TwitterOauth from '@/components/OAuth/TwitterOAuthComponent.vue';
  import GoogleOauth from "@/components/OAuth/GoogleOAuthComponent.vue";
  import { XIcon } from "@heroicons/vue/solid";

  export default defineComponent ({
    name: 'OAuthButtonsComponent',
    components: { DialogComponent, DiscordOauth, GitHubOauth, SpotifyOauth, TwitchOauth, TwitterOauth, GoogleOauth, XIcon },
    props: ['text', 'bgColor', 'src'],
    methods: {
      ...mapActions("about", ["get"]),
      ...mapActions("service", ["getAccounts", "deleteAccount"]),
      async deleteService(provider: String, id: String, accountKey: number) {
        await this.deleteAccount({provider: provider, id: id, key: accountKey});
      },
      color(name: string) {
        name = name.toLowerCase();
        switch (name) {
          case 'twitter':
            return "#A3E7EE";
          case 'spotify':
            return "#B4E1DC";
          case 'discord':
            return "#D9D1EA";
          case 'github':
            return "#F5CDCB";
          case 'google':
          case 'youtube':
            return "#F8CBAA";
          case 'twitch':
            return "#FFFFC7";
        }
      }
    },
    computed: {
      accounts(): any {
        return this.$store.state.service.accounts;
      },
      services(): any {
        return this.$store.state.about.info?.server?.services || []
      }
    },
    created: async function() {
      this.get().then(() => {
        for (const service in this.services) {
          this.getAccounts(this.services[service].name);
        }
      });
    }
  });
</script>