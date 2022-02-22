import { createStore } from 'vuex'
import createPersistedState from "vuex-persistedstate";
import User from '@/store/user'
import About from '@/store/about'
import Area from '@/store/area'
import SignIn from '@/store/signIn'
import Service from '@/store/service'

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
    'area': Area,
    'signIn': SignIn,
    'service': Service
  }
});

export default store;

export function authHeader() : HeadersInit {
  if (store.getters["signIn/isLoggedIn"])
    return { 'Authorization': 'Bearer ' + store.getters["signIn/token"] };
  return {};
}