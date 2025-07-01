import { reactRouter } from '@react-router/dev/vite'
import tailwindcss from '@tailwindcss/vite'
import { defineConfig } from 'vite'
import tsconfigPaths from 'vite-tsconfig-paths'

export default defineConfig({
  plugins: [tailwindcss(), reactRouter(), tsconfigPaths()],
  server: {
    host: true,
    port: parseInt(process.env.VITE_PORT ?? '3000'),
    allowedHosts: true,
    // proxy: {
    //   '/api': {
    //     target: process.env.services__weatherapi__https__0 || process.env.services__weatherapi__http__0,
    //     changeOrigin: true,
    //     rewrite: path => path.replace(/^\/api/, ''),
    //     secure: false
    //   }
    // }
  },
})
