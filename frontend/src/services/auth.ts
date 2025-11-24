import { request } from "@/lib/httpClient";
import { useAuthStore } from "@/stores/auth";

export type LoginRequest = {
  username: string;
  password: string;
};

export type LoginResponse = {
  authenticationToken: string;
  username: string;
};

export async function login(data: LoginRequest): Promise<LoginResponse> {
  const res = await request<LoginResponse>({
    method: "POST",
    url: "/auth/client/login",
    data,
  });

  if (res?.authenticationToken) {
    const auth = useAuthStore();
    auth.setToken(res.authenticationToken, res.username);
  }

  return res;
}

export function logout() {
  const auth = useAuthStore();
  auth.clearToken();
}
