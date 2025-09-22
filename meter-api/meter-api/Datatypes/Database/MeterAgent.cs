namespace meter_api.Datatypes.Database
{
    public class MeterAgent
    {
        public required string Id { get; set; } = Guid.NewGuid().ToString();
        public required string DisplayName { get; set; }
        public required MeterAgentCredentials Credentials { get; set; }
        public required List<MeterAgentReading> Readings { get; set; }
        public required double TotalUsage { get; set; } = 0.0d;
    }
}
