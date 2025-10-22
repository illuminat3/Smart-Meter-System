<template>
  <div class="min-h-screen flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <Card>
        <template #title>
          <div class="text-center">Welcome to Smart Meter</div>
        </template>
        <template #content>
          <div class="mb-4">
            <UsernameComponent v-model="username"
                               :validation="v$.username"/>
          </div>
          <div class="mb-4">
            <PasswordComponent v-model="password"
                               label="Password"
                               id="password"
                               :validation="v$.password"/>
    </div>
          <div>
            <Button label="Login"
                    class="w-full"
                    @click="login"/>
          </div>
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
import { useToast } from "primevue/usetoast";

const toast = useToast();

const username = ref('');
const password = ref('');

const rules = {
  username: {
    required: helpers.withMessage('Username is required.', required)
  },
  password: {
    required: helpers.withMessage('Password is required.', required)
  }
};

const v$ = useVuelidate(rules, { username, password });

const login = async () => {
  const isValid = await v$.value.$validate();
  if (!isValid) {
    return;
  }

  try {
    const response = await apiLogin({ username: username.value, password: password.value });
    toast.add({ severity: 'success', summary: 'Login successful', detail: `Welcome, ${response.username}` });
    // TODO: Navigate to dashboard upon success
  } catch (error: any) {
    toast.add({ severity: 'error', summary: 'Login failed', detail: 'Incorrect username or password' });
  }
};
</script>
