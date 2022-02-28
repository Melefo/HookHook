<template>
  <div class="flex flex-col items-center">
    <span class="text-red-500" v-if="error">{{ this.error }}</span>
    <form class="flex flex-col" @submit.prevent="send">
      <span class="text-red-500" v-if="errors && errors.username">{{
        this.errors.username
      }}</span>
      <Input
        class="pb-5"
        type="text"
        label="Username/Email"
        :reverse="true"
        :required="true"
        v-model="username"
      />
      <span class="text-red-500" v-if="errors && errors.password">{{
        this.errors.password
      }}</span>
      <Input
        class="pb-2"
        type="password"
        label="Password"
        :reverse="true"
        :required="true"
        v-model="password"
      />
      <RouterLink to="/forgot">
        <div class="pb-5 text-black dark:text-white underline">
          Forgot password?
        </div>
      </RouterLink>
      <Input type="submit" class="pb-5" value="Login" :reverse="true" />
      <RouterLink to="/register">
        <Input type="submit" value="Register"/>
      </RouterLink>
    </form>
  </div>
</template>

<script lang="ts">
import Input from "@/components/InputComponent.vue";
import { mapActions } from "vuex";
import { defineComponent } from "vue";

export default defineComponent({
  components: { Input },
  data() {
    return {
      username: "",
      password: "",
      error: null,
      errors: null,
    };
  },
  methods: {
    ...mapActions("signIn", ["login"]),
    async send() {
      const { errors, error } = await this.login({
        username: this.username,
        password: this.password,
      });
      this.errors = errors || null;
      this.error = error || null;
      if (!this.error && !this.errors) {
        this.$router.push("/dashboard");
      }
    },
  },
});
</script>