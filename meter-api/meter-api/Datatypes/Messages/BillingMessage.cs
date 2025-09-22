namespace meter_api.Datatypes.Messages
{
    public class BillingMessage
    {
        public const string MessageName = "Billing";
        public required string MeterId { get; set; }
        public required float TotalCost { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
