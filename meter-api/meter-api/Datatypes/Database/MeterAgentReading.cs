namespace meter_api.Datatypes.Database
{
    public class MeterAgentReading
    {
        public required string Id { get; set; }
        public required string MeterId {  get; set; }
        public required string PreviousReadingId { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public required double Usage { get; set; }
        public decimal Billing { get; set; } = 0.0m;
    }
}
