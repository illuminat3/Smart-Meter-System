namespace meter_api.Datatypes
{
    public class MeterSnapshot
    { 
        public required string MeterId { get; set; }
        public required string DisplayName { get; set; }
        public required double CurrentUsage { get; set; }
        public required float TotalCost { get; set; }
    }
}
