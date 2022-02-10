import { createStore } from 'vuex'
import createPersistedState from "vuex-persistedstate";
import User from '@/store/user'

const store = createStore({
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
});

export default store;

export function authHeader() : HeadersInit {
  if (store.getters["user/isLoggedIn"])
    return { 'Authorization': 'Bearer ' + store.getters["user/token"] };
  return {};
}