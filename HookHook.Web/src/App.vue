<template>
  <router-view v-bind="slotData" />
</template>

<script lang="ts">
import Vue from 'vue'

export default {
  name: 'App'
}
</script>