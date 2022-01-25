import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import './index.css'
import Default from '@/layouts/defaultLayout.vue'

const app = createApp(App)

app.use(store).use(router).mount('#app')
app.component('DefaultLayout', Default)
