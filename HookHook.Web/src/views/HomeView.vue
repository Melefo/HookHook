<template>
  <div>
    <div class="text-black mt-14 dark:text-white">
      <p>Welcome!</p>
      <router-link v-if="!isLoggedIn" to="/login">
        <p>Login</p>
      </router-link>
      <router-link v-else to="/dashboard">
        <p>Dashboard</p>
      </router-link>
    </div>
    <div class="text-black mt-14 dark:text-white">
      <img class="object-contain w-140 h-140 absolute bottom-10 left-10" src="@/assets/pinguin/Pingu1.gif" />
      <img class="object-contain w-140 h-140 absolute bottom-50 left-50 scale-x-[-1]" src="@/assets/pinguin/Pingu1.gif" />
    </div>
  </div>
  </template>

<script>
import { mapGetters } from 'vuex';
export default {
  computed: {
    ...mapGetters("user", ["isLoggedIn"])
  }
}
</script>