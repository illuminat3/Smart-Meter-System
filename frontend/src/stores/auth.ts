import { defineStore } from 'pinia';

const TOKEN_KEY = 'auth_token';

interface State {
  token: string | undefined;
  user: string | undefined;
}

export const useAuthStore = defineStore('auth', {
  state: (): State => ({
    token: undefined,
    user: undefined,
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
    setToken(token: string, user: string) {
      this.token = token;
      if (user !== undefined) this.user = user;
      try {
        localStorage.setItem(TOKEN_KEY, token);
      } catch (_) {
        // ignore
      }
    },
    clearToken() {
      this.token = undefined;
      this.user = undefined;

      localStorage.removeItem(TOKEN_KEY);
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
