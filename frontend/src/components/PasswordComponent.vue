<template>
  <InputComponent id="username" label="Username" :validation="validation">
    <Password
      class="w-full"
      input-class="w-full"
      :invalid="validation.$error"
      @blur="validation.$touch()"
      v-model="model"
      :feedback="false"
    />
  </InputComponent>
</template>

<script setup lang="ts">
import Password from "primevue/password";
import InputComponent from "@/components/InputComponent.vue";

defineProps<{
  validation?: any;
}>();

const model = defineModel<string>();
</script>
