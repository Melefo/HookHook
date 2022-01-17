<template>
    <Html v-bind:class="{dark: darkMode}">
        <input type="checkbox" v-model="darkMode">
        <NuxtPage/>
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