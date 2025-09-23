using meter_api.Datatypes;
using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public interface IDatabaseService
    {
        Task<ICredential> GetCredentialsFromUsername(string username); 
        Task<Client> GetClientFromUsername(string username); 
        Task<MeterAgent> GetAgentFromUsername(string username);
    }
}
