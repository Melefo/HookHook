<template>
    <div>
        <h2 class="text-4xl mt-10 inline-block text-black dark:text-white transition-colors duration-200">
            Hello {{username}},
        </h2>
        <h2 class="text-4xl mt-10 inline-block text-gray-400 dark:text-gray-400 transition-colors duration-200">
            &nbsp; welcome back!
        </h2>

        <h2 class="text-2xl mt-10 text-black dark:text-white transition-colors duration-200">
            Services
        </h2>

        <div class="flex gap-x-5 mt-8">
            <div class="rounded-lg w-32 h-32 bg-[#A3E7EE]">
                <img class="mx-auto mt-3.5 w-24 h-24" alt="twitter" src="@/assets/img/twitter.svg" />
            </div>
            <div class="rounded-lg w-32 h-32 bg-[#B4E1DC]">
                <img class="mx-auto mt-3.5 w-24 h-24" alt="spotify" src="@/assets/img/spotify.svg" />
            </div>
            <div class="rounded-lg w-32 h-32 bg-[#D9D1EA]">
                <img class="mx-auto mt-3.5 w-24 h-24" alt="discord" src="@/assets/img/discord.svg" />
            </div>
            <div class="rounded-lg w-32 h-32 bg-[#F5CDCB]">
                <img class="mx-auto mt-3.5 w-24 h-24" alt="github" src="@/assets/img/github.svg" />
            </div>
            <div class="rounded-lg w-32 h-32 bg-[#F8CBAA]">
                <img class="mx-auto mt-3.5 w-24 h-24" alt="youtube" src="@/assets/img/youtube.svg" />
            </div>
            <div class="rounded-lg w-32 h-32 bg-[#FFFFC7]">
                <img class="mx-auto mt-3.5 w-24 h-24" alt="twitch" src="@/assets/img/twitch.svg" />
            </div>
        </div>

        <h2 class="text-2xl mt-10 text-black dark:text-white transition-colors duration-200">
            Creator
        </h2>
        <div class="rounded-lg w-1/3 h-96 bg-[#3B3F43]">
            <h2 class="text-2xl ml-5 text-black dark:text-white transition-colors duration-200">
                AREA name:
            </h2>
        </div>
    </div>
</template>


<script lang="ts">
// import { defineComponent } from "vue";

// * faudrait mettre le username dans le computed
export default {
    data() {
        return ({
            username: "Arthur"
        })
    }
};
</script>