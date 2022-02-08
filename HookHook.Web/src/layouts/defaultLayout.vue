<template>
  <div
    class="
      transition-colors
      duration-200
      bg-[#F9F9F9]
      dark:bg-[#181A1E]
      min-h-screen min-w-screen
      p-10
    "
  >
    <header class="flex flex-col sm:flex-row justify-between">
      <RouterLink to="/">
        <HookHook class="text-5xl" />
      </RouterLink>
      <div class="mt-6 sm:mt-0 flex flex-row gap-4">
        <Switch @ToggleDarkMode="this.$emit('ToggleDarkMode')" />
        <div
          v-if="isLoggedIn"
          class="
            bg-[#3B3F43]
            relative
            inline-flex
            flex-shrink-0
            h-[38px]
            w-[90px]
            border-2 border-transparent
            rounded-xl
            justify-evenly
            flex flex-row
          "
        >
          <Menu as="div" class="relative inline-block text-left">
            <div>
              <MenuButton class="w-8">
                <MenuIcon class="text-white" />
              </MenuButton>
            </div>

            <transition
              enter-active-class="transition duration-100 ease-out"
              enter-from-class="transform scale-95 opacity-0"
              enter-to-class="transform scale-100 opacity-100"
              leave-active-class="transition duration-75 ease-in"
              leave-from-class="transform scale-100 opacity-100"
              leave-to-class="transform scale-95 opacity-0"
            >
              <MenuItems
                class="
                  absolute
                  right-[-49px]
                  w-52
                  bg-[#3B3F43]
                  rounded-xl
                  focus:outline-none
                  text-white
                  p-3
                "
              >
                <MenuItem><RouterLink to="/dashboard" class="grid grid-cols-3 p-1 text-xl dark:hover:bg-[#181A1E] dark:hover:text-white hover:bg-white hover:text-[#3B3F43] rounded-xl"><ViewGridAddIcon class="h-8" /><span class="col-span-2 self-center">Dashboard</span></RouterLink></MenuItem>
                <MenuItem><a class="grid grid-cols-3 p-1 text-xl dark:hover:bg-[#181A1E] dark:hover:text-white hover:bg-white hover:text-[#3B3F43] rounded-xl"><PencilIcon class="h-8" /><span class="col-span-2 self-center">Edit Profile</span></a></MenuItem>
                <MenuItem><RouterLink to="/admin" class="grid grid-cols-3 p-1 text-xl dark:hover:bg-[#181A1E] dark:hover:text-white hover:bg-white hover:text-[#3B3F43] rounded-xl"><CogIcon class="h-8" /><span class="col-span-2 self-center">Admin</span></RouterLink></MenuItem>
                <MenuItem><a href="/" @click.prevent="preventLogout" class="grid grid-cols-3 p-1 text-xl dark:hover:bg-[#181A1E] dark:hover:text-white hover:bg-white hover:text-[#3B3F43] rounded-xl"><LogoutIcon class="h-8" /><span class="col-span-2 self-center">Logout</span></a></MenuItem>
              </MenuItems>
            </transition>
          </Menu>
          <UserIcon
            class="
              text-[#3B3F43]
              bg-white
              rounded-full
              m-[2px]
              border-4 border-white
            "
          />
        </div>
      </div>
    </header>
    <slot />
  </div>
</template>

<script lang="ts">
import Switch from "@/components/SwitchComponent.vue";
import HookHook from "@/components/HookHookComponent.vue";
import { defineComponent } from "vue";
import { mapGetters, mapActions } from "vuex";
import { UserIcon, MenuIcon, PencilIcon } from "@heroicons/vue/solid";
import { ViewGridAddIcon, LogoutIcon, CogIcon } from "@heroicons/vue/outline";
import { Menu, MenuButton, MenuItems, MenuItem } from "@headlessui/vue";

export default defineComponent({
  components: {
    Switch,
    HookHook,
    UserIcon,
    MenuIcon,
    MenuButton,
    Menu,
    MenuItems,
    MenuItem,
    ViewGridAddIcon,
    PencilIcon,
    LogoutIcon,
    CogIcon
  },
  computed: {
    ...mapGetters("user", ["isLoggedIn", "isAdmin"]),
  },
  methods: {
    ...mapActions('user', ["logout"]),
    preventLogout() {
      this.logout();
      this.$router.push('/');
    }
  }
});
</script>