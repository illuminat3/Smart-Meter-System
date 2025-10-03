import { request } from '@/lib/httpClient';
import { useAuthStore } from '@/stores/auth';

export type LoginRequest = {
  username: string;
  password: string;
};

export type LoginResponse = {
  token: string;
  user?: {
    id: string | number;
    username: string;
    [key: string]: any;
  };
};

/**
 * Performs login against the backend.
 * The endpoint path defaults to '/auth/login'. You can override by passing `path`.
 */
export async function login(data: LoginRequest, path: string = '/auth/login'): Promise<LoginResponse> {
  const res = await request<LoginResponse>({
    method: 'POST',
    url: path,
    data
  });

  if (res?.token) {
    const auth = useAuthStore();
    auth.setToken(res.token, res.user ?? null);
  }
  return res;
}

export function logout() {
  const auth = useAuthStore();
  auth.clearToken();
}
