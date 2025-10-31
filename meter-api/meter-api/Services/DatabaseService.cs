using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using Microsoft.Extensions.Options;
using System.Text;

namespace meter_api.Services
{
    public class DatabaseService(DatabaseHttpClient databaseClient, IOptions<DatabaseOptions> options) : IDatabaseService
    {
        private readonly DatabaseOptions _databaseOptions = options.Value;

        public async Task<FullMeterAgent> GetFullMeterAgentFromId(string id)
        {
            var meterAgent = await Get<MeterAgent>(new Dictionary<string, string> { { "id", id } });
            var readings = await GetCollection<MeterAgentReading>(new Dictionary<string, string> { { "meterId", id } });
            var credentials = await Get<MeterAgentCredentials>(new Dictionary<string, string> { { "meterId", id } });

            var fullMeterAgent = new FullMeterAgent
            {
                Id = meterAgent.Id,
                DisplayName = meterAgent.DisplayName,
                Credentials = credentials,
                Readings = readings,
                TotalUsage = meterAgent.TotalUsage,
                TotalBilling = meterAgent.TotalBilling
            };

            return fullMeterAgent;
        }

        #region Generic Methods

        public async Task<T> Create<T>(T entity)
        {
            var resource = GetResourcePathFor<T>();
            var url = $"{_databaseOptions.ConnectionUrl}/{resource}";
            var created = await databaseClient.PostAsync<T>(url, entity)
                ?? throw new InvalidOperationException($"Failed to create {typeof(T).Name} at {url}");
            return created;
        }
        public async Task<T> Update<T>(string id, T entity)
        {
            var resource = GetResourcePathFor<T>();
            var url = $"{_databaseOptions.ConnectionUrl}/{resource}/{Uri.EscapeDataString(id)}";
            var updated = await databaseClient.PutAsync<T>(url, entity)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} with id '{id}' not found or not updated.");
            return updated;
        }

        public async Task<T> Get<T>(Dictionary<string, string> paramValue)
        {
            var resource = GetResourcePathFor<T>();
            var url = BuildUrl(_databaseOptions.ConnectionUrl, resource, paramValue);

            var entity = await databaseClient.GetFirstOrDefaultAsync<T>(url)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} with specified parameters not found.");

            return entity;
        }

        public async Task<List<T>> GetCollection<T>(Dictionary<string, string> paramValue)
        {
            var resource = GetResourcePathFor<T>();
            var url = BuildUrl(_databaseOptions.ConnectionUrl, resource, paramValue);

            var entities = await databaseClient.GetListAsync<T>(url)
                ?? throw new KeyNotFoundException($"{typeof(T).Name} collection with specified parameters not found.");

            return entities;
        }

        private static string GetResourcePathFor<T>() => typeof(T).Name switch
        {
            nameof(Client) => "clients",
            nameof(ClientCredentials) => "clientCredentials",
            nameof(MeterAgent) => "meterAgents",
            nameof(MeterAgentCredentials) => "meterAgentCredentials",
            nameof(MeterAgentReading) => "meterAgentReading",
            _ => throw new NotSupportedException($"No resource path configured for type {typeof(T).Name}.")
        };

        private static string BuildUrl(string baseUrl, string resource, Dictionary<string, string>? parameters = null)
        {
            var sb = new StringBuilder($"{baseUrl}/{resource}");

            if (parameters != null && parameters.Count > 0)
            {
                sb.Append('?');
                sb.Append(string.Join("&", parameters.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")));
            }

            return sb.ToString();
        }
        #endregion 
    }
}
