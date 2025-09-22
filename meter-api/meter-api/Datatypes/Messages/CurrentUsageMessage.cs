namespace meter_api.Datatypes.Messages
{
    public class CurrentUsageMessage
    {
        public const string MessageName = "CurrentUsage";
        public required string MeterId { get; set; }
        public required double CurrentUsage { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
