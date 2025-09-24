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
            var meterIds = client.MeterIds ?? [];
            var meterSnapshots = new List<MeterSnapshot>();

            foreach (var meterId in meterIds)
            {
                try
                {
                    var meterSnapshot = await databaseService.GetMeterSnapshotFromId(meterId);
                    if (meterSnapshot != null)
                    {
                        meterSnapshots.Add(meterSnapshot);
                    }
                }
                catch (KeyNotFoundException)
                {
                   
                }
            }

            return meterSnapshots;
        }
    }
}
