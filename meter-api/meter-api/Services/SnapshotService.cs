using meter_api.Datatypes;

namespace meter_api.Services
{
    public class SnapshotService(IDatabaseService databaseService) : ISnapshotService
    {
        public async Task<MeterSnapshot> GetMeterSnapshot(string meterId)
        {
            return await databaseService.GetMeterSnapshotFromId(meterId);
        }

        public async Task<List<MeterSnapshot>> GetMeterSnapshotsForClient(string clientId)
        {
            var client = await databaseService.GetClientFromId(clientId);
            if (client?.MeterIds == null || client.MeterIds.Count == 0)
                return [];

            var tasks = client.MeterIds.Select(databaseService.GetMeterSnapshotFromId);
            return (await Task.WhenAll(tasks)).ToList();
        }
    }
}
