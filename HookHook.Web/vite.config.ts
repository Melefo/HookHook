import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  server: {
    proxy: {
      '^/api': {
        target: process.env.BACKEND_URL ||"http://localhost:8080/",
        ws: true,
        rewrite: (path) => path.replace('^/api', '')
      }
    },
    port: 80,
    host: process.env.PUBLIC_URL
  }
})
