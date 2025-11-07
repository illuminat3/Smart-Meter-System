namespace meter_api.Datatypes.Database
{
    public class Database
    {
        public bool IsInitialised { get; set; } = false;
        public List<Client> Clients { get; set; } = [];
        public List<MeterAgent> MeterAgents { get; set; } = [];
        public List<ClientCredentials> ClientCredentials { get; set; } = [];
        public List<MeterAgentCredentials> MeterAgentCredentials { get; set; } = [];
        public List<MeterAgentReading> MeterAgentReadings { get; set; } = [];
    }
}
