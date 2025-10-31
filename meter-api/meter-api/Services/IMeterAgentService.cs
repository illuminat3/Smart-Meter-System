using meter_api.Datatypes;

namespace meter_api.Services
{
    public interface IMeterAgentService
    {
        bool IsMeterAgentConnected(string meterId);
        void AgentConnected(string meterId, string connectionId);
        void AgentDisconnected(string meterId, string connectionId);
        Task HandleUsageUpdate(string meterId, AgentUsage usage);
        Task HandleErrorUpdate(string meterId, AgentError error);
    }
}
