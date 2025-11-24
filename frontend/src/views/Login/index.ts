import type { RouteRecordRaw } from "vue-router";
import LoginView from "./LoginView.vue";

export const loginRoutes: RouteRecordRaw[] = [
  {
    path: "/",
    name: "login",
    component: LoginView,
  },
];

export default loginRoutes;
