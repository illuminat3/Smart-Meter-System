import { login as apiLogin } from '@/services/auth';
import { ref } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, helpers } from '@vuelidate/validators';
import router from "@/router";
import {useAuthStore} from "@/stores/auth";
import {ToastServiceMethods} from "primevue";

export const checkIsAuthenticatedAndRedirect = async () => {
    const authStore = useAuthStore();
    if (authStore.isAuthenticated) {
        await router.push({ name: 'meters' });
    }
};

export const login = async (toast: ToastServiceMethods) => {
    const isValid = await v$.value.$validate();
    if (!isValid) {
        return;
    }

    try {
        const response = await apiLogin({ username: username.value, password: password.value });
        toast.add({ severity: 'success', summary: 'Login successful', detail: `Welcome, ${response.username}` });
        await router.push({ name: 'meters' });
    } catch (error: any) {
        toast.add({ severity: 'error', summary: 'Login failed', detail: 'Incorrect username or password' });
    }
};

export const username = ref('');
export const password = ref('');

const rules = {
    username: {
        required: helpers.withMessage('Username is required.', required)
    },
    password: {
        required: helpers.withMessage('Password is required.', required)
    }
};

export const v$ = useVuelidate(rules, { username, password });