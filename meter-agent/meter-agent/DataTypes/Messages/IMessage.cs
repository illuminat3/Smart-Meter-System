namespace meter_agent.DataTypes.Messages;

public interface IMessage<TBody>
{
    string MessageName { get; }
    DateTime Timestamp { get; set; }
    TBody Body { get; set; }
}
