using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace meter_api.Services
{
    public class DatabaseHttpClient
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

        public async Task<T?> GetSingleAsync<T>(string url)
        {
            using var resp = await _httpClient.GetAsync(url);
            if (resp.StatusCode == HttpStatusCode.NotFound) return default;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        public async Task<T?> GetFirstOrDefaultAsync<T>(string url)
        {
            var list = await GetListAsync<T>(url);
            
            if (list == null || list.Count == 0)
                return default;

            return list.FirstOrDefault();
        }

        public async Task<List<T>?> GetListAsync<T>(string url)
        {
            using var resp = await _httpClient.GetAsync(url);
            if (resp.StatusCode == HttpStatusCode.NotFound) return default;
            resp.EnsureSuccessStatusCode();
            var stream = await resp.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<List<T>>(stream, _jsonOptions);
            return data;
        }
    }
}
