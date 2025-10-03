import { defineStore } from 'pinia';

const TOKEN_KEY = 'auth_token';

export type AuthUser = {
  id?: string | number;
  username?: string;
  // extend as needed
  [key: string]: any;
} | null;

interface State {
  token: string | null;
  user: AuthUser;
}

export const useAuthStore = defineStore('auth', {
  state: (): State => ({
    token: null,
    user: null,
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
  },
  actions: {
    loadFromStorage() {
      try {
        const t = localStorage.getItem(TOKEN_KEY);
        if (t) {
          this.token = t;
        }
      } catch (_) {
        // no-op: localStorage might be unavailable
      }
    },
    setToken(token: string, user?: AuthUser) {
      this.token = token;
      if (user !== undefined) this.user = user;
      try {
        localStorage.setItem(TOKEN_KEY, token);
      } catch (_) {
        // ignore
      }
    },
    clearToken() {
      this.token = null;
      this.user = null;
      try {
        localStorage.removeItem(TOKEN_KEY);
      } catch (_) {
        // ignore
      }
    },
  },
});

export function getStoredToken(): string | null {
  try {
    return localStorage.getItem(TOKEN_KEY);
  } catch (_) {
    return null;
  }
}

export { TOKEN_KEY };
