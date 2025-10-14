namespace meter_api.Datatypes.Messages.Client
{
    public class ClientInitialStateMessage : IMessage<List<MeterSnapshot>>
    {
        public string MessageName { get; } = "ClientInitialState";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public List<MeterSnapshot> Body { get; set; } = [];
    }
}
