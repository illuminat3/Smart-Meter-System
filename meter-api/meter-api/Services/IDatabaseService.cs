using meter_api.Datatypes.Database;

namespace meter_api.Services
{
    public interface IDatabaseService
    {
        Task<MeterAgentCredentials> GetCredentialsFromUsername(string username); 
    }
}
