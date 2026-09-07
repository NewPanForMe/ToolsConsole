import { httpDelete, httpGet, httpPost, httpPut } from './http';
import type {
  ConnectionStringDelivery,
  DbConnItem,
  DbConnSavePayload,
  PagedResult,
} from '@/types';

export interface PageQuery {
  keyword?: string;
  pageIndex: number;
  pageSize: number;
}

export function getPageApi(query: PageQuery): Promise<PagedResult<DbConnItem>> {
  return httpGet<PagedResult<DbConnItem>>('/DbConn/GetPage', query);
}

export function getByIdApi(id: number): Promise<DbConnItem> {
  return httpGet<DbConnItem>(`/DbConn/GetById/${id}`);
}

export function createApi(payload: DbConnSavePayload): Promise<DbConnItem> {
  return httpPost<DbConnItem>('/DbConn/Create', payload);
}

export function updateApi(id: number, payload: DbConnSavePayload): Promise<DbConnItem> {
  return httpPut<DbConnItem>(`/DbConn/Update/${id}`, payload);
}

export function deleteApi(id: number): Promise<void> {
  return httpDelete<void>(`/DbConn/Delete/${id}`);
}

/** 按 ConnName 获取链接串（匿名接口，返回密文 + 解密说明，明文不出网） */
export function getConnectionStringApi(connName: string): Promise<ConnectionStringDelivery> {
  return httpGet<ConnectionStringDelivery>('/ConnectionString/GetByConnName', { connName });
}
