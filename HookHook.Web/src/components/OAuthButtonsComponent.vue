<template>
  <div class="text-black mt-2 justify-start">
    <DialogComponent v-for="(item, key) in services" :key="key" :text="item.name" :src="item.name + '.svg'" :bgColor="color(item.name)" />
  </div>
</template>

<script lang="ts">
  import { defineComponent } from "vue";
  import DialogComponent from "@/components/DialogComponent.vue";
  import { mapActions } from "vuex";

  export default defineComponent ({
    name: 'OAuthButtonsComponent',
    components: { DialogComponent },
    props: ['text', 'bgColor', 'src'],
    methods: {
      ...mapActions("about", ["get"]),
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
            return "#F8CBAA";
          case 'twitch':
            return "#FFFFC7";
        }
      }
    },
    data: function() {
      return {
        services: [],
      }
    },
    created: async function() {
      const { server: { services } } = await this.get();
      this.services = services;
    }
  });
</script>