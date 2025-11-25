namespace meter_api.Datatypes.Database;

public class MeterAgentReading : IDatabaseObject
{
    public string Id { get; set; } = string.Empty;
    public required string MeterId {  get; set; }
    public required string PreviousReadingId { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public required decimal Usage { get; set; }
    public decimal Billing { get; set; } = 0.0m;
}
