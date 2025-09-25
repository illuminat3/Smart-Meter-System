namespace meter_api.Datatypes.Messages
{
    public class InitialStateMessage : IMessage<List<MeterSnapshot>>
    {
        public string MessageName { get; } = "InitialState";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public List<MeterSnapshot> Body { get; set; } = [];
    }
}
