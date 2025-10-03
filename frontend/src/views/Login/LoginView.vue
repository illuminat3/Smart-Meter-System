<template>
  <div class="min-h-screen flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <Card>
        <template #title>
          <div class="text-center">Welcome to Smart Meter</div>
        </template>
        <template #content>
          <form class="space-y-4" @submit="login" novalidate>
            <UsernameComponent v-model="username"
                               :validation="v$.username" />
            <PasswordComponent v-model="password"
                               label="Password"
                               id="password"
                               :validation="v$.password" />
            <Button type="submit"
                    label="Login"
                    class="w-full" />
          </form>
        </template>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useVuelidate } from '@vuelidate/core';
import { required, helpers } from '@vuelidate/validators';
import Card from 'primevue/card';
import Button from 'primevue/button';
import UsernameComponent from '@/components/UsernameComponent.vue';
import PasswordComponent from '@/components/PasswordComponent.vue';
import { login as apiLogin } from '@/services/auth';

const username = ref('');
const password = ref('');
const loading = ref(false);
const errorMsg = ref<string | null>(null);

const rules = {
  username: {
    required: helpers.withMessage('Username is required.', required)
  },
  password: {
    required: helpers.withMessage('Password is required.', required)
  }
};

const v$ = useVuelidate(rules, { username, password });

const login = async (e: Event) => {
  e.preventDefault();
  errorMsg.value = null;
  const isValid = await v$.value.$validate();
  if (!isValid) {
    return;
  }
  try {
    loading.value = true;
    const res = await apiLogin({ username: username.value, password: password.value });
    console.log('Logged in:', res);
    // TODO: Navigate to dashboard upon success
  } catch (err: any) {
    console.error('Login failed', err);
    errorMsg.value = err?.message || 'Login failed';
  } finally {
    loading.value = false;
  }
};
</script>
