<template>
  <button type="button" :class="`button bg-[${bgColor}]`" @click="openModal">
    <img class="w-20 h-20 m-auto" :src="require(`@/assets/img/coloredsvg/${src}`)"/>
  </button>
  <TransitionRoot appear :show="IsOpen" as="template">
    <Dialog as="div" @close="closeModal">
      <div class="fixed inset-0 z-10 overflow-y-auto">
        <div class="min-h-screen px-4 text-center">
          <span class="inline-block h-screen align-middle" aria-hidden="true"/>
          <TransitionChild as="template">
            <div
              :class="`inline-block w-full max-w-md p-6 my-8 overflow-hidden text-left align-middle transition-all transform bg-[${bgColor}] shadow-xl rounded-2xl`"
            >
              <DialogTitle as="h3" class="text-lg font-medium leading-6 text-grey-800">
                {{ text }}
              </DialogTitle>
              <div class="mt-2">
                <p class="text-sm text-gray-500">
                  lorem ipsum
                </p>
              </div>

              <div class="mt-4">
                <button
                  type="button"
                  class="inline-flex justify-center px-4 py-2 text-sm font-medium text-black bg-neutral-200 border border-transparent rounded-md hover:bg-neutral-300 focus:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:ring-neutral-500"
                  @click="closeModal"
                >
                  Close this popup
                </button>
              </div>
            </div>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script lang="ts">
  import { defineComponent, ref } from "vue";
  import { TransitionRoot, TransitionChild, Dialog, DialogTitle,} from "@headlessui/vue";

  export default defineComponent ({
    name: 'DialogComponent',
    components: { TransitionRoot, TransitionChild, Dialog, DialogTitle },
    props: ['text', 'bgColor', 'src'],
    setup() {
      const IsOpen = ref(false)
    
      return {
        IsOpen,
        closeModal() {
          IsOpen.value = false
        },
        openModal() {
          IsOpen.value = true
        },
      }
    },
  });
</script>