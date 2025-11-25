namespace meter_api.Datatypes.Requests;

public class ClientLoginRequest : ILoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
