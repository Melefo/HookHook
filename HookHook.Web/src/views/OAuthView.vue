<template>
    <div></div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
    beforeCreate() {
        if (window.opener) {
            window.opener.postMessage(window.location.search);
        }
        window.close();
    }
})
</script>