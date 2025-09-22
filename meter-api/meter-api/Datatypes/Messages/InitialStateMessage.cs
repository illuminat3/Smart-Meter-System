namespace meter_api.Datatypes.Messages
{
    public class InitialStateMessage
    {
        public const string MessageName = "InitialState";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public List<MeterSnapshot> MeterSnapshots { get; set; } = [];
    }
}
