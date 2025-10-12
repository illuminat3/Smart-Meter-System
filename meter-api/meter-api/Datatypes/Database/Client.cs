namespace meter_api.Datatypes.Database
{
    public class Client : IDatabaseObject
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public List<string> MeterIds { get; set; } = [];
    }
}
