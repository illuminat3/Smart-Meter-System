namespace meter_api.Datatypes
{
    public class MeterAgentReading
    {
        public required string Id { get; set; } = Guid.NewGuid().ToString();
        public required string MeterId {  get; set; }
        public required string PreviousReadingId { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public required double Usage { get; set; } 
    }
}
