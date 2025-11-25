namespace meter_agent.Datatypes.Responses;

public interface ILoginResponse
{
    string Username { get; set; }
    string AuthenticationToken { get; set; }
}
