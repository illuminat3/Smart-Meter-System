using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using System.Collections.Concurrent;

namespace meter_api.Services
{
    public class MeterAgentService(IDatabaseService databaseService, IBillingService billingService, IClientService clientService) : IMeterAgentService
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

        public Task HandleErrorUpdate(string meterId, AgentError error)
        {
            throw new NotImplementedException();
        }

        public async Task HandleUsageUpdate(string meterId, AgentUsage usage)
        {
            var fullMeterAgent = await databaseService.GetFullMeterAgentFromId(meterId);
            var previousReading = fullMeterAgent.PreviousReading;

            var currentReading = new MeterAgentReading
            {
                MeterId = meterId,
                PreviousReadingId = previousReading?.Id ?? string.Empty,
                TimestampUtc = DateTime.UtcNow,
                Usage = usage.EnergyUsedKWh,
            };

            var cost = billingService.CalculateCost(currentReading, previousReading);
            currentReading.Billing = cost;

            await databaseService.Create<MeterAgentReading>(currentReading);
            await UpdateAgent(meterId);
            await clientService.MeterAgentUpdate(meterId);
        }

        public async Task UpdateAgent(string meterId)
        {
            var readings = await databaseService.GetCollection<MeterAgentReading>(new Dictionary<string, string> { { "meterId", meterId } });
            var meterAgent = await databaseService.Get<MeterAgent>(new Dictionary<string, string> { { "id", meterId } });

            meterAgent.TotalBilling = readings.Sum(r => r.Billing);
            meterAgent.TotalUsage = readings.Sum(r => r.Usage);

            await databaseService.Put(meterAgent);
        }

        public bool IsMeterAgentConnected(string meterId) => _meterAgentConnections.TryGetValue(meterId, out var connections) && !connections.IsEmpty;
    }
}
