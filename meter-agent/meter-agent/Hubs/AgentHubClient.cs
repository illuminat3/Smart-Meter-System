using meter_agent.Datatypes.Requests;
using meter_agent.DataTypes.Messages;
using meter_agent.Services;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace meter_agent.Hubs
{
    public class AgentHubClient(IAuthenticationService authenticationService, AgentLoginRequest request, string hubUrl) : IAgentHubClient, IAsyncDisposable
    {
        private HubConnection? _connection;
        private bool _disposed;
        private readonly SemaphoreSlim _gate = new(1, 1);

        private async Task<string?> GetToken()
        {
            var response = await authenticationService.Login(request);
            return response?.AuthenticationToken;
        }

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            await _gate.WaitAsync(cancellationToken);
            try
            {
                if (_connection is { State: HubConnectionState.Connected or HubConnectionState.Connecting or HubConnectionState.Reconnecting })
                {
                    return;
                }

                _connection = new HubConnectionBuilder()
               .WithUrl(hubUrl, options =>
               {
                   options.AccessTokenProvider = GetToken;
                   options.Transports = HttpTransportType.WebSockets;
               })
               .WithAutomaticReconnect(
               [
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
               ])
               .Build();

                await _connection.StartAsync(cancellationToken);
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            await _gate.WaitAsync(cancellationToken);
            try
            {
                if (_connection is null)
                {
                    return;
                }

                if (_connection.State is HubConnectionState.Connected or HubConnectionState.Reconnecting or HubConnectionState.Connecting)
                {
                    await _connection.StopAsync(cancellationToken);
                }

                await _connection.DisposeAsync();
                _connection = null;
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task SendMessageAsync<TBody>(IMessage<TBody> message, CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
            {
                await ConnectAsync(cancellationToken);
            }

            var raw = JsonSerializer.Serialize(message);

            if (_connection is not null)
            {
                await _connection.InvokeAsync("ReceiveMessage", raw, cancellationToken);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            await _gate.WaitAsync();
            try
            {
                if (_connection is not null)
                {
                    await _connection.DisposeAsync();
                    _connection = null;
                }
            }
            finally
            {
                _gate.Release();
                _gate.Dispose();
            }
        }
    }
}
