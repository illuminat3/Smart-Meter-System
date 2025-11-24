<template>
  <div class="min-h-screen">
    <div class="flex items-center justify-between p-4">
      <div class="text-sm text-gray-500">
        Status:
        <span :class="connectionStatusClass">{{ connectionStatus }}</span>
      </div>
      <LogoutComponent />
    </div>

    <div class="p-4">
      <div
        v-if="meterList.length === 0"
        class="text-center text-gray-500 py-16"
      >
        No meters to display.
      </div>

      <div
        v-else
        class="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4"
      >
        <Card v-for="m in meterList" :key="m.meterId" class="shadow-sm">
          <template #title>
            {{ m.displayName }}
          </template>
          <template #subtitle> ID: {{ m.meterId }} </template>
          <template #content>
            <div class="space-y-3">
              <div class="flex items-center justify-between">
                <span class="text-gray-500">Current usage</span>
                <span class="font-semibold">{{ fmt(m.currentUsage) }} kW</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-gray-500">Total usage</span>
                <span class="font-semibold">{{ fmt(m.totalUsage) }} kWh</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-gray-500">Total cost</span>
                <span class="font-semibold">£{{ fmt(m.totalCost) }}</span>
              </div>
            </div>
          </template>
          <template #footer>
            <div class="text-xs text-gray-500 text-right">
              Last updated: {{ fmtTime(m.lastUpdated) }}
            </div>
          </template>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import LogoutComponent from "@/components/LogoutComponent.vue";
import { useMeters } from "@/composables/meters/meters";
import Card from "primevue/card";
import { computed } from "vue";

const { connectionStatus, meters } = useMeters();

const meterList = computed(() => Object.values(meters.value));

const fmt = (n: number) => (Number.isFinite(n) ? n.toFixed(3) : "0.000");
const fmtTime = (ms?: number) => {
  if (!ms) return "—";
  try {
    const d = new Date(ms);
    return d.toLocaleString();
  } catch {
    return "—";
  }
};

const connectionStatusClass = computed(() => {
  const s = String(connectionStatus.value || "").toLowerCase();
  return s === "connected"
    ? "text-green-600"
    : s === "connecting"
    ? "text-yellow-600"
    : "text-red-600";
});
</script>
