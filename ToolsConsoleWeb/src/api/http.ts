import axios, { AxiosError, type AxiosInstance, type AxiosRequestConfig } from 'axios';
import type { ApiResult } from '@/types';
import { clearAuthStorage, getToken } from '@/utils/storage';

const http: AxiosInstance = axios.create({
  // 所有请求统一带 /ToolsConsole 前置（后端 UsePathBase 挂载）
  baseURL: '/ToolsConsole',
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' },
});

// 请求拦截：自动携带 JWT
http.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

function extractMessage(error: AxiosError<ApiResult<unknown>>): string {
  const data = error.response?.data;
  if (data && typeof data.message === 'string' && data.message) {
    return data.message;
  }
  if (error.response?.status === 401) {
    return '登录已失效，请重新登录';
  }
  return error.message || '请求失败';
}

// 响应拦截：code!==0 视为业务失败；401 清除登录态并跳登录页
http.interceptors.response.use(
  (response) => {
    const body = response.data as ApiResult<unknown>;
    if (body && typeof body.code === 'number' && body.code !== 0) {
      return Promise.reject(new Error(body.message || '请求失败'));
    }
    return response;
  },
  (error: AxiosError<ApiResult<unknown>>) => {
    if (error.response?.status === 401) {
      clearAuthStorage();
      // 避免与路由模块循环依赖，直接整页跳转（hash 路由 + /ToolsConsole 前置）
      if (!window.location.hash.startsWith('#/login')) {
        window.location.href = '/ToolsConsole/#/login';
      }
    }
    return Promise.reject(new Error(extractMessage(error)));
  },
);

async function request<T>(config: AxiosRequestConfig): Promise<T> {
  const response = await http.request<ApiResult<T>>(config);
  return (response.data as ApiResult<T>).data as T;
}

export function httpGet<T>(url: string, params?: object): Promise<T> {
  return request<T>({ method: 'GET', url, params });
}

export function httpPost<T>(url: string, data?: object): Promise<T> {
  return request<T>({ method: 'POST', url, data });
}

export function httpDelete<T = void>(url: string): Promise<T> {
  return request<T>({ method: 'DELETE', url });
}

export default http;
