<template>
  <div>
    <div class="text-black mt-14 dark:text-white text-center">
      <p>Welcome!</p>
      <router-link v-if="!isLoggedIn" to="/login">
        <div class="button-74">
          <p>Login</p>
        </div>
      </router-link>
      <router-link v-else to="/dashboard">
        <div class="button-74">
          <p>Dashboard</p>
        </div>
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
    <div class="flex lg:flex-col sm:flex-row flex-col lg:items-end items-center justify-evenly dark:text-white">
      <p>Creators :</p>
      <button class="flex flex-row items-center cursor-default">Halfa<img class="h-10 dark:invert" src="@/assets/img/halfa.svg" /></button>
      <button class="flex flex-row items-center cursor-default">The_Law<img class="h-10 dark:invert" src="@/assets/img/the_law.svg" /></button>
      <button class="flex flex-row items-center cursor-default">Melefo<img class="h-10 dark:invert" src="@/assets/img/melefo.svg" /></button>
      <button class="flex flex-row items-center cursor-default">Nywes<img class="h-10 dark:invert" src="@/assets/img/nywes.svg" /></button>
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

.button-74 {
  background-color: #fbeee0;
  border: 2px solid #F09113;
  border-radius: 30px;
  box-shadow: #F09113 4px 4px 0 0;
  color: #F09113;
  cursor: pointer;
  display: inline-block;
  font-weight: 600;
  font-size: 18px;
  padding: 0 18px;
  line-height: 50px;
  text-align: center;
  text-decoration: none;
  user-select: none;
  -webkit-user-select: none;
  touch-action: manipulation;
}

.dark .button-74 {
  background-color: #11272a;
  border: 2px solid #A3E7EE;
  box-shadow: #A3E7EE 4px 4px 0 0;
  color: #A3E7EE;
}

.button-74:hover {
  background-color: #fff;
}

.dark:hover .button-74:hover {
  background-color: #1f4146;
}

.button-74:active {
  box-shadow: #F09113 2px 2px 0 0;
  transform: translate(2px, 2px);
}

.dark:active .button-74:active {
  box-shadow: #A3E7EE 2px 2px 0 0;
}

@media (min-width: 768px) {
  .button-74 {
    min-width: 120px;
    padding: 0 25px;
  }
}

</style>

<script>
import { mapGetters } from 'vuex';
export default {
  computed: {
    ...mapGetters("signIn", ["isLoggedIn"])
  }
}
</script>