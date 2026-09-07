# ToolsConsole

**数据库链接字串维护**工具（全栈）。

- 后端：ASP.NET Core 10 Web API，**DDD 四层**（Domain / Application / Infrastructure / Api）
- 前端：Vue 3 + Vite + TypeScript + TDesign 管理后台（登录页 + Layout + DbConn 维护页）
- 数据库：PostgreSQL（线上 `47.102.153.9:54321`，系统库 `ToolsConsole`）
- 鉴权：JWT；链接串密码：**AES-256-GCM 加密落库**；获取接口**不返回明文**，仅返回用分发密钥加密后的密文与解密说明

## 目录结构

```
ToolsConsole/                  DDD 解决方案
├── ToolsConsole.slnx
├── ToolsConsole.Domain/           领域层：实体 User/DbConn、仓储与加解密/哈希抽象、基类
├── ToolsConsole.Application/      应用层：DTO、IAuthService/IDbConnService、链接串组装器
├── ToolsConsole.Infrastructure/   基础设施：EF Core(Npgsql) DbContext、仓储实现、AES/密码哈希、建库建表+种子
└── ToolsConsole.Api/              Web 宿主：JWT、CORS、全局异常中间件、控制器、.http 联调示例
ToolsConsoleWeb/                Vue3+TDesign 前端（开发端口 5173，/Auth、/DbConn、/ConnectionString 代理到 5091）
```

## 快速开始

### 0. 环境
- .NET 10 SDK、Node.js 18+
- 可访问线上 PostgreSQL（`47.102.153.9:54321`，postgres 账号），或修改 `ToolsConsole.Api/appsettings.json` 的 `ConnectionStrings:Default`

### 1. 后端
```bash
cd ToolsConsole
dotnet restore          # 首次需联网拉取 NuGet 包
dotnet run --project ToolsConsole.Api   # http://localhost:5091
```
首次启动会自动：
1. 若数据库/表不存在则 **EnsureCreated 建库建表**（users、db_conns）；
2. 若 users 为空则**播种默认管理员 `admin / 123`**（PBKDF2 加盐哈希存储）。

**统一请求前缀**：所有接口（含前端请求）带 `/ToolsConsole` 前置，例如 `http://localhost:5091/ToolsConsole/Auth/Login`
（后端 `UsePathBase("/ToolsConsole")` 挂载；不带前缀的路径在本地调试时仍兼容）。开发环境下可访问：
- **Scalar API 文档**（可视化调试，可携带 Bearer Token）：http://localhost:5091/ToolsConsole/scalar/v1
- OpenAPI JSON：http://localhost:5091/ToolsConsole/openapi/v1.json

> 提示：如需改为标准的 EF Migrations 流程，可在联网环境执行
> `dotnet ef migrations add Init --project ToolsConsole.Infrastructure --startup-project ToolsConsole.Api`
> `dotnet ef database update --project ToolsConsole.Infrastructure --startup-project ToolsConsole.Api`
> 并将 `Infrastructure/Seed/DbInitializer.cs` 中的 `EnsureCreatedAsync` 换成迁移逻辑（勿在已用 EnsureCreated 建好的库上混用）。

### 2. 前端
```bash
cd ToolsConsoleWeb
npm install             # 首次需联网
npm run dev             # http://localhost:5173/ToolsConsoleWeb/（登录后进入 DbConn 维护页）
npm run build           # 产物输出到 dist/（vite base=/ToolsConsoleWeb/）
```
> 本地联调：dev 代理默认指向公网隧道 `https://www.tunn.work`；如需指向本地后端，
> 启动前设置 `$env:VITE_API_PROXY='http://localhost:5091'` 再 `npm run dev`。

## 密码传输加密

