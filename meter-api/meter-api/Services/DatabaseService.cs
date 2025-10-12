using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using Microsoft.Extensions.Options;

namespace meter_api.Services
{
    public class DatabaseService(DatabaseHttpClient databaseClient, IOptions<DatabaseOptions> options) : IDatabaseService
    {
        private readonly DatabaseOptions _databaseOptions = options.Value;

        #region Client

        public Task<Client> GetClientFromId(string id)
        {
            throw new NotImplementedException();
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

        public Task<ICredential> GetClientCredentialsFromUsername(string username)
        {
            throw new NotImplementedException();
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

        public Task<MeterSnapshot> GetMeterSnapshotFromId(string id)
        {
            throw new NotImplementedException();
        }

        public Task<FullMeterAgent> GetFullMeterAgentFromId(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<FullMeterAgent> GetFullMeterAgentFromUsername(string username)
        {
            var meterAgent = await GetAgentFromUsername(username);
            var readings = await GetReadingsFromMeterId(meterAgent.Id);
            var credentials = await GetCredentialsFromUsername(username);

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

        public Task<ICredential> GetCredentialsFromUsername(string username)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region MeterAgentReading

        public async Task<List<MeterAgentReading>> GetReadingsFromMeterId(string meterId)
        {
            var meterAgentReadingsUrl = $"{_databaseOptions.ConnectionUrl}/meterAgentReading?meterId={Uri.EscapeDataString(meterId)}";
            var meterAgentReadings = await databaseClient.GetListAsync<MeterAgentReading>(meterAgentReadingsUrl)
                ?? throw new KeyNotFoundException($"No meter agent readings for meterId: {meterId}");

            return meterAgentReadings;
        }

        #endregion
    }
}
