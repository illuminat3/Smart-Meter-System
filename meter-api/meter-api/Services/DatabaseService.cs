using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace meter_api.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseOptions _databaseOptions;
        private readonly JsonSerializerOptions _jsonOptions;

        public DatabaseService(HttpClient httpClient, IOptions<DatabaseOptions> options)
        {
            _httpClient = httpClient;
            _databaseOptions = options.Value;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<MeterAgent> GetAgentFromUsername(string username)
        {
            var meterAgentCredentialUrl = $"{_databaseOptions.ConnectionUrl}/meterAgentCredentials?username={Uri.EscapeDataString(username)}";
            var meterAgentCredential = await GetFirstOrDefaultAsync<MeterAgentCredentials>(meterAgentCredentialUrl) ?? throw new KeyNotFoundException($"No meter agent credentials for username '{username}'.");

            var meterAgentUrl = $"{_databaseOptions.ConnectionUrl}/meterAgents/{Uri.EscapeDataString(meterAgentCredential.MeterId)}";
            var meterAgent = await GetSingleAsync<MeterAgent>(meterAgentUrl) ?? throw new KeyNotFoundException($"Meter Agent '{meterAgentCredential.MeterId}' not found.");

            return meterAgent;
        }

        public async Task<Client> GetClientFromUsername(string username)
        {
            var clientCredentialUrl = $"{_databaseOptions.ConnectionUrl}/clientCredentials?username={Uri.EscapeDataString(username)}";
            var clientCredential = await GetFirstOrDefaultAsync<ClientCredentials>(clientCredentialUrl) ?? throw new KeyNotFoundException($"No client credentials for username '{username}'.");

            var clientUrl = $"{_databaseOptions.ConnectionUrl}/clients/{Uri.EscapeDataString(clientCredential.ClientId)}";
            var client = await GetSingleAsync<Client>(clientUrl) ?? throw new KeyNotFoundException($"Client '{clientCredential.ClientId}' not found.");

            return client;
        }

        public Task<ICredential> GetCredentialsFromUsername(string username)
        {
            throw new NotImplementedException(); 
        }

        private async Task<T?> GetSingleAsync<T>(string url)
        {
            using var resp = await _httpClient.GetAsync(url);
            if (resp.StatusCode == HttpStatusCode.NotFound) return default;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        private async Task<T?> GetFirstOrDefaultAsync<T>(string url)
        {
            var list = await GetListAsync<T>(url);
            return list.FirstOrDefault();
        }

        private async Task<List<T>> GetListAsync<T>(string url)
        {
            using var resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var stream = await resp.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<List<T>>(stream, _jsonOptions);
            return data ?? [];
        }
    }
}
