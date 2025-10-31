using meter_agent.Datatypes.Requests;
using meter_agent.Datatypes.Responses;

namespace meter_agent.Services
{
    public interface IAuthenticationService
    {
        Task<AgentLoginResponse> Login(AgentLoginRequest request);
    }
}
