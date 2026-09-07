import { reactive } from 'vue';
import { loginApi } from '@/api/auth';
import type { LoginResponse } from '@/types';
import { clearAuthStorage, getStoredUser, getToken, setStoredUser, setToken } from '@/utils/storage';

/** 轻量登录态（不使用 pinia，读写 localStorage） */
export const authState = reactive<{ token: string; userName: string; displayName: string }>({
  token: getToken(),
  userName: getStoredUser().userName,
  displayName: getStoredUser().displayName,
});

export function isLoggedIn(): boolean {
  return Boolean(authState.token);
}

export function applyLogin(res: LoginResponse): void {
  authState.token = res.token;
  authState.userName = res.userName;
  authState.displayName = res.displayName;
  setToken(res.token);
  setStoredUser({ userName: res.userName, displayName: res.displayName });
}

export function logout(): void {
  authState.token = '';
  authState.userName = '';
  authState.displayName = '';
  clearAuthStorage();
}

export async function doLogin(userName: string, password: string): Promise<void> {
  const res = await loginApi({ userName, password });
  applyLogin(res);
}
