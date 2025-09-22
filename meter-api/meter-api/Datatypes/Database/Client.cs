namespace meter_api.Datatypes.Database
{
    public class Client
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public List<string> MeterIds { get; set; } = [];
    }
}
