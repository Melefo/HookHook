import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/views/HomeView.vue'
import Login from '@/views/LoginView.vue'
import Register from '@/views/RegisterView.vue'
import Dashboard from '@/views/DashboardView.vue'
import OAuth from '@/views/OAuthView.vue'
import Admin from '@/views/AdminView.vue'
import Verify from "@/views/VerifyView.vue"
import Forgot from "@/views/ForgotPasswordView.vue"
import Confirm from "@/views/ConfirmPasswordView.vue"
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
    path: '/register',
    name: 'register',
    component: Register,
    meta: {
      onlyGuest: true
    }
  },
  {
    path: '/dashboard',
    name: 'dashboard',
    component: Dashboard,
    meta: {
      onlyUser: true
    }
  },
  {
    path: '/oauth',
    name: 'oauth',
    component: OAuth
  },
  {
    path: '/admin',
    name: 'admin',
    component: Admin,
    meta: {
      onlyAdmin: true
    }
  },
  {
    path: '/verify/:id',
    name: 'verify',
    component: Verify,
    meta: {
      onlyGuest: true
    },
    props: true
  },
  {
    path: '/forgot',
    name: 'forgot',
    component: Forgot,
    meta: {
      onlyGuest: true
    }
  },
  {
    path: '/confirm/:id',
    name: 'confirm',
    component: Confirm,
    meta: {
      onlyGuest: true
    },
    props: true
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})


router.beforeEach((to, from, next) => {
  if (store.getters["signIn/isLoggedIn"]) {
    const jwt = parseJwt(store.getters["signIn/token"]);
    if (jwt !== null && jwt.exp < Date.now() / 1000) {
        store.dispatch("signIn/logout");
    }
  }

  if (to.meta && to.meta.onlyAdmin && !store.getters["signIn/isAdmin"]) {
    return next('/dashboard');
  }
  if (to.meta && to.meta.onlyUser && !store.getters["signIn/isLoggedIn"]) {
    return next('/login');
  }
  if (to.meta && to.meta.onlyGuest && store.getters["signIn/isLoggedIn"]) {
    return next('/');
  }
  next();
})

declare interface Jwt {
  email: String,
  role: String,
  unique_name: String,
  given_name: String,
  nbf: Number,
  exp: Number,
  iat: Number
}

export function parseJwt(token: String): Jwt|null {
  if (token === null)
    return null;
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(atob(base64).split("").map(function (c) {
    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
  }).join(''));

  return JSON.parse(jsonPayload);
}

export default router