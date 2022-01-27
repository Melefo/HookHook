import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/views/HomeView.vue'
import Login from '@/views/LoginView.vue'
import Dashbaord from '@/views/DashbaordView.vue'
import store from '@/store'

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home,
    meta: {
      layout: "TitleLayout"
    }
  },
  {
    path: '/login',
    name: 'login',
    component: Login,
    meta: {
      onlyGuest: true
    }
  },
  {
    path: '/dashboard',
    name: 'dashboard',
    component: Dashbaord,
    meta: {
      onlyUser: true
    }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})


router.beforeEach((to, from, next) => {
  if (to.meta && to.meta.onlyAdmin && !store.getters["user/isAdmin"]) {
    return next('/dashboard');
  }
  if (to.meta && to.meta.onlyUser && !store.getters["user/isLoggedIn"]) {
    return next('/login');
  }
  if (to.meta && to.meta.onlyGuest && store.getters["user/isLoggedIn"]) {
    return next('/');
  }
  next();
})

declare interface Jwt {
  email: String,
  Role: String,
  unique_name: String,
  given_name: String,
  nbf: Date,
  exp: Date,
  iat: Date
}

export function parseJwt(token: String): Jwt {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(atob(base64).split("").map(function (c) {
    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
  }).join(''));

  return JSON.parse(jsonPayload);
}

export default router