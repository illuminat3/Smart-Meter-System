namespace meter_api.Datatypes.Messages
{
    public interface IMessage<TBody>
    {
        string MessageName { get; }
        DateTime Timestamp { get; set; }
        TBody Body { get; }
    }
}
