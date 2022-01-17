import { defineNuxtConfig } from 'nuxt3'

export default defineNuxtConfig({
    nitro: {
        preset: 'browser'
    },
    css: [
        "@/assets/css/main.css",
    ],
    typescript: {
        strict: true
    },
    build: {
        postcss: {
            postcssOptions: require("./postcss.config.js"),
        },
    }
})