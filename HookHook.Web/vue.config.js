const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    proxy: {
        '^/api': {
            target: process.env.BACKEND_URL || "http://localhost:8080/",
            ws: true,
            pathRewrite: {
                '^/api': ''
            }
        }
    },
    port: 80,
    allowedHosts: 'all'
  },
  configureWebpack: {
    resolve: {
      fallback: {
        "https": false
      }
    }
  }
})