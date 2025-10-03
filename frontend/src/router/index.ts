import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import loginRoutes from '@/views/Login';

const routes: RouteRecordRaw[] = [
  ...loginRoutes,
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
