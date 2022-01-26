import { createStore } from 'vuex'
import createPersistedState from "vuex-persistedstate";
import User from '@/store/user'

export default createStore({
  plugins: [createPersistedState()],
  state: {
  },
  getters: {
  },
  mutations: {
  },
  actions: {
  },
  modules: {
    'user': User
  }
})
