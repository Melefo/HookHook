<template>
  <div class="text-white mt-14">
    <p>Welcome!</p>
    <router-link v-if="!isLoggedIn" to="/login">
      <p>Login</p>
    </router-link>
    <router-link v-else to="/dashboard">
      <p>Dashboard</p>
    </router-link>
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