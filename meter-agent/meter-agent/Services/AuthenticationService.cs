using meter_agent.Datatypes.Requests;
using meter_agent.Datatypes.Responses;
using System.Net.Http.Json;

namespace meter_agent.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(string apiBase)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiBase)
            };
        }

        public async Task<AgentLoginResponse> Login(AgentLoginRequest request)
        {
            string path = "auth/agent/login";

            var response = await _httpClient.PostAsJsonAsync(path, request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AgentLoginResponse>();

            return result ?? throw new InvalidOperationException("Failed to deserialize login response.");
        }
    }
}
