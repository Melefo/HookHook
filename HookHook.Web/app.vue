<template>
    <Html v-bind:class="{dark: darkMode}">
        <div class="bg-white dark:bg-black">
            <input type="checkbox" v-model="darkMode">
            <NuxtPage/>
        </div>
    </Html>
</template>

<style>
    @import url('https://fonts.googleapis.com/css2?family=Comfortaa&display=swap');
</style>

<script>

    export default {
        data() {
            return {
                darkMode: true
            }
        }
    }
</script>