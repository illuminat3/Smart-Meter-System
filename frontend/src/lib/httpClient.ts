import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';
import { getStoredToken, TOKEN_KEY } from '@/stores/auth';

const API_BASE_URL = (import.meta as any).env?.VITE_API_BASE_URL as string | undefined;

function createClient(): AxiosInstance {
  const instance = axios.create({
    baseURL: API_BASE_URL || '/api',
    timeout: 15000,
    headers: {
      'Content-Type': 'application/json'
    }
  });

  // Attach Authorization header if token exists
  instance.interceptors.request.use((config) => {
    const t = getStoredToken();
    if (t) {
      config.headers = config.headers || {};
      (config.headers as any)['Authorization'] = `Bearer ${t}`;
    }
    return config;
  });

  // Basic error normalization
  instance.interceptors.response.use(
    (response: AxiosResponse) => response,
    (error) => {
      if (error.response) {
        // Pass-through but keep a friendlier message
        const status = error.response.status;
        const msg =
          (error.response.data && (error.response.data.message || error.response.data.error)) ||
          error.message ||
          'Request failed';
        return Promise.reject({ status, message: msg, data: error.response.data });
      }
      if (error.request) {
        return Promise.reject({ status: 0, message: 'Network error or no response from server' });
      }
      return Promise.reject({ status: 0, message: error.message || 'Unknown error' });
    }
  );

  return instance;
}

export const http: AxiosInstance = createClient();

// Helper for typed requests
export async function request<T = any>(config: AxiosRequestConfig): Promise<T> {
  const res = await http.request<T>(config);
  return res.data as T;
}

export default http;
