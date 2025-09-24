namespace meter_api.Datatypes.Messages
{
    public class MeterUpdateMessage
    {
        public const string MessageName = "MeterUpdate";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public required MeterSnapshot Snapshot { get; set; }
    }
}
