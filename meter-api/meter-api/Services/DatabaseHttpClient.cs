using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace meter_api.Services
{
    public class DatabaseHttpClient : IDatabaseHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public DatabaseHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<List<T>?> GetListAsync<T>(string url)
        {
            using var resp = await _httpClient.GetAsync(url);
            if (resp.StatusCode == HttpStatusCode.NotFound) return [];
            resp.EnsureSuccessStatusCode();
            var stream = await resp.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<List<T>>(stream, _jsonOptions);
            return data;
        }
    }
}
