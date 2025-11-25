using meter_api.Datatypes;

namespace meter_api.Services;

public interface ISnapshotService
{
    Task<MeterSnapshot> GetMeterSnapshot(string meterId);  
    Task<List<MeterSnapshot>> GetMeterSnapshotsForClient(string clientId);
}
