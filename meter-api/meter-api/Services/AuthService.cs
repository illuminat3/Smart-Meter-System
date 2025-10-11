using meter_api.Datatypes.Requests;
using meter_api.Datatypes.Responses;

namespace meter_api.Services
{
    public class AuthService(IDatabaseService databaseService, IHashService hashService, IJwtService jwtService, IAgentTokenService agentTokenService) : IAuthService
    {
        public async Task<AgentLoginResponse> AgentLogin(AgentLoginRequest request)
        {
            var credential = await databaseService.GetAgentCredentialsFromMeterIdAndUsername(request.MeterId, request.Username);
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
            var credential = await databaseService.GetClientCredentialsFromUsername(request.Username);
            var hashedPassword = hashService.GetHash(request.Password);

            if (credential.HashedPassword != hashedPassword)
            {
                throw new UnauthorizedAccessException();
            }

            var client = await databaseService.GetClientFromUsername(request.Username);

            var authToken = jwtService.GetClientJwt(client);

            var response = new ClientLoginResponse 
            { 
                AuthenticationToken = authToken,
                Username = request.Username 
            };

            return response;
        }
    }
}
