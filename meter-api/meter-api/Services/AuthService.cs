using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using meter_api.Datatypes.Requests;
using meter_api.Datatypes.Responses;

namespace meter_api.Services
{
    public class AuthService(IDatabaseService databaseService, IHashService hashService, IJwtService jwtService, IAgentTokenService agentTokenService) : IAuthService
    {
        public async Task<AgentLoginResponse> AgentLogin(AgentLoginRequest request)
        {
            var credential = await databaseService.Get<MeterAgentCredentials>(new Dictionary<string, string>
            {
                { "meterId", request.MeterId },
                { "username", request.Username }
            });

            var hashedPassword = hashService.GetHash(request.Password);

            if (credential.HashedPassword != hashedPassword)
            {
                throw new UnauthorizedAccessException();
            }

            var agent = await databaseService.GetFullMeterAgentFromId(request.MeterId);
            var authToken = agentTokenService.GetAgentToken(agent);

            var response = new AgentLoginResponse
            {
                MeterId = request.MeterId,
                Username = request.Username,
                AuthenticationToken = authToken,
            };

            return response;
        }

        public async Task<ClientLoginResponse> ClientLogin(ClientLoginRequest request)
        {
            var credential = await databaseService.Get<ClientCredentials>(new Dictionary<string, string>
            {
                { "username", request.Username }
            });
            var hashedPassword = hashService.GetHash(request.Password);

            if (credential.HashedPassword != hashedPassword)
            {
                throw new UnauthorizedAccessException();
            }

            var client = await databaseService.Get<Client>(new Dictionary<string, string> { { "username", request.Username } });

            var authToken = jwtService.GetClientJwt(client);

            var response = new ClientLoginResponse 
            { 
                AuthenticationToken = authToken,
                Username = request.Username 
            };

            return response;
        }

        public string? TryGetBearerToken(string? authHeader)
        {
            if (string.IsNullOrWhiteSpace(authHeader)) return null;
            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return null;
            return authHeader["Bearer ".Length..].Trim();
        }

        public bool IsTokenAuthorised(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;
            return jwtService.IsValidJwt(token) || agentTokenService.IsAgentTokenValid(token);
        }

        public bool IsAuthorised(HttpContext httpContext)
        {
            var token = TryGetBearerToken(httpContext.Request.Headers.Authorization);
            return IsTokenAuthorised(token);
        }
    }
}
