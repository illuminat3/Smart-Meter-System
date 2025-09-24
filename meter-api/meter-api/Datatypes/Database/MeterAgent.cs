using System.Text.Json.Serialization;

namespace meter_api.Datatypes.Database
{
    public class MeterAgent
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
        public required MeterAgentCredentials Credentials { get; set; }
        public required List<MeterAgentReading> Readings { get; set; }
        public required double TotalUsage { get; set; }
        public required decimal TotalBilling { get; set; }

        [JsonIgnore]
        public string PreviousReadingId => Readings.OrderByDescending(r => r.TimestampUtc).FirstOrDefault()?.Id ?? string.Empty;
    }
}
