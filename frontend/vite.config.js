import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/ (documentação do Vite)
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
  // Faz proxy das requisições para `/api` ao backend durante o desenvolvimento.
  // Atualize o `target` para a porta onde sua API .NET estiver rodando.
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      }
    }
  }
})