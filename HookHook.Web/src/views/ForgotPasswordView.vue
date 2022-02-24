<template>
  <div>
    <div class="flex flex-row justify-center items-center whitespace-nowrap">
      <img class="w-20 h-20" src="@/assets/pinguin/breakdance.gif" />
      <h2
        class="
          p-2
          font-medium
          text-3xl text-black
          dark:text-white
          transition-colors
          duration-200
        "
      >
        Welcome to H<span class="blink">oo</span>kH<span class="blink">oo</span
        >k
      </h2>
      <img class="w-20 h-20" src="@/assets/pinguin/breakdance.gif" />
    </div>
    <h2
      class="
        flex flex-row
        justify-center
        items-center
        p-5
        text-2xl text-black
        dark:text-white
      "
    >
      Forgot your password?
    </h2>
    <section class="flex flex-col justify-center items-center gap-10">
      <Bloc
        class="
          flex flex-col
          justify-center
          items-center
          xl:w-[40%]
          lg:w-[60%]
          md:w-[80%]
          w-[90%]
        "
      >
        <div class="flex flex-col items-center">
          <span class="text-black dark:text-white text-center">{{ this.msg }}</span>
          <form class="flex flex-col" @submit.prevent="send">
            <Input
              class="pb-5"
              type="text"
              label="Username/Email"
              :reverse="true"
              :required="true"
              v-model="username"
            />
            <Input type="submit" class="pb-5" value="Send" :reverse="true" />
          </form>
        </div>
      </Bloc>
    </section>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Bloc from "@/components/BlocComponent.vue";
import Input from "@/components/InputComponent.vue";
import { mapActions } from "vuex";

export default defineComponent({
  setup() {},
  components: {
    Bloc,
    Input,
  },
  data() {
      return {
          msg: "",
          username: ""
      }
  },
  methods: {
      ...mapActions("signIn", ["forgot"]),
      async send() {
          await this.forgot(this.username);
          this.msg = "If an account with this email or username exists an email has been sent to recover your password"
      }
  }
});
</script>