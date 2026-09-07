import { httpPost } from './http';
import type { LoginRequest, LoginResponse } from '@/types';
import { aesEncryptPassword } from '@/utils/crypto';

/** 登录：密码先 AES-256-CBC 加密再传输，后端解密后校验 */
export async function loginApi(payload: LoginRequest): Promise<LoginResponse> {
  const encrypted = await aesEncryptPassword(payload.password);
  return httpPost<LoginResponse>('/Auth/Login', {
    userName: payload.userName,
    password: encrypted,
  });
}
