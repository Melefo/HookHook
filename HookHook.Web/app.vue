<template>
    <Html v-bind:class="{dark: darkMode}">
        <div class="bg-white dark:bg-black">
            <input type="checkbox" v-model="darkMode">
            <NuxtPage/>
        </div>
    </Html>
</template>

<script>

    export default {
        data() {
            return {
                darkMode: true
            }
        }
    }
</script>