import {
  connect,
  disconnect,
  off as offEvent,
  on as onEvent,
  useSignalRStatus,
} from "@/lib/signalr";
import { computed, onMounted, onUnmounted, ref } from "vue";

export function useMeters() {
  const status = useSignalRStatus();
  const connectionStatus = computed(() => status.value);

  const meters = ref<Record<string, Meter>>({});

  function normalizeMeters(payload: any): Meter[] {
    const body = payload?.Body ?? payload?.body ?? payload;
    const list = Array.isArray(body) ? body : body ? [body] : [];
    return list.map((x: any) => normalizeMeter(x)).filter(Boolean) as Meter[];
  }

  function normalizeMeter(x: any): Meter | undefined {
    if (!x) return undefined;
    const meterId: string = String(x.meterId ?? x.MeterId ?? x.id ?? "").trim();
    if (!meterId) return undefined;
    const displayName: string = String(
      x.displayName ?? x.DisplayName ?? x.name ?? x.Name ?? meterId
    );
    const currentUsage: number = Number(
      x.currentUsage ?? x.CurrentUsage ?? x.current ?? x.value ?? 0
    );
    const totalUsage: number = Number(x.totalUsage ?? x.TotalUsage ?? 0);
    const totalCost: number = Number(x.totalCost ?? x.TotalCost ?? 0);
    return { meterId, displayName, currentUsage, totalUsage, totalCost };
  }

  function upsertMeters(list: Meter[]) {
    const current = meters.value;
    const next: Record<string, Meter> = { ...current };
    let changed = false;
    for (const m of list) {
      const prev = next[m.meterId];
      const merged = {
        ...(prev || {
          meterId: m.meterId,
          displayName: m.displayName,
          currentUsage: 0,
          totalUsage: 0,
          totalCost: 0,
        }),
        ...m,
      } as Meter;
      if (
        !prev ||
        prev.displayName !== merged.displayName ||
        prev.currentUsage !== merged.currentUsage ||
        prev.totalUsage !== merged.totalUsage ||
        prev.totalCost !== merged.totalCost
      ) {
        next[m.meterId] = merged;
        changed = true;
      }
    }
    if (changed) {
      meters.value = next;
    }
  }

  const handleMeterUpdate = (payload: any) => {
    const list = normalizeMeters(payload);
    if (!list.length) return;
    const now = Date.now();
    const current = meters.value;
    const annotated = list.map((m) => {
      const prev = current[m.meterId];
      const changed =
        !prev ||
        prev.displayName !== m.displayName ||
        prev.currentUsage !== m.currentUsage ||
        prev.totalUsage !== m.totalUsage ||
        prev.totalCost !== m.totalCost;
      return changed ? { ...m, lastUpdated: now } : m;
    });
    upsertMeters(annotated);
  };

  const handleInitialState = (payload: any) => {
    const list = normalizeMeters(payload?.Body ?? payload);
    if (list.length) upsertMeters(list);
  };
  const initialEventNames = ["ClientInitialState", "ClientInitialStateMessage"];

  onMounted(() => {
    // Subscribe to hub events (adjust names to match your server if different)
    onEvent("ClientUpdate", handleMeterUpdate);
    initialEventNames.forEach((evt) => onEvent(evt, handleInitialState));

    // Establish connection after handlers are in place
    connect().catch(() => {});
  });

  onUnmounted(async () => {
    offEvent("ClientUpdate", handleMeterUpdate);
    initialEventNames.forEach((evt) => offEvent(evt, handleInitialState));

    await disconnect();
  });

  return { connectionStatus, meters };
}

export type Meter = {
  meterId: string;
  displayName: string;
  currentUsage: number;
  totalUsage: number;
  totalCost: number;
  // Unix epoch milliseconds when this meter last changed (via handleMeterUpdate)
  lastUpdated?: number;
};
