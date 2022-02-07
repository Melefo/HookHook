<template>
  <div>
    <div class="text-black mt-14 dark:text-white text-center">
      <p>Welcome!</p>
      <router-link v-if="!isLoggedIn" to="/login">
        <p>Login</p>
      </router-link>
      <router-link v-else to="/dashboard">
        <p>Dashboard</p>
      </router-link>
    </div>
    <div class="text-black mt-14 dark:text-white grid justify-center">
      <img class="object-contain row-start-1 row-end-1 col-start-1 col-end-1" src="@/assets/pinguin/Pingu1.gif" />
      <img class="object-contain row-start-1 row-end-1 col-start-1 col-end-1 scale-x-[-1]" src="@/assets/pinguin/Pingu1.gif" />
      <!--<img class="object-contain row-start-1 row-end-1 col-start-1 col-end-1" src="@/assets/pinguin/Pingu2.gif" />
      <img class="object-contain row-start-1 row-end-1 col-start-1 col-end-1" src="@/assets/pinguin/Pingu2reverse.gif" />-->
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