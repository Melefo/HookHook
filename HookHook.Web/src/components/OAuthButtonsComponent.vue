<template>
  <div class="text-black justify-start">
    <DialogComponent v-for="(item, key) in services" :key="key" :text="item.name" :src="item.name.toLowerCase() + '.svg'" :bgColor="color(item.name)">
      <div v-for="(account, keyy) in item.accounts" :key="account.userId" class="flex justify-between items-center mx-16">
        <p>{{ account.username }}</p>
        <button @click.prevent="async () => await deleteService(item.name, account.userId, keyy)">
          <XIcon class="h-8" />
        </button>
      </div>
      <component :is="item.name + 'Oauth'" :oauth="false">
        ADD
      </component>
    </DialogComponent>
  </div>
</template>

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
    data: function() {
      return {
        services: [] as any[],
      }
    },
    computed: {
      accounts(): any {
        return this.$store.state.service.accounts;
      }
    },
    created: async function() {
      this.get();
      const services = this.$store.state.about.info?.server?.services || [];
      this.services = await Promise.all(services.map(async (s: any) => {
        this.getAccounts(s.name);

        return {...s, accounts: this.accounts[s.name] };
      }));
    }
  });
</script>