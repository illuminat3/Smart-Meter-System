using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public interface IAgentTokenService
    {
        string GetAgentToken(MeterAgent agent);

        bool isAgentTokenValid(string agentToken);
    }
}
