
<template>
  <div class="flex flex-col items-center">
    <span class="text-white font-bold mb-2" v-if="!errors && !error && sent"
      >You can now authenticate</span
    >
    <span class="text-red-500" v-if="error">{{ this.error }}</span>
    <form class="flex flex-col" @submit.prevent="send">
      <span class="text-red-500" v-if="errors && errors.firstname">{{
        this.errors.firstname
      }}</span>
      <span class="text-red-500" v-if="errors && errors.lastname">{{
        this.errors.lastname
      }}</span>
      <div class="grid grid-cols-2 gap-4">
        <Input
          class="pb-5"
          type="text"
          label="First name"
          :reverse="true"
          :required="true"
          v-model="firstname"
        />
        <Input
          class="pb-5"
          type="text"
          label="Last name"
          :reverse="true"
          :required="true"
          v-model="lastname"
        />
      </div>
      <span class="text-red-500" v-if="errors && errors.email">{{
        this.errors.email
      }}</span>

      <Input
        class="pb-5"
        type="email"
        label="Email"
        :reverse="true"
        :required="true"
        v-model="email"
      />
      <span class="text-red-500" v-if="errors && errors.username">{{
        this.errors.username
      }}</span>
      <Input
        class="pb-5"
        type="text"
        label="Username"
        :reverse="true"
        :required="true"
        v-model="username"
      />
      <span class="text-red-500" v-if="errors && errors.password">{{
        this.errors.password
      }}</span>
      <div class="grid grid-cols-2 gap-4">
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
      </div>
      <Input type="submit" value="Register" :reverse="true" />
    </form>
  </div>
</template>

<script lang="ts">
import Input from "@/components/InputComponent.vue";
import { mapActions } from "vuex";
import { defineComponent } from "vue";

declare interface Errors {
  firstname: String;
  lastname: String;
  username: String;
  email: String;
  password: String;
}

export default defineComponent({
  components: { Input },
  data() {
    return {
      firstname: "",
      lastname: "",
      username: "",
      email: "",
      password: "",
      confirm: "",
      errors: null as Errors | null,
      error: null as String | null,
      sent: false,
    };
  },
  methods: {
    ...mapActions("signIn", ["register"]),
    async send() {
      if (this.password !== this.confirm) {
        this.errors = {} as Errors;
        this.errors.password = "Confirmation doesn't match password";
        return;
      }

      const { errors, error } = await this.register({
        firstName: this.firstname,
        lastName: this.lastname,
        username: this.username,
        email: this.email,
        password: this.password,
      });
      this.errors = errors || null;
      this.error = error || null;
      this.sent = true;
    },
  },
});
</script>