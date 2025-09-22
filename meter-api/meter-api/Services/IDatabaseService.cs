using meter_api.Datatypes;

namespace meter_api.Services
{
    public interface IDatabaseService
    {
        Task<MeterAgentCredentials> GetCredentialsFromUsername(string username); 
    }
}
