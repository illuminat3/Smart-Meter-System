namespace meter_api.Datatypes.Database;

public class Client : IDatabaseObject
{
    public string Id { get; set; } = string.Empty;
    public required string Name { get; set; }
    public List<string> MeterIds { get; set; } = [];
}
