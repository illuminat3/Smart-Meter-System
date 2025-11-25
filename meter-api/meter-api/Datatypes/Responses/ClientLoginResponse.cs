namespace meter_api.Datatypes.Responses;

public class ClientLoginResponse : ILoginResponse
{
    public required string Username { get; set; }
    public required string AuthenticationToken { get; set; }   
}
