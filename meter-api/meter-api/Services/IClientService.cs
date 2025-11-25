using meter_api.Datatypes;

namespace meter_api.Services;

public interface IClientService
{
    Task MeterAgentUpdate(string meterId);
    Task MeterAgentErrorUpdate(string meterId, AgentError error);
}
