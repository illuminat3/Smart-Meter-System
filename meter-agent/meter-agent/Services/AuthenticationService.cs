using meter_agent.Datatypes.Requests;
using meter_agent.Datatypes.Responses;
using System.Net.Http.Json;

namespace meter_agent.Services;

public class AuthenticationService(HttpClient httpClient) : IAuthenticationService
{
    public async Task<AgentLoginResponse> Login(AgentLoginRequest request)
    {
        var path = "auth/agent/login";

        var response = await httpClient.PostAsJsonAsync(path, request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AgentLoginResponse>();

        return result ?? throw new InvalidOperationException("Failed to deserialize login response.");
    }
}
