using meter_api.Datatypes;
using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public interface IDatabaseService
    {
        Task<ICredential> GetClientCredentialsFromUsername(string username); 
        Task<ICredential> GetAgentCredentialsFromMeterIdAndUsername(string meterId, string username);
        Task<Client> GetClientFromUsername(string username);
        Task<Client> GetClientFromId(string id);
        Task<MeterAgent> GetAgentFromUsername(string username);
        Task<FullMeterAgent> GetFullMeterAgentFromId(string id);
        Task<MeterSnapshot> GetMeterSnapshotFromId(string id);
        Task<FullMeterAgent> GetFullMeterAgentFromUsername(string username);
        Task<List<MeterAgentReading>> GetReadingsFromMeterId(string meterId);
    }
}
