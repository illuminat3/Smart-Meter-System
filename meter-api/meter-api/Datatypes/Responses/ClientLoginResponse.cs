namespace meter_api.Datatypes.Responses
{
    public class ClientLoginResponse
    {
        public required string AuthenticationToken { get; set; }
        public required string Username { get; set; }
    }
}
