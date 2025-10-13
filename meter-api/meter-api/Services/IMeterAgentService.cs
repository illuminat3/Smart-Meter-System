using meter_api.Datatypes;

namespace meter_api.Services
{
    public interface IMeterAgentService
    {
        bool IsMeterAgentConnected(string meterId);
        void AgentConnected(string meterId, string connectionId);
        void AgentDisconnected(string meterId, string connectionId);
        void HandleUsageUpdate(string meterId, AgentUsage usage);
        void HandleErrorUpdate(string meterId, AgentError error);
    }
}
