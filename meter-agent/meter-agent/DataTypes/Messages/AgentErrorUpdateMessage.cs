namespace meter_agent.DataTypes.Messages;

public class AgentErrorUpdateMessage : IMessage<AgentError>
{
    public string MessageName { get; } = "AgentErrorUpdate";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public required AgentError Body { get; set; }
}
