using meter_api.Datatypes;
using System.Collections.Concurrent;

namespace meter_api.Services
{
    public class MeterAgentService : IMeterAgentService
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _meterAgentConnections = new();

        public void AgentConnected(string meterId, string connectionId)
        {
            var connections = _meterAgentConnections.GetOrAdd(meterId, _ => new ConcurrentDictionary<string, byte>());
            connections[connectionId] = 0;
        }

        public void AgentDisconnected(string meterId, string connectionId)
        {
            if (_meterAgentConnections.TryGetValue(meterId, out var connections))
            {
                connections.TryRemove(connectionId, out _);
                if (connections.IsEmpty)
                {
                    _meterAgentConnections.TryRemove(meterId, out _);
                }
            }
        }

        public void HandleErrorUpdate(string meterId, AgentError error)
        {
            throw new NotImplementedException();
        }

        public void HandleUsageUpdate(string meterId, AgentUsage usage)
        {
            throw new NotImplementedException();
        }

        public bool IsMeterAgentConnected(string meterId) => _meterAgentConnections.TryGetValue(meterId, out var connections) && !connections.IsEmpty;
    }
}
