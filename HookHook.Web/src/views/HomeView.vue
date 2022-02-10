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
    <div class="text-black mt-14 dark:text-white gridPingus justify-center">
      <img class="pingu1 row-start-1 row-end-1 col-start-1 col-end-1" src="@/assets/pinguin/Pingu1.gif" />
      <img class="pingu2 row-start-1 row-end-1 col-start-1 col-end-1 scale-x-[-1]" src="@/assets/pinguin/Pingu1.gif" />
      <!--<img class="object-contain row-start-1 row-end-1 col-start-1 col-end-1" src="@/assets/pinguin/Pingu2.gif" />
      <img class="object-contain row-start-1 row-end-1 col-start-1 col-end-1" src="@/assets/pinguin/Pingu2reverse.gif" />-->
      <img class="pingu3 row-start-1 row-end-1 col-start-1 col-end-1 scale-[0.25]" src="@/assets/pinguin/maracas.gif" />
      <img class="pingu4 row-start-1 row-end-1 col-start-1 col-end-1 scale-[0.25]" src="@/assets/pinguin/maracas.gif" />
    </div>
  </div>
</template>

<style scoped>

.gridPingus {
  display: grid;
  grid-template-columns: 30% 40% 30%;
  grid-template-rows: 100%;
}
.pingu1,
.pingu2 {
  grid-column: 2 / span 1;
}
.pingu3 {
  grid-column: 1 / span 1;
}
.pingu4 {
  grid-column: 3 / span 1;
}

.pingu1,.pingu2,.pingu3,.pingu4 {
  justify-self: center;
  align-self: start;
}

</style>

<script>
import { mapGetters } from 'vuex';
export default {
  computed: {
    ...mapGetters("user", ["isLoggedIn"])
  }
}
</script>