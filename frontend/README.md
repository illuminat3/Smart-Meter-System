# Smart Meter App

This is a smart meter app with:

- TypeScript
- PrimeVue v4 (Aura theme) and PrimeIcons
- Tailwind CSS
- Vue Router
- Vite
- Vuelidate
- Axios

## Getting Started

0. Ensure you are in the right folder

   - Make sure you are in the frontend folder.

1. Install dependencies

   - npm: `npm install`

2. Configure API base URL

   - Run `cp .env.example .env` and set `VITE_API_BASE_URL` to your backend URL.
   - This is: `VITE_API_BASE_URL=https://localhost:7268`

3. Start the dev server

   - npm: `npm run dev`

4. Build for production

   - npm: `npm run build`

5. Preview the production build
   - npm: `npm run preview`

## API Usage

- A reusable axios client is available at `src/lib/httpClient.ts`.
  - It reads the base URL from `VITE_API_BASE_URL`.
  - Automatically attaches a Bearer token from storage (key: `auth_token`) if present.
  - Exposes `request<T>(config)` helper.
- Auth state is managed by a Pinia store at `src/stores/auth.ts`.
  - Store actions: `setToken(token, user?)`, `clearToken()`, `loadFromStorage()`; getter: `isAuthenticated`.
- Example auth service at `src/services/auth.ts`:
  - `login({ username, password }, path='/auth/login')` returns `{ token, user? }` and stores the token via the Pinia store.
  - `logout()` clears the stored token via the Pinia store.
- In a view component, import and use:
  - `import { login } from '@/services/auth';`
  - `await login({ username, password });`

## Project Structure

- `src/main.ts`: App entry. Registers PrimeVue v4 with Aura theme, Tailwind, and Router.
- `src/router/index.ts`: Route definitions (Login `/`, Dashboard `/dashboard`, About `/about`).
- `src/views/`: Page components.
- `src/assets/main.css`: Tailwind entry with `@tailwind` directives.
- `src/lib/httpClient.ts`: Configurable axios client and helpers.
- `src/services/auth.ts`: Example service using the axios client.

## SignalR docs

- [SignalR](https://www.npmjs.com/package/@microsoft/signalr)
