import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

// 后端地址：默认公网隧道根地址；本地联调可用环境变量覆盖：
//   PowerShell: $env:VITE_API_PROXY='http://localhost:5091'
//   前端请求统一以 /ToolsConsole 为前置（后端 UsePathBase 挂载），代理原样转发即可
const API_PROXY = process.env.VITE_API_PROXY ?? 'https://www.tunn.work';

export default defineConfig({
  base: '/ToolsConsoleWeb/',
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/ToolsConsole': { target: API_PROXY, changeOrigin: true },
    },
  },
});
