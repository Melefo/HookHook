<template>
  <div>
    <section class="text-black mt-14 dark:text-white text-center">
      <div class="flex flex-col lg:flex-row justify-around">
        <img src="@/assets/pinguin/zen.gif" class="h-20 hidden lg:block" />
        <h2 class="font-thin text-xl lg:text-4xl lg:w-[50%] uppercase">
          The best way to automate your work and make it more relaxing
        </h2>
        <img src="@/assets/pinguin/zen.gif" class="h-20 object-contain" />
      </div>
      <div>
        <p class="my-8">Welcome! Connect your apps and be more zen!</p>
        <router-link v-if="!isLoggedIn" to="/login">
          <div class="button-74 transition-colors duration-200">
            <p>Start for free</p>
          </div>
        </router-link>
        <router-link v-else to="/dashboard">
          <div class="button-74 transition-colors duration-200">
            <p>Dashboard</p>
          </div>
        </router-link>
      </div>
    </section>
    <section
      class="
        dark:bg-[#f0f0f0]
        bg-[#181A1E]
        flex flex-col lg:flex-row
        justify-around
        mt-8
        py-4
        text-white
        dark:text-center
      "
    >
      <div
        class="
          text-white
          dark:text-black
          justify-center
          self-center
          text-center
        "
      >
        <h3 class="text-xl mb-2 text-[#A3E7EE] dark:text-[#f09113]">Lack of communication?</h3>
        <p>Automate the publishing of your content on your social networks</p>
        <p>When <b>X</b>, Send <b>Y</b> to <b>Z</b></p>
      </div>
      <div class="grid justify-center">
        <img
          class="row-start-1 row-end-1 col-start-1 col-end-1"
          src="@/assets/pinguin/Pingu1.gif"
        />
        <img
          class="row-start-1 row-end-1 col-start-1 col-end-1 scale-x-[-1]"
          src="@/assets/pinguin/Pingu1.gif"
        />
      </div>
    </section>
    <section class="flex flex-col-reverse lg:flex-row justify-around mt-8 py-2 dark:text-white text-center">
      <div class="justify-self-center self-center">
        <img class="h-20" src="@/assets/pinguin/maracas.gif" />
      </div>
      <div class="justify-center self-center text-center">
        <h3 class="text-xl mb-2 dark:text-[#A3E7EE] text-[#f09113]">
          Want more time to devote to your personal life?
        </h3>
        <p>Automate all your repetitive tasks</p>
        <p>When <b>X</b> is done, Do <b>Y</b></p>
      </div>
    </section>
    <section
      class="
        dark:bg-[#f0f0f0]
        bg-[#181A1E]
        flex flex-col lg:flex-row justify-around
        mt-8
        py-10
        text-white
        dark:text-center
      "
    >
      <div
        class="
          text-white
          dark:text-black
          justify-center
          self-center
          text-center
        "
      >
        <h3 class="text-xl mb-2 text-[#A3E7EE] dark:text-[#f09113]">Want to share your creations?</h3>
        <p>Share on only one platform and the rest will follow!</p>
        <p>When <b>X</b> is shared, share to <b>Y</b> and <b>Z</b></p>
      </div>
      <div class="justify-self-center self-center">
        <img class="h-20" src="@/assets/pinguin/photo.gif" />
      </div>
    </section>
    <section class="flex flex-col-reverse justify-around lg:flex-row mt-8 py-2 dark:text-white text-center">
      <div class="justify-self-center self-center">
        <img class="h-32" src="@/assets/pinguin/dance.gif" />
      </div>
      <div class="justify-center self-center text-center">
        <h3 class="text-xl mb-2 dark:text-[#A3E7EE] text-[#f09113]">Creators</h3>
        <div class="flex justify-evenly flex-col lg:flex-row lg:gap-8">
          <a
            href="https://github.com/mllusar"
            class="flex flex-row items-center"
          >
            Halfa<img class="h-10 dark:invert" src="@/assets/img/halfa.svg" />
          </a>
          <a
            href="https://portfolio-the-law-1.vercel.app/en"
            class="flex flex-row items-center"
          >
            The_Law<img
              class="h-10 dark:invert"
              src="@/assets/img/the_law.svg"
            />
          </a>
          <a href="https://melefo.xyz" class="flex flex-row items-center">
            Melefo<img class="h-10 dark:invert" src="@/assets/img/melefo.svg" />
          </a>
          <a href="https://github.com/Nywes" class="flex flex-row items-center">
            Nywes<img class="h-10 dark:invert" src="@/assets/img/nywes.svg" />
          </a>
        </div>
      </div>
    </section>
  </div>
</template>

<style>
.button-74 {
  background-color: #fbeee0;
  border: 2px solid #f09113;
  border-radius: 30px;
  box-shadow: #f09113 4px 4px 0 0;
  color: #f09113;
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
  border: 2px solid #a3e7ee;
  box-shadow: #a3e7ee 4px 4px 0 0;
  color: #a3e7ee;
}

.button-74:hover {
  background-color: #fff;
}

.dark:hover .button-74:hover {
  background-color: #1f4146;
}

.button-74:active {
  box-shadow: #f09113 2px 2px 0 0;
  transform: translate(2px, 2px);
}

.dark:active .button-74:active {
  box-shadow: #a3e7ee 2px 2px 0 0;
}

@media (min-width: 768px) {
  .button-74 {
    min-width: 120px;
    padding: 0 25px;
  }
}
</style>

<script>
import { mapGetters } from "vuex";
export default {
  computed: {
    ...mapGetters("signIn", ["isLoggedIn"]),
  },
};
</script>