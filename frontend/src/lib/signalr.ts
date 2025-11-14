import { HubConnection, HubConnectionBuilder, LogLevel, HttpTransportType, HubConnectionState } from '@microsoft/signalr';
import { ref } from 'vue';
import { getStoredToken } from '@/stores/auth';

// Connection state as a simple union type
export type ConnectionStatus = 'disconnected' | 'connecting' | 'connected' | 'reconnecting' | 'error';

const status = ref<ConnectionStatus>('disconnected');
let connection: HubConnection | null = null;

function getHubUrl(): string {
  const env: any = (import.meta as any).env || {};
  const explicit = env.VITE_SIGNALR_URL as string | undefined;
  if (explicit) return explicit;

  const api = (env.VITE_API_BASE_URL as string | undefined) || '';
  // Default path expected by the backend hub
  return api + '/hub/clients';
}

function buildConnection(): HubConnection {
  const hubUrl = getHubUrl();

  const builder = new HubConnectionBuilder()
    .withUrl(hubUrl, {
      accessTokenFactory: () => getStoredToken() || '',
      transport: HttpTransportType.WebSockets,
      skipNegotiation: true,
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(LogLevel.Information);

  return builder.build();
}

export async function connect(): Promise<void> {
  if (connection && (connection.state === HubConnectionState.Connected || connection.state === HubConnectionState.Connecting)) {
    return;
  }

  if (!connection) {
    connection = buildConnection();

    // Wire connection lifecycle to our reactive status
    connection.onreconnecting(() => {
      status.value = 'reconnecting';
    });
    connection.onreconnected(() => {
      status.value = 'connected';
    });
    connection.onclose(() => {
      status.value = 'disconnected';
    });
  }

  try {
    status.value = 'connecting';
    await connection.start();
    status.value = 'connected';
    // Optionally, you can invoke a server method to announce presence if needed
  } catch (e) {
    console.error('SignalR connection failed:', e);
    status.value = 'error';
    throw e;
  }
}

export async function disconnect(): Promise<void> {
  if (connection) {
    try {
      await connection.stop();
    } finally {
      status.value = 'disconnected';
      // Keep connection instance for potential reuse; set to null to force rebuild next time
      // connection = null;
    }
  }
}

export function on<T extends any[]>(event: string, callback: (...args: T) => void): void {
    if (!connection) {
    // Build a connection so that handlers can be registered before connect()
    connection = buildConnection();
  }
  connection.on(event, callback as any);
}

export function off(event: string, callback?: (...args: any[]) => void): void {
  if (!connection) return;
  if (callback) connection.off(event, callback);
  else connection.off(event);
}

export function getConnection(): HubConnection | null {
  return connection;
}

export function useSignalRStatus() {
  return status;
}

// Optional helper to call server hub methods from the client
export async function invoke<T = any>(methodName: string, ...args: any[]): Promise<T> {
  if (!connection) {
    connection = buildConnection();
  }
  if (connection.state !== HubConnectionState.Connected) {
    await connect();
  }
  return connection.invoke<T>(methodName, ...args);
}
