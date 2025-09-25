namespace meter_api.Datatypes.Messages
{
    public class MeterUpdateMessage : IMessage<MeterSnapshot>
    {
        public string MessageName { get; } = "MeterUpdate";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public required MeterSnapshot Body { get; set; }
    }
}
