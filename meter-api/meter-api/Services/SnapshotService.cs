using meter_api.Datatypes;
using meter_api.Datatypes.Database;
using System.Data;

namespace meter_api.Services
{
    public class SnapshotService(IDatabaseService databaseService, IMeterAgentService meterAgentService) : ISnapshotService
    {
        public async Task<MeterSnapshot> GetMeterSnapshot(string meterId)
        {
            var fullMeterAgent = await databaseService.GetFullMeterAgentFromId(meterId);

            return new MeterSnapshot
            {
                MeterId = fullMeterAgent.Id,
                ConnectionState = meterAgentService.IsMeterAgentConnected(meterId) ? ConnectionState.Open : ConnectionState.Closed,
                DisplayName = fullMeterAgent.DisplayName,
                CurrentUsage = fullMeterAgent.PreviousReading?.Usage ?? 0m,
                TotalUsage = fullMeterAgent.TotalUsage,
                TotalCost = fullMeterAgent.TotalBilling
            };
        }

        public async Task<List<MeterSnapshot>> GetMeterSnapshotsForClient(string clientId)
        {
            var client = await databaseService.Get<Client>(new Dictionary<string, string> { { "id", clientId } });
            if (client?.MeterIds == null || client.MeterIds.Count == 0)
                return [];

            var tasks = client.MeterIds.Select(GetMeterSnapshot);
            return [.. (await Task.WhenAll(tasks))];
        }
    }
}
