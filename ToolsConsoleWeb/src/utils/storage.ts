const TOKEN_KEY = 'tc_token';
const USER_KEY = 'tc_user';

export function getToken(): string {
  return localStorage.getItem(TOKEN_KEY) ?? '';
}

export function setToken(token: string): void {
  localStorage.setItem(TOKEN_KEY, token);
}

export function getStoredUser(): { userName: string; displayName: string } {
  try {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as { userName: string; displayName: string }) : { userName: '', displayName: '' };
  } catch {
    return { userName: '', displayName: '' };
  }
}

export function setStoredUser(user: { userName: string; displayName: string }): void {
  localStorage.setItem(USER_KEY, JSON.stringify(user));
}

export function clearAuthStorage(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}
