using meter_api.Datatypes.Requests;
using meter_api.Datatypes.Responses;

namespace meter_api.Services;

public interface IAuthService
{
    Task<AgentLoginResponse> AgentLogin(AgentLoginRequest request);
    Task<ClientLoginResponse> ClientLogin(ClientLoginRequest request);
    string? TryGetBearerToken(string? authHeader);
    bool IsTokenAuthorised(string? token);
    bool IsAuthorised(HttpContext httpContext);
}
