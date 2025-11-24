<template>
  <div class="min-h-screen flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <Card>
        <template #title>
          <div class="text-center">Welcome to Smart Meter</div>
        </template>
        <template #content>
          <div class="mb-4">
            <UsernameComponent v-model="username" :validation="v$.username" />
          </div>
          <div class="mb-4">
            <PasswordComponent
              v-model="password"
              label="Password"
              id="password"
              :validation="v$.password"
            />
          </div>
          <div>
            <Button label="Login" class="w-full" @click="login(toast)" />
          </div>
        </template>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import Card from "primevue/card";
import Button from "primevue/button";
import UsernameComponent from "@/components/UsernameComponent.vue";
import PasswordComponent from "@/components/PasswordComponent.vue";
import { useToast } from "primevue/usetoast";

import {
  useLogin,
  checkIsAuthenticatedAndRedirect,
} from "@/composables/login/login";

const toast = useToast();

const { username, password, v$, login } = useLogin();

onMounted(async () => {
  await checkIsAuthenticatedAndRedirect();
});
</script>
