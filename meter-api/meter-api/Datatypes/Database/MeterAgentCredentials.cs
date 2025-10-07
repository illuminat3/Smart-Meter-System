namespace meter_api.Datatypes.Database
{
    public class MeterAgentCredentials : ICredential
    {
        public required string Id { get; set; }
        public required string MeterId {  get; set; }
        public required string Username { get; set; }
        public required string HashedPassword { get; set; }

    }
}
