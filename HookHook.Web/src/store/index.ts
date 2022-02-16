import { createStore } from 'vuex'
import createPersistedState from "vuex-persistedstate";
import User from '@/store/user'
import About from '@/store/about'
import SignIn from '@/store/signIn'

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
    'user': User,
    'about': About,
    'signIn': SignIn
  }
});

export default store;

export function authHeader() : HeadersInit {
  if (store.getters["signIn/isLoggedIn"])
    return { 'Authorization': 'Bearer ' + store.getters["signIn/token"] };
  return {};
}