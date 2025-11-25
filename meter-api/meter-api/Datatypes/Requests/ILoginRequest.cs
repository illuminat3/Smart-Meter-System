namespace meter_api.Datatypes.Requests;

public interface ILoginRequest
{
    string Username { get; set; }
    string Password { get; set; }
}
