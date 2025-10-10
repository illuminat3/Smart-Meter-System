namespace meter_api.Datatypes.Requests
{
    public class AgentLoginRequest : ILoginRequest
    {
        public required string MeterId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
