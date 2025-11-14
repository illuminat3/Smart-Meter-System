import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'node:path';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    proxy: {
      // Proxy REST API in dev to avoid CORS
      '/api': {
        target: 'http://localhost:5234',
        changeOrigin: true,
      },
      // Proxy SignalR hub with WebSocket support
      '/hub': {
        target: 'http://localhost:5234',
        ws: true,
        changeOrigin: true,
      },
    },
  },
});
