namespace meter_api.Datatypes.Responses
{
    public class AgentLoginResponse
    {
        public required string MeterId { get; set; }
        public required string Username { get; set; }
        public required string PreviousReadingId { get; set; }
    }
}
