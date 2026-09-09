import type { SiteConfigResponse } from '@/types/site-config'

interface ApiResult<T> {
  code: number
  message: string
  data: T | null
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/ToolsConsole'

export async function getSiteConfigApi(): Promise<SiteConfigResponse> {
  const response = await fetch(`${API_BASE_URL}/SiteConfig/Get`)
  if (!response.ok) {
    throw new Error(`站点配置加载失败：${response.status}`)
  }

  const body = (await response.json()) as ApiResult<SiteConfigResponse>
  if (body.code !== 0) {
    throw new Error(body.message || '站点配置加载失败')
  }

  return body.data ?? {}
}
