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
        Welcome back to H<span class="blink">oo</span>kH<span class="blink"
          >oo</span
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
      Enter your new password.
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
          <span class="text-red-500">{{ this.msg }}</span>
          <form class="flex flex-col" @submit.prevent="send">
            <Input
              class="pb-5"
              type="password"
              label="Password"
              :reverse="true"
              :required="true"
              v-model="password"
            />
            <Input
              class="pb-5"
              type="password"
              label="Confirm password"
              :reverse="true"
              :required="true"
              v-model="confirm"
            />
            <Input type="submit" value="Register" :reverse="true" />
          </form>
        </div>
      </Bloc>
    </section>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { mapActions } from "vuex";
import Bloc from "@/components/BlocComponent.vue";
import Input from "@/components/InputComponent.vue";

export default defineComponent({
  setup() {},
  data() {
    return {
      password: "",
      confirm: "",
      msg: "",
    };
  },
  methods: {
      ...mapActions("signIn", ["confirmPassword"]),
    async send() {
      if (this.password !== this.confirm) {
        this.msg = "Confirmation doesn't match password";
        return;
      }
      this.msg = "";
      const res = await this.confirmPassword({ id: this.id, password: this.password });
      if (res.error !== undefined)
        this.msg = res.error;
      if (this.msg === "")
        this.$router.push("/dashboard");
    },
  },
  components: {
    Bloc,
    Input,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
  },
});
</script>
