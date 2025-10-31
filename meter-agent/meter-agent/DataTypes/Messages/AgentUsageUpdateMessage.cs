namespace meter_agent.DataTypes.Messages
{
    public class AgentUsageUpdateMessage : IMessage<AgentUsage>
    {
        public string MessageName { get; } = "AgentUsageUpdate";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public required AgentUsage Body { get; set; }
    }
}
