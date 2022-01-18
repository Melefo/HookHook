<template>
    <div :class="{dark: darkMode}">
        <router-view @myEvent="ToggleDarkMode"/>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'

    export default {
        name: 'App',
        data() {
            return ({
                darkMode: false
            })
        },
        methods: {
            ToggleDarkMode() {
                this.darkMode = !this.darkMode;
            }
        }
    }
</script>