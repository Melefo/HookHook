import { createRouter, createWebHistory } from 'vue-router'
import Home from '@/views/HomeView.vue'
import OAuth from '@/views/OAuthView.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home
  },
  {
    path: '/oauth',
    name: 'oauth',
    component: OAuth
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
