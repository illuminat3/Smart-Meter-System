namespace meter_api.Datatypes.Messages.Agent
{
    public class AgentErrorUpdate : IMessage<AgentError>
    {
        public string MessageName { get; } = "AgentErrorUpdate";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public required AgentError Body { get; set; }
    }
}
