import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import './index.css'
import Default from '@/layouts/defaultLayout.vue'
import Title from '@/layouts/titleLayout.vue'
// import GAuth from 'vue3-google-oauth2'
const GAuth = require('vue3-google-oauth2');

const app = createApp(App)

const gAuthOptions = { clientId: process.env.VUE_APP_GOOGLE_CLIENTID, scope: 'email', prompt: 'consent', fetch_basic_profile: false }
app.use(GAuth, gAuthOptions);

app.use(store).use(router).mount('#app')
app.component('DefaultLayout', Default)
app.component('TitleLayout', Title)