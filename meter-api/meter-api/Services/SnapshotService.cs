using meter_api.Datatypes;
using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public class SnapshotService(IDatabaseService databaseService) : ISnapshotService
    {
        public async Task<MeterSnapshot> GetMeterSnapshot(string meterId)
        {
            var fullMeterAgent = await databaseService.GetFullMeterAgentFromId(meterId);

            return new MeterSnapshot
            {
                MeterId = fullMeterAgent.Id,
                DisplayName = fullMeterAgent.DisplayName,
                CurrentUsage = fullMeterAgent.Readings.FirstOrDefault(r => r.Id == fullMeterAgent.PreviousReadingId)?.Usage ?? 0m,
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