- **登录、新增用户、重置密码**提交给后端的密码均为 **AES-256-CBC（PKCS7）密文**（Base64），后端解密后才做 PBKDF2 校验/哈希，链路中不出现明文密码；
- 密钥/IV：`appsettings.json` 中 `Security:TransportKey`（32 字节 Base64）与 `Security:TransportIv`（16 字节 Base64）；
- 前端已在 `ToolsConsoleWeb/src/utils/crypto.ts` 封装 `aesEncryptPassword()`，登录/用户管理接口自动调用；生产更换密钥时须**前后端同步修改**（前端同文件常量 + 后端 appsettings）。

## 默认账号

| 账号 | 密码 | 说明 |
|---|---|---|
| admin | 123 | 初始化管理员（首次启动自动播种，请尽快修改并妥善保管） |

> 登录后可在侧边栏「用户管理」中新建用户、重置密码、启停账号；内置 `admin` 与当前登录账号不允许被删除/禁用（后端强制校验）。

## 接口清单（统一省略 /ToolsConsole 前缀展示，实际调用均需带前缀）

| 接口 | 鉴权 | 说明 |
|---|---|---|
| `POST /Auth/Login` | 匿名 | 登录，返回 JWT |
| `GET  /Auth/Info` | JWT | 当前登录用户信息 |
| `GET  /DbConn/GetPage?keyword=&pageIndex=1&pageSize=20` | JWT | 分页（不返回密码） |
| `GET  /DbConn/GetById/{id}` | JWT | 详情 |
| `POST /DbConn/Create` | JWT | 新增（ConnName 唯一，密码加密落库） |
| `PUT  /DbConn/Update/{id}` | JWT | 编辑（password 传 `******` 或省略 = 不修改） |
| `DELETE /DbConn/Delete/{id}` | JWT | 删除 |
| `GET  /User/GetPage?keyword=&pageIndex=1&pageSize=20` | JWT | 用户分页（不返回密码） |
| `GET  /User/GetById/{id}` | JWT | 用户详情 |
| `POST /User/Create` | JWT | 新增用户（用户名唯一，密码 PBKDF2 哈希存储） |
| `PUT  /User/Update/{id}` | JWT | 编辑显示名/状态（内置 admin 与当前登录账号不可禁用） |
| `POST /User/ResetPassword/{id}` | JWT | 重置密码（body: `{ "password": "新密码" }`） |
| `DELETE /User/Delete/{id}` | JWT | 删除用户（内置 admin 与当前登录账号不可删除） |
| `GET  /ConnectionString/GetByConnName?connName=xxx` | **匿名** | 按 ConnName 返回 **AES-256-GCM 加密后的链接串** + 解密说明（明文不出网） |

统一响应：`{ "code": 0成功, "message": "...", "data": ... }`；错误附带对应 HTTP 状态码。

### 取串接口规则（独立匿名控制器）
- `connName` 为空 / 纯空白 → **400**（`code=400, message="connName 不能为空"`）
- 不存在或已禁用 → **404**
- 命中 → `200`，`data` 结构：

```jsonc
{
  "connName": "MyLoveBaby",
  "dbType": "PostgreSQL",
  "connectionStringEncrypted": "Base64(nonce[12]+密文+tag[16])",  // 明文绝不出现在响应中
  "algorithm": "AES-256-GCM",
  "keySource": "Security:DeliveryKey",
  "payloadLayout": "Base64(nonce[12] + ciphertext + tag[16])",
  "decryptInstructions": "解密方式说明（见下方章节）"
}
```

> 🔐 出于安全考虑，服务端先解密存储密文、组装链接串（仅存于进程内），再用「分发密钥」加密后才返回；
> 解密密钥需**调用方与服务端线下提前共享**（对应 `Security:DeliveryKey` 配置值），不随接口下发。
> 该接口仍为匿名，建议部署时通过**网关/IP 白名单/内网访问控制**限制可调用方。

### 取串接口解密（调用方）

