namespace meter_api.Datatypes
{
    public class MeterSnapshot
    { 
        public required string MeterId { get; set; }
        public required string DisplayName { get; set; }
        public required decimal CurrentUsage { get; set; }
        public required decimal TotalUsage { get; set; }
        public required decimal TotalCost { get; set; }
    }
}
