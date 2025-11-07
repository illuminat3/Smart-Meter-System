namespace meter_api.Datatypes.Database
{
    public class ClientCredentials : ICredential, IDatabaseObject
    {
        public string Id { get; set; } = string.Empty;
        public required string ClientId { get; set; }
        public required string Username { get; set; }
        public required string HashedPassword { get; set; }
    }
}
