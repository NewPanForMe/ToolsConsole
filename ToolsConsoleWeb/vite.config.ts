import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

// 后端地址：默认公网隧道根地址，不要在 target 后追加 /ToolsConsole；
// 前端 baseURL 已经带 /ToolsConsole，代理会把完整路径原样转发。
// 本地联调可用环境变量覆盖：
//   PowerShell: $env:VITE_API_PROXY='http://localhost:5091'
const API_PROXY =  'http://localhost:5091';

export default defineConfig({
  base: '',
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
