<template>
  <InputComponent id="username" label="Username" :validation="validation">
    <InputText
      class="w-full"
      :invalid="validation.$error"
      @blur="validation.$touch()"
      v-model="model"
    />
  </InputComponent>
</template>

<script setup lang="ts">
import InputText from "primevue/inputtext";
import InputComponent from "@/components/InputComponent.vue";

defineProps<{
  validation?: any;
}>();

const model = defineModel<string>();
</script>
