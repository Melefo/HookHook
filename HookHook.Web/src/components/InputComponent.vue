<template>
  <div class="flex flex-col">
    <label v-if="label !== ''" v-html="label" class="text-black dark:text-white"></label>
    <input
      v-if="type === 'submit'"
      class="
        rounded-lg
        w-auto
        p-2
        text-black
        dark:text-white
        dark:bg-[#181A1E]
        dark:hover:bg-[#292b30]
        dark:active:bg-[#181A1E]
        bg-[#E9E9E9]
        hover:bg-[#E2E2E2]
        active:bg-[#E9E9E9]
        hover:cursor-pointer
        transition-colors
        duration-200
      "
      :type="type"
      :value="value"
    />
    <input v-else class="rounded-lg w-auto p-2 border-0" :type="type" :required="required" :placeholder="label" minlength="4" maxlength="256" @input="handleInput($event.target.value)" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  props: {
    type: {
      type: String,
      required: true,
    },
    value: {
      type: String,
      required: false,
      default: "",
    },
    label: {
      type: String,
      required: false,
      default: "",
    },
    reverse: {
      type: Boolean,
      required: false,
      default: false,
    },
    required: {
      type: Boolean,
      required: false,
      default: false
    },
    modelValue: {
      type: String,
      required: false,
      default: function(props: any) {
        return props.value;
      }
    },
  },
  emits: ['update:modelValue'],
  methods: {
    handleInput(input: String) {
      this.$emit('update:modelValue', input);
    }
  }
});
</script>