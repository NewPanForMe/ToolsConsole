import { httpGet, httpPost } from './http';
import type {
  PagedResult,
  SystemConfigCreatePayload,
  SystemConfigItem,
  SystemConfigSaveByKeyPayload,
  SystemConfigUpdatePayload,
} from '@/types';

export interface PageQuery {
  keyword?: string;
  pageIndex: number;
  pageSize: number;
}

export function getPageApi(query: PageQuery): Promise<PagedResult<SystemConfigItem>> {
  return httpGet<PagedResult<SystemConfigItem>>('/SystemConfig/GetPage', query);
}

export function getByIdApi(id: number): Promise<SystemConfigItem> {
  return httpGet<SystemConfigItem>(`/SystemConfig/GetById/${id}`);
}

export function getByKeyApi(configKey: string): Promise<SystemConfigItem> {
  return httpGet<SystemConfigItem>('/SystemConfig/GetByKey', { configKey });
}

export function createApi(payload: SystemConfigCreatePayload): Promise<SystemConfigItem> {
  return httpPost<SystemConfigItem>('/SystemConfig/Create', payload);
}

export function updateApi(id: number, payload: SystemConfigUpdatePayload): Promise<SystemConfigItem> {
  return httpPost<SystemConfigItem>(`/SystemConfig/Update/${id}`, payload);
}

export function saveByKeyApi(payload: SystemConfigSaveByKeyPayload): Promise<SystemConfigItem> {
  return httpPost<SystemConfigItem>('/SystemConfig/SaveByKey', payload);
}
