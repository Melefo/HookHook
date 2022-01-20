<template>
    <div :class="{dark: darkMode}">
        <router-view @ToggleDarkMode="ToggleDarkMode"/>
    </div>
</template>

<script lang="ts">
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