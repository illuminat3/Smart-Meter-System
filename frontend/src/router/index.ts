import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import {useAuthStore} from '@/stores/auth';
import loginRoutes from '@/views/Login';
import metersRoutes from '@/views/Meters';

const routes: RouteRecordRaw[] = [
    ...loginRoutes,
    ...metersRoutes,
    {
        path: '/:pathMatch(.*)*',
        name: 'notFound',
        redirect: { name: 'login' }
    }
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
    const authStore = useAuthStore();

    if (to.meta.requiresAuth && !authStore.isAuthenticated) {
        next({name: 'login'});
    } else {
        next();
    }
});

export default router;