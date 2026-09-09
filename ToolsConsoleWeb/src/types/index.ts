// 与后端 DTO 对齐的接口类型定义

/** 后端统一响应包装 */
export interface ApiResult<T> {
  code: number;
  message: string;
  data: T | null;
}

/** 登录响应 */
export interface LoginResponse {
  token: string;
  userName: string;
  displayName: string;
  expiresIn: number;
}

/** 登录请求 */
export interface LoginRequest {
  userName: string;
  password: string;
}

/** 分页结果 */
export interface PagedResult<T> {
  list: T[];
  total: number;
  pageIndex: number;
  pageSize: number;
}

/** DbConn 列表/详情项（不含任何密码信息） */
export interface DbConnItem {
  id: number;
  connName: string;
  dbType: string;
  host: string;
  port: number;
  database: string;
  username: string;
  options: string | null;
  remark: string | null;
  status: number;
  createdAt: string;
  updatedAt: string;
}

/** 新增/编辑表单载荷。
 * 编辑时 password 语义：''（未填）→ 不修改；'******' → 不修改；其它 → 重新加密。 */
export interface DbConnSavePayload {
  connName: string;
  dbType: string;
  host: string;
  port: number;
  database: string;
  username: string;
  password?: string | null;
  options?: string | null;
  remark?: string | null;
  status: number;
}

/** 按 ConnName 获取链接串的结果：明文不出网，返回密文与解密说明 */
export interface ConnectionStringDelivery {
  connName: string;
  dbType: string;
  connectionStringEncrypted: string;
  algorithm: string;
  keySource: string;
  payloadLayout: string;
  decryptInstructions: string;
}

/** 用户列表/详情项（不含任何密码信息） */
export interface UserItem {
  id: number;
  userName: string;
  displayName: string;
  status: number;
  lastLoginAt: string | null;
  createdAt: string;
  updatedAt: string;
}

/** 新增用户载荷 */
export interface UserCreatePayload {
  userName: string;
  password: string;
  displayName?: string;
}

/** 编辑用户载荷（displayName/status 缺省 = 保持不变） */
export interface UserUpdatePayload {
  displayName?: string;
  status?: number;
}

/** 系统配置项 */
export interface SystemConfigItem {
  id: number;
  configKey: string;
  configValue: string;
  remark: string | null;
  createdAt: string;
  updatedAt: string;
}

/** 新增系统配置载荷 */
export interface SystemConfigCreatePayload {
  configKey: string;
  configValue: string;
  remark?: string | null;
}

/** 编辑系统配置载荷 */
export interface SystemConfigUpdatePayload {
  configValue: string;
  remark?: string | null;
}

/** 按 Key 保存系统配置载荷 */
export interface SystemConfigSaveByKeyPayload {
  configKey: string;
  configValue: string;
  remark?: string | null;
}

/** 站点技术栈配置项 */
export interface SiteTechStackItem {
  id: number;
  name: string;
  description: string;
  category: string;
  icon: string;
  tags: string[];
}

/** 站点项目经历配置项 */
export interface SiteProjectItem {
  id: number;
  name: string;
  role: string;
  period: string;
  company: string;
  description: string;
  responsibilities: string[];
  technologies: string[];
  achievements: string[];
  category: string;
}

/** 可用的目标数据库类型 */
export const DB_TYPES: Array<{ label: string; value: string }> = [
  { label: 'PostgreSQL', value: 'PostgreSQL' },
  { label: 'SqlServer（预留）', value: 'SqlServer' },
  { label: 'MySql（预留）', value: 'MySql' },
];
