import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  // Without this, Vite's PostCSS loader walks up the directory tree and picks up
  // an unrelated C:\postcss.config.js (which references a tailwindcss plugin this
  // project doesn't have installed), crashing CSS processing. An inline empty
  // config makes Vite skip that file-based search entirely.
  css: {
    postcss: {
      plugins: [],
    },
  },
})
