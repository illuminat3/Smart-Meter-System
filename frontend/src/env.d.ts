/// <reference types="vite/client" />

declare module "@primeuix/forms" {
  export const Form: any;
  export const FormField: any;
  export function useForm(...args: any[]): any;
}

declare global {
  interface ImportMetaEnv {
    readonly VITE_API_BASE_URL?: string;
  }
  interface ImportMeta {
    readonly env: ImportMetaEnv;
  }
}
export {};
