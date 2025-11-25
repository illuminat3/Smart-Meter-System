using meter_agent.DataTypes.Messages;

namespace meter_agent.Hubs;

public interface IAgentHubClient
{
    bool IsConnected { get; }
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    Task SendMessageAsync<TBody>(IMessage<TBody> message, CancellationToken cancellationToken = default);
}
