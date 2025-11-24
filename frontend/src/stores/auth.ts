import { defineStore } from "pinia";

const TOKEN_KEY = "auth_token";

interface State {
  token: string | undefined;
  user: string | undefined;
}

export const useAuthStore = defineStore("auth", {
  state: (): State => ({
    token: undefined,
    user: undefined,
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
  },
  actions: {
    loadFromStorage() {
      const token = localStorage.getItem(TOKEN_KEY);
      if (token) {
        this.token = token;
      }
    },
    setToken(token: string, user: string) {
      this.token = token;
      this.user = user;

      localStorage.setItem(TOKEN_KEY, token);
    },
    clearToken() {
      this.token = undefined;
      this.user = undefined;

      localStorage.removeItem(TOKEN_KEY);
    },
  },
});

export function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export { TOKEN_KEY };
