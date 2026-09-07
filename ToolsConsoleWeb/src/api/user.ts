import { httpDelete, httpGet, httpPost } from './http';
import type { PagedResult, UserCreatePayload, UserItem, UserUpdatePayload } from '@/types';
import { aesEncryptPassword } from '@/utils/crypto';

export interface PageQuery {
  keyword?: string;
  pageIndex: number;
  pageSize: number;
}

export function getPageApi(query: PageQuery): Promise<PagedResult<UserItem>> {
  return httpGet<PagedResult<UserItem>>('/User/GetPage', query);
}

export function getByIdApi(id: number): Promise<UserItem> {
  return httpGet<UserItem>(`/User/GetById/${id}`);
}

/** 新增用户：初始密码先 AES 加密再传输 */
export async function createApi(payload: UserCreatePayload): Promise<UserItem> {
  const encrypted = await aesEncryptPassword(payload.password);
  return httpPost<UserItem>('/User/Create', {
    userName: payload.userName,
    password: encrypted,
    displayName: payload.displayName,
  });
}

export function updateApi(id: number, payload: UserUpdatePayload): Promise<UserItem> {
  return httpPost<UserItem>(`/User/Update/${id}`, payload);
}

/** 重置密码：新密码先 AES 加密再传输 */
export async function resetPasswordApi(id: number, password: string): Promise<void> {
  const encrypted = await aesEncryptPassword(password);
  return httpPost<void>(`/User/ResetPassword/${id}`, { password: encrypted });
}

export function deleteApi(id: number): Promise<void> {
  return httpDelete<void>(`/User/Delete/${id}`);
}
