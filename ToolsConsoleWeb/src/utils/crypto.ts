/**
 * 传输加密工具：AES-256-CBC（PKCS7），供登录/新增用户/重置密码提交前加密密码。
 * 密钥与后端 Security:TransportKey / Security:TransportIv 保持一致：
 *   key = Base64 解码后 32 字节（utf8: "0123456789abcdef0123456789abcdef"）
 *   iv  = Base64 解码后 16 字节（utf8: "0123456789abcdef"）
 * 生产环境更换密钥时需同时修改后端 appsettings 与本文件常量。
 */

const TRANSPORT_KEY_BASE64 = 'MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=';
const TRANSPORT_IV_BASE64 = 'MDEyMzQ1Njc4OWFiY2RlZg==';

function base64ToBytes(base64: string): Uint8Array<ArrayBuffer> {
  const binary = atob(base64);
  const bytes = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i += 1) {
    bytes[i] = binary.charCodeAt(i);
  }
  return bytes;
}

function bytesToBase64(bytes: Uint8Array): string {
  let binary = '';
  const chunk = 0x8000;
  for (let i = 0; i < bytes.length; i += chunk) {
    binary += String.fromCharCode(...bytes.subarray(i, i + chunk));
  }
  return btoa(binary);
}

/** AES-256-CBC 加密明文，返回 Base64 密文（默认 PKCS7 填充，与后端一致） */
export async function aesEncryptPassword(plainText: string): Promise<string> {
  const keyBytes = base64ToBytes(TRANSPORT_KEY_BASE64);
  const iv = base64ToBytes(TRANSPORT_IV_BASE64);
  const key = await crypto.subtle.importKey('raw', keyBytes, { name: 'AES-CBC' }, false, ['encrypt']);
  const data = new Uint8Array(new TextEncoder().encode(plainText));
  const cipher = await crypto.subtle.encrypt({ name: 'AES-CBC', iv }, key, data);
  return bytesToBase64(new Uint8Array(cipher));
}
