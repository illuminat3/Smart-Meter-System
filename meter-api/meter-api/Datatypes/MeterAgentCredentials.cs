namespace meter_api.Datatypes
{
    public class MeterAgentCredentials
    {
        public required string Id { get; set; } = Guid.NewGuid().ToString();
        public required string MeterId {  get; set; }
        public required string Username { get; set; }
        public required string HashedPassword { get; set; }
    }
}
