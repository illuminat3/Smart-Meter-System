using System.Data;

namespace meter_api.Datatypes;

public class MeterSnapshot
{ 
    public required string MeterId { get; set; }
    public ConnectionState ConnectionState { get; set; } = ConnectionState.Closed;
    public required string DisplayName { get; set; }
    public required decimal CurrentUsage { get; set; }
    public required decimal TotalUsage { get; set; }
    public required decimal TotalCost { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
