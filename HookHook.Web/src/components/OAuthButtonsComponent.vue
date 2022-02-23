<template>
  <div class="text-black justify-start">
    <DialogComponent v-for="(item, key) in services" :key="key" :text="item.name" :src="item.name + '.svg'" :bgColor="color(item.name)">
      <div v-for="(account, keyy) in item.accounts" :key="account.userId" class="flex justify-between items-center mx-16">
        <p>{{ account.username }}</p>
        <button @click.prevent="async () => await deleteService(item.name, account.userId, key, keyy)">
          <XIcon class="h-8" />
        </button>
      </div>
      <component :is="item.name + 'Oauth'" :oauth="false" @addAccount="handleAdd($event, key)">
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
  import GithubOauth from "@/components/OAuth/GitHubOAuthComponent.vue";
  import SpotifyOauth from "@/components/OAuth/SpotifyOAuthComponent.vue";
  import TwitchOauth from "@/components/OAuth/TwitchOAuthComponent.vue";
  import TwitterOauth from '@/components/OAuth/TwitterOAuthComponent.vue';
  import GoogleOauth from "@/components/OAuth/GoogleOAuthComponent.vue";
  import { XIcon } from "@heroicons/vue/solid";

  export default defineComponent ({
    name: 'OAuthButtonsComponent',
    components: { DialogComponent, DiscordOauth, GithubOauth, SpotifyOauth, TwitchOauth, TwitterOauth, GoogleOauth, XIcon },
    props: ['text', 'bgColor', 'src'],
    methods: {
      ...mapActions("about", ["get"]),
      ...mapActions("service", ["getAccounts", "deleteAccount"]),
      handleAdd(e: any, key: number) {
        if (e.userId === undefined || e.username === undefined) {
          return;
        }
        this.services[key].accounts.push(e);
      },
      async deleteService(provider: String, id: String, serviceKey: number, accountKey: number) {
        await this.deleteAccount({provider: provider, id: id});
        this.services[serviceKey].accounts.splice(accountKey, 1);
      },
      color(name: string) {
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
    created: async function() {
      const { server: { services } } = await this.get();
      this.services = await Promise.all(services.map(async (s: any) => {
        const accounts = await this.getAccounts(s.name);

        return {...s, accounts: accounts };
      }));
    }
  });
</script>