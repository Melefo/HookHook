
<template>
    <div class="bg-white dark:bg-black">
        <h1 class="va-1 underline dark:text-white">Home</h1>
        <Switch @myEvent="this.$emit('myEvent')"/>
    </div>
</template>

<script>
    import Switch from '../components/Switch.vue';

    export default {
        components: { Switch },

    }
</script>