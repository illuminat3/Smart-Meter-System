namespace meter_agent.Datatypes
{
    public class Credentials(string meterId, string username, string password)
    {
        public required string MeterId { get; init; } = meterId;
        public required string Username { get; init; } = username;
        public required string Password { get; init; } = password;
    }
}
