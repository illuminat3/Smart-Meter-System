import type { RouteRecordRaw } from 'vue-router';
import MetersView from './MetersView.vue';

export const metersRoutes: RouteRecordRaw[] = [
  {
      path: '/meters',
      name: 'meters',
      component: MetersView,
      meta: { requiresAuth: true }
  },
];

export default metersRoutes;
