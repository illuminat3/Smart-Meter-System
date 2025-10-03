<template>
  <IftaLabel>
    <slot :id="id"/>
    <label :for="id">{{ label }} <span v-if="props.validator?.required" class="text-red-500">*</span></label>
  </IftaLabel>

  <div v-if="validation.$dirty && (validation.$error || validation.$invalid)">
    <small class="text-red-500" v-for="error in validation.$errors">{{ error.$message }}</small>
  </div>
</template>

<script setup lang="ts">
import IftaLabel from "primevue/iftalabel";
import Message from "primevue/message";

const props = defineProps<{
  id: string;
  label: string;
  validation: any;
}>();
</script>