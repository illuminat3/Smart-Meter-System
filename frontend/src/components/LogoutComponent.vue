<template>
  <Button @click="logout">Logout</Button>
</template>

<script setup lang="ts">
import Button from 'primevue/button';
import {useAuthStore} from "@/stores/auth";
import {useRouter} from "vue-router";
import { disconnect } from '@/lib/signalr';

const authStore = useAuthStore();
const router = useRouter();

const logout = async () => {
  try { await disconnect(); } catch {}
  authStore.clearToken();
  await router.push('/login');
}
</script>