密钥：`appsettings.json` 中 `Security:DeliveryKey` 的 **Base64 字符串解码后的 32 字节**即为 AES-256-GCM 密钥
（开发默认值 `ZmVkY2JhOTg3NjU0MzIxMGZlZGNiYTk4NzY1NDMyMTA=`，生产必须双方单独约定）。
密文载荷 = `Base64(nonce[12字节] + ciphertext + tag[16字节])`，nonce 每次请求随机 → 每次密文不同属正常。

**C#（.NET）**
```csharp
using System.Security.Cryptography;
using System.Text;

string Decrypt(string payloadBase64, byte[] key) // key = Convert.FromBase64String("Security:DeliveryKey 的值")
{
    var payload = Convert.FromBase64String(payloadBase64);
    var nonce = payload[..12];
    var cipher = payload[12..^16];
    var tag = payload[^16..];
    var plain = new byte[cipher.Length];
    using var aes = new AesGcm(key, 16);
    aes.Decrypt(nonce, cipher, tag, plain);
    return Encoding.UTF8.GetString(plain);
}
```

**Node.js**
```js
const crypto = require('crypto');
const KEY = Buffer.from('Security:DeliveryKey 的值', 'base64'); // 32 字节
function decrypt(payloadBase64) {
  const payload = Buffer.from(payloadBase64, 'base64');
  const nonce = payload.subarray(0, 12);
  const tag = payload.subarray(payload.length - 16);
  const data = payload.subarray(12, payload.length - 16);
  const decipher = crypto.createDecipheriv('aes-256-gcm', KEY, nonce);
  decipher.setAuthTag(tag);
  return Buffer.concat([decipher.update(data), decipher.final()]).toString('utf8');
}
```

**Python**
```python
import base64
from cryptography.hazmat.primitives.ciphers.aead import AESGCM

KEY = base64.b64decode('Security:DeliveryKey 的值')  # 32 字节
def decrypt(payload_b64: str) -> str:
    payload = base64.b64decode(payload_b64)
    nonce, cipher, tag = payload[:12], payload[12:-16], payload[-16:]
    return AESGCM(KEY).decrypt(nonce, cipher + tag).decode('utf-8')
```

解密得到的就是标准链接串，例如 `Host=47.102.153.9;Port=54321;Database=MyLoveBaby;Username=postgres;Password=hyper;SSL Mode=Disable`。

## DbConn 表字段（db_conns）

`conn_name`(唯一) / `db_type`(默认 PostgreSQL) / `host` / `port` / `database` / `username` / `password_enc`(AES-256-GCM 密文) / `options` / `remark` / `status` / `created_at` / `updated_at`

- 密码**永不明文落库**；列表/详情接口不回传任何密码信息。
- `db_type` 预留扩展（SqlServer/MySql 已可入库，仅组装暂未实现），组装逻辑集中在 `Application/Services/ConnectionStringBuilder.cs`。

## 安全配置（appsettings.json）

| 配置 | 说明 |
|---|---|
| `ConnectionStrings:Default` | 系统库连接串（当前指向线上 ToolsConsole 库） |
| `JwtSettings:Key` | JWT 签名密钥（>=32 字符），**生产请改为环境变量/User Secrets** |
| `Security:EncryptionKey` | 存储密钥：AES-256-GCM（32 字节 Base64），用于密码**落库**加解密，未配置时使用代码内置开发默认值，**生产必须配置** |
| `Security:DeliveryKey` | 分发密钥：AES-256-GCM（32 字节 Base64），用于取串接口**返回密文**，需与调用方线下共享；未配置时使用开发默认值，**生产必须单独配置并与存储密钥区分** |
| `Security:TransportKey` | 传输密钥：AES-256-CBC 密钥（32 字节 Base64），用于解密前端提交的密码密文；须与前端 `utils/crypto.ts` 常量一致，**生产必须单独配置** |
| `Security:TransportIv` | 传输 IV：AES-256-CBC 的 16 字节 IV（Base64），同上与前端一致 |
| `Cors:Origins` | 允许跨域的前端地址（默认 http://localhost:5173） |
