using System.Text.Json.Serialization;

namespace meter_api.Datatypes.Database
{
    public class MeterAgent : IDatabaseObject
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
        public required decimal TotalUsage { get; set; }
        public required decimal TotalBilling { get; set; }
    }
}
