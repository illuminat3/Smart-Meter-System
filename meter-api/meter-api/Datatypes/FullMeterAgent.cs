using meter_api.Datatypes.Database;
using System.Text.Json.Serialization;

namespace meter_api.Datatypes;

public class FullMeterAgent : MeterAgent
{
    public required MeterAgentCredentials Credentials { get; set; }
    public required List<MeterAgentReading> Readings { get; set; }

    [JsonIgnore]
    public MeterAgentReading? PreviousReading => Readings.OrderByDescending(r => r.TimestampUtc).FirstOrDefault();
}
