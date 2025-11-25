namespace meter_api.Datatypes.Responses;

public class AgentLoginResponse : ILoginResponse
{
    public required string MeterId { get; set; }
    public required string Username { get; set; }
    public required string AuthenticationToken { get; set; }
}
