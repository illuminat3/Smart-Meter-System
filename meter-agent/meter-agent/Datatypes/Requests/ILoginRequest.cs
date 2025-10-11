namespace meter_agent.Datatypes.Requests
{
    public interface ILoginRequest
    {
        string Username { get; set; }
        string Password { get; set; }
    }
}
