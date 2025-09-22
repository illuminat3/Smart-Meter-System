using meter_api.Datatypes;

namespace meter_api.Services
{
    public class DatabaseService : IDatabaseService
    {
        public Task<MeterAgentCredentials> GetCredentialsFromUsername(string username)
        {
            throw new NotImplementedException();
        }
    }
}
