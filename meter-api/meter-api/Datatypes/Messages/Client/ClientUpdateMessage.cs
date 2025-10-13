namespace meter_api.Datatypes.Messages.Client
{
    public class ClientUpdateMessage : IMessage<MeterSnapshot>
    {
        public string MessageName { get; } = "MeterUpdate";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public required MeterSnapshot Body { get; set; }
    }
}
