using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using Microsoft.Extensions.Options;

namespace meter_api.Services
{
    public class DatabaseService(DatabaseHttpClient databaseClient, IOptions<DatabaseOptions> options) : IDatabaseService
    {
        private readonly DatabaseOptions _databaseOptions = options.Value;

        #region Client

        public async Task<Client> GetClientFromId(string id)
        {
            var clientUrl = $"{_databaseOptions.ConnectionUrl}/clients/{Uri.EscapeDataString(id)}";
            var client = await databaseClient.GetSingleAsync<Client>(clientUrl)
                ?? throw new KeyNotFoundException($"Client: {id} not found");

            return client;
        }

        public async Task<Client> GetClientFromUsername(string username)
        {
            var clientCredentialUrl = $"{_databaseOptions.ConnectionUrl}/clientCredentials?username={Uri.EscapeDataString(username)}";
            var clientCredential = await databaseClient.GetFirstOrDefaultAsync<ClientCredentials>(clientCredentialUrl)
                ?? throw new KeyNotFoundException($"No client credentials for username: {username}");

            var clientUrl = $"{_databaseOptions.ConnectionUrl}/clients/{Uri.EscapeDataString(clientCredential.ClientId)}";
            var client = await databaseClient.GetSingleAsync<Client>(clientUrl)
                ?? throw new KeyNotFoundException($"Client: {clientCredential.ClientId} not found");

            return client;
        }

        #endregion

        #region ClientCredentials

        public async Task<ICredential> GetClientCredentialsFromUsername(string username)
        {
            var clientCredentialUrl = $"{_databaseOptions.ConnectionUrl}/clientCredentials?username={Uri.EscapeDataString(username)}";
            var clientCredential = await databaseClient.GetFirstOrDefaultAsync<ClientCredentials>(clientCredentialUrl)
                ?? throw new KeyNotFoundException($"No client credentials for username: {username}");

            return clientCredential;
        }

        #endregion

        #region MeterAgent

        public async Task<MeterAgent> GetAgentFromId(string id)
        {
            var meterAgentUrl = $"{_databaseOptions.ConnectionUrl}/meterAgents/{Uri.EscapeDataString(id)}";
            var meterAgent = await databaseClient.GetSingleAsync<MeterAgent>(meterAgentUrl)
                ?? throw new KeyNotFoundException($"Meter Agent: {id} not found");

            return meterAgent;
        }

        public async Task<MeterAgent> GetAgentFromUsername(string username)
        {
            var meterAgentCredentialUrl = $"{_databaseOptions.ConnectionUrl}/meterAgentCredentials?username={Uri.EscapeDataString(username)}";
            var meterAgentCredential = await databaseClient.GetFirstOrDefaultAsync<MeterAgentCredentials>(meterAgentCredentialUrl)
                ?? throw new KeyNotFoundException($"No meter agent credentials for username: {username}");

            var meterAgentUrl = $"{_databaseOptions.ConnectionUrl}/meterAgents/{Uri.EscapeDataString(meterAgentCredential.MeterId)}";
            var meterAgent = await databaseClient.GetSingleAsync<MeterAgent>(meterAgentUrl)
                ?? throw new KeyNotFoundException($"Meter Agent: {meterAgentCredential.MeterId} not found");

            return meterAgent;
        }

        public async Task<MeterSnapshot> GetMeterSnapshotFromId(string id)
        {
           var fullMeterAgent = await GetFullMeterAgentFromId(id);

            return new MeterSnapshot
            {
                MeterId = fullMeterAgent.Id,
                DisplayName = fullMeterAgent.DisplayName,
                CurrentUsage = fullMeterAgent.Readings.FirstOrDefault(r => r.Id == fullMeterAgent.PreviousReadingId)?.Usage ?? 0m,
                TotalUsage = fullMeterAgent.TotalUsage,
                TotalCost = fullMeterAgent.TotalBilling
            };
        }

        public async Task<FullMeterAgent> GetFullMeterAgentFromId(string id)
        {
            var meterAgent = await GetAgentFromId(id);
            var readings = await GetReadingsFromMeterId(id);
            var credentials = await GetAgentCredentialsFromMeterId(id);

            var fullMeterAgent = new FullMeterAgent
            {
                Id = meterAgent.Id,
                DisplayName = meterAgent.DisplayName,
                Credentials = (MeterAgentCredentials)credentials,
                Readings = readings,
                TotalUsage = meterAgent.TotalUsage,
                TotalBilling = meterAgent.TotalBilling
            };

            return fullMeterAgent;
        }

        public async Task<FullMeterAgent> GetFullMeterAgentFromUsername(string username)
        {
            var meterAgent = await GetAgentFromUsername(username);
            var readings = await GetReadingsFromMeterId(meterAgent.Id);
            var credentials = await GetAgentCredentialsFromMeterId(meterAgent.Id);

            var fullMeterAgent = new FullMeterAgent
            {
                Id = meterAgent.Id,
                DisplayName = meterAgent.DisplayName,
                Credentials = (MeterAgentCredentials)credentials,
                Readings = readings,
                TotalUsage = meterAgent.TotalUsage,
                TotalBilling = meterAgent.TotalBilling
            };

            return fullMeterAgent;
        }

        #endregion

        #region MeterAgentCredentials

        public async Task<ICredential> GetAgentCredentialsFromMeterIdAndUsername(string meterId, string username)
        {
            var meterAgentCredentialUrl =
                $"{_databaseOptions.ConnectionUrl}/meterAgentCredentials?meterId={Uri.EscapeDataString(meterId)}&username={Uri.EscapeDataString(username)}";

            var meterAgentCredential = await databaseClient.GetFirstOrDefaultAsync<MeterAgentCredentials>(meterAgentCredentialUrl)
                ?? throw new KeyNotFoundException($"No meter agent credentials found for meterId: {meterId} and username: {username}");

            return meterAgentCredential;
        }

        public async Task<ICredential> GetAgentCredentialsFromMeterId(string meterId)
        {
            var meterAgentCredentialUrl =
                $"{_databaseOptions.ConnectionUrl}/meterAgentCredentials?meterId={Uri.EscapeDataString(meterId)}";

            var meterAgentCredential = await databaseClient.GetFirstOrDefaultAsync<MeterAgentCredentials>(meterAgentCredentialUrl)
                ?? throw new KeyNotFoundException($"No meter agent credentials found for meterId: {meterId}");

            return meterAgentCredential;
        }


        #endregion

        #region MeterAgentReading

        public async Task<List<MeterAgentReading>> GetReadingsFromMeterId(string meterId)
        {
            var meterAgentReadingsUrl = $"{_databaseOptions.ConnectionUrl}/meterAgentReading?meterId={Uri.EscapeDataString(meterId)}";
            var meterAgentReadings = await databaseClient.GetListAsync<MeterAgentReading>(meterAgentReadingsUrl)
                ?? [];

            return meterAgentReadings;
        }

        #endregion
    }
}
